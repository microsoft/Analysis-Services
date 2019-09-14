using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.AnalysisServices;
using System.Drawing;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class ConnectionsAlmt : Form
    {
        private ComparisonInfo _comparisonInfo;
        private float _dpiScaleFactor;
        private bool _sourceDatabaseBound = false;
        private bool _targetDatabaseBound = false;

        public ConnectionsAlmt()
        {
            InitializeComponent();
        }

        private void Connections_Load(object sender, EventArgs e)
        {
            this.Width = Convert.ToInt32(this.Width * 1.3);

            if (_dpiScaleFactor > 1)
            {
                //DPI
                float dpiScaleFactorFudged = _dpiScaleFactor * HighDPIUtils.PrimaryFudgeFactor;
                float fudgeFactorWidth = 0.95f;

                this.Scale(new SizeF(dpiScaleFactorFudged * (_dpiScaleFactor > 1.7 ? 1 : HighDPIUtils.SecondaryFudgeFactor), dpiScaleFactorFudged * HighDPIUtils.SecondaryFudgeFactor));
                this.Width = Convert.ToInt32(this.Width * dpiScaleFactorFudged * fudgeFactorWidth);
                foreach (Control control in HighDPIUtils.GetChildInControl(this)) //.OfType<Button>())
                {
                    if (control is GroupBox || control is Button)
                    {
                        control.Font = new Font(control.Font.FontFamily,
                                          control.Font.Size * dpiScaleFactorFudged * HighDPIUtils.SecondaryFudgeFactor,
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

            bool boundTargetDatabase = false;
            BindSourceConnectionInfo();
            BindTargetConnectionInfo(out boundTargetDatabase);
        }

        private bool BindSourceConnectionInfo()
        {
            bool returnVal = false;

            if (_comparisonInfo?.ConnectionInfoSource != null)
            {
                if (!String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoSource.ServerName) && !String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoSource.DatabaseName))
                {
                    cboSourceServer.Text = _comparisonInfo.ConnectionInfoSource.ServerName;
                    cboSourceDatabase.Text = _comparisonInfo.ConnectionInfoSource.DatabaseName;

                    returnVal = true;
                }
            }

            return returnVal;
        }

        private bool BindTargetConnectionInfo(out bool boundTargetDatabase)
        {
            bool returnVal = false;
            boundTargetDatabase = false;

            if (_comparisonInfo?.ConnectionInfoTarget != null)
            {
                if (!String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoTarget.ServerName) && !String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoTarget.DatabaseName))
                {
                    cboTargetServer.Text = _comparisonInfo.ConnectionInfoTarget.ServerName;
                    cboTargetDatabase.Text = _comparisonInfo.ConnectionInfoTarget.DatabaseName;

                    boundTargetDatabase = true;
                    returnVal = true;
                }
            }

            return returnVal;
        }

        private static void IterateProject(SortedList projects, EnvDTE.Project project, string derivedProjectName = "")
        {
            if (project.ProjectItems != null)  //if project is unloaded, its ProjectItems==null
            {
                derivedProjectName = AppendProjectName(project, derivedProjectName);
                if (project.FileName.EndsWith(".smproj"))
                {
                    projects.Add(derivedProjectName, project);
                }
                else if (project.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}")
                {
                    foreach (EnvDTE.ProjectItem projectItem in project.ProjectItems)
                    {
                        if (projectItem.SubProject != null)
                        {
                            IterateProject(projects, projectItem.SubProject, derivedProjectName);
                        }
                    }
                }
            }
        }

        private static string AppendProjectName(EnvDTE.Project project, string derivedProjectName)
        {
            if (derivedProjectName != "")
            {
                derivedProjectName += "\\";
            }
            derivedProjectName += project.Name;

            return derivedProjectName;
        }

        EnvDTE80.DTE2 _dte; //EnvDTE._DTE _dte;
        public EnvDTE80.DTE2 Dte // EnvDTE._DTE DTE
        {
            get { return _dte; }
            set { _dte = value; }
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            _comparisonInfo.ConnectionInfoSource.UseProject = false;
            _comparisonInfo.ConnectionInfoSource.ServerName = cboSourceServer.Text;
            _comparisonInfo.ConnectionInfoSource.DatabaseName = cboSourceDatabase.Text;
            _comparisonInfo.ConnectionInfoSource.ProjectName = null;
            _comparisonInfo.ConnectionInfoSource.ProjectFile = null;

            _comparisonInfo.ConnectionInfoTarget.UseProject = false;
            _comparisonInfo.ConnectionInfoTarget.ServerName = cboTargetServer.Text;
            _comparisonInfo.ConnectionInfoTarget.DatabaseName = cboTargetDatabase.Text;
            _comparisonInfo.ConnectionInfoTarget.ProjectName = null;
            _comparisonInfo.ConnectionInfoTarget.ProjectFile = null;
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            ConnectionInfo infoSourceTemp = new ConnectionInfo();

            infoSourceTemp.ServerName = cboSourceServer.Text;
            infoSourceTemp.DatabaseName = cboSourceDatabase.Text;

            cboSourceServer.Text = cboTargetServer.Text;
            cboSourceDatabase.Text = cboTargetDatabase.Text;

            cboTargetServer.Text = infoSourceTemp.ServerName;
            cboTargetDatabase.Text = infoSourceTemp.DatabaseName;
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

        private void BindDatabaseList(string serverName, ComboBox cboCatalog)
        {
            try
            {
                // bind to databases from server
                string currentDb = cboCatalog.Text;

                // discover databases
                Server server = new Server();
                server.Connect($"Provider=MSOLAP;Data Source={serverName};");
                List<string> databases = new List<string>();
                foreach (Database database in server.Databases)
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
