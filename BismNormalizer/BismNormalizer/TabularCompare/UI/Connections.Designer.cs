namespace BismNormalizer.TabularCompare.UI
{
    partial class Connections
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
            this.cboSourceDatabase = new System.Windows.Forms.ComboBox();
            this.cboSourceServer = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlSourceDb = new System.Windows.Forms.Panel();
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.pnlSourceFile = new System.Windows.Forms.Panel();
            this.btnSourceFileOpen = new System.Windows.Forms.Button();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.rdoSourceFile = new System.Windows.Forms.RadioButton();
            this.pnlSourceProject = new System.Windows.Forms.Panel();
            this.cboSourceProject = new System.Windows.Forms.ComboBox();
            this.rdoSourceProject = new System.Windows.Forms.RadioButton();
            this.rdoSourceDb = new System.Windows.Forms.RadioButton();
            this.grpTarget = new System.Windows.Forms.GroupBox();
            this.pnlTargetFile = new System.Windows.Forms.Panel();
            this.btnTargetFileOpen = new System.Windows.Forms.Button();
            this.txtTargetFile = new System.Windows.Forms.TextBox();
            this.rdoTargetFile = new System.Windows.Forms.RadioButton();
            this.pnlTargetProject = new System.Windows.Forms.Panel();
            this.cboTargetProject = new System.Windows.Forms.ComboBox();
            this.rdoTargetProject = new System.Windows.Forms.RadioButton();
            this.rdoTargetDb = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlTargetDb = new System.Windows.Forms.Panel();
            this.cboTargetServer = new System.Windows.Forms.ComboBox();
            this.cboTargetDatabase = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSwitch = new System.Windows.Forms.Button();
            this.pnlSourceDb.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.pnlSourceFile.SuspendLayout();
            this.pnlSourceProject.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.pnlTargetFile.SuspendLayout();
            this.pnlTargetProject.SuspendLayout();
            this.pnlTargetDb.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboSourceDatabase
            // 
            this.cboSourceDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSourceDatabase.FormattingEnabled = true;
            this.cboSourceDatabase.Location = new System.Drawing.Point(16, 60);
            this.cboSourceDatabase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboSourceDatabase.MaxDropDownItems = 11;
            this.cboSourceDatabase.Name = "cboSourceDatabase";
            this.cboSourceDatabase.Size = new System.Drawing.Size(318, 28);
            this.cboSourceDatabase.TabIndex = 12;
            this.cboSourceDatabase.Enter += new System.EventHandler(this.cboSourceDatabase_Enter);
            // 
            // cboSourceServer
            // 
            this.cboSourceServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSourceServer.FormattingEnabled = true;
            this.cboSourceServer.Location = new System.Drawing.Point(16, 11);
            this.cboSourceServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboSourceServer.MaxDropDownItems = 11;
            this.cboSourceServer.Name = "cboSourceServer";
            this.cboSourceServer.Size = new System.Drawing.Size(318, 28);
            this.cboSourceServer.TabIndex = 9;
            this.cboSourceServer.TextChanged += new System.EventHandler(this.cboSourceServer_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 166);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "Database";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 117);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Server";
            // 
            // pnlSourceDb
            // 
            this.pnlSourceDb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSourceDb.Controls.Add(this.cboSourceServer);
            this.pnlSourceDb.Controls.Add(this.cboSourceDatabase);
            this.pnlSourceDb.Location = new System.Drawing.Point(100, 102);
            this.pnlSourceDb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlSourceDb.Name = "pnlSourceDb";
            this.pnlSourceDb.Size = new System.Drawing.Size(350, 101);
            this.pnlSourceDb.TabIndex = 1;
            // 
            // grpSource
            // 
            this.grpSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpSource.Controls.Add(this.pnlSourceFile);
            this.grpSource.Controls.Add(this.rdoSourceFile);
            this.grpSource.Controls.Add(this.pnlSourceProject);
            this.grpSource.Controls.Add(this.rdoSourceProject);
            this.grpSource.Controls.Add(this.rdoSourceDb);
            this.grpSource.Controls.Add(this.label1);
            this.grpSource.Controls.Add(this.label2);
            this.grpSource.Controls.Add(this.pnlSourceDb);
            this.grpSource.Location = new System.Drawing.Point(18, 18);
            this.grpSource.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpSource.Name = "grpSource";
            this.grpSource.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpSource.Size = new System.Drawing.Size(458, 274);
            this.grpSource.TabIndex = 16;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source";
            // 
            // pnlSourceFile
            // 
            this.pnlSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSourceFile.Controls.Add(this.btnSourceFileOpen);
            this.pnlSourceFile.Controls.Add(this.txtSourceFile);
            this.pnlSourceFile.Location = new System.Drawing.Point(100, 216);
            this.pnlSourceFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlSourceFile.Name = "pnlSourceFile";
            this.pnlSourceFile.Size = new System.Drawing.Size(350, 47);
            this.pnlSourceFile.TabIndex = 20;
            // 
            // btnSourceFileOpen
            // 
            this.btnSourceFileOpen.Location = new System.Drawing.Point(294, 10);
            this.btnSourceFileOpen.Name = "btnSourceFileOpen";
            this.btnSourceFileOpen.Size = new System.Drawing.Size(40, 28);
            this.btnSourceFileOpen.TabIndex = 1;
            this.btnSourceFileOpen.Text = "...";
            this.btnSourceFileOpen.UseVisualStyleBackColor = true;
            this.btnSourceFileOpen.Click += new System.EventHandler(this.btnSourceFileOpen_Click);
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Location = new System.Drawing.Point(16, 11);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.Size = new System.Drawing.Size(271, 26);
            this.txtSourceFile.TabIndex = 0;
            // 
            // rdoSourceFile
            // 
            this.rdoSourceFile.AutoSize = true;
            this.rdoSourceFile.Location = new System.Drawing.Point(10, 231);
            this.rdoSourceFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdoSourceFile.Name = "rdoSourceFile";
            this.rdoSourceFile.Size = new System.Drawing.Size(52, 24);
            this.rdoSourceFile.TabIndex = 19;
            this.rdoSourceFile.Text = "File";
            this.rdoSourceFile.UseVisualStyleBackColor = true;
            this.rdoSourceFile.CheckedChanged += new System.EventHandler(this.rdoSourceFile_CheckedChanged);
            // 
            // pnlSourceProject
            // 
            this.pnlSourceProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSourceProject.Controls.Add(this.cboSourceProject);
            this.pnlSourceProject.Location = new System.Drawing.Point(100, 14);
            this.pnlSourceProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlSourceProject.Name = "pnlSourceProject";
            this.pnlSourceProject.Size = new System.Drawing.Size(350, 47);
            this.pnlSourceProject.TabIndex = 18;
            // 
            // cboSourceProject
            // 
            this.cboSourceProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSourceProject.DisplayMember = "Name";
            this.cboSourceProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSourceProject.FormattingEnabled = true;
            this.cboSourceProject.Location = new System.Drawing.Point(16, 11);
            this.cboSourceProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboSourceProject.MaxDropDownItems = 11;
            this.cboSourceProject.Name = "cboSourceProject";
            this.cboSourceProject.Size = new System.Drawing.Size(318, 28);
            this.cboSourceProject.TabIndex = 9;
            this.cboSourceProject.SelectedIndexChanged += new System.EventHandler(this.cboSourceProject_SelectedIndexChanged);
            // 
            // rdoSourceProject
            // 
            this.rdoSourceProject.AutoSize = true;
            this.rdoSourceProject.Location = new System.Drawing.Point(10, 29);
            this.rdoSourceProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdoSourceProject.Name = "rdoSourceProject";
            this.rdoSourceProject.Size = new System.Drawing.Size(76, 24);
            this.rdoSourceProject.TabIndex = 17;
            this.rdoSourceProject.Text = "Project";
            this.rdoSourceProject.UseVisualStyleBackColor = true;
            this.rdoSourceProject.CheckedChanged += new System.EventHandler(this.rdoSourceProject_CheckedChanged);
            // 
            // rdoSourceDb
            // 
            this.rdoSourceDb.AutoSize = true;
            this.rdoSourceDb.Checked = true;
            this.rdoSourceDb.Location = new System.Drawing.Point(10, 71);
            this.rdoSourceDb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdoSourceDb.Name = "rdoSourceDb";
            this.rdoSourceDb.Size = new System.Drawing.Size(97, 24);
            this.rdoSourceDb.TabIndex = 16;
            this.rdoSourceDb.TabStop = true;
            this.rdoSourceDb.Text = "Database";
            this.rdoSourceDb.UseVisualStyleBackColor = true;
            this.rdoSourceDb.CheckedChanged += new System.EventHandler(this.rdoSourceDb_CheckedChanged);
            // 
            // grpTarget
            // 
            this.grpTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTarget.Controls.Add(this.pnlTargetFile);
            this.grpTarget.Controls.Add(this.rdoTargetFile);
            this.grpTarget.Controls.Add(this.pnlTargetProject);
            this.grpTarget.Controls.Add(this.rdoTargetProject);
            this.grpTarget.Controls.Add(this.rdoTargetDb);
            this.grpTarget.Controls.Add(this.label3);
            this.grpTarget.Controls.Add(this.label4);
            this.grpTarget.Controls.Add(this.pnlTargetDb);
            this.grpTarget.Location = new System.Drawing.Point(548, 18);
            this.grpTarget.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpTarget.Name = "grpTarget";
            this.grpTarget.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpTarget.Size = new System.Drawing.Size(458, 274);
            this.grpTarget.TabIndex = 17;
            this.grpTarget.TabStop = false;
            this.grpTarget.Text = "Target";
            // 
            // pnlTargetFile
            // 
            this.pnlTargetFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTargetFile.Controls.Add(this.btnTargetFileOpen);
            this.pnlTargetFile.Controls.Add(this.txtTargetFile);
            this.pnlTargetFile.Location = new System.Drawing.Point(100, 216);
            this.pnlTargetFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTargetFile.Name = "pnlTargetFile";
            this.pnlTargetFile.Size = new System.Drawing.Size(348, 47);
            this.pnlTargetFile.TabIndex = 20;
            // 
            // btnTargetFileOpen
            // 
            this.btnTargetFileOpen.Location = new System.Drawing.Point(294, 10);
            this.btnTargetFileOpen.Name = "btnTargetFileOpen";
            this.btnTargetFileOpen.Size = new System.Drawing.Size(40, 28);
            this.btnTargetFileOpen.TabIndex = 2;
            this.btnTargetFileOpen.Text = "...";
            this.btnTargetFileOpen.UseVisualStyleBackColor = true;
            this.btnTargetFileOpen.Click += new System.EventHandler(this.btnTargetFileOpen_Click);
            // 
            // txtTargetFile
            // 
            this.txtTargetFile.Location = new System.Drawing.Point(16, 11);
            this.txtTargetFile.Name = "txtTargetFile";
            this.txtTargetFile.Size = new System.Drawing.Size(271, 26);
            this.txtTargetFile.TabIndex = 1;
            // 
            // rdoTargetFile
            // 
            this.rdoTargetFile.AutoSize = true;
            this.rdoTargetFile.Location = new System.Drawing.Point(10, 231);
            this.rdoTargetFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdoTargetFile.Name = "rdoTargetFile";
            this.rdoTargetFile.Size = new System.Drawing.Size(52, 24);
            this.rdoTargetFile.TabIndex = 19;
            this.rdoTargetFile.Text = "File";
            this.rdoTargetFile.UseVisualStyleBackColor = true;
            this.rdoTargetFile.CheckedChanged += new System.EventHandler(this.rdoTargetFile_CheckedChanged);
            // 
            // pnlTargetProject
            // 
            this.pnlTargetProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTargetProject.Controls.Add(this.cboTargetProject);
            this.pnlTargetProject.Location = new System.Drawing.Point(100, 14);
            this.pnlTargetProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTargetProject.Name = "pnlTargetProject";
            this.pnlTargetProject.Size = new System.Drawing.Size(348, 47);
            this.pnlTargetProject.TabIndex = 18;
            // 
            // cboTargetProject
            // 
            this.cboTargetProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetProject.DisplayMember = "Name";
            this.cboTargetProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTargetProject.FormattingEnabled = true;
            this.cboTargetProject.Location = new System.Drawing.Point(16, 11);
            this.cboTargetProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboTargetProject.MaxDropDownItems = 11;
            this.cboTargetProject.Name = "cboTargetProject";
            this.cboTargetProject.Size = new System.Drawing.Size(318, 28);
            this.cboTargetProject.TabIndex = 9;
            // 
            // rdoTargetProject
            // 
            this.rdoTargetProject.AutoSize = true;
            this.rdoTargetProject.Location = new System.Drawing.Point(10, 29);
            this.rdoTargetProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdoTargetProject.Name = "rdoTargetProject";
            this.rdoTargetProject.Size = new System.Drawing.Size(76, 24);
            this.rdoTargetProject.TabIndex = 17;
            this.rdoTargetProject.Text = "Project";
            this.rdoTargetProject.UseVisualStyleBackColor = true;
            this.rdoTargetProject.CheckedChanged += new System.EventHandler(this.rdoTargetProject_CheckedChanged);
            // 
            // rdoTargetDb
            // 
            this.rdoTargetDb.AutoSize = true;
            this.rdoTargetDb.Checked = true;
            this.rdoTargetDb.Location = new System.Drawing.Point(10, 71);
            this.rdoTargetDb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rdoTargetDb.Name = "rdoTargetDb";
            this.rdoTargetDb.Size = new System.Drawing.Size(97, 24);
            this.rdoTargetDb.TabIndex = 16;
            this.rdoTargetDb.TabStop = true;
            this.rdoTargetDb.Text = "Database";
            this.rdoTargetDb.UseVisualStyleBackColor = true;
            this.rdoTargetDb.CheckedChanged += new System.EventHandler(this.rdoTargetDb_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 117);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "Server";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 166);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 20);
            this.label4.TabIndex = 11;
            this.label4.Text = "Database";
            // 
            // pnlTargetDb
            // 
            this.pnlTargetDb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTargetDb.Controls.Add(this.cboTargetServer);
            this.pnlTargetDb.Controls.Add(this.cboTargetDatabase);
            this.pnlTargetDb.Location = new System.Drawing.Point(100, 102);
            this.pnlTargetDb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTargetDb.Name = "pnlTargetDb";
            this.pnlTargetDb.Size = new System.Drawing.Size(348, 101);
            this.pnlTargetDb.TabIndex = 15;
            // 
            // cboTargetServer
            // 
            this.cboTargetServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetServer.FormattingEnabled = true;
            this.cboTargetServer.Location = new System.Drawing.Point(16, 11);
            this.cboTargetServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboTargetServer.MaxDropDownItems = 11;
            this.cboTargetServer.Name = "cboTargetServer";
            this.cboTargetServer.Size = new System.Drawing.Size(318, 28);
            this.cboTargetServer.TabIndex = 9;
            this.cboTargetServer.TextChanged += new System.EventHandler(this.cboTargetServer_TextChanged);
            // 
            // cboTargetDatabase
            // 
            this.cboTargetDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetDatabase.FormattingEnabled = true;
            this.cboTargetDatabase.Location = new System.Drawing.Point(16, 60);
            this.cboTargetDatabase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboTargetDatabase.MaxDropDownItems = 11;
            this.cboTargetDatabase.Name = "cboTargetDatabase";
            this.cboTargetDatabase.Size = new System.Drawing.Size(318, 28);
            this.cboTargetDatabase.TabIndex = 12;
            this.cboTargetDatabase.Enter += new System.EventHandler(this.cboTargetDatabase_Enter);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(759, 308);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 35);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(880, 308);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 35);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSwitch
            // 
            this.btnSwitch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSwitch.BackgroundImage = global::BismNormalizer.Resources.ButtonSwitch;
            this.btnSwitch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSwitch.Location = new System.Drawing.Point(484, 108);
            this.btnSwitch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(54, 49);
            this.btnSwitch.TabIndex = 20;
            this.btnSwitch.UseVisualStyleBackColor = true;
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            // 
            // Connections
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1023, 361);
            this.Controls.Add(this.btnSwitch);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpTarget);
            this.Controls.Add(this.grpSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Connections";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Connections";
            this.Load += new System.EventHandler(this.Connections_Load);
            this.pnlSourceDb.ResumeLayout(false);
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.pnlSourceFile.ResumeLayout(false);
            this.pnlSourceFile.PerformLayout();
            this.pnlSourceProject.ResumeLayout(false);
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
            this.pnlTargetFile.ResumeLayout(false);
            this.pnlTargetFile.PerformLayout();
            this.pnlTargetProject.ResumeLayout(false);
            this.pnlTargetDb.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboSourceDatabase;
        private System.Windows.Forms.ComboBox cboSourceServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlSourceDb;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.RadioButton rdoSourceDb;
        private System.Windows.Forms.RadioButton rdoSourceProject;
        private System.Windows.Forms.ComboBox cboSourceProject;
        private System.Windows.Forms.GroupBox grpTarget;
        private System.Windows.Forms.ComboBox cboTargetProject;
        private System.Windows.Forms.RadioButton rdoTargetProject;
        private System.Windows.Forms.RadioButton rdoTargetDb;
        private System.Windows.Forms.Panel pnlTargetDb;
        private System.Windows.Forms.ComboBox cboTargetServer;
        private System.Windows.Forms.ComboBox cboTargetDatabase;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSwitch;
        private System.Windows.Forms.Panel pnlSourceProject;
        private System.Windows.Forms.Panel pnlTargetProject;
        private System.Windows.Forms.Panel pnlSourceFile;
        private System.Windows.Forms.RadioButton rdoSourceFile;
        private System.Windows.Forms.Panel pnlTargetFile;
        private System.Windows.Forms.RadioButton rdoTargetFile;
        private System.Windows.Forms.Button btnSourceFileOpen;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.Button btnTargetFileOpen;
        private System.Windows.Forms.TextBox txtTargetFile;
    }
}