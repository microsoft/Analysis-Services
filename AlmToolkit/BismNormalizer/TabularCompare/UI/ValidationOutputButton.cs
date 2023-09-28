using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class ValidationOutputButton : UserControl
    {
        private bool _warningImage = false;
        private bool _realChangeIndex = false;
        private bool _selected = true;

        public event EventHandler ValueChangedHandler;
        public delegate void ValueChangedDelegate(object sender, EventArgs e);

        public ValidationOutputButton()
        {
            InitializeComponent();
        }

        //THIS WHOLE CLASS IS A COMPLETE FUDGE SO THAT THE VALIDATION OUTPUT WINDOW LOOKS LIKE THE VS ERROR LIST WINDOW

        private void ValidationOutputButton_Load(object sender, EventArgs e)
        {
            tabControl1.Width = 250;
            SetImage();
        }

        private void tabControl1_MouseUp(object sender, MouseEventArgs e)
        {
            _realChangeIndex = true;
            _selected = !_selected;
            //if (tabControl1.SelectedIndex == 0)
            if (_selected)
            {
                tabControl1.SelectedIndex = 0;
            }
            else
            {
                tabControl1.SelectedIndex = 1;
            }
            if (ValueChangedHandler != null)
            {
                ValueChangedHandler(this, null);
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (_realChangeIndex)
            {
                _realChangeIndex = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        public bool Selected
        {
            get { return tabControl1.SelectedIndex == 0; }
            set
            {
                _realChangeIndex = true;
                _selected = value;
                switch (_selected)
                {
                    case true:
                        tabControl1.SelectedIndex = 0;
                        break;
                    case false:
                        tabControl1.SelectedIndex = 1;
                        break;
                }
                if (ValueChangedHandler != null)
                {
                    ValueChangedHandler(this, null);
                }
            }
        }

        public bool WarningImage
        {
            get
            {
                return _warningImage;
            }
            set
            {
                _warningImage = value;
                SetImage();
            }
        }

        private void SetImage()
        {
            tabControl1.TabPages[0].ImageIndex = (_warningImage ? 1 : 0);
        }

        public override string Text
        {
            get
            {
                return tabControl1.TabPages[0].Text;
            }
            set
            {
                tabControl1.TabPages[0].Text = value;
            }
        }

    }
}
