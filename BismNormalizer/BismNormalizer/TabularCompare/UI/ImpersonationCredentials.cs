using System;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Windows.Forms;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class ImpersonationCredentials : Form
    {
        private string _authenticationKind;
        private string _connectionName;
        private string _username;
        private string _password;
        private string _privacyLevel;
        private float _dpiScaleFactor;

        public string AuthenticationKind
        {
            get { return _authenticationKind; }
            set { _authenticationKind = value; }
        }
        public string ConnectionName
        {
            get { return _connectionName; }
            set { _connectionName = value; }
        }
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string PrivacyLevel
        {
            get { return _privacyLevel; }
            set { _privacyLevel = value; }
        }
        public float DpiScaleFactor
        {
            get { return _dpiScaleFactor; }
            set { _dpiScaleFactor = value; }
        }

        public ImpersonationCredentials()
        {
            InitializeComponent();
        }

        private void ImpersonationCredentials_Load(object sender, EventArgs e)
        {
            if (_dpiScaleFactor > 1)
            {
                //DPI
                float dpiScaleFactorFudged = _dpiScaleFactor * HighDPIUtils.PrimaryFudgeFactor;

                if (Settings.Default.OptionHighDpiLocal)
                {
                    this.Scale(new SizeF(dpiScaleFactorFudged * 0.44f, dpiScaleFactorFudged * 0.35f));
                    this.Width = Convert.ToInt32(this.Width * dpiScaleFactorFudged * 0.6f);

                    foreach (Control control in HighDPIUtils.GetChildInControl(this))
                    {
                        if (control is Button)
                        {
                            control.Font = new Font(control.Font.FontFamily,
                                              control.Font.Size * dpiScaleFactorFudged * 1.1f * HighDPIUtils.PrimaryFudgeFactor,
                                              control.Font.Style);
                        }
                        else
                        {
                            control.Font = new Font(control.Font.FontFamily,
                                            //cbw todo check * 1.4f works on remote desktop setting
                                            control.Font.Size * dpiScaleFactorFudged * 1.4f * HighDPIUtils.PrimaryFudgeFactor,
                                            control.Font.Style);
                        }
                    }
                }
                else
                {
                    this.Scale(new SizeF(dpiScaleFactorFudged * 0.44f, dpiScaleFactorFudged * 0.38f));
                    this.Width = Convert.ToInt32(this.Width * dpiScaleFactorFudged * 0.6f);
                    foreach (Control control in HighDPIUtils.GetChildInControl(this))
                    {
                        control.Font = new Font(control.Font.FontFamily,
                                          control.Font.Size * dpiScaleFactorFudged * HighDPIUtils.PrimaryFudgeFactor,
                                          control.Font.Style);
                    }
                }
            }

            this.KeyPreview = true;

            switch (_authenticationKind)
            {
                case "Windows":
                    this.Text = "Impersonation Credentials";
                    break;

                default:
                    this.Text = "Database Username & Password";
                    break;
            }

            txtConnectionName.Text = _connectionName;
            txtUsername.Text = _username;
            txtPassword.Text = _password;
            cboPrivacyLevel.Text = "None";

            if (_privacyLevel == "NA")
            { //Fudge for provider data sources
                lblPrivacyLevel.Enabled = false;
                cboPrivacyLevel.Enabled = false;
            }

            this.ActiveControl = txtPassword;
        }

        private void ImpersonationCredentials_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                //User Cancelling, so do nothing
                return;
            }

            if (_authenticationKind == "Windows")
            {
                //Validate username/password in domain and cancel closing if invalid
                bool valid = false;

                //Try domain first, then machine, then app directory
                try
                {
                    using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                    {
                        if (context.ValidateCredentials(txtUsername.Text, txtPassword.Text))
                        {
                            valid = true;
                        }
                    }
                }
                catch
                {
                    try
                    {
                        using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
                        {
                            if (context.ValidateCredentials(txtUsername.Text, txtPassword.Text))
                            {
                                valid = true;
                            }
                        }
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        //FileNotFoundException on some machines because missing registry key, so ignore. http://stackoverflow.com/questions/34971400/c-sharp-cannot-use-principalcontextcontexttype-machine-on-windows-10-the-sy
                        valid = true;
                    }
                    catch
                    {
                        try
                        {
                            using (PrincipalContext context = new PrincipalContext(ContextType.ApplicationDirectory))
                            {
                                if (context.ValidateCredentials(txtUsername.Text, txtPassword.Text))
                                {
                                    valid = true;
                                }
                            }
                        }
                        catch
                        {
                            //If here, was unable validate, so might still be good credentials (especially if unexpected exception like FileNotFoundException above)
                            valid = true;
                        }
                    }
                }

                if (!valid)
                {
                    MessageBox.Show("Username or password is invalid.", "BISM Normalizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }
            }

            _username = txtUsername.Text;
            _password = txtPassword.Text;
            _privacyLevel = cboPrivacyLevel.Text;
        }
    }
}
