using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Tom=Microsoft.AnalysisServices.Tabular;
using System.Drawing;
using BismNormalizer.TabularCompare.UI.DesktopInstances;
using System.Linq.Expressions;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class ConnectionsAlmt : Form
    {
        #region Private members

        private ComparisonInfo _comparisonInfo;
        private float _dpiScaleFactor;
        private bool _sourceDatabaseBound = false;
        private bool _targetDatabaseBound = false;
        private List<PowerBIInstance> _powerBIInstances = new List<PowerBIInstance>();

        #endregion

        public ConnectionsAlmt()
        {
            InitializeComponent();
        }

        private void Connections_Load(object sender, EventArgs e)
        {
            //Settings.Default.SourceServerAutoCompleteEntries = "localhost|";
            //Settings.Default.TargetServerAutoCompleteEntries = "localhost|";
            //Settings.Default.SourceCatalog = "";
            //Settings.Default.TargetCatalog = "";
            //Settings.Default.Save();


            //this.Width = Convert.ToInt32(this.Width * 1.3);
            this.Height = Convert.ToInt32(grpSource.Height * 2.6);

            if (_dpiScaleFactor > 1)
            {
                //DPI
                float dpiScaleFactorFudged = _dpiScaleFactor * Utils.PrimaryFudgeFactor;
                float fudgeFactorWidth = 0.95f;

                this.Scale(new SizeF(dpiScaleFactorFudged * (_dpiScaleFactor > 1.7 ? 1 : Utils.SecondaryFudgeFactor), dpiScaleFactorFudged * Utils.SecondaryFudgeFactor));
                this.Width = Convert.ToInt32(this.Width * dpiScaleFactorFudged * fudgeFactorWidth);
                foreach (Control control in Utils.GetChildInControl(this)) //.OfType<Button>())
                {
                    if (control is GroupBox || control is Button)
                    {
                        control.Font = new Font(control.Font.FontFamily,
                                          control.Font.Size * dpiScaleFactorFudged * Utils.SecondaryFudgeFactor,
                                          control.Font.Style);
                    }
                    if (control is GroupBox || control.Name == "btnSwitch")
                    {
                        control.Width = Convert.ToInt32(control.Width * dpiScaleFactorFudged * fudgeFactorWidth);
                    }
                    if (control is ComboBox)
                    {
                        control.Width = Convert.ToInt32(control.Width * fudgeFactorWidth);
                    }
                    if (control is Panel)
                    {
                        control.Left = Convert.ToInt32(control.Left * dpiScaleFactorFudged);
                    }
                }
                this.btnSwitch.Left = grpSource.Right + Convert.ToInt32(12 * dpiScaleFactorFudged);
                this.grpTarget.Left = btnSwitch.Right + Convert.ToInt32(12 * dpiScaleFactorFudged);
            }

            cboSourceServer.DataSource = ComparisonControl.ReverseArray<string>(Settings.Default.SourceServerAutoCompleteEntries.Substring(0, Settings.Default.SourceServerAutoCompleteEntries.Length - 1).Split("|".ToCharArray()));
            cboTargetServer.DataSource = ComparisonControl.ReverseArray<string>(Settings.Default.TargetServerAutoCompleteEntries.Substring(0, Settings.Default.TargetServerAutoCompleteEntries.Length - 1).Split("|".ToCharArray()));

            cboSourceDatabase.Text = Settings.Default.SourceCatalog;
            cboTargetDatabase.Text = Settings.Default.TargetCatalog;

            #region Prep Desktop/SSDT instances

            cboSourceDesktop.Items.Clear();
            cboTargetDesktop.Items.Clear();

            BindingSource desktopBindingSource = new BindingSource();
            BindingSource desktopBindingTarget = new BindingSource();
            _powerBIInstances.Clear();
            try
            {
                _powerBIInstances = PowerBIHelper.GetLocalInstances(includePBIRS:false);
            }
            catch { }

            if (_powerBIInstances.Count > 0)
            {
                rdoSourceDesktop.Enabled = true;
                rdoTargetDesktop.Enabled = true;

                desktopBindingSource.DataSource = _powerBIInstances;
                desktopBindingTarget.DataSource = _powerBIInstances;

                cboSourceDesktop.DataSource = desktopBindingSource;
                cboSourceDesktop.ValueMember = "Port";
                cboSourceDesktop.DisplayMember = "Name";

                cboTargetDesktop.DataSource = desktopBindingTarget;
                cboTargetDesktop.ValueMember = "Port";
                cboTargetDesktop.DisplayMember = "Name";
            }
            else
            {
                rdoSourceDesktop.Enabled = false;
                rdoTargetDesktop.Enabled = false;
            }

            #endregion

            BindSourceConnectionInfo();
            BindTargetConnectionInfo();
        }

        private bool BindSourceConnectionInfo()
        {
            bool boundSuccessfully = false;

            if (_comparisonInfo?.ConnectionInfoSource != null)
            {
                if (_comparisonInfo.ConnectionInfoSource.UseBimFile)
                {
                    rdoSourceFile.Checked = true;

                    pnlSourceDataset.Enabled = false;
                    pnlSourceDesktop.Enabled = false;
                    pnlSourceFile.Enabled = true;

                    txtSourceFile.Text = _comparisonInfo.ConnectionInfoSource.BimFile;

                    boundSuccessfully = true;
                }
                else if (_comparisonInfo.ConnectionInfoSource.UseDesktop)
                {
                    if (_powerBIInstances.Count > 0)
                    {
                        rdoSourceDesktop.Checked = true;

                        pnlSourceDataset.Enabled = false;
                        pnlSourceDesktop.Enabled = true;
                        pnlSourceFile.Enabled = false;

                        int portFromConnectionInfo = -1;
                        if (_comparisonInfo.ConnectionInfoSource.ServerName != null &&
                            int.TryParse(_comparisonInfo.ConnectionInfoSource.ServerName.ToUpper().Replace("localhost:".ToUpper(), ""), out portFromConnectionInfo))
                        {
                            for (int i = 0; i < ((BindingSource)cboSourceDesktop.DataSource).Count; i++)
                            {
                                if (((PowerBIInstance)((BindingSource)cboSourceDesktop.DataSource)[i]).Port == portFromConnectionInfo)
                                {
                                    cboSourceDesktop.SelectedIndex = i;
                                    boundSuccessfully = true;
                                    break;
                                }
                            }
                        }
                        //For case when open .almt file and want to connect to new server/port for same dataset
                        if (_comparisonInfo.ConnectionInfoSource.ServerName == null && _comparisonInfo.ConnectionInfoSource.DesktopName != null)
                        {
                            for (int i = 0; i < ((BindingSource)cboSourceDesktop.DataSource).Count; i++)
                            {
                                if (((PowerBIInstance)((BindingSource)cboSourceDesktop.DataSource)[i]).Name == _comparisonInfo.ConnectionInfoSource.DesktopName)
                                {
                                    cboSourceDesktop.SelectedIndex = i;
                                    boundSuccessfully = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoSource.ServerName) && !String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoSource.DatabaseName))
                {
                    rdoSourceDataset.Checked = true;

                    pnlSourceDataset.Enabled = true;
                    pnlSourceDesktop.Enabled = false;
                    pnlSourceFile.Enabled = false;

                    cboSourceServer.Text = _comparisonInfo.ConnectionInfoSource.ServerName;
                    cboSourceDatabase.Text = _comparisonInfo.ConnectionInfoSource.DatabaseName;

                    boundSuccessfully = true;
                }
            }

            return boundSuccessfully;
        }

        private bool BindTargetConnectionInfo()
        {
            bool boundSuccessfully = false;

            if (_comparisonInfo?.ConnectionInfoTarget != null)
            {
                if (_comparisonInfo.ConnectionInfoTarget.UseBimFile)
                {
                    pnlTargetDataset.Enabled = false;
                    pnlTargetDesktop.Enabled = false;
                    rdoTargetFile.Checked = true;
                    pnlTargetFile.Enabled = true;
                    txtTargetFile.Text = _comparisonInfo.ConnectionInfoTarget.BimFile;

                    boundSuccessfully = true;
                }
                else if (_comparisonInfo.ConnectionInfoTarget.UseDesktop)
                {
                    if (_powerBIInstances.Count > 0)
                    {

                        rdoTargetDesktop.Checked = true;

                        pnlTargetDataset.Enabled = false;
                        pnlTargetDesktop.Enabled = true;
                        pnlTargetFile.Enabled = false;

                        int portFromConnectionInfo = -1;
                        if (_comparisonInfo.ConnectionInfoTarget.ServerName != null &&
                            int.TryParse(_comparisonInfo.ConnectionInfoTarget.ServerName.ToUpper().Replace("localhost:".ToUpper(), ""), out portFromConnectionInfo))
                        {
                            for (int i = 0; i < ((BindingSource)cboTargetDesktop.DataSource).Count; i++)
                            {
                                if (((PowerBIInstance)((BindingSource)cboTargetDesktop.DataSource)[i]).Port == portFromConnectionInfo)
                                {
                                    cboTargetDesktop.SelectedIndex = i;
                                    boundSuccessfully = true;
                                    break;
                                }
                            }
                        }
                        //For case when open .almt file and want to connect to new server/port for same dataset
                        if (_comparisonInfo.ConnectionInfoTarget.ServerName == null && _comparisonInfo.ConnectionInfoTarget.DesktopName != null)
                        {
                            for (int i = 0; i < ((BindingSource)cboTargetDesktop.DataSource).Count; i++)
                            {
                                if (((PowerBIInstance)((BindingSource)cboTargetDesktop.DataSource)[i]).Name == _comparisonInfo.ConnectionInfoTarget.DesktopName)
                                {
                                    cboTargetDesktop.SelectedIndex = i;
                                    boundSuccessfully = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoTarget.ServerName) && !String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoTarget.DatabaseName))
                {
                    rdoTargetDataset.Checked = true;
                    pnlTargetDataset.Enabled = true;
                    pnlTargetDesktop.Enabled = false;
                    pnlTargetFile.Enabled = false;

                    cboTargetServer.Text = _comparisonInfo.ConnectionInfoTarget.ServerName;
                    cboTargetDatabase.Text = _comparisonInfo.ConnectionInfoTarget.DatabaseName;

                    boundSuccessfully = true;
                }
            }

            return boundSuccessfully;
        }

        public ComparisonInfo ComparisonInfo
        {
            get { return _comparisonInfo; }
            set { _comparisonInfo = value; }
        }

        public float DpiScaleFactor
        {
            get { return _dpiScaleFactor; }
            set { _dpiScaleFactor = value; }
        }

        private void rdoSourceDataset_CheckedChanged(object sender, EventArgs e)
        {
            pnlSourceDataset.Enabled = true;
            pnlSourceDesktop.Enabled = false;
            pnlSourceFile.Enabled = false;
            cboSourceServer.Focus();
        }
        private void rdoSourceDesktop_CheckedChanged(object sender, EventArgs e)
        {
            pnlSourceDataset.Enabled = false;
            pnlSourceDesktop.Enabled = true;
            pnlSourceFile.Enabled = false;
            cboSourceDesktop.Focus();
        }
        private void rdoSourceFile_CheckedChanged(object sender, EventArgs e)
        {
            pnlSourceDataset.Enabled = false;
            pnlSourceDesktop.Enabled = false;
            pnlSourceFile.Enabled = true;
            btnSourceFileOpen.Focus();
        }
        private void rdoTargetDataset_CheckedChanged(object sender, EventArgs e)
        {
            pnlTargetDataset.Enabled = true;
            pnlTargetDesktop.Enabled = false;
            pnlTargetFile.Enabled = false;
            cboTargetServer.Focus();
        }
        private void rdoTargetDesktop_CheckedChanged(object sender, EventArgs e)
        {
            pnlTargetDataset.Enabled = false;
            pnlTargetDesktop.Enabled = true;
            pnlTargetFile.Enabled = false;
            cboTargetDesktop.Focus();
        }
        private void rdoTargetFile_CheckedChanged(object sender, EventArgs e)
        {
            pnlTargetDataset.Enabled = false;
            pnlTargetDesktop.Enabled = false;
            pnlTargetFile.Enabled = true;
            btnTargetFileOpen.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rdoSourceDesktop.Checked)
            {
                _comparisonInfo.ConnectionInfoSource.UseProject = false;
                _comparisonInfo.ConnectionInfoSource.UseBimFile = false;
                _comparisonInfo.ConnectionInfoSource.UseDesktop = true;
                _comparisonInfo.ConnectionInfoSource.ServerName = "localhost:" + cboSourceDesktop.SelectedValue.ToString();
                _comparisonInfo.ConnectionInfoSource.DatabaseName = ""; //For Desktop/SSDT don't have DB name but get first when connect
                _comparisonInfo.ConnectionInfoSource.DesktopName = cboSourceDesktop.Text;
                _comparisonInfo.ConnectionInfoSource.ProjectName = null;
                _comparisonInfo.ConnectionInfoSource.ProjectFile = null;
                _comparisonInfo.ConnectionInfoSource.BimFile = null;
            }
            else if (rdoSourceFile.Checked)
            {
                _comparisonInfo.ConnectionInfoSource.UseProject = false;
                _comparisonInfo.ConnectionInfoSource.UseBimFile = true;
                _comparisonInfo.ConnectionInfoSource.UseDesktop = false;
                _comparisonInfo.ConnectionInfoSource.BimFile = txtSourceFile.Text;
                _comparisonInfo.ConnectionInfoSource.DesktopName = null;
                _comparisonInfo.ConnectionInfoSource.ProjectName = null;
                _comparisonInfo.ConnectionInfoSource.ProjectFile = null;
            }
            else
            {
                _comparisonInfo.ConnectionInfoSource.UseProject = false;
                _comparisonInfo.ConnectionInfoSource.UseBimFile = false;
                _comparisonInfo.ConnectionInfoSource.UseDesktop = false;
                _comparisonInfo.ConnectionInfoSource.ServerName = cboSourceServer.Text;
                _comparisonInfo.ConnectionInfoSource.DatabaseName = cboSourceDatabase.Text;
                _comparisonInfo.ConnectionInfoSource.DesktopName = null;
                _comparisonInfo.ConnectionInfoSource.ProjectName = null;
                _comparisonInfo.ConnectionInfoSource.ProjectFile = null;
                _comparisonInfo.ConnectionInfoSource.BimFile = null;
            }

            if (rdoTargetDesktop.Checked)
            {
                _comparisonInfo.ConnectionInfoTarget.UseProject = false;
                _comparisonInfo.ConnectionInfoTarget.UseBimFile = false;
                _comparisonInfo.ConnectionInfoTarget.UseDesktop = true;
                _comparisonInfo.ConnectionInfoTarget.ServerName = "localhost:" + cboTargetDesktop.SelectedValue.ToString();
                _comparisonInfo.ConnectionInfoTarget.DatabaseName = ""; //For Desktop/SSDT don't have DB name but get first when connect
                _comparisonInfo.ConnectionInfoTarget.DesktopName = cboTargetDesktop.Text;
                _comparisonInfo.ConnectionInfoTarget.ProjectName = null;
                _comparisonInfo.ConnectionInfoTarget.ProjectFile = null;
                _comparisonInfo.ConnectionInfoTarget.BimFile = null;
            }
            else if (rdoTargetFile.Checked)
            {
                _comparisonInfo.ConnectionInfoTarget.UseProject = false;
                _comparisonInfo.ConnectionInfoTarget.UseBimFile = true;
                _comparisonInfo.ConnectionInfoTarget.UseDesktop = false;
                _comparisonInfo.ConnectionInfoTarget.BimFile = txtTargetFile.Text;
                _comparisonInfo.ConnectionInfoTarget.DesktopName = null;
                _comparisonInfo.ConnectionInfoTarget.ProjectName = null;
                _comparisonInfo.ConnectionInfoTarget.ProjectFile = null;
            }
            else
            {
                _comparisonInfo.ConnectionInfoTarget.UseProject = false;
                _comparisonInfo.ConnectionInfoTarget.UseBimFile = false;
                _comparisonInfo.ConnectionInfoTarget.UseDesktop = false;
                _comparisonInfo.ConnectionInfoTarget.ServerName = cboTargetServer.Text;
                _comparisonInfo.ConnectionInfoTarget.DatabaseName = cboTargetDatabase.Text;
                _comparisonInfo.ConnectionInfoTarget.DesktopName = null;
                _comparisonInfo.ConnectionInfoTarget.ProjectName = null;
                _comparisonInfo.ConnectionInfoTarget.ProjectFile = null;
                _comparisonInfo.ConnectionInfoTarget.BimFile = null;
            }
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            ConnectionInfo infoSourceTemp = new ConnectionInfo();
            infoSourceTemp.UseProject = rdoSourceDesktop.Checked;
            infoSourceTemp.UseBimFile = rdoSourceFile.Checked;
            infoSourceTemp.ProjectName = cboSourceDesktop.Text;

            //Fudge start
            //infoSourceTemp.Project = (PowerBIInstance)cboSourceDesktop.SelectedValue;
            int portSourceTemp = -1;
            if (cboSourceDesktop.SelectedValue != null)
            {
                portSourceTemp = (int)cboSourceDesktop.SelectedValue;
            }

            infoSourceTemp.ServerName = cboSourceServer.Text;
            infoSourceTemp.DatabaseName = cboSourceDatabase.Text;
            infoSourceTemp.BimFile = txtSourceFile.Text;

            rdoSourceDesktop.Checked = rdoTargetDesktop.Checked;
            rdoSourceFile.Checked = rdoTargetFile.Checked;
            rdoSourceDataset.Checked = rdoTargetDataset.Checked;
            cboSourceDesktop.Text = cboTargetDesktop.Text;
            cboSourceDesktop.SelectedValue = cboTargetDesktop.SelectedValue;
            cboSourceServer.Text = cboTargetServer.Text;
            cboSourceDatabase.Text = cboTargetDatabase.Text;
            txtSourceFile.Text = txtTargetFile.Text;

            rdoTargetDesktop.Checked = infoSourceTemp.UseProject;
            rdoTargetFile.Checked = infoSourceTemp.UseBimFile;
            rdoTargetDataset.Checked = (!infoSourceTemp.UseProject && !infoSourceTemp.UseBimFile);
            cboTargetDesktop.Text = infoSourceTemp.ProjectName;

            //cboTargetDesktop.SelectedValue = infoSourceTemp.Project;
            if (portSourceTemp != -1)
            { 
                cboTargetDesktop.SelectedValue = portSourceTemp;
            }
            //Fudge end

            cboTargetServer.Text = infoSourceTemp.ServerName;
            cboTargetDatabase.Text = infoSourceTemp.DatabaseName;
            txtTargetFile.Text = infoSourceTemp.BimFile;
        }

        private void cboSourceServer_TextChanged(object sender, EventArgs e)
        {
            _sourceDatabaseBound = false;
        }

        private void cboTargetServer_TextChanged(object sender, EventArgs e)
        {
            _targetDatabaseBound = false;
        }

        private void cboSourceDatabase_Enter(object sender, EventArgs e)
        {
            if (!_sourceDatabaseBound && cboSourceServer.Text != "")
            {
                BindDatabaseList(cboSourceServer.Text, cboSourceDatabase);
                _sourceDatabaseBound = true;
            }
        }

        private void cboTargetDatabase_Enter(object sender, EventArgs e)
        {
            if (!_targetDatabaseBound && cboTargetServer.Text != "")
            {
                BindDatabaseList(cboTargetServer.Text, cboTargetDatabase);
                _targetDatabaseBound = true;
            }
        }

        private void btnSourceFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = OpenBimFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtSourceFile.Text = ofd.FileName;
                Settings.Default.LastBimFileLocation = ofd.FileName;
            }
        }

        private void btnTargetFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = OpenBimFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtTargetFile.Text = ofd.FileName;
                Settings.Default.LastBimFileLocation = ofd.FileName;
            }
        }

        private OpenFileDialog OpenBimFileDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = (String.IsNullOrEmpty(Settings.Default.LastBimFileLocation) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Settings.Default.LastBimFileLocation);
            ofd.Filter = "PBIT/BIM files (*.pbit;*.bim)|*.pbit;*.bim|All files (*.*)|*.*";
            ofd.Title = "Open";
            return ofd;
        }

        private void BindDatabaseList(string serverName, ComboBox cboCatalog)
        {
            try
            {
                // bind to databases from server
                string currentDb = cboCatalog.Text;

                // discover databases
                Tom.Server server = new Tom.Server();
                server.Connect($"Provider=MSOLAP;Data Source={serverName};");
                List<string> databases = new List<string>();
                foreach (Tom.Database database in server.Databases)
                {
                    databases.Add(database.Name);
                }
                databases.Sort();

                cboCatalog.DataSource = databases;
                cboCatalog.Text = currentDb;
            }
            catch (Exception)
            { // if user entered duff server name, just ignore
                cboCatalog.DataSource = null;
            }
        }

    }
}
