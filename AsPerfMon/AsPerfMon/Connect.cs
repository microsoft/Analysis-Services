using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASPerfMon
{
    public partial class Connect : Form
    {
        public string ServerName { get; set; }
        public bool IntegratedAuth { get; set; }
        public string UserName { get; set; }
        public string Passwrod { get; set; }
        public int SampleInterval { get; set; }

        public Connect()
        {
            InitializeComponent();
        }

        private void Connect_Load(object sender, EventArgs e)
        {
            this.txtServer.Text = Settings.Default.ServerName;
            this.chkIntegratedAuth.Checked = Settings.Default.IntegratedAuth;
            this.txtUserName.Text = Settings.Default.UserName;
            this.txtSampleInterval.Text = Settings.Default.SampleInterval.ToString();

            this.txtServer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_MouseDown);
            this.txtUserName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_MouseDown);
            this.txtPassword.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_MouseDown);
            this.txtSampleInterval.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_MouseDown);

        }

        private void chkIntegratedAuth_CheckedChanged(object sender, EventArgs e)
        {
            txtUserName.Enabled = !chkIntegratedAuth.Checked;
            txtPassword.Enabled = !chkIntegratedAuth.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int sampleInterval;
            if (!int.TryParse(txtSampleInterval.Text, out sampleInterval))
            {
                MessageBox.Show("Sample Interval must be a positive integer", "AS PerfMon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.ServerName = txtServer.Text;
            this.IntegratedAuth = chkIntegratedAuth.Checked;
            this.UserName = txtUserName.Text;
            this.Passwrod = txtPassword.Text;
            this.SampleInterval = sampleInterval;

            this.DialogResult = DialogResult.OK;
        }

        private void txt_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }
    }
}
