using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using BismNormalizer.TabularCompare.UI;
using System.Drawing;

namespace BismNormalizer
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "3", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(WarningList),
        Style = VsDockStyle.Tabbed,
        Transient = true,   //Transient means will not show up automatically when reopen VS
        Window = "D78612C7-9962-4B83-95D9-268046DAD23A"  //this is the guid of the VS error window (NOT YOUR CUSTOM WINDOW)
    )]
    [ProvideEditorExtension(typeof(EditorFactory), ".bsmn", 50,
        ProjectGuid = VSConstants.CLSID.MiscellaneousFilesProject_string,
        TemplateDir = "Templates",
        NameResourceID = 105,
        DefaultName = "BismNormalizer")]
    [ProvideEditorExtension(typeof(EditorFactory), ".bsmn", 1000,
        ProjectGuid = "{6870E480-7721-4708-BFB8-9AE898AA21B3}",  //GUID for tabular BI projects
        TemplateDir = "Templates",
        NameResourceID = 105,
        DefaultName = "BismNormalizer")]
    [ProvideKeyBindingTable(GuidList.guidBismNormalizerEditorFactoryString, 102)]
    [ProvideEditorLogicalView(typeof(EditorFactory), VSConstants.LOGVIEWID.Any_string)] //VSConstants.LOGVIEWID.TextView_string)]
    [ProvideAutoLoad(UIContextGuids.SolutionHasSingleProject)]  //Microsoft.VisualStudio.VSConstants.UICONTEXT.NoSolution_string
    [Guid(GuidList.guidBismNormalizerPkgString)]
    public sealed class BismNormalizerPackage : Package, IDisposable
    {
        private DTE2 _dte;
        private ValidationOutput _validationOutput;
        private List<EditorPane> _editorPanes;
        private DteInitializer _dteInitializer;
        private IVsWindowFrame _toolWindowFrame;
        private EditorFactory _editorFactory;

        public BismNormalizerPackage() { }

        protected override void Initialize()
        {
            base.Initialize();
            InitializeDTE();

            _editorPanes = new List<EditorPane>();

            base.RegisterEditorFactory(new EditorFactory(this));

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                //Command for New Comparison from Tools menu
                CommandID menuToolMenuNewComparisonCommandID = new CommandID(GuidList.guidBismNormalizerCmdSet, (int)PkgCmdIDList.cmdidToolMenuNewComparison);
                MenuCommand menuToolMenuNewComparison = new MenuCommand(NewComparison, menuToolMenuNewComparisonCommandID );
                mcs.AddCommand( menuToolMenuNewComparison );

                //Command for New Comparison from project context menu in solution explorer
                CommandID menuProjectMenuNewComparisonCommandID = new CommandID(GuidList.guidBismNormalizerCmdSet, (int)PkgCmdIDList.cmdidProjectMenuNewComparison);
                OleMenuCommand menuProjectMenuNewComparison = new OleMenuCommand(AddNewComparison, menuProjectMenuNewComparisonCommandID);
                menuProjectMenuNewComparison.BeforeQueryStatus += menuItem_BeforeQueryStatusNewComparison;
                mcs.AddCommand(menuProjectMenuNewComparison);

                //Command for View Code from file context menu in solution explorer
                CommandID menuFileMenuViewCodeCommandID = new CommandID(GuidList.guidBismNormalizerCmdSet, (int)PkgCmdIDList.cmdidFileMenuViewCode);
                OleMenuCommand menuFileMenuViewCode = new OleMenuCommand(ViewCode, menuFileMenuViewCodeCommandID);
                menuFileMenuViewCode.BeforeQueryStatus += menuItem_BeforeQueryStatusCodeBehind;
                mcs.AddCommand(menuFileMenuViewCode);

                //Command for View Designer from file context menu in solution explorer
                CommandID menuFileMenuViewDesignerCommandID = new CommandID(GuidList.guidBismNormalizerCmdSet, (int)PkgCmdIDList.cmdidFileMenuViewDesigner);
                OleMenuCommand menuFileMenuViewDesigner = new OleMenuCommand(ViewDesigner, menuFileMenuViewDesignerCommandID);
                menuFileMenuViewDesigner.BeforeQueryStatus += menuItem_BeforeQueryStatusCodeBehind;
                mcs.AddCommand(menuFileMenuViewDesigner);

                //Command for View Designer from file context menu in solution explorer
                CommandID menuFileMenuInstallIconCommandID = new CommandID(GuidList.guidBismNormalizerCmdSet, (int)PkgCmdIDList.cmdidFileMenuInstallIcon);
                OleMenuCommand menuFileMenuInstallIcon = new OleMenuCommand(InstallIcon, menuFileMenuInstallIconCommandID);
                menuFileMenuInstallIcon.BeforeQueryStatus += menuItem_BeforeQueryStatusInstallIcon;
                mcs.AddCommand(menuFileMenuInstallIcon);

                //Command for BISM Normalizer Warning List
                CommandID toolwndCommandID = new CommandID(GuidList.guidBismNormalizerCmdSet, (int)PkgCmdIDList.cmdidValidationOutput);
                MenuCommand menuValidationOutput = new MenuCommand(InitializeToolWindow, toolwndCommandID);
                mcs.AddCommand( menuValidationOutput );
            }
        }

        private void NewComparison(object sender, EventArgs e)
        {
            try
            {
                _dte.ItemOperations.NewFile(@"BISM Normalizer\Tabular Compare");
            }
            catch (Exception)
            {
                ShowMessage("Cannot launch BISM Normalizer.  Please check installation, or try creating a new text file with .bsmn extension.", OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_CRITICAL);
            }
        }

        private void AddNewComparison(object sender, EventArgs e)
        {
            try
            {
                EnvDTE.ProjectItem projItem = _dte.ItemOperations.AddNewItem(@"BISM Normalizer\Tabular Compare");
                //Can't use while can't change name of file while editor open:
                //_dte.ItemOperations.OpenFile(projItem.FileNames[0]);
            }
            catch (Exception)
            {
                ShowMessage("Cannot add BISM Normalizer comparison.  Please check installation, or try creating a new text file with .bsmn extension.", OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_CRITICAL);
            }
        }

        private void ViewCode(object sender, EventArgs e)
        {
            try
            {
                OleMenuCommand menuCommand = sender as OleMenuCommand;

                if (menuCommand != null)
                {
                    menuCommand.Visible = false;  // default to not visible

                    if (_dte != null && _dte.SelectedItems != null && _dte.SelectedItems.Count == 1)  //only support 1 selected file
                    {
                        foreach (EnvDTE.SelectedItem selectedItem in _dte.SelectedItems)
                        {
                            if (selectedItem.Name != null &&
                                selectedItem.Name.ToUpper().EndsWith(".bsmn".ToUpper()) &&
                                selectedItem.ProjectItem != null
                               )
                            {
                                EnvDTE.ProjectItem projItem = selectedItem.ProjectItem;
                                _dte.ItemOperations.OpenFile(projItem.FileNames[0], EnvDTE.Constants.vsViewKindCode);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowMessage("Cannot view code.  Please check installation.", OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_CRITICAL);
            }
        }

        private void ViewDesigner(object sender, EventArgs e)
        {
            try
            {
                OleMenuCommand menuCommand = sender as OleMenuCommand;

                if (menuCommand != null)
                {
                    menuCommand.Visible = false;  // default to not visible

                    if (_dte != null && _dte.SelectedItems != null && _dte.SelectedItems.Count == 1)  //only support 1 selected file
                    {
                        foreach (EnvDTE.SelectedItem selectedItem in _dte.SelectedItems)
                        {
                            if (selectedItem.Name != null &&
                                selectedItem.Name.ToUpper().EndsWith(".bsmn".ToUpper()) &&
                                selectedItem.ProjectItem != null
                               )
                            {
                                EnvDTE.ProjectItem projItem = selectedItem.ProjectItem;
                                _dte.ItemOperations.OpenFile(projItem.FileNames[0], EnvDTE.Constants.vsViewKindPrimary);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowMessage("Cannot view designer.  Please check installation.", OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_CRITICAL);
            }
        }

        private void InstallIcon(object sender, EventArgs e)
        {
            string message = "Setting up icon for Solution Explorer will require running a separate process as administrator.";
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "Bism Normalizer",
                       message,
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_WARNING,
                       0,        // false
                       out result));
            if (result != 1)
            {
                // If !=OK then backout
                return;
            }

            try
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                string workingDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("\\BismNormalizer.dll", "");
                proc.WorkingDirectory = workingDirectory;
                proc.FileName = workingDirectory + "\\BismNormalizer.IconSetup.exe";
                proc.Verb = "runas";
                Process.Start(proc);
            }
            catch (Exception exc)
            {
                ShowMessage(exc.Message, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_CRITICAL);
            }
        }

        void menuItem_BeforeQueryStatusNewComparison(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;

            if (menuCommand != null)
            {
                menuCommand.Visible = false;  // default to not visible

                if (_dte != null)
                {
                    Array selectedProjects = (Array)_dte.ActiveSolutionProjects;

                    //only support 1 selected project
                    if (selectedProjects.Length == 1)
                    {
                        EnvDTE.Project project = (EnvDTE.Project)selectedProjects.GetValue(0);

                        if (project.FullName.EndsWith(".smproj"))
                        {
                            menuCommand.Visible = true;
                        }
                    }
                }
            }
        }

        void menuItem_BeforeQueryStatusCodeBehind(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;

            if (menuCommand != null)
            {
                menuCommand.Visible = false;  // default to not visible

                if (_dte != null && _dte.SelectedItems != null && _dte.SelectedItems.Count == 1)  //only support 1 selected file
                {
                    foreach (EnvDTE.SelectedItem selectedItem in _dte.SelectedItems)
                    {
                        if (selectedItem.Name != null &&
                            selectedItem.Name.ToUpper().EndsWith(".bsmn".ToUpper())
                           )
                        {
                            menuCommand.Visible = true;
                        }
                    }
                }
            }
        }

        void menuItem_BeforeQueryStatusInstallIcon(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;

            if (menuCommand != null)
            {
                menuCommand.Visible = false;  // default to not visible

                if (_dte != null && _dte.SelectedItems != null && _dte.SelectedItems.Count == 1)  //only support 1 selected file
                {
                    foreach (EnvDTE.SelectedItem selectedItem in _dte.SelectedItems)
                    {
                        if (selectedItem.Name != null &&
                            selectedItem.Name.ToUpper().EndsWith(".bsmn".ToUpper())
                           )
                        {
                            //Check if icon already installed and .bsmn files are associated with VS
                            try
                            {
                                if (Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.bsmn\\UserChoice", false) == null)
                                {
                                    menuCommand.Visible = true;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        public void ShowMessage(string message, OLEMSGBUTTON msgButton, OLEMSGICON msgIcon)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "Bism Normalizer",
                       string.Format(CultureInfo.CurrentCulture, message, this.ToString()),
                       string.Empty,
                       0,
                       msgButton,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       msgIcon,
                       0,        // false
                       out result));
        }

        private void InitializeToolWindow(object sender, EventArgs e)
        {
            InitializeToolWindowInternal();
        }

        internal void InitializeToolWindowInternal(float dpiFactor = 0)
        {
            ToolWindowPane window = this.FindToolWindow(typeof(WarningList), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            _validationOutput = (ValidationOutput)window.Window;
            if (dpiFactor != 0)
            {
                _validationOutput.Rescale(dpiFactor);
            }
            _toolWindowFrame = (IVsWindowFrame)window.Frame;
            ShowToolWindow();
        }

        public void ShowToolWindow()
        {
            if (_toolWindowFrame != null)
            {
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(_toolWindowFrame.Show());
            }
        }

        private void InitializeDTE()
        {
            IVsShell shellService;

            this._dte = this.GetService(typeof(SDTE)) as DTE2;

            if (this._dte == null) // The IDE is not yet fully initialized
            {
                shellService = this.GetService(typeof(SVsShell)) as IVsShell;
                this._dteInitializer = new DteInitializer(shellService, this.InitializeDTE);
            }
            else
            {
                this._dteInitializer = null;
                _documentEvents = ((EnvDTE80.Events2)_dte.Events).get_DocumentEvents();
                _documentEvents.DocumentOpened += new EnvDTE._dispDocumentEvents_DocumentOpenedEventHandler(DocumentEvents_DocumentOpened);

                //Unfortunately, this does not fire for tabular projects - only regular C# type projects
                _projectItemEvents = ((EnvDTE80.Events2)_dte.Events).ProjectItemsEvents;
                _projectItemEvents.ItemRenamed += _projectItemEvents_ItemRenamed;
            }
        }

        public DTE2 Dte => this._dte;

        public ValidationOutput ValidationOutput
        {
            get
            {
                return this._validationOutput;
            }
            set
            {
                this._validationOutput = value;
            }
        }

        public List<EditorPane> EditorPanes => _editorPanes;

        #region IDisposable Pattern
        /// <summary>
        /// Releases the resources used by the Package object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the resources used by the Package object.
        /// </summary>
        /// <param name="disposing">This parameter determines whether the method has been called directly or indirectly by a user's code.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Dispose() of: {0}", this.ToString()));
                if (disposing)
                {
                    if (_editorFactory != null)
                    {
                        _editorFactory.Dispose();
                        _editorFactory = null;
                    }
                    GC.SuppressFinalize(this);
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion

        EnvDTE.DocumentEvents _documentEvents;
        void DocumentEvents_DocumentOpened(EnvDTE.Document document)
        {
            try
            {
                if (document.FullName.EndsWith(".bim"))
                {
                    string message = "";
                    foreach (EditorPane editorPane in EditorPanes)
                    {
                        if (editorPane.BismNormalizerForm != null && editorPane.BismNormalizerForm.CompareState != CompareState.NotCompared)
                        {
                            // check if open diff has project that contains BIM file being opened.
                            if (editorPane.BismNormalizerForm.ComparisonInfo.ConnectionInfoSource.UseProject)
                            {
                                foreach (EnvDTE.ProjectItem projectItem in editorPane.BismNormalizerForm.ComparisonInfo.ConnectionInfoSource.Project.ProjectItems)
                                {
                                    if (projectItem.Document != null && projectItem.Document.FullName == document.FullName)
                                    {
                                        editorPane.BismNormalizerForm.SetNotComparedState();
                                        message += " - " + editorPane.Name + "\n";
                                        break;
                                    }
                                }
                            }
                            if (editorPane.BismNormalizerForm.CompareState != CompareState.NotCompared && editorPane.BismNormalizerForm.ComparisonInfo.ConnectionInfoTarget.UseProject)
                            {
                                foreach (EnvDTE.ProjectItem projectItem in editorPane.BismNormalizerForm.ComparisonInfo.ConnectionInfoTarget.Project.ProjectItems)
                                {
                                    if (projectItem.Document != null && projectItem.Document.FullName == document.FullName)
                                    {
                                        editorPane.BismNormalizerForm.SetNotComparedState();
                                        message += " - " + editorPane.Name + "\n";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (message.Length > 0)
                    {
                        ShowMessage("Opening this file will invalidate the following comparisons.\n" + message, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_WARNING);
                    }
                }
            }
            catch { }
        }

        EnvDTE.ProjectItemsEvents _projectItemEvents;
        void _projectItemEvents_ItemRenamed(EnvDTE.ProjectItem projectItem, string oldName)
        {
            if (projectItem.IsOpen && oldName.EndsWith(".bsmn") && projectItem.Document != null)
            {
                this.ShowMessage("Changing file name while editor is open is not supported.  File will close now.", OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_WARNING);
                projectItem.Document.Close();
            }
        }
    }

    public class DteInitializer : IVsShellPropertyEvents
    {
        private IVsShell shellService;
        private uint cookie;
        private Action callback;

        public DteInitializer(IVsShell shellService, Action callback)
        {
            int hr;

            this.shellService = shellService;
            this.callback = callback;

            // Set an event handler to detect when the IDE is fully initialized
            hr = this.shellService.AdviseShellPropertyChanges(this, out this.cookie);

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
        }

        int IVsShellPropertyEvents.OnShellPropertyChange(int propid, object var)
        {
            int hr;
            bool isZombie;

            if (propid == (int)__VSSPROPID.VSSPROPID_Zombie)
            {
                isZombie = (bool)var;

                if (!isZombie)
                {
                    // Release the event handler to detect when the IDE is fully initialized
                    hr = this.shellService.UnadviseShellPropertyChanges(this.cookie);

                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);

                    this.cookie = 0;

                    this.callback();
                }
            }
            return VSConstants.S_OK;
        }
    }

}
