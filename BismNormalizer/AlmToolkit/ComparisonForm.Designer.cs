namespace AlmToolkit
{
    partial class ComparisonForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComparisonForm));
            this.StatusBarComparsion = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.tabHome = new System.Windows.Forms.RibbonTab();
            this.panelCompare = new System.Windows.Forms.RibbonPanel();
            this.btnCompareTabularModels = new System.Windows.Forms.RibbonButton();
            this.ddSelectActions = new System.Windows.Forms.RibbonButton();
            this.mnuHideSkipObjects = new System.Windows.Forms.RibbonButton();
            this.mnuHideSkipObjectsWithSameDefinition = new System.Windows.Forms.RibbonButton();
            this.mnuShowSkipObjects = new System.Windows.Forms.RibbonButton();
            this.ribbonSeparator1 = new System.Windows.Forms.RibbonSeparator();
            this.mnuSkipAllObjectsMissingInSource = new System.Windows.Forms.RibbonButton();
            this.mnuDeleteAllObjectsMissingInSource = new System.Windows.Forms.RibbonButton();
            this.mnuSkipAllObjectsMissingInTarget = new System.Windows.Forms.RibbonButton();
            this.mnuCreateAllObjectsMissingInTarget = new System.Windows.Forms.RibbonButton();
            this.mnuSkipAllObjectsWithDifferentDefinitions = new System.Windows.Forms.RibbonButton();
            this.mnuUpdateAllObjectsWithDifferentDefinitions = new System.Windows.Forms.RibbonButton();
            this.btnValidateSelection = new System.Windows.Forms.RibbonButton();
            this.btnUpdate = new System.Windows.Forms.RibbonButton();
            this.btnGenerateScript = new System.Windows.Forms.RibbonButton();
            this.panelMisc = new System.Windows.Forms.RibbonPanel();
            this.btnOptions = new System.Windows.Forms.RibbonButton();
            this.btnReportDifferences = new System.Windows.Forms.RibbonButton();
            this.tabHelp = new System.Windows.Forms.RibbonTab();
            this.panelHelp = new System.Windows.Forms.RibbonPanel();
            this.btnHelp = new System.Windows.Forms.RibbonButton();
            this.pnlRibbon = new System.Windows.Forms.Panel();
            this.maqSoftwareLogo = new System.Windows.Forms.PictureBox();
            this.powerBiLogo = new System.Windows.Forms.PictureBox();
            this.ribbonMain = new System.Windows.Forms.Ribbon();
            this.mnuOpen = new System.Windows.Forms.RibbonOrbMenuItem();
            this.mnuSave = new System.Windows.Forms.RibbonOrbMenuItem();
            this.mnuSaveAs = new System.Windows.Forms.RibbonOrbMenuItem();
            this.mnuExit = new System.Windows.Forms.RibbonOrbMenuItem();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.spltSourceTarget = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.StatusBarComparsion.SuspendLayout();
            this.pnlRibbon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maqSoftwareLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.powerBiLogo)).BeginInit();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltSourceTarget)).BeginInit();
            this.spltSourceTarget.Panel1.SuspendLayout();
            this.spltSourceTarget.Panel2.SuspendLayout();
            this.spltSourceTarget.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusBarComparsion
            // 
            this.StatusBarComparsion.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.StatusBarComparsion.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.StatusBarComparsion.Location = new System.Drawing.Point(0, 454);
            this.StatusBarComparsion.Name = "StatusBarComparsion";
            this.StatusBarComparsion.Size = new System.Drawing.Size(825, 22);
            this.StatusBarComparsion.TabIndex = 48;
            this.StatusBarComparsion.Text = "Comparison Status";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // tabHome
            // 
            this.tabHome.Name = "tabHome";
            this.tabHome.Panels.Add(this.panelCompare);
            this.tabHome.Panels.Add(this.panelMisc);
            this.tabHome.Text = "Home";
            // 
            // panelCompare
            // 
            this.panelCompare.ButtonMoreVisible = false;
            this.panelCompare.Items.Add(this.btnCompareTabularModels);
            this.panelCompare.Items.Add(this.ddSelectActions);
            this.panelCompare.Items.Add(this.btnValidateSelection);
            this.panelCompare.Items.Add(this.btnUpdate);
            this.panelCompare.Items.Add(this.btnGenerateScript);
            this.panelCompare.Name = "panelCompare";
            this.panelCompare.Text = "";
            // 
            // btnCompareTabularModels
            // 
            this.btnCompareTabularModels.Image = ((System.Drawing.Image)(resources.GetObject("btnCompareTabularModels.Image")));
            this.btnCompareTabularModels.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnCompareTabularModels.LargeImage")));
            this.btnCompareTabularModels.Name = "btnCompareTabularModels";
            this.btnCompareTabularModels.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnCompareTabularModels.SmallImage")));
            this.btnCompareTabularModels.Text = "Compare";
            this.btnCompareTabularModels.Click += new System.EventHandler(this.btnCompareTabularModels_Click);
            // 
            // ddSelectActions
            // 
            this.ddSelectActions.DrawDropDownIconsBar = false;
            this.ddSelectActions.DropDownItems.Add(this.mnuHideSkipObjects);
            this.ddSelectActions.DropDownItems.Add(this.mnuHideSkipObjectsWithSameDefinition);
            this.ddSelectActions.DropDownItems.Add(this.mnuShowSkipObjects);
            this.ddSelectActions.DropDownItems.Add(this.ribbonSeparator1);
            this.ddSelectActions.DropDownItems.Add(this.mnuSkipAllObjectsMissingInSource);
            this.ddSelectActions.DropDownItems.Add(this.mnuDeleteAllObjectsMissingInSource);
            this.ddSelectActions.DropDownItems.Add(this.mnuSkipAllObjectsMissingInTarget);
            this.ddSelectActions.DropDownItems.Add(this.mnuCreateAllObjectsMissingInTarget);
            this.ddSelectActions.DropDownItems.Add(this.mnuSkipAllObjectsWithDifferentDefinitions);
            this.ddSelectActions.DropDownItems.Add(this.mnuUpdateAllObjectsWithDifferentDefinitions);
            this.ddSelectActions.Image = ((System.Drawing.Image)(resources.GetObject("ddSelectActions.Image")));
            this.ddSelectActions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ddSelectActions.LargeImage")));
            this.ddSelectActions.Name = "ddSelectActions";
            this.ddSelectActions.SmallImage = ((System.Drawing.Image)(resources.GetObject("ddSelectActions.SmallImage")));
            this.ddSelectActions.Style = System.Windows.Forms.RibbonButtonStyle.DropDown;
            this.ddSelectActions.Text = "Select Actions";
            // 
            // mnuHideSkipObjects
            // 
            this.mnuHideSkipObjects.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuHideSkipObjects.Image = ((System.Drawing.Image)(resources.GetObject("mnuHideSkipObjects.Image")));
            this.mnuHideSkipObjects.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuHideSkipObjects.LargeImage")));
            this.mnuHideSkipObjects.Name = "mnuHideSkipObjects";
            this.mnuHideSkipObjects.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuHideSkipObjects.SmallImage")));
            this.mnuHideSkipObjects.Text = "Hide Skip Objects";
            this.mnuHideSkipObjects.Click += new System.EventHandler(this.mnuHideSkipObjects_Click);
            // 
            // mnuHideSkipObjectsWithSameDefinition
            // 
            this.mnuHideSkipObjectsWithSameDefinition.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuHideSkipObjectsWithSameDefinition.Image = ((System.Drawing.Image)(resources.GetObject("mnuHideSkipObjectsWithSameDefinition.Image")));
            this.mnuHideSkipObjectsWithSameDefinition.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuHideSkipObjectsWithSameDefinition.LargeImage")));
            this.mnuHideSkipObjectsWithSameDefinition.Name = "mnuHideSkipObjectsWithSameDefinition";
            this.mnuHideSkipObjectsWithSameDefinition.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuHideSkipObjectsWithSameDefinition.SmallImage")));
            this.mnuHideSkipObjectsWithSameDefinition.Text = "Hide Skip Objects with Same Definition";
            this.mnuHideSkipObjectsWithSameDefinition.Click += new System.EventHandler(this.mnuHideSkipObjectsWithSameDefinition_Click);
            // 
            // mnuShowSkipObjects
            // 
            this.mnuShowSkipObjects.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuShowSkipObjects.Image = ((System.Drawing.Image)(resources.GetObject("mnuShowSkipObjects.Image")));
            this.mnuShowSkipObjects.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuShowSkipObjects.LargeImage")));
            this.mnuShowSkipObjects.Name = "mnuShowSkipObjects";
            this.mnuShowSkipObjects.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuShowSkipObjects.SmallImage")));
            this.mnuShowSkipObjects.Text = "Show Skip Objects";
            this.mnuShowSkipObjects.Click += new System.EventHandler(this.mnuShowSkipObjects_Click);
            // 
            // ribbonSeparator1
            // 
            this.ribbonSeparator1.Name = "ribbonSeparator1";
            // 
            // mnuSkipAllObjectsMissingInSource
            // 
            this.mnuSkipAllObjectsMissingInSource.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuSkipAllObjectsMissingInSource.Image = ((System.Drawing.Image)(resources.GetObject("mnuSkipAllObjectsMissingInSource.Image")));
            this.mnuSkipAllObjectsMissingInSource.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuSkipAllObjectsMissingInSource.LargeImage")));
            this.mnuSkipAllObjectsMissingInSource.Name = "mnuSkipAllObjectsMissingInSource";
            this.mnuSkipAllObjectsMissingInSource.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuSkipAllObjectsMissingInSource.SmallImage")));
            this.mnuSkipAllObjectsMissingInSource.Text = "Skip all objects Missing in Source";
            this.mnuSkipAllObjectsMissingInSource.Click += new System.EventHandler(this.mnuSkipAllObjectsMissingInSource_Click);
            // 
            // mnuDeleteAllObjectsMissingInSource
            // 
            this.mnuDeleteAllObjectsMissingInSource.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuDeleteAllObjectsMissingInSource.Image = ((System.Drawing.Image)(resources.GetObject("mnuDeleteAllObjectsMissingInSource.Image")));
            this.mnuDeleteAllObjectsMissingInSource.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuDeleteAllObjectsMissingInSource.LargeImage")));
            this.mnuDeleteAllObjectsMissingInSource.Name = "mnuDeleteAllObjectsMissingInSource";
            this.mnuDeleteAllObjectsMissingInSource.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuDeleteAllObjectsMissingInSource.SmallImage")));
            this.mnuDeleteAllObjectsMissingInSource.Text = "Delete all objects Missing in Source";
            this.mnuDeleteAllObjectsMissingInSource.Click += new System.EventHandler(this.mnuDeleteAllObjectsMissingInSource_Click);
            // 
            // mnuSkipAllObjectsMissingInTarget
            // 
            this.mnuSkipAllObjectsMissingInTarget.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuSkipAllObjectsMissingInTarget.Image = ((System.Drawing.Image)(resources.GetObject("mnuSkipAllObjectsMissingInTarget.Image")));
            this.mnuSkipAllObjectsMissingInTarget.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuSkipAllObjectsMissingInTarget.LargeImage")));
            this.mnuSkipAllObjectsMissingInTarget.Name = "mnuSkipAllObjectsMissingInTarget";
            this.mnuSkipAllObjectsMissingInTarget.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuSkipAllObjectsMissingInTarget.SmallImage")));
            this.mnuSkipAllObjectsMissingInTarget.Text = "Skip all objects Missing In Target";
            this.mnuSkipAllObjectsMissingInTarget.Click += new System.EventHandler(this.mnuSkipAllObjectsMissingInTarget_Click);
            // 
            // mnuCreateAllObjectsMissingInTarget
            // 
            this.mnuCreateAllObjectsMissingInTarget.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuCreateAllObjectsMissingInTarget.Image = ((System.Drawing.Image)(resources.GetObject("mnuCreateAllObjectsMissingInTarget.Image")));
            this.mnuCreateAllObjectsMissingInTarget.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuCreateAllObjectsMissingInTarget.LargeImage")));
            this.mnuCreateAllObjectsMissingInTarget.Name = "mnuCreateAllObjectsMissingInTarget";
            this.mnuCreateAllObjectsMissingInTarget.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuCreateAllObjectsMissingInTarget.SmallImage")));
            this.mnuCreateAllObjectsMissingInTarget.Text = "Create all objects Missing in Target";
            this.mnuCreateAllObjectsMissingInTarget.Click += new System.EventHandler(this.mnuCreateAllObjectsMissingInTarget_Click);
            // 
            // mnuSkipAllObjectsWithDifferentDefinitions
            // 
            this.mnuSkipAllObjectsWithDifferentDefinitions.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuSkipAllObjectsWithDifferentDefinitions.Image = ((System.Drawing.Image)(resources.GetObject("mnuSkipAllObjectsWithDifferentDefinitions.Image")));
            this.mnuSkipAllObjectsWithDifferentDefinitions.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuSkipAllObjectsWithDifferentDefinitions.LargeImage")));
            this.mnuSkipAllObjectsWithDifferentDefinitions.Name = "mnuSkipAllObjectsWithDifferentDefinitions";
            this.mnuSkipAllObjectsWithDifferentDefinitions.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuSkipAllObjectsWithDifferentDefinitions.SmallImage")));
            this.mnuSkipAllObjectsWithDifferentDefinitions.Text = "Skip all objects with Different Definitions";
            this.mnuSkipAllObjectsWithDifferentDefinitions.Click += new System.EventHandler(this.mnuSkipAllObjectsWithDifferentDefinitions_Click);
            // 
            // mnuUpdateAllObjectsWithDifferentDefinitions
            // 
            this.mnuUpdateAllObjectsWithDifferentDefinitions.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuUpdateAllObjectsWithDifferentDefinitions.Image = ((System.Drawing.Image)(resources.GetObject("mnuUpdateAllObjectsWithDifferentDefinitions.Image")));
            this.mnuUpdateAllObjectsWithDifferentDefinitions.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuUpdateAllObjectsWithDifferentDefinitions.LargeImage")));
            this.mnuUpdateAllObjectsWithDifferentDefinitions.Name = "mnuUpdateAllObjectsWithDifferentDefinitions";
            this.mnuUpdateAllObjectsWithDifferentDefinitions.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuUpdateAllObjectsWithDifferentDefinitions.SmallImage")));
            this.mnuUpdateAllObjectsWithDifferentDefinitions.Text = "Update all objects with Different Definitions";
            this.mnuUpdateAllObjectsWithDifferentDefinitions.Click += new System.EventHandler(this.mnuUpdateAllObjectsWithDifferentDefinitions_Click);
            // 
            // btnValidateSelection
            // 
            this.btnValidateSelection.Image = ((System.Drawing.Image)(resources.GetObject("btnValidateSelection.Image")));
            this.btnValidateSelection.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnValidateSelection.LargeImage")));
            this.btnValidateSelection.Name = "btnValidateSelection";
            this.btnValidateSelection.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnValidateSelection.SmallImage")));
            this.btnValidateSelection.Text = "Validate Selection";
            this.btnValidateSelection.Click += new System.EventHandler(this.btnValidateSelection_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdate.Image")));
            this.btnUpdate.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnUpdate.LargeImage")));
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnUpdate.SmallImage")));
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnGenerateScript
            // 
            this.btnGenerateScript.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerateScript.Image")));
            this.btnGenerateScript.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnGenerateScript.LargeImage")));
            this.btnGenerateScript.Name = "btnGenerateScript";
            this.btnGenerateScript.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnGenerateScript.SmallImage")));
            this.btnGenerateScript.Text = "Generate Script";
            this.btnGenerateScript.Click += new System.EventHandler(this.btnGenerateScript_Click);
            // 
            // panelMisc
            // 
            this.panelMisc.ButtonMoreVisible = false;
            this.panelMisc.Items.Add(this.btnOptions);
            this.panelMisc.Items.Add(this.btnReportDifferences);
            this.panelMisc.Name = "panelMisc";
            this.panelMisc.Text = null;
            // 
            // btnOptions
            // 
            this.btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.Image")));
            this.btnOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnOptions.LargeImage")));
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnOptions.SmallImage")));
            this.btnOptions.Text = "Options";
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnReportDifferences
            // 
            this.btnReportDifferences.Image = ((System.Drawing.Image)(resources.GetObject("btnReportDifferences.Image")));
            this.btnReportDifferences.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnReportDifferences.LargeImage")));
            this.btnReportDifferences.Name = "btnReportDifferences";
            this.btnReportDifferences.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnReportDifferences.SmallImage")));
            this.btnReportDifferences.Text = "Report Differences";
            this.btnReportDifferences.Click += new System.EventHandler(this.btnReportDifferences_Click);
            // 
            // tabHelp
            // 
            this.tabHelp.Name = "tabHelp";
            this.tabHelp.Panels.Add(this.panelHelp);
            this.tabHelp.Text = "Help";
            // 
            // panelHelp
            // 
            this.panelHelp.ButtonMoreVisible = false;
            this.panelHelp.Items.Add(this.btnHelp);
            this.panelHelp.Name = "panelHelp";
            this.panelHelp.Text = null;
            // 
            // btnHelp
            // 
            this.btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("btnHelp.Image")));
            this.btnHelp.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnHelp.LargeImage")));
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.SmallImage = ((System.Drawing.Image)(resources.GetObject("btnHelp.SmallImage")));
            this.btnHelp.Text = "Info";
            // 
            // pnlRibbon
            // 
            this.pnlRibbon.Controls.Add(this.maqSoftwareLogo);
            this.pnlRibbon.Controls.Add(this.powerBiLogo);
            this.pnlRibbon.Controls.Add(this.ribbonMain);
            this.pnlRibbon.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRibbon.Location = new System.Drawing.Point(0, 0);
            this.pnlRibbon.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.pnlRibbon.Name = "pnlRibbon";
            this.pnlRibbon.Size = new System.Drawing.Size(825, 115);
            this.pnlRibbon.TabIndex = 49;
            // 
            // maqSoftwareLogo
            // 
            this.maqSoftwareLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maqSoftwareLogo.BackColor = System.Drawing.Color.White;
            this.maqSoftwareLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.maqSoftwareLogo.Image = ((System.Drawing.Image)(resources.GetObject("maqSoftwareLogo.Image")));
            this.maqSoftwareLogo.Location = new System.Drawing.Point(511, 32);
            this.maqSoftwareLogo.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.maqSoftwareLogo.Name = "maqSoftwareLogo";
            this.maqSoftwareLogo.Size = new System.Drawing.Size(163, 50);
            this.maqSoftwareLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.maqSoftwareLogo.TabIndex = 3;
            this.maqSoftwareLogo.TabStop = false;
            this.maqSoftwareLogo.Click += new System.EventHandler(this.maqSoftwareLogo_Click);
            // 
            // powerBiLogo
            // 
            this.powerBiLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.powerBiLogo.Image = ((System.Drawing.Image)(resources.GetObject("powerBiLogo.Image")));
            this.powerBiLogo.Location = new System.Drawing.Point(671, 28);
            this.powerBiLogo.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.powerBiLogo.Name = "powerBiLogo";
            this.powerBiLogo.Size = new System.Drawing.Size(149, 58);
            this.powerBiLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.powerBiLogo.TabIndex = 2;
            this.powerBiLogo.TabStop = false;
            // 
            // ribbonMain
            // 
            this.ribbonMain.CaptionBarVisible = false;
            this.ribbonMain.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ribbonMain.Location = new System.Drawing.Point(0, 0);
            this.ribbonMain.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.ribbonMain.Minimized = false;
            this.ribbonMain.Name = "ribbonMain";
            // 
            // 
            // 
            this.ribbonMain.OrbDropDown.BorderRoundness = 8;
            this.ribbonMain.OrbDropDown.Location = new System.Drawing.Point(0, 0);
            this.ribbonMain.OrbDropDown.MenuItems.Add(this.mnuOpen);
            this.ribbonMain.OrbDropDown.MenuItems.Add(this.mnuSave);
            this.ribbonMain.OrbDropDown.MenuItems.Add(this.mnuSaveAs);
            this.ribbonMain.OrbDropDown.MenuItems.Add(this.mnuExit);
            this.ribbonMain.OrbDropDown.Name = "";
            this.ribbonMain.OrbDropDown.Size = new System.Drawing.Size(140, 248);
            this.ribbonMain.OrbDropDown.TabIndex = 0;
            this.ribbonMain.OrbStyle = System.Windows.Forms.RibbonOrbStyle.Office_2013;
            this.ribbonMain.OrbText = "File";
            this.ribbonMain.RibbonTabFont = new System.Drawing.Font("Trebuchet MS", 9F);
            this.ribbonMain.Size = new System.Drawing.Size(825, 90);
            this.ribbonMain.TabIndex = 1;
            this.ribbonMain.Tabs.Add(this.tabHome);
            this.ribbonMain.Tabs.Add(this.tabHelp);
            this.ribbonMain.TabsMargin = new System.Windows.Forms.Padding(5, 2, 20, 0);
            this.ribbonMain.TabSpacing = 4;
            // 
            // mnuOpen
            // 
            this.mnuOpen.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuOpen.Image")));
            this.mnuOpen.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuOpen.LargeImage")));
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuOpen.SmallImage")));
            this.mnuOpen.Text = "Open";
            this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // mnuSave
            // 
            this.mnuSave.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuSave.Image = ((System.Drawing.Image)(resources.GetObject("mnuSave.Image")));
            this.mnuSave.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuSave.LargeImage")));
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuSave.SmallImage")));
            this.mnuSave.Text = "Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuSaveAs
            // 
            this.mnuSaveAs.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("mnuSaveAs.Image")));
            this.mnuSaveAs.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuSaveAs.LargeImage")));
            this.mnuSaveAs.Name = "mnuSaveAs";
            this.mnuSaveAs.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuSaveAs.SmallImage")));
            this.mnuSaveAs.Text = "Save As";
            this.mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
            // 
            // mnuExit
            // 
            this.mnuExit.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.mnuExit.Image = ((System.Drawing.Image)(resources.GetObject("mnuExit.Image")));
            this.mnuExit.LargeImage = ((System.Drawing.Image)(resources.GetObject("mnuExit.LargeImage")));
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.SmallImage = ((System.Drawing.Image)(resources.GetObject("mnuExit.SmallImage")));
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.SystemColors.HighlightText;
            this.pnlHeader.Controls.Add(this.spltSourceTarget);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 115);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(825, 33);
            this.pnlHeader.TabIndex = 50;
            // 
            // spltSourceTarget
            // 
            this.spltSourceTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltSourceTarget.IsSplitterFixed = true;
            this.spltSourceTarget.Location = new System.Drawing.Point(0, 0);
            this.spltSourceTarget.Name = "spltSourceTarget";
            // 
            // spltSourceTarget.Panel1
            // 
            this.spltSourceTarget.Panel1.Controls.Add(this.label1);
            this.spltSourceTarget.Panel1.Controls.Add(this.txtSource);
            this.spltSourceTarget.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // spltSourceTarget.Panel2
            // 
            this.spltSourceTarget.Panel2.Controls.Add(this.txtTarget);
            this.spltSourceTarget.Panel2.Controls.Add(this.label2);
            this.spltSourceTarget.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.spltSourceTarget.Size = new System.Drawing.Size(825, 33);
            this.spltSourceTarget.SplitterDistance = 418;
            this.spltSourceTarget.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "Source";
            // 
            // txtSource
            // 
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSource.Location = new System.Drawing.Point(49, 6);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(330, 20);
            this.txtSource.TabIndex = 41;
            // 
            // txtTarget
            // 
            this.txtTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTarget.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTarget.Location = new System.Drawing.Point(45, 6);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(329, 20);
            this.txtTarget.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 40;
            this.label2.Text = "Target";
            // 
            // ComparisonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 476);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.StatusBarComparsion);
            this.Controls.Add(this.pnlRibbon);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(769, 487);
            this.Name = "ComparisonForm";
            this.Text = "ALM Toolkit for Power BI";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ComparisonForm_Load);
            this.Shown += new System.EventHandler(this.ComparisonForm_Shown);
            this.StatusBarComparsion.ResumeLayout(false);
            this.StatusBarComparsion.PerformLayout();
            this.pnlRibbon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.maqSoftwareLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.powerBiLogo)).EndInit();
            this.pnlHeader.ResumeLayout(false);
            this.spltSourceTarget.Panel1.ResumeLayout(false);
            this.spltSourceTarget.Panel1.PerformLayout();
            this.spltSourceTarget.Panel2.ResumeLayout(false);
            this.spltSourceTarget.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltSourceTarget)).EndInit();
            this.spltSourceTarget.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip StatusBarComparsion;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.RibbonTab tabHome;
        private System.Windows.Forms.RibbonPanel panelCompare;
        private System.Windows.Forms.RibbonButton btnCompareTabularModels;
        private System.Windows.Forms.RibbonButton ddSelectActions;
        private System.Windows.Forms.RibbonButton mnuHideSkipObjects;
        private System.Windows.Forms.RibbonButton mnuHideSkipObjectsWithSameDefinition;
        private System.Windows.Forms.RibbonButton mnuShowSkipObjects;
        private System.Windows.Forms.RibbonSeparator ribbonSeparator1;
        private System.Windows.Forms.RibbonButton mnuSkipAllObjectsMissingInSource;
        private System.Windows.Forms.RibbonButton mnuDeleteAllObjectsMissingInSource;
        private System.Windows.Forms.RibbonButton mnuSkipAllObjectsMissingInTarget;
        private System.Windows.Forms.RibbonButton mnuCreateAllObjectsMissingInTarget;
        private System.Windows.Forms.RibbonButton mnuSkipAllObjectsWithDifferentDefinitions;
        private System.Windows.Forms.RibbonButton mnuUpdateAllObjectsWithDifferentDefinitions;
        private System.Windows.Forms.RibbonButton btnValidateSelection;
        private System.Windows.Forms.RibbonButton btnUpdate;
        private System.Windows.Forms.RibbonButton btnGenerateScript;
        private System.Windows.Forms.RibbonPanel panelMisc;
        private System.Windows.Forms.RibbonButton btnOptions;
        private System.Windows.Forms.RibbonButton btnReportDifferences;
        private System.Windows.Forms.RibbonTab tabHelp;
        private System.Windows.Forms.RibbonPanel panelHelp;
        private System.Windows.Forms.RibbonButton btnHelp;
        private System.Windows.Forms.Panel pnlRibbon;
        private System.Windows.Forms.Ribbon ribbonMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.SplitContainer spltSourceTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox powerBiLogo;
        private System.Windows.Forms.PictureBox maqSoftwareLogo;
        private System.Windows.Forms.RibbonOrbMenuItem mnuOpen;
        private System.Windows.Forms.RibbonOrbMenuItem mnuSave;
        private System.Windows.Forms.RibbonOrbMenuItem mnuSaveAs;
        private System.Windows.Forms.RibbonOrbMenuItem mnuExit;
    }
}

