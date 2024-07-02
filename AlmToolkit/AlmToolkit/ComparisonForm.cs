using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using BismNormalizer;
using BismNormalizer.TabularCompare;
using BismNormalizer.TabularCompare.Core;
using BismNormalizer.TabularCompare.UI;
using CefSharp;
using CefSharp.WinForms;
using Microsoft.AnalysisServices.Tabular;

namespace AlmToolkit
{
    public partial class ComparisonForm : Form
    {
        #region Private Members

        private ComparisonInfo _comparisonInfo;
        private Comparison _comparison;
        private ComparisonJSInteraction _comparisonInter; // CEFSharp Interface to connect to Angular Tree Control
        private ChromiumWebBrowser chromeBrowser;
        private CompareState _compareState = CompareState.NotCompared;
        private string _fileName = null;
        private bool _unsaved = false;
        private bool _newerVersionAvailable = false;
        private string _latestVersion = null;

        #endregion

        #region Methods

        public ComparisonForm()
        {
            InitializeComponent();
            InitializeChromium();
        }

        private void InitializeChromium()
        {
            try
            {
                // Check if the page exists
                string page = string.Format(@"{0}\html-resources\dist\index.html", Application.StartupPath);
                if (!File.Exists(page))
                {
                    MessageBox.Show("Error html file doesn't exist : " + page, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                CefSettings settings = new CefSettings();
                // Initialize cef with the provided settings
                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                settings.CefCommandLineArgs.Add("disable-web-security", "1"); // to prevent CORS error in runtime.js, polyfills.js and main.js
                settings.CefCommandLineArgs.Add("allow-file-access-from-files", "1"); //  to allow access to local files

                string relativePath = @"x86\CefSharp.BrowserSubprocess.exe";
                string absolutePath = Path.GetFullPath(relativePath);
                settings.BrowserSubprocessPath = absolutePath;

                Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

                // Create a browser component
                chromeBrowser = new ChromiumWebBrowser(page);

                // Add it to the form and fill it to the form window.
                this.Controls.Add(chromeBrowser);
                chromeBrowser.Dock = DockStyle.Fill;
                chromeBrowser.BringToFront();


                // Initialize the interaction variable
                _comparisonInter = new ComparisonJSInteraction(this);

                // Register C# objects
                chromeBrowser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;

                chromeBrowser.JavascriptObjectRepository.Register("chromeDebugger", new ChromeDebugger(chromeBrowser, this), isAsync: true, options: BindingOptions.DefaultBinder);
                chromeBrowser.JavascriptObjectRepository.Register("comparisonJSInteraction", _comparisonInter, isAsync: true, options: BindingOptions.DefaultBinder);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("FileNotFoundException in InitializeChromium().\n\nPlease try to install C++ Redistributable Packages for Visual Studio x86, min version www.microsoft.com/download/details.aspx?id=40784", Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComparisonForm_Load(object sender, EventArgs e)
        {
            if (_comparisonInfo == null)
            {
                _comparisonInfo = new ComparisonInfo();
                _comparisonInfo.AppName = Utils.AssemblyProduct;

                //GetFromAutoCompleteSource();
                //GetFromAutoCompleteTarget();

                SetNotComparedState();
            }

            Task.Run(() => CheckForNewVersion());

            //hdpi
            Rescale();
        }

        private void ComparisonForm_Shown(object sender, EventArgs e)
        {
            this.InitializeAndCompareTabularModels();
        }

        private async void CheckForNewVersion()
        {
            try
            {
                var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Microsoft"));
                var releases = await client.Repository.Release.GetAll("Microsoft", "Analysis-Services");

                var result = 0;
                for (int i = 0; i < releases.Count - 1; i++)
                {
                    if (!releases[i].Prerelease && releases[i].Name.StartsWith("ALM Toolkit"))
                    {
                        var latest = releases[i];
                        _latestVersion = latest.TagName;
                        var installedVersion = new Version(Utils.AssemblyVersion);
                        var latestVersion = new Version(_latestVersion);
                        result = latestVersion.CompareTo(installedVersion);
                        break;
                    }
                }

                if (result > 0)
                {
                    //There is a newer release on GitHub
                    _newerVersionAvailable = true;
                    NewVersionLink.Text = $"New version available: {_latestVersion}";
                    NewVersionLink.Visible = true;
                }
            }
            catch { }
        }

        private void NewVersionLink_Click(object sender, EventArgs e)
        {
            NewVersionLink.LinkVisited = true;
            System.Diagnostics.Process.Start(Utils.LatestVersionDownloadUrl);
        }

        private void SetNotComparedState()
        {
            if (_comparison != null)
            {
                _comparison.Disconnect();
            }

            btnCompareTabularModels.Enabled = true;
            ddSelectActions.Enabled = false;
            mnuHideSkipObjects.Enabled = false;
            mnuShowSkipObjects.Enabled = false;
            mnuSkipAllObjectsMissingInSource.Enabled = false;
            mnuDeleteAllObjectsMissingInSource.Enabled = false;
            mnuSkipAllObjectsMissingInTarget.Enabled = false;
            mnuCreateAllObjectsMissingInTarget.Enabled = false;
            mnuSkipAllObjectsWithDifferentDefinitions.Enabled = false;
            mnuUpdateAllObjectsWithDifferentDefinitions.Enabled = false;
            btnValidateSelection.Enabled = false;
            btnUpdate.Enabled = false;
            btnGenerateScript.Enabled = false;
            btnReportDifferences.Enabled = false;
            toolStripStatusLabel1.Text = "";

            //ComparisonCtrl.SetNotComparedState();

            _compareState = CompareState.NotCompared;
            SetGridState(false);
        }

        private void SetComparedState()
        {
            btnCompareTabularModels.Enabled = true;
            ddSelectActions.Enabled = true;
            mnuHideSkipObjects.Enabled = true;
            mnuShowSkipObjects.Enabled = true;
            mnuSkipAllObjectsMissingInSource.Enabled = true;
            mnuDeleteAllObjectsMissingInSource.Enabled = true;
            mnuSkipAllObjectsMissingInTarget.Enabled = true;
            mnuCreateAllObjectsMissingInTarget.Enabled = true;
            mnuSkipAllObjectsWithDifferentDefinitions.Enabled = true;
            mnuUpdateAllObjectsWithDifferentDefinitions.Enabled = true;
            btnValidateSelection.Enabled = true;
            btnUpdate.Enabled = false;
            btnGenerateScript.Enabled = false;
            btnReportDifferences.Enabled = true;

            //ComparisonCtrl.SetComparedState();

            // NG: Disable skip and other actions for the control here
            _compareState = CompareState.Compared;

            SetGridState(true);
        }

        private void SetValidatedState()
        {
            btnUpdate.Enabled = true;
            btnGenerateScript.Enabled = true;

            _compareState = CompareState.Validated;
        }

        private bool ShowConnectionsForm()
        {
            if (_compareState != CompareState.NotCompared)
            {
                //just in case user has some selections, store them to the SkipSelections collection
                _comparison.RefreshSkipSelectionsFromComparisonObjects();
            }

            ConnectionsAlmt connForm = new ConnectionsAlmt();
            connForm.ComparisonInfo = _comparisonInfo;
            connForm.StartPosition = FormStartPosition.CenterParent;
            connForm.DpiScaleFactor = _dpiScaleFactor;
            connForm.ShowDialog();
            if (connForm.DialogResult == DialogResult.OK)
            {
                SetNotComparedState();
                return true;
            }
            else return false;
        }

        public void InitializeAndCompareTabularModelsNg()
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    InitializeAndCompareTabularModels();
                }));
            }
        }

        public void InitializeAndCompareTabularModels()
        {

            try
            {
                string sourceTemp = txtSource.Text;
                string targetTemp = txtTarget.Text;
                string sourceDesktopNameTemp = _comparisonInfo.ConnectionInfoSource.DesktopName;
                string targetDesktopNameTemp = _comparisonInfo.ConnectionInfoTarget.DesktopName;

                if (!ShowConnectionsForm()) return;

                Cursor = Cursors.WaitCursor;
                changeCursor(true);
                toolStripStatusLabel1.Text = "ALM Toolkit - comparing datasets ...";

                PopulateSourceTargetTextBoxes();
                if (
                        (_comparisonInfo.ConnectionInfoSource.UseDesktop && sourceDesktopNameTemp != _comparisonInfo.ConnectionInfoSource.DesktopName) ||
                        (!_comparisonInfo.ConnectionInfoSource.UseDesktop && sourceTemp != txtSource.Text) ||
                        (_comparisonInfo.ConnectionInfoTarget.UseDesktop && targetDesktopNameTemp != _comparisonInfo.ConnectionInfoTarget.DesktopName) ||
                        (!_comparisonInfo.ConnectionInfoTarget.UseDesktop && targetTemp != txtTarget.Text)
                   )
                {
                    // New connections
                    //ComparisonCtrl.TriggerComparisonChanged();
                    _comparisonInfo.SkipSelections.Clear();
                    SetFileNameTitle(true);
                }

                this.CompareTabularModels();
                toolStripStatusLabel1.Text = "ALM Toolkit - finished comparing datasets";
            }
            catch (TomInternalException)
            {
                //todo: delete extra info once Oren's fix
                MessageBox.Show("TOM internal serialization error occurred.", Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
            finally
            {
                Cursor = Cursors.Default;
                changeCursor(false);
            }

        }

        public void CompareTabularModels()
        {
            bool userCancelled;
            _comparison = ComparisonFactory.CreateComparison(_comparisonInfo, out userCancelled);

            if (!userCancelled)
            {
                //_comparison.ValidationMessage += HandleValidationMessage;
                //_comparison.ResizeValidationHeaders += HandleResizeValidationHeaders;
                _comparison.DatabaseDeployment += HandleDatabaseDeployment;
                _comparison.Connect();
                SetAutoComplete();
                _comparison.CompareTabularModels();

                // Avoid conflict for validate with existing control
                //ComparisonCtrl.ComparisonChanged += HandleComparisonChanged;
                //ComparisonCtrl.Comparison = _comparison;
                //ComparisonCtrl.DataBindComparison();

                _comparisonInter.Comparison = _comparison;
                transformAndRefreshGridControl();

                SetComparedState();
            }
        }

        #region Angular tree control handlers
        private void transformAndRefreshGridControl()
        {
            _comparisonInter.SetComparisonData();
            // Send notification to refresh the grid
            refreshGridControl(false);
        }

        /// <summary>
        /// Send notification to refresh the grid control on UI
        /// </summary>
        public void refreshGridControl(bool mergeActions)
        {
            // Invoke method in Angular
            string script = "window.angularComponentRef.zone.run(() => { window.angularComponentRef.showTree(" + (mergeActions ? "true" : "false") + "); })";
            chromeBrowser.ExecuteScriptAsync(script);
        }

        /// <summary>
        /// Change the cursor as per status
        /// </summary>
        /// <param name="showWaitCursor">Show wait cursor or not</param>
        public void changeCursor(bool showWaitCursor)
        {
            string script = "window.angularComponentRef.zone.run(() => { window.angularComponentRef.changeCursor(" + (showWaitCursor ? "true" : "false") + "); })";
            chromeBrowser.ExecuteScriptAsync(script);
        }

        private void SetGridState(bool showGrid)
        {
            // Check if we need to clear the comparison node and comparison list as well


            // Call Angular method to show/hide grid here
            string script = "window.angularComponentRef.zone.run(() => { window.angularComponentRef.clearTree(" + (showGrid ? "true" : "false") + "); })";
            if (chromeBrowser.IsBrowserInitialized)
            {
                chromeBrowser.ExecuteScriptAsync(script);
            }
        }
        #endregion



        //private void GetFromAutoCompleteSource()
        //{
        //    string serverNameSource = ReverseArray<string>(Settings.Default.SourceServerAutoCompleteEntries.Substring(0,
        //        Settings.Default.SourceServerAutoCompleteEntries.Length - 1).Split("|".ToCharArray()))[0]; //.Reverse().ToArray();
        //    _connectionInfoSource = new ConnectionInfo(serverNameSource, Settings.Default.SourceCatalog);
        //}

        //private void GetFromAutoCompleteTarget()
        //{
        //    string serverNameTarget = ReverseArray<string>(Settings.Default.TargetServerAutoCompleteEntries.Substring(0,
        //        Settings.Default.TargetServerAutoCompleteEntries.Length - 1).Split("|".ToCharArray()))[0];
        //    //_connectionInfoTarget = new ConnectionInfo(serverNameTarget, Settings.Default.TargetCatalog);
        //}

        internal static T[] ReverseArray<T>(T[] array)
        {
            T[] newArray = null;
            int count = array == null ? 0 : array.Length;
            if (count > 0)
            {
                newArray = new T[count];
                for (int i = 0, j = count - 1; i < count; i++, j--)
                {
                    newArray[i] = array[j];
                }
            }
            return newArray;
        }

        private void SetAutoComplete()
        {
            if (!_comparisonInfo.ConnectionInfoSource.UseProject && !_comparisonInfo.ConnectionInfoSource.UseDesktop && !_comparisonInfo.ConnectionInfoSource.UseBimFile && !_comparisonInfo.ConnectionInfoSource.UseTmdlFolder)
            {
                if (Settings.Default.SourceServerAutoCompleteEntries.IndexOf(_comparisonInfo.ConnectionInfoSource.ServerName + "|") > -1)
                {
                    Settings.Default.SourceServerAutoCompleteEntries =
                        Settings.Default.SourceServerAutoCompleteEntries.Remove(
                            Settings.Default.SourceServerAutoCompleteEntries.IndexOf(_comparisonInfo.ConnectionInfoSource.ServerName + "|"),
                            (_comparisonInfo.ConnectionInfoSource.ServerName + "|").Length);
                }
                Settings.Default.SourceServerAutoCompleteEntries += _comparisonInfo.ConnectionInfoSource.ServerName + "|";
                Settings.Default.SourceCatalog = _comparisonInfo.ConnectionInfoSource.DatabaseName;

                Settings.Default.Save();
                //GetFromAutoCompleteSource();
            }

            if (!_comparisonInfo.ConnectionInfoTarget.UseProject && !_comparisonInfo.ConnectionInfoTarget.UseDesktop && !_comparisonInfo.ConnectionInfoTarget.UseBimFile && !_comparisonInfo.ConnectionInfoTarget.UseTmdlFolder)
            {
                if (Settings.Default.TargetServerAutoCompleteEntries.IndexOf(_comparisonInfo.ConnectionInfoTarget.ServerName + "|") > -1)
                {
                    Settings.Default.TargetServerAutoCompleteEntries =
                        Settings.Default.TargetServerAutoCompleteEntries.Remove(
                            Settings.Default.TargetServerAutoCompleteEntries.IndexOf(_comparisonInfo.ConnectionInfoTarget.ServerName + "|"),
                            (_comparisonInfo.ConnectionInfoTarget.ServerName + "|").Length);
                }
                Settings.Default.TargetServerAutoCompleteEntries += _comparisonInfo.ConnectionInfoTarget.ServerName + "|";
                Settings.Default.TargetCatalog = _comparisonInfo.ConnectionInfoTarget.DatabaseName;

                Settings.Default.Save();
                //GetFromAutoCompleteTarget();
            }
        }

        #endregion

        #region Event Handlers

        private void PopulateSourceTargetTextBoxes()
        {
            if (_comparisonInfo.ConnectionInfoSource.UseDesktop)
            {
                txtSource.Text = "PBI Desktop: " + _comparisonInfo.ConnectionInfoSource.ServerName + ";" + _comparisonInfo.ConnectionInfoSource.DesktopName;
            }
            else if (_comparisonInfo.ConnectionInfoSource.UseTmdlFolder)
            {
                txtSource.Text = "Folder: " + _comparisonInfo.ConnectionInfoSource.TmdlFolder;
            }
            else if (_comparisonInfo.ConnectionInfoSource.UseBimFile)
            {
                txtSource.Text = "File: " + _comparisonInfo.ConnectionInfoSource.BimFile;
            }
            else
            {
                txtSource.Text = "Dataset: " + _comparisonInfo.ConnectionInfoSource.ServerName + ";" + _comparisonInfo.ConnectionInfoSource.DatabaseName;
            }

            if (_comparisonInfo.ConnectionInfoTarget.UseDesktop)
            {
                txtTarget.Text = "PBI Desktop: " + _comparisonInfo.ConnectionInfoTarget.ServerName + ";" + _comparisonInfo.ConnectionInfoTarget.DesktopName;
            }
            else if (_comparisonInfo.ConnectionInfoTarget.UseTmdlFolder)
            {
                txtTarget.Text = "Folder: " + _comparisonInfo.ConnectionInfoTarget.TmdlFolder;
            }
            else if (_comparisonInfo.ConnectionInfoTarget.UseBimFile)
            {
                txtTarget.Text = "File: " + _comparisonInfo.ConnectionInfoTarget.BimFile;
            }
            else
            {
                txtTarget.Text = "Dataset: " + _comparisonInfo.ConnectionInfoTarget.ServerName + ";" + _comparisonInfo.ConnectionInfoTarget.DatabaseName;
            }

        }

        private void btnGenerateScript_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                changeCursor(true);
                toolStripStatusLabel1.Text = "Creating script ...";

                //If we get here, there was a problem generating the xmla file (maybe file item templates not installed), so offer saving to a file instead
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFile.Filter = "XMLA Files|*.xmla|JSON Files|*.json|Text Files|*.txt|All files|*.*";
                saveFile.CheckFileExists = false;
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFile.FileName, _comparison.ScriptDatabase());
                    toolStripStatusLabel1.Text = "ALM Toolkit - finished generating script";
                    MessageBox.Show("Created script\n" + saveFile.FileName, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                changeCursor(false);
                toolStripStatusLabel1.Text = "";
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            Options optionsForm = new Options();
            optionsForm.ComparisonInfo = _comparisonInfo;
            optionsForm.StartPosition = FormStartPosition.CenterParent;
            optionsForm.DpiScaleFactor = _dpiScaleFactor;
            optionsForm.ShowDialog();
            if (optionsForm.DialogResult == DialogResult.OK)
            {
                //ComparisonCtrl.TriggerComparisonChanged();
                //if (ComparisonCtrl.CompareState != CompareState.NotCompared)
                //{
                //    SetNotComparedState();
                //    toolStripStatusLabel1.Text = "Comparison invalidated. Please re-run the comparison.";
                //}

                if (_compareState != CompareState.NotCompared)
                {
                    SetNotComparedState();
                    toolStripStatusLabel1.Text = "Comparison invalidated. Please re-run the comparison.";
                }
            }
        }

        private void btnReportDifferences_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                changeCursor(true);
                toolStripStatusLabel1.Text = "ALM Toolkit - generating report ...";
                toolStripProgressBar1.Visible = true;
                _comparison.ReportDifferences(toolStripProgressBar1);
                toolStripStatusLabel1.Text = "ALM Toolkit - finished generating report";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                toolStripProgressBar1.Visible = false;
                Cursor.Current = Cursors.Default;
                changeCursor(false);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //Todo: not firing

            if (keyData == (Keys.Control | Keys.S))
            {
                Save();
                return true;
            }

            if (keyData == (Keys.Shift | Keys.Alt | Keys.C))
            {
                this.InitializeAndCompareTabularModels();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnCompareTabularModels_Click(object sender, EventArgs e)
        {
            InitializeAndCompareTabularModels();
        }

        private void mnuHideSkipObjects_Click(object sender, EventArgs e)
        {
            //ComparisonCtrl.ShowHideNodes(true);

            _comparisonInter.ShowHideSkipNodes(true);
            refreshGridControl(true);
        }

        private void mnuHideSkipObjectsWithSameDefinition_Click(object sender, EventArgs e)
        {
            //ComparisonCtrl.ShowHideNodes(true, sameDefinitionFilter: true);

            _comparisonInter.ShowHideSkipNodes(true, sameDefinitionFilter: true);
            refreshGridControl(true);
        }

        private void mnuShowSkipObjects_Click(object sender, EventArgs e)
        {
            //ComparisonCtrl.ShowHideNodes(false);

            _comparisonInter.ShowHideSkipNodes(false);
            refreshGridControl(true);
        }

        private void mnuSkipAllObjectsMissingInSource_Click(object sender, EventArgs e)
        {
            //ComparisonCtrl.SkipItems(false, ComparisonObjectStatus.MissingInSource);
            SetComparedState();

            _comparisonInter.SkipItems(false, ComparisonObjectStatus.MissingInSource);
            refreshGridControl(true);
        }

        private void mnuDeleteAllObjectsMissingInSource_Click(object sender, EventArgs e)
        {
            //ComparisonCtrl.ShowHideNodes(false);
            //ComparisonCtrl.DeleteItems(false);
            SetComparedState();

            _comparisonInter.ShowHideSkipNodes(false);
            _comparisonInter.DeleteItems(false);
            refreshGridControl(true);
        }

        private void mnuSkipAllObjectsMissingInTarget_Click(object sender, EventArgs e)
        {
            //ComparisonCtrl.SkipItems(false, ComparisonObjectStatus.MissingInTarget);
            SetComparedState();

            _comparisonInter.SkipItems(false, ComparisonObjectStatus.MissingInTarget);
            refreshGridControl(true);
        }

        private void mnuCreateAllObjectsMissingInTarget_Click(object sender, EventArgs e)
        {
            //ComparisonCtrl.ShowHideNodes(false);
            //ComparisonCtrl.CreateItems(false);
            SetComparedState();

            _comparisonInter.ShowHideSkipNodes(false);
            _comparisonInter.CreateItems(false);
            refreshGridControl(true);
        }

        private void mnuSkipAllObjectsWithDifferentDefinitions_Click(object sender, EventArgs e)
        {
            //ComparisonCtrl.SkipItems(false, ComparisonObjectStatus.DifferentDefinitions);
            SetComparedState();

            _comparisonInter.SkipItems(false, ComparisonObjectStatus.DifferentDefinitions);
            refreshGridControl(true);
        }

        private void mnuUpdateAllObjectsWithDifferentDefinitions_Click(object sender, EventArgs e)
        {
            //ComparisonCtrl.ShowHideNodes(false);
            //ComparisonCtrl.UpdateItems(false);
            SetComparedState();

            _comparisonInter.ShowHideSkipNodes(false);
            _comparisonInter.UpdateItems(false);
            refreshGridControl(true);
        }

        private void btnValidateSelection_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                changeCursor(true);
                toolStripStatusLabel1.Text = "ALM Toolkit - validating ...";

                // Not required since _comparison object is always updated with latest updates
                //ComparisonCtrl.RefreshDiffResultsFromGrid();

                WarningListForm warningList = new WarningListForm();
                warningList.Comparison = _comparison;
                //warningList.TreeGridImageList = ComparisonCtrl.TreeGridImageList;
                warningList.StartPosition = FormStartPosition.CenterParent;
                warningList.ShowDialog();

                SetValidatedState();
                toolStripStatusLabel1.Text = "ALM Toolkit - finished validating";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                changeCursor(false);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to update the target?", Utils.AssemblyProduct, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                changeCursor(true);
                toolStripStatusLabel1.Text = "ALM Toolkit - committing changes ...";
                // Not required since _comparison object is always updated with latest updates
                //ComparisonCtrl.RefreshSkipSelections();

                if (_compareState != CompareState.NotCompared && _comparison != null)
                {
                    _comparison.RefreshSkipSelectionsFromComparisonObjects();

                    bool update = _comparison.Update();
                    toolStripStatusLabel1.Text = "ALM Toolkit - finished committing changes";

                    SetNotComparedState();
                    if (update && MessageBox.Show($"Updated the target.\n\nDo you want to refresh the comparison?", Utils.AssemblyProduct, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.CompareTabularModels();
                    }
                }
                else
                {
                    toolStripStatusLabel1.Text = "ALM Toolkit - require validation for changes";
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                changeCursor(false);
            }
        }

        private void HandleDatabaseDeployment(object sender, DatabaseDeploymentEventArgs e)
        {
            Deployment deployForm = new Deployment();
            deployForm.Comparison = _comparison;
            deployForm.ComparisonInfo = _comparisonInfo;
            deployForm.DpiScaleFactor = _dpiScaleFactor;
            deployForm.StartPosition = FormStartPosition.CenterParent;
            deployForm.ShowDialog();
            e.DeploymentSuccessful = (deployForm.DialogResult == DialogResult.OK);
        }

        private void HandleComparisonChanged(object sender, EventArgs e)
        {
            //If user changes a skip selection after validation, need to disable Update button
            //if (ComparisonCtrl.CompareState == CompareState.Validated)
            //{
            //    SetComparedState();
            //    toolStripStatusLabel1.Text = "ALM Toolkit - datasets compared";
            //}
        }

        public void HandleComparisonChanged()
        {
            //If user changes a skip selection after validation, need to disable Update button
            if (_compareState == CompareState.Validated)
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        SetComparedState();
                        toolStripStatusLabel1.Text = "ALM Toolkit - datasets compared";
                    }));
                }
            }

            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    SetFileNameTitle(true);
                }));
            }
        }

        private void maqSoftwareLogo_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://maqsoftware.com/");
            }
            catch { }
        }

        public void LoadFromDesktop(string serverName, string databaseName)
        {
            _comparisonInfo = new ComparisonInfo();
            _comparisonInfo.AppName = Utils.AssemblyProduct;

            _comparisonInfo.ConnectionInfoSource.UseDesktop = true;
            _comparisonInfo.ConnectionInfoSource.ServerName = serverName;
            _comparisonInfo.ConnectionInfoSource.DatabaseName = databaseName;

            //GetFromAutoCompleteSource();
            //GetFromAutoCompleteTarget();

            SetNotComparedState();
        }

        public void LoadFile(string fileName)
        {
            try
            {
                if (File.ReadAllText(fileName) == "")
                {
                    //Blank file not saved to yet
                    return;
                }
                _comparisonInfo = ComparisonInfo.DeserializeBsmnFile(fileName, Utils.AssemblyProduct);
                _fileName = fileName;
                SetFileNameTitle(false);
                PopulateSourceTargetTextBoxes();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Error loading file {fileName}\n{exc.Message}\n\nPlease save over this file with a new version.", Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetFileNameTitle(bool unsaved)
        {
            _unsaved = unsaved;

            if (String.IsNullOrEmpty(_fileName))
            {
                this.Text = Utils.AssemblyProduct;
            }
            else
            {
                this.Text = Utils.AssemblyProduct + " - " + Path.GetFileName(_fileName);
                if (unsaved)
                {
                    this.Text += " *";
                }
            }
        }

        public void SaveFile(string fileName)
        {
            try
            {
                CleanUpConnectionInfo();

                _fileName = fileName;
                XmlSerializer writer = new XmlSerializer(typeof(ComparisonInfo));
                StreamWriter file = new System.IO.StreamWriter(fileName);
                _comparison.RefreshSkipSelectionsFromComparisonObjects();
                writer.Serialize(file, _comparisonInfo);
                file.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Error saving file {fileName}\n{exc.Message}", Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CleanUpConnectionInfo()
        {
            if (_comparisonInfo.ConnectionInfoSource.UseDesktop)
            {
                _comparisonInfo.ConnectionInfoSource.ServerName = null;
                _comparisonInfo.ConnectionInfoSource.DatabaseName = null;
            }
            else
            {
                _comparisonInfo.ConnectionInfoSource.DesktopName = null;
            }

            if (_comparisonInfo.ConnectionInfoTarget.UseDesktop)
            {
                _comparisonInfo.ConnectionInfoTarget.ServerName = null;
                _comparisonInfo.ConnectionInfoTarget.DatabaseName = null;
            }
            else
            {
                _comparisonInfo.ConnectionInfoTarget.DesktopName = null;
            }
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (_unsaved && SaveChanges() == DialogResult.Cancel)
                {
                    return;
                }

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "ALM Toolkit Files (.almt)|*.almt";
                ofd.Title = "Open";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    SetNotComparedState();
                    this.LoadFile(ofd.FileName);
                    InitializeAndCompareTabularModels();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            Save();
        }
        public void SaveNg()
        {
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    Save();
                }));
            }
        }
        public void Save()
        {

            try
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    SaveFileAs();
                }
                else
                {
                    this.SaveFile(_fileName);
                }
                SetFileNameTitle(false);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }

        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileAs();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Utils.AssemblyProduct, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetNotComparedState();
            }
        }

        private void SaveFileAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "ALM Toolkit Files (.almt)|*.almt";
            sfd.Title = "Save As";

            if (String.IsNullOrEmpty(_fileName))
            {
                sfd.FileName = "Comparison1";
            }
            else
            {
                sfd.FileName = Path.GetFileName(_fileName);
            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _fileName = sfd.FileName;
                SetFileNameTitle(false);
                this.SaveFile(_fileName);
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.NewerVersionAvailable = _newerVersionAvailable;
            aboutForm.LatestVersion = _latestVersion;
            aboutForm.ShowDialog();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (_unsaved && SaveChanges() == DialogResult.Cancel)
            {
                return;
            }
            Application.Exit();
        }

        private DialogResult SaveChanges()
        {
            DialogResult result = MessageBox.Show("Do you want to save changes?", Utils.AssemblyProduct, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            switch (result)
            {
                case DialogResult.Yes:
                    Save();
                    break;
                default:
                    break;
            }
            return result;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            this.SetNotComparedState();
            base.OnHandleDestroyed(e);
        }

        #endregion

        #region DPI

        private float _dpiScaleFactor = 1;
        private void Rescale()
        {
            float fudgedDpiScaleFactor = _dpiScaleFactor * BismNormalizer.TabularCompare.UI.Utils.PrimaryFudgeFactor;

            //pnlRibbon.Height = Convert.ToInt32(Convert.ToDouble(ribbonMain.Height) * HighDPIUtils.SecondaryFudgeFactor * 0.93);
            ribbonMain.Height = pnlRibbon.Height;
            spltSourceTarget.SplitterDistance = Convert.ToInt32(Convert.ToDouble(spltSourceTarget.Width) * 0.5);
            txtSource.Width = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(spltSourceTarget.Width) * 0.5) * 0.9);
            txtTarget.Width = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(spltSourceTarget.Width) * 0.5) * 0.9);

            this._dpiScaleFactor = BismNormalizer.TabularCompare.UI.Utils.GetDpiFactor();
            if (this._dpiScaleFactor == 1) return;

            this.Scale(new SizeF(fudgedDpiScaleFactor, fudgedDpiScaleFactor));

            this.Font = new Font(this.Font.FontFamily,
                                 this.Font.Size * fudgedDpiScaleFactor,
                                 this.Font.Style);
            pnlHeader.Font = new Font(pnlHeader.Font.FontFamily,
                                pnlHeader.Font.Size * fudgedDpiScaleFactor,
                                pnlHeader.Font.Style);

            txtSource.Left = Convert.ToInt32(txtSource.Left * fudgedDpiScaleFactor * 0.9);
            txtTarget.Left = Convert.ToInt32(txtTarget.Left * fudgedDpiScaleFactor * 0.9);
        }

        #endregion
    }
}
