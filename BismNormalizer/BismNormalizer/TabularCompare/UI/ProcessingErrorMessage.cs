using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class ProcessingErrorMessage : Form
    {
        private string _errorMessage;
        private float _dpiScaleFactor;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public float DpiScaleFactor
        {
            get { return _dpiScaleFactor; }
            set { _dpiScaleFactor = value; }
        }

        public ProcessingErrorMessage()
        {
            InitializeComponent();
        }

        private void ErrorMessage_Load(object sender, EventArgs e)
        {
            if (_dpiScaleFactor > 1)
            {
                //DPI
                _dpiScaleFactor = _dpiScaleFactor * HighDPIUtils.PrimaryFudgeFactor;
                float fudgeFactor = HighDPIUtils.SecondaryFudgeFactor;
                this.Scale(new SizeF(_dpiScaleFactor, _dpiScaleFactor * fudgeFactor));
                this.Width = Convert.ToInt32(this.Width * _dpiScaleFactor);
                foreach (Control control in HighDPIUtils.GetChildInControl(this))
                {
                    control.Font = new Font(control.Font.FontFamily,
                                            control.Font.Size * _dpiScaleFactor * fudgeFactor,
                                            control.Font.Style);
                }
            }

            txtErrorMessage.Text = _errorMessage;

            //do not want the OK button selected (closes inadvertently)
            txtErrorMessage.Focus();
            txtErrorMessage.SelectionStart = 0;
            txtErrorMessage.SelectionLength = 0;
        }
    }
}
