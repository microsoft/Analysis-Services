namespace BismNormalizer.TabularCompare.UI
{
    partial class ComparisonControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComparisonControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.spltSourceTarget = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnCompareTabularModels = new System.Windows.Forms.ToolStripButton();
            this.ddSelectActions = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuHideSkipObjects = new System.Windows.Forms.ToolStripMenuItem();
            this.hideSkipObjectsWithSameDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowSkipObjects = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSkipAllObjectsMissingInSource = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeleteAllObjectsMissingInSource = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSkipAllObjectsMissingInTarget = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCreateAllObjectsMissingInTarget = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSkipAllObjectsWithDifferentDefinitions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUpdateAllObjectsWithDifferentDefinitions = new System.Windows.Forms.ToolStripMenuItem();
            this.btnValidateSelection = new System.Windows.Forms.ToolStripButton();
            this.btnUpdate = new System.Windows.Forms.ToolStripButton();
            this.btnGenerateScript = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOptions = new System.Windows.Forms.ToolStripButton();
            this.btnReportDifferences = new System.Windows.Forms.ToolStripButton();
            this.scDifferenceResults = new System.Windows.Forms.SplitContainer();
            this.TreeGridImageList = new System.Windows.Forms.ImageList(this.components);
            this.scObjectDefinitions = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.pnlProgressBar = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.treeGridComparisonResults = new BismNormalizer.TabularCompare.UI.TreeGridViewComparison();
            this.txtSourceObjectDefinition = new BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox();
            this.txtTargetObjectDefinition = new BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltSourceTarget)).BeginInit();
            this.spltSourceTarget.Panel1.SuspendLayout();
            this.spltSourceTarget.Panel2.SuspendLayout();
            this.spltSourceTarget.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDifferenceResults)).BeginInit();
            this.scDifferenceResults.Panel1.SuspendLayout();
            this.scDifferenceResults.Panel2.SuspendLayout();
            this.scDifferenceResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scObjectDefinitions)).BeginInit();
            this.scObjectDefinitions.Panel1.SuspendLayout();
            this.scObjectDefinitions.Panel2.SuspendLayout();
            this.scObjectDefinitions.SuspendLayout();
            this.pnlProgressBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridComparisonResults)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.spltSourceTarget);
            this.pnlHeader.Controls.Add(this.toolStrip1);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(980, 85);
            this.pnlHeader.TabIndex = 46;
            // 
            // spltSourceTarget
            // 
            this.spltSourceTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltSourceTarget.IsSplitterFixed = true;
            this.spltSourceTarget.Location = new System.Drawing.Point(0, 25);
            this.spltSourceTarget.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.spltSourceTarget.Name = "spltSourceTarget";
            // 
            // spltSourceTarget.Panel1
            // 
            this.spltSourceTarget.Panel1.Controls.Add(this.label1);
            this.spltSourceTarget.Panel1.Controls.Add(this.txtSource);
            // 
            // spltSourceTarget.Panel2
            // 
            this.spltSourceTarget.Panel2.Controls.Add(this.txtTarget);
            this.spltSourceTarget.Panel2.Controls.Add(this.label2);
            this.spltSourceTarget.Size = new System.Drawing.Size(980, 60);
            this.spltSourceTarget.SplitterDistance = 481;
            this.spltSourceTarget.SplitterWidth = 6;
            this.spltSourceTarget.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 20);
            this.label1.TabIndex = 39;
            this.label1.Text = "Source";
            // 
            // txtSource
            // 
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.BackColor = System.Drawing.SystemColors.Control;
            this.txtSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSource.Location = new System.Drawing.Point(74, 11);
            this.txtSource.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(401, 26);
            this.txtSource.TabIndex = 41;
            this.txtSource.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_KeyDown);
            // 
            // txtTarget
            // 
            this.txtTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTarget.BackColor = System.Drawing.SystemColors.Control;
            this.txtTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTarget.Location = new System.Drawing.Point(68, 11);
            this.txtTarget.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(408, 26);
            this.txtTarget.TabIndex = 42;
            this.txtTarget.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_KeyDown);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 20);
            this.label2.TabIndex = 40;
            this.label2.Text = "Target";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCompareTabularModels,
            this.ddSelectActions,
            this.btnValidateSelection,
            this.btnUpdate,
            this.btnGenerateScript,
            this.toolStripButton1,
            this.btnOptions,
            this.btnReportDifferences});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(980, 25);
            this.toolStrip1.TabIndex = 46;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnCompareTabularModels
            // 
            this.btnCompareTabularModels.Image = ((System.Drawing.Image)(resources.GetObject("btnCompareTabularModels.Image")));
            this.btnCompareTabularModels.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompareTabularModels.Name = "btnCompareTabularModels";
            this.btnCompareTabularModels.Size = new System.Drawing.Size(85, 22);
            this.btnCompareTabularModels.Text = "Compare...";
            this.btnCompareTabularModels.ToolTipText = "Compare (Shift+Alt+C)";
            this.btnCompareTabularModels.Click += new System.EventHandler(this.btnCompareTabularModels_Click);
            // 
            // ddSelectActions
            // 
            this.ddSelectActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHideSkipObjects,
            this.hideSkipObjectsWithSameDefinitionToolStripMenuItem,
            this.mnuShowSkipObjects,
            this.toolStripSeparator1,
            this.mnuSkipAllObjectsMissingInSource,
            this.mnuDeleteAllObjectsMissingInSource,
            this.mnuSkipAllObjectsMissingInTarget,
            this.mnuCreateAllObjectsMissingInTarget,
            this.mnuSkipAllObjectsWithDifferentDefinitions,
            this.mnuUpdateAllObjectsWithDifferentDefinitions});
            this.ddSelectActions.Enabled = false;
            this.ddSelectActions.Image = ((System.Drawing.Image)(resources.GetObject("ddSelectActions.Image")));
            this.ddSelectActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddSelectActions.Name = "ddSelectActions";
            this.ddSelectActions.Size = new System.Drawing.Size(110, 22);
            this.ddSelectActions.Text = "Select Actions";
            // 
            // mnuHideSkipObjects
            // 
            this.mnuHideSkipObjects.Name = "mnuHideSkipObjects";
            this.mnuHideSkipObjects.Size = new System.Drawing.Size(303, 22);
            this.mnuHideSkipObjects.Text = "Hide Skip Objects";
            this.mnuHideSkipObjects.Click += new System.EventHandler(this.mnuHideSkipObjects_Click);
            // 
            // hideSkipObjectsWithSameDefinitionToolStripMenuItem
            // 
            this.hideSkipObjectsWithSameDefinitionToolStripMenuItem.Name = "hideSkipObjectsWithSameDefinitionToolStripMenuItem";
            this.hideSkipObjectsWithSameDefinitionToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.hideSkipObjectsWithSameDefinitionToolStripMenuItem.Text = "Hide Skip Objects with Same Definition";
            this.hideSkipObjectsWithSameDefinitionToolStripMenuItem.Click += new System.EventHandler(this.mnuHideSkipObjectsWithSameDefinition_Click);
            // 
            // mnuShowSkipObjects
            // 
            this.mnuShowSkipObjects.Name = "mnuShowSkipObjects";
            this.mnuShowSkipObjects.Size = new System.Drawing.Size(303, 22);
            this.mnuShowSkipObjects.Text = "Show Skip Objects";
            this.mnuShowSkipObjects.Click += new System.EventHandler(this.mnuShowSkipObjects_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(300, 6);
            // 
            // mnuSkipAllObjectsMissingInSource
            // 
            this.mnuSkipAllObjectsMissingInSource.Name = "mnuSkipAllObjectsMissingInSource";
            this.mnuSkipAllObjectsMissingInSource.Size = new System.Drawing.Size(303, 22);
            this.mnuSkipAllObjectsMissingInSource.Text = "Skip all objects Missing in Source";
            this.mnuSkipAllObjectsMissingInSource.Click += new System.EventHandler(this.mnuSkipAllObjectsMissingInSource_Click);
            // 
            // mnuDeleteAllObjectsMissingInSource
            // 
            this.mnuDeleteAllObjectsMissingInSource.Name = "mnuDeleteAllObjectsMissingInSource";
            this.mnuDeleteAllObjectsMissingInSource.Size = new System.Drawing.Size(303, 22);
            this.mnuDeleteAllObjectsMissingInSource.Text = "Delete all objects Missing in Source";
            this.mnuDeleteAllObjectsMissingInSource.Click += new System.EventHandler(this.mnuDeleteAllObjectsMissingInSource_Click);
            // 
            // mnuSkipAllObjectsMissingInTarget
            // 
            this.mnuSkipAllObjectsMissingInTarget.Name = "mnuSkipAllObjectsMissingInTarget";
            this.mnuSkipAllObjectsMissingInTarget.Size = new System.Drawing.Size(303, 22);
            this.mnuSkipAllObjectsMissingInTarget.Text = "Skip all objects Missing in Target";
            this.mnuSkipAllObjectsMissingInTarget.Click += new System.EventHandler(this.mnuSkipAllObjectsMissingInTarget_Click);
            // 
            // mnuCreateAllObjectsMissingInTarget
            // 
            this.mnuCreateAllObjectsMissingInTarget.Name = "mnuCreateAllObjectsMissingInTarget";
            this.mnuCreateAllObjectsMissingInTarget.Size = new System.Drawing.Size(303, 22);
            this.mnuCreateAllObjectsMissingInTarget.Text = "Create all objects Missing in Target";
            this.mnuCreateAllObjectsMissingInTarget.Click += new System.EventHandler(this.mnuCreateAllObjectsMissingInTarget_Click);
            // 
            // mnuSkipAllObjectsWithDifferentDefinitions
            // 
            this.mnuSkipAllObjectsWithDifferentDefinitions.Name = "mnuSkipAllObjectsWithDifferentDefinitions";
            this.mnuSkipAllObjectsWithDifferentDefinitions.Size = new System.Drawing.Size(303, 22);
            this.mnuSkipAllObjectsWithDifferentDefinitions.Text = "Skip all objects with Different Definitions";
            this.mnuSkipAllObjectsWithDifferentDefinitions.Click += new System.EventHandler(this.mnuSkipAllObjectsWithDifferentDefinitions_Click);
            // 
            // mnuUpdateAllObjectsWithDifferentDefinitions
            // 
            this.mnuUpdateAllObjectsWithDifferentDefinitions.Name = "mnuUpdateAllObjectsWithDifferentDefinitions";
            this.mnuUpdateAllObjectsWithDifferentDefinitions.Size = new System.Drawing.Size(303, 22);
            this.mnuUpdateAllObjectsWithDifferentDefinitions.Text = "Update all objects with Different Definitions";
            this.mnuUpdateAllObjectsWithDifferentDefinitions.Click += new System.EventHandler(this.mnuUpdateAllObjectsWithDifferentDefinitions_Click);
            // 
            // btnValidateSelection
            // 
            this.btnValidateSelection.Enabled = false;
            this.btnValidateSelection.Image = ((System.Drawing.Image)(resources.GetObject("btnValidateSelection.Image")));
            this.btnValidateSelection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnValidateSelection.Name = "btnValidateSelection";
            this.btnValidateSelection.Size = new System.Drawing.Size(119, 22);
            this.btnValidateSelection.Text = "Validate Selection";
            this.btnValidateSelection.Click += new System.EventHandler(this.btnValidateSelection_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdate.Image")));
            this.btnUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(65, 22);
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnGenerateScript
            // 
            this.btnGenerateScript.Enabled = false;
            this.btnGenerateScript.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerateScript.Image")));
            this.btnGenerateScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGenerateScript.Name = "btnGenerateScript";
            this.btnGenerateScript.Size = new System.Drawing.Size(107, 22);
            this.btnGenerateScript.Text = "Generate Script";
            this.btnGenerateScript.Click += new System.EventHandler(this.btnGenerateScript_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOptions
            // 
            this.btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.Image")));
            this.btnOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(69, 22);
            this.btnOptions.Text = "Options";
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnReportDifferences
            // 
            this.btnReportDifferences.Enabled = false;
            this.btnReportDifferences.Image = ((System.Drawing.Image)(resources.GetObject("btnReportDifferences.Image")));
            this.btnReportDifferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReportDifferences.Name = "btnReportDifferences";
            this.btnReportDifferences.Size = new System.Drawing.Size(124, 22);
            this.btnReportDifferences.Text = "Report Differences";
            this.btnReportDifferences.Click += new System.EventHandler(this.btnReportDifferences_Click);
            // 
            // scDifferenceResults
            // 
            this.scDifferenceResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scDifferenceResults.Location = new System.Drawing.Point(0, 85);
            this.scDifferenceResults.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.scDifferenceResults.Name = "scDifferenceResults";
            this.scDifferenceResults.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scDifferenceResults.Panel1
            // 
            this.scDifferenceResults.Panel1.Controls.Add(this.pnlProgressBar);
            this.scDifferenceResults.Panel1.Controls.Add(this.treeGridComparisonResults);
            // 
            // scDifferenceResults.Panel2
            // 
            this.scDifferenceResults.Panel2.Controls.Add(this.scObjectDefinitions);
            this.scDifferenceResults.Size = new System.Drawing.Size(980, 784);
            this.scDifferenceResults.SplitterDistance = 570;
            this.scDifferenceResults.SplitterWidth = 6;
            this.scDifferenceResults.TabIndex = 2;
            // 
            // TreeGridImageList
            // 
            this.TreeGridImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeGridImageList.ImageStream")));
            this.TreeGridImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.TreeGridImageList.Images.SetKeyName(0, "Connection.png");
            this.TreeGridImageList.Images.SetKeyName(1, "Table.png");
            this.TreeGridImageList.Images.SetKeyName(2, "Relationship.png");
            this.TreeGridImageList.Images.SetKeyName(3, "BismMeasure.png");
            this.TreeGridImageList.Images.SetKeyName(4, "KPI.png");
            this.TreeGridImageList.Images.SetKeyName(5, "DeleteAction.png");
            this.TreeGridImageList.Images.SetKeyName(6, "UpdateAction.png");
            this.TreeGridImageList.Images.SetKeyName(7, "CreateAction.png");
            this.TreeGridImageList.Images.SetKeyName(8, "SkipAction.png");
            this.TreeGridImageList.Images.SetKeyName(9, "Plus.png");
            this.TreeGridImageList.Images.SetKeyName(10, "Minus.png");
            this.TreeGridImageList.Images.SetKeyName(11, "Informational.png");
            this.TreeGridImageList.Images.SetKeyName(12, "Warning.png");
            this.TreeGridImageList.Images.SetKeyName(13, "WarningToolWindow.png");
            this.TreeGridImageList.Images.SetKeyName(14, "Role.png");
            this.TreeGridImageList.Images.SetKeyName(15, "Perspective.png");
            this.TreeGridImageList.Images.SetKeyName(16, "Action.png");
            this.TreeGridImageList.Images.SetKeyName(17, "CompareBismModels_Small.png");
            this.TreeGridImageList.Images.SetKeyName(18, "DeleteActionGrey.png");
            this.TreeGridImageList.Images.SetKeyName(19, "SkipActionGrey.png");
            this.TreeGridImageList.Images.SetKeyName(20, "CreateActionGrey.png");
            this.TreeGridImageList.Images.SetKeyName(21, "Culture.png");
            this.TreeGridImageList.Images.SetKeyName(22, "Expression.png");
            // 
            // scObjectDefinitions
            // 
            this.scObjectDefinitions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scObjectDefinitions.Location = new System.Drawing.Point(0, 0);
            this.scObjectDefinitions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.scObjectDefinitions.Name = "scObjectDefinitions";
            // 
            // scObjectDefinitions.Panel1
            // 
            this.scObjectDefinitions.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.scObjectDefinitions.Panel1.Controls.Add(this.txtSourceObjectDefinition);
            this.scObjectDefinitions.Panel1.Controls.Add(this.label4);
            // 
            // scObjectDefinitions.Panel2
            // 
            this.scObjectDefinitions.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.scObjectDefinitions.Panel2.Controls.Add(this.txtTargetObjectDefinition);
            this.scObjectDefinitions.Panel2.Controls.Add(this.label5);
            this.scObjectDefinitions.Size = new System.Drawing.Size(980, 208);
            this.scObjectDefinitions.SplitterDistance = 496;
            this.scObjectDefinitions.SplitterWidth = 6;
            this.scObjectDefinitions.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 2);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(181, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Source Object Definition";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 2);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 20);
            this.label5.TabIndex = 1;
            this.label5.Text = "Target Object Definition";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(150, 16);
            // 
            // pnlProgressBar
            // 
            this.pnlProgressBar.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.pnlProgressBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.progressBar});
            this.pnlProgressBar.Location = new System.Drawing.Point(0, 548);
            this.pnlProgressBar.Name = "pnlProgressBar";
            this.pnlProgressBar.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.pnlProgressBar.Size = new System.Drawing.Size(980, 22);
            this.pnlProgressBar.TabIndex = 49;
            this.pnlProgressBar.Text = "Comparison Status";
            this.pnlProgressBar.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // treeGridComparisonResults
            // 
            this.treeGridComparisonResults.AllowUserToAddRows = false;
            this.treeGridComparisonResults.AllowUserToDeleteRows = false;
            this.treeGridComparisonResults.AllowUserToResizeRows = false;
            this.treeGridComparisonResults.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.treeGridComparisonResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.treeGridComparisonResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.treeGridComparisonResults.Comparison = null;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.treeGridComparisonResults.DefaultCellStyle = dataGridViewCellStyle2;
            this.treeGridComparisonResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGridComparisonResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.treeGridComparisonResults.ImageList = this.TreeGridImageList;
            this.treeGridComparisonResults.Location = new System.Drawing.Point(0, 0);
            this.treeGridComparisonResults.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.treeGridComparisonResults.Name = "treeGridComparisonResults";
            this.treeGridComparisonResults.RowHeadersVisible = false;
            this.treeGridComparisonResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.treeGridComparisonResults.Size = new System.Drawing.Size(980, 570);
            this.treeGridComparisonResults.TabIndex = 0;
            this.treeGridComparisonResults.Unloading = false;
            this.treeGridComparisonResults.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.treeGridComparisonResults_DataError);
            this.treeGridComparisonResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeGridComparisonResults_MouseUp);
            // 
            // txtSourceObjectDefinition
            // 
            this.txtSourceObjectDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceObjectDefinition.BackColor = System.Drawing.Color.White;
            this.txtSourceObjectDefinition.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSourceObjectDefinition.Location = new System.Drawing.Point(0, 25);
            this.txtSourceObjectDefinition.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSourceObjectDefinition.Name = "txtSourceObjectDefinition";
            this.txtSourceObjectDefinition.ReadOnly = true;
            this.txtSourceObjectDefinition.Size = new System.Drawing.Size(494, 181);
            this.txtSourceObjectDefinition.TabIndex = 1;
            this.txtSourceObjectDefinition.Text = "";
            this.txtSourceObjectDefinition.WordWrap = false;
            this.txtSourceObjectDefinition.vScroll += new BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox.vScrollEventHandler(this.txtSourceObjectDefinition_vScroll);
            this.txtSourceObjectDefinition.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSourceObjectDefinition_KeyUp);
            // 
            // txtTargetObjectDefinition
            // 
            this.txtTargetObjectDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetObjectDefinition.BackColor = System.Drawing.Color.White;
            this.txtTargetObjectDefinition.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetObjectDefinition.Location = new System.Drawing.Point(0, 25);
            this.txtTargetObjectDefinition.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTargetObjectDefinition.Name = "txtTargetObjectDefinition";
            this.txtTargetObjectDefinition.ReadOnly = true;
            this.txtTargetObjectDefinition.Size = new System.Drawing.Size(469, 181);
            this.txtTargetObjectDefinition.TabIndex = 2;
            this.txtTargetObjectDefinition.Text = "";
            this.txtTargetObjectDefinition.WordWrap = false;
            this.txtTargetObjectDefinition.vScroll += new BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox.vScrollEventHandler(this.txtTargetObjectDefinition_vScroll);
            this.txtTargetObjectDefinition.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtTargetObjectDefinition_KeyUp);
            // 
            // ComparisonControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scDifferenceResults);
            this.Controls.Add(this.pnlHeader);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ComparisonControl";
            this.Size = new System.Drawing.Size(980, 869);
            this.Load += new System.EventHandler(this.BismNormalizer_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.spltSourceTarget.Panel1.ResumeLayout(false);
            this.spltSourceTarget.Panel1.PerformLayout();
            this.spltSourceTarget.Panel2.ResumeLayout(false);
            this.spltSourceTarget.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltSourceTarget)).EndInit();
            this.spltSourceTarget.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.scDifferenceResults.Panel1.ResumeLayout(false);
            this.scDifferenceResults.Panel1.PerformLayout();
            this.scDifferenceResults.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDifferenceResults)).EndInit();
            this.scDifferenceResults.ResumeLayout(false);
            this.scObjectDefinitions.Panel1.ResumeLayout(false);
            this.scObjectDefinitions.Panel1.PerformLayout();
            this.scObjectDefinitions.Panel2.ResumeLayout(false);
            this.scObjectDefinitions.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scObjectDefinitions)).EndInit();
            this.scObjectDefinitions.ResumeLayout(false);
            this.pnlProgressBar.ResumeLayout(false);
            this.pnlProgressBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridComparisonResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TreeGridViewComparison treeGridComparisonResults;
        private System.Windows.Forms.SplitContainer scDifferenceResults;
        private System.Windows.Forms.SplitContainer scObjectDefinitions;
        private SynchronizedScrollRichTextBox txtSourceObjectDefinition;
        private System.Windows.Forms.Label label4;
        private SynchronizedScrollRichTextBox txtTargetObjectDefinition;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlHeader;
        public System.Windows.Forms.ImageList TreeGridImageList;
        private System.Windows.Forms.SplitContainer spltSourceTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnCompareTabularModels;
        private System.Windows.Forms.ToolStripDropDownButton ddSelectActions;
        private System.Windows.Forms.ToolStripMenuItem mnuHideSkipObjects;
        private System.Windows.Forms.ToolStripMenuItem mnuShowSkipObjects;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuSkipAllObjectsMissingInSource;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteAllObjectsMissingInSource;
        private System.Windows.Forms.ToolStripMenuItem mnuSkipAllObjectsMissingInTarget;
        private System.Windows.Forms.ToolStripMenuItem mnuCreateAllObjectsMissingInTarget;
        private System.Windows.Forms.ToolStripMenuItem mnuSkipAllObjectsWithDifferentDefinitions;
        private System.Windows.Forms.ToolStripMenuItem mnuUpdateAllObjectsWithDifferentDefinitions;
        private System.Windows.Forms.ToolStripButton btnValidateSelection;
        private System.Windows.Forms.ToolStripButton btnUpdate;
        private System.Windows.Forms.ToolStripButton btnGenerateScript;
        private System.Windows.Forms.ToolStripSeparator toolStripButton1;
        private System.Windows.Forms.ToolStripButton btnOptions;
        private System.Windows.Forms.ToolStripButton btnReportDifferences;
        private System.Windows.Forms.ToolStripMenuItem hideSkipObjectsWithSameDefinitionToolStripMenuItem;
        private System.Windows.Forms.StatusStrip pnlProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
    }
}
