using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.AnalysisServices;
using System.Drawing;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class Connections : Form
    {
        private ComparisonInfo _comparisonInfo;
        private float _dpiScaleFactor;
        private bool _sourceDatabaseBound = false;
        private bool _targetDatabaseBound = false;
        private bool _SetTargetDatabaseFromSourceProjectConfigurationAllowed = false;
        private SortedList _projects;

        public Connections()
        {
            InitializeComponent();
        }

        private void Connections_Load(object sender, EventArgs e)
        {
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

            cboSourceProject.Items.Clear();
            cboTargetProject.Items.Clear();

            BindingSource projectsBindingSource = new BindingSource();
            BindingSource projectsBindingTarget = new BindingSource();
            _projects = new SortedList();

            if (_dte != null)
            {
                foreach (EnvDTE.Project project in _dte.Solution)
                {
                    IterateProject(_projects, project);
                }
            }

            if (_projects.Count == 0)
            {
                rdoSourceProject.Enabled = false;
                rdoTargetProject.Enabled = false;

                pnlSourceProject.Enabled = false;
                pnlTargetProject.Enabled = false;

                rdoSourceDb.Checked = true;
                rdoTargetDb.Checked = true;
            }
            else
            {
                rdoSourceProject.Enabled = true;
                rdoTargetProject.Enabled = true;

                projectsBindingSource.DataSource = _projects;
                projectsBindingTarget.DataSource = _projects;

                cboSourceProject.DataSource = projectsBindingSource;
                cboSourceProject.ValueMember = "Value";
                cboSourceProject.DisplayMember = "Key";

                cboTargetProject.DataSource = projectsBindingTarget;
                cboTargetProject.ValueMember = "Value";
                cboTargetProject.DisplayMember = "Key";

                bool boundTargetDatabase = false;
                if (!(BindSourceConnectionInfo() && BindTargetConnectionInfo(out boundTargetDatabase)))
                {
                    // Either new comparison with no existing connection info, or loaded from file but project not present

                    if (_projects.Count == 1)
                    {
                        //if only one project, default to only the source project dropdown active
                        rdoSourceProject.Checked = true;
                        pnlSourceProject.Enabled = true;

                        rdoTargetDb.Checked = true;
                        pnlTargetProject.Enabled = false;
                    }
                    else
                    {
                        //if more than one project, default to both project dropdowns active
                        rdoSourceProject.Checked = true;
                        pnlSourceProject.Enabled = true;

                        rdoTargetProject.Checked = true;
                        pnlTargetProject.Enabled = true;

                        cboTargetProject.SelectedIndex = 1;
                    }
                }

                // We know there is at least 1 project, so unless bound target db from BSMN file, set target from source project configuration
                if (!boundTargetDatabase)
                {
                    SetTargetDatabaseFromSourceProjectConfiguration();
                }
            }

            _SetTargetDatabaseFromSourceProjectConfigurationAllowed = true;
        }

        private void SetTargetDatabaseFromSourceProjectConfiguration()
        {
            ConnectionInfo connectionInfoTemp = new ConnectionInfo();
            
            connectionInfoTemp.UseProject = true;
            connectionInfoTemp.Project = (EnvDTE.Project)cboSourceProject.SelectedValue;
            connectionInfoTemp.ProjectName = cboSourceProject.Text;
            connectionInfoTemp.ProjectFile = ((EnvDTE.Project)cboSourceProject.SelectedValue).FullName;
            connectionInfoTemp.ReadProjectFile();
            
            if (!String.IsNullOrEmpty(connectionInfoTemp.DeploymentServerName))
            {
                cboTargetServer.Text = connectionInfoTemp.DeploymentServerName;
            }
            if (!String.IsNullOrEmpty(connectionInfoTemp.DeploymentServerDatabase))
            {
                cboTargetDatabase.Text = connectionInfoTemp.DeploymentServerDatabase;
            }
        }

        private bool BindSourceConnectionInfo()
        {
            bool returnVal = false;

            if (_comparisonInfo?.ConnectionInfoSource != null)
            {
                if (_comparisonInfo.ConnectionInfoSource.UseProject)
                {
                    if (_projects.ContainsKey(_comparisonInfo.ConnectionInfoSource.ProjectName))
                    {
                        rdoSourceProject.Checked = true;
                        pnlSourceProject.Enabled = true;

                        for (int i = 0; i < ((BindingSource)cboSourceProject.DataSource).Count; i++)
                        {
                            if (Convert.ToString(((DictionaryEntry)((BindingSource)cboSourceProject.DataSource)[i]).Key) == _comparisonInfo.ConnectionInfoSource.ProjectName)
                            {
                                cboSourceProject.SelectedIndex = i;
                                break;
                            }
                        }

                        returnVal = true;
                    }
                }
                else if (!String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoSource.ServerName) && !String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoSource.DatabaseName))
                {
                    rdoSourceDb.Checked = true;
                    pnlSourceProject.Enabled = false;

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
                if (_comparisonInfo.ConnectionInfoTarget.UseProject)
                {
                    if (_projects.ContainsKey(_comparisonInfo.ConnectionInfoTarget.ProjectName))
                    {
                        rdoTargetProject.Checked = true;
                        pnlTargetProject.Enabled = true;

                        for (int i = 0; i < ((BindingSource)cboTargetProject.DataSource).Count; i++)
                        {
                            if (Convert.ToString(((DictionaryEntry)((BindingSource)cboTargetProject.DataSource)[i]).Key) == _comparisonInfo.ConnectionInfoTarget.ProjectName)
                            {
                                cboTargetProject.SelectedIndex = i;
                                break;
                            }
                        }

                        returnVal = true;
                    }
                }
                else if (!String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoTarget.ServerName) && !String.IsNullOrEmpty(_comparisonInfo.ConnectionInfoTarget.DatabaseName))
                {
                    rdoTargetDb.Checked = true;
                    pnlTargetProject.Enabled = false;

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
                else if (project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
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

        private void rdoSourceProject_CheckedChanged(object sender, EventArgs e)
        {
            pnlSourceProject.Enabled = true;
            pnlSourceDb.Enabled = false;
            cboSourceProject.Focus();
        }
        private void rdoSourceDb_CheckedChanged(object sender, EventArgs e)
        {
            pnlSourceProject.Enabled = false;
            pnlSourceDb.Enabled = true;
            cboSourceServer.Focus();
        }
        private void rdoTargetProject_CheckedChanged(object sender, EventArgs e)
        {
            pnlTargetProject.Enabled = true;
            pnlTargetDb.Enabled = false;
            cboTargetProject.Focus();
        }
        private void rdoTargetDb_CheckedChanged(object sender, EventArgs e)
        {
            pnlTargetProject.Enabled = false;
            pnlTargetDb.Enabled = true;
            cboTargetServer.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rdoSourceProject.Checked)
            {
                _comparisonInfo.ConnectionInfoSource.UseProject = true;
                _comparisonInfo.ConnectionInfoSource.Project = (EnvDTE.Project)cboSourceProject.SelectedValue;
                _comparisonInfo.ConnectionInfoSource.ProjectName = cboSourceProject.Text;
                _comparisonInfo.ConnectionInfoSource.ProjectFile = ((EnvDTE.Project)cboSourceProject.SelectedValue).FullName;
            }
            else
            {
                _comparisonInfo.ConnectionInfoSource.UseProject = false;
                _comparisonInfo.ConnectionInfoSource.ServerName = cboSourceServer.Text;
                _comparisonInfo.ConnectionInfoSource.DatabaseName = cboSourceDatabase.Text;
                _comparisonInfo.ConnectionInfoSource.ProjectName = null;
                _comparisonInfo.ConnectionInfoSource.ProjectFile = null;
            }

            if (rdoTargetProject.Checked)
            {
                _comparisonInfo.ConnectionInfoTarget.UseProject = true;
                _comparisonInfo.ConnectionInfoTarget.Project = (EnvDTE.Project)cboTargetProject.SelectedValue;
                _comparisonInfo.ConnectionInfoTarget.ProjectName = cboTargetProject.Text;
                _comparisonInfo.ConnectionInfoTarget.ProjectFile = ((EnvDTE.Project)cboTargetProject.SelectedValue).FullName;
            }
            else
            {
                _comparisonInfo.ConnectionInfoTarget.UseProject = false;
                _comparisonInfo.ConnectionInfoTarget.ServerName = cboTargetServer.Text;
                _comparisonInfo.ConnectionInfoTarget.DatabaseName = cboTargetDatabase.Text;
                _comparisonInfo.ConnectionInfoTarget.ProjectName = null;
                _comparisonInfo.ConnectionInfoTarget.ProjectFile = null;
            }
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            _SetTargetDatabaseFromSourceProjectConfigurationAllowed = false;

            ConnectionInfo infoSourceTemp = new ConnectionInfo();
            infoSourceTemp.UseProject = rdoSourceProject.Checked;
            infoSourceTemp.ProjectName = cboSourceProject.Text;
            infoSourceTemp.Project = (EnvDTE.Project)cboSourceProject.SelectedValue;
            infoSourceTemp.ServerName = cboSourceServer.Text;
            infoSourceTemp.DatabaseName = cboSourceDatabase.Text;

            rdoSourceProject.Checked = rdoTargetProject.Checked;
            rdoSourceDb.Checked = rdoTargetDb.Checked;
            cboSourceProject.Text = cboTargetProject.Text;
            cboSourceProject.SelectedValue = cboTargetProject.SelectedValue;
            cboSourceServer.Text = cboTargetServer.Text;
            cboSourceDatabase.Text = cboTargetDatabase.Text;

            rdoTargetProject.Checked = infoSourceTemp.UseProject;
            rdoTargetDb.Checked = !infoSourceTemp.UseProject;
            cboTargetProject.Text = infoSourceTemp.ProjectName;
            cboTargetProject.SelectedValue = infoSourceTemp.Project;
            cboTargetServer.Text = infoSourceTemp.ServerName;
            cboTargetDatabase.Text = infoSourceTemp.DatabaseName;

            _SetTargetDatabaseFromSourceProjectConfigurationAllowed = true;
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

        private void cboSourceProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_SetTargetDatabaseFromSourceProjectConfigurationAllowed)
            {
                SetTargetDatabaseFromSourceProjectConfiguration();
            }
        }

        private void BindDatabaseList(string serverName, ComboBox cboCatalog)
        {
            try
            {
                // bind to databases from source server
                string currentDb = cboCatalog.Text;

                Server server = new Server();
                server.Connect("Provider=MSOLAP;Data Source=" + serverName);
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
