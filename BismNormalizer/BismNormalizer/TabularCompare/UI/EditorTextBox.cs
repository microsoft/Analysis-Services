
using System;
using System.Windows.Forms;
using System.Security.Permissions;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class EditorTextBox : RichTextBox
    {
        private bool m_FilterMouseClickMessages;

        public bool FilterMouseClickMessages
        {
            get { return m_FilterMouseClickMessages; }
            set { m_FilterMouseClickMessages = value; }
        }

        public EditorTextBox()
        {
            InitializeComponent();
        }

        // Override WndProc so that we can ignore the mouse clicks when macro recording
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case BismNormalizer.NativeMethods.WM_LBUTTONDOWN:
                case BismNormalizer.NativeMethods.WM_RBUTTONDOWN:
                case BismNormalizer.NativeMethods.WM_MBUTTONDOWN:
                case BismNormalizer.NativeMethods.WM_LBUTTONDBLCLK:
                    if (m_FilterMouseClickMessages)
                    {
                        Focus();
                        return;
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        private void richTextBoxCtrl_MouseRecording(object sender, EventArgs e)
        {
            SetCursor(m_FilterMouseClickMessages);
        }

        private void richTextBoxCtrl_MouseLeave(object sender, EventArgs e)
        {
            if(m_FilterMouseClickMessages)
                SetCursor(!m_FilterMouseClickMessages);
        }

        private void SetCursor(bool cursorNo)
        {
            Cursor = cursorNo ? Cursors.No : Cursors.Default;
        }
    }
}
