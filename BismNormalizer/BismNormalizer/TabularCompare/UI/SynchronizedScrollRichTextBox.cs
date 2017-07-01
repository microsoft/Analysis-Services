using System;
using System.Windows.Forms;

namespace BismNormalizer.TabularCompare.UI
{
    public class SynchronizedScrollRichTextBox : RichTextBox
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event vScrollEventHandler vScroll;
        public delegate void vScrollEventHandler(Message message);

        public const int WM_VSCROLL = 0x115;
        public const int WM_HSCROLL = 0x114;
        public const int WM_MOUSEWHEEL = 0x020A;
        //public const int WM_COMMAND = 0x111;
        //public const int WM_KEYUP = 0x0101;

        //List of windows message codes: https://www.autoitscript.com/autoit3/docs/appendix/WinMsgCodes.htm

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WM_VSCROLL ||
                message.Msg == WM_HSCROLL ||
                message.Msg == WM_MOUSEWHEEL
               )
            {
                if (vScroll != null)
                {
                    vScroll(message);
                }
            }
            base.WndProc(ref message);
        }

        public void PubWndProc(ref Message message)
        {
            base.WndProc(ref message);
        }
    }
}
