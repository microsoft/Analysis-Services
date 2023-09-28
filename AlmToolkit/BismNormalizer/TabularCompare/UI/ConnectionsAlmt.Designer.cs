namespace BismNormalizer.TabularCompare.UI
{
    partial class ConnectionsAlmt
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
            this.pnlSourceDataset = new System.Windows.Forms.Panel();
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.pnlSourceFile = new System.Windows.Forms.Panel();
            this.btnSourceFileOpen = new System.Windows.Forms.Button();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.pnlSourceDesktop = new System.Windows.Forms.Panel();
            this.cboSourceDesktop = new System.Windows.Forms.ComboBox();
            this.rdoSourceFile = new System.Windows.Forms.RadioButton();
            this.rdoSourceDesktop = new System.Windows.Forms.RadioButton();
            this.rdoSourceDataset = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSwitch = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grpTarget = new System.Windows.Forms.GroupBox();
            this.pnlTargetFile = new System.Windows.Forms.Panel();
            this.btnTargetFileOpen = new System.Windows.Forms.Button();
            this.txtTargetFile = new System.Windows.Forms.TextBox();
            this.rdoTargetFile = new System.Windows.Forms.RadioButton();
            this.pnlTargetDesktop = new System.Windows.Forms.Panel();
            this.cboTargetDesktop = new System.Windows.Forms.ComboBox();
            this.rdoTargetDesktop = new System.Windows.Forms.RadioButton();
            this.rdoTargetDataset = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlTargetDataset = new System.Windows.Forms.Panel();
            this.cboTargetServer = new System.Windows.Forms.ComboBox();
            this.cboTargetDatabase = new System.Windows.Forms.ComboBox();
            this.pnlSourceDataset.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.pnlSourceFile.SuspendLayout();
            this.pnlSourceDesktop.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.pnlTargetFile.SuspendLayout();
            this.pnlTargetDesktop.SuspendLayout();
            this.pnlTargetDataset.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboSourceDatabase
            // 
            this.cboSourceDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSourceDatabase.FormattingEnabled = true;
            this.cboSourceDatabase.Location = new System.Drawing.Point(22, 75);
            this.cboSourceDatabase.Margin = new System.Windows.Forms.Padding(6);
            this.cboSourceDatabase.MaxDropDownItems = 11;
            this.cboSourceDatabase.Name = "cboSourceDatabase";
            this.cboSourceDatabase.Size = new System.Drawing.Size(808, 34);
            this.cboSourceDatabase.TabIndex = 2;
            this.cboSourceDatabase.Enter += new System.EventHandler(this.cboSourceDatabase_Enter);
            // 
            // cboSourceServer
            // 
            this.cboSourceServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSourceServer.FormattingEnabled = true;
            this.cboSourceServer.Location = new System.Drawing.Point(22, 14);
            this.cboSourceServer.Margin = new System.Windows.Forms.Padding(6);
            this.cboSourceServer.MaxDropDownItems = 11;
            this.cboSourceServer.Name = "cboSourceServer";
            this.cboSourceServer.Size = new System.Drawing.Size(808, 34);
            this.cboSourceServer.TabIndex = 1;
            this.cboSourceServer.TextChanged += new System.EventHandler(this.cboSourceServer_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 191);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Dataset";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 128);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Workspace";
            // 
            // pnlSourceDataset
            // 
            this.pnlSourceDataset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSourceDataset.Controls.Add(this.cboSourceServer);
            this.pnlSourceDataset.Controls.Add(this.cboSourceDatabase);
            this.pnlSourceDataset.Location = new System.Drawing.Point(200, 109);
            this.pnlSourceDataset.Margin = new System.Windows.Forms.Padding(6);
            this.pnlSourceDataset.Name = "pnlSourceDataset";
            this.pnlSourceDataset.Size = new System.Drawing.Size(852, 127);
            this.pnlSourceDataset.TabIndex = 1;
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.pnlSourceFile);
            this.grpSource.Controls.Add(this.pnlSourceDesktop);
            this.grpSource.Controls.Add(this.rdoSourceFile);
            this.grpSource.Controls.Add(this.rdoSourceDesktop);
            this.grpSource.Controls.Add(this.rdoSourceDataset);
            this.grpSource.Controls.Add(this.label5);
            this.grpSource.Controls.Add(this.label1);
            this.grpSource.Controls.Add(this.label2);
            this.grpSource.Controls.Add(this.pnlSourceDataset);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSource.Location = new System.Drawing.Point(0, 0);
            this.grpSource.Margin = new System.Windows.Forms.Padding(6);
            this.grpSource.Name = "grpSource";
            this.grpSource.Padding = new System.Windows.Forms.Padding(6);
            this.grpSource.Size = new System.Drawing.Size(1064, 406);
            this.grpSource.TabIndex = 16;
            this.grpSource.TabStop = false;
            // 
            // pnlSourceFile
            // 
            this.pnlSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSourceFile.Controls.Add(this.btnSourceFileOpen);
            this.pnlSourceFile.Controls.Add(this.txtSourceFile);
            this.pnlSourceFile.Enabled = false;
            this.pnlSourceFile.Location = new System.Drawing.Point(200, 319);
            this.pnlSourceFile.Margin = new System.Windows.Forms.Padding(6);
            this.pnlSourceFile.Name = "pnlSourceFile";
            this.pnlSourceFile.Size = new System.Drawing.Size(852, 59);
            this.pnlSourceFile.TabIndex = 21;
            // 
            // btnSourceFileOpen
            // 
            this.btnSourceFileOpen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSourceFileOpen.Location = new System.Drawing.Point(780, 14);
            this.btnSourceFileOpen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSourceFileOpen.Name = "btnSourceFileOpen";
            this.btnSourceFileOpen.Size = new System.Drawing.Size(54, 39);
            this.btnSourceFileOpen.TabIndex = 7;
            this.btnSourceFileOpen.Text = "...";
            this.btnSourceFileOpen.UseVisualStyleBackColor = true;
            this.btnSourceFileOpen.Click += new System.EventHandler(this.btnSourceFileOpen_Click);
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFile.Location = new System.Drawing.Point(22, 14);
            this.txtSourceFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.Size = new System.Drawing.Size(745, 32);
            this.txtSourceFile.TabIndex = 6;
            // 
            // pnlSourceDesktop
            // 
            this.pnlSourceDesktop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSourceDesktop.Controls.Add(this.cboSourceDesktop);
            this.pnlSourceDesktop.Enabled = false;
            this.pnlSourceDesktop.Location = new System.Drawing.Point(344, 248);
            this.pnlSourceDesktop.Margin = new System.Windows.Forms.Padding(6);
            this.pnlSourceDesktop.Name = "pnlSourceDesktop";
            this.pnlSourceDesktop.Size = new System.Drawing.Size(708, 59);
            this.pnlSourceDesktop.TabIndex = 19;
            // 
            // cboSourceDesktop
            // 
            this.cboSourceDesktop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSourceDesktop.DisplayMember = "Name";
            this.cboSourceDesktop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSourceDesktop.FormattingEnabled = true;
            this.cboSourceDesktop.Location = new System.Drawing.Point(22, 16);
            this.cboSourceDesktop.Margin = new System.Windows.Forms.Padding(6);
            this.cboSourceDesktop.MaxDropDownItems = 11;
            this.cboSourceDesktop.Name = "cboSourceDesktop";
            this.cboSourceDesktop.Size = new System.Drawing.Size(664, 34);
            this.cboSourceDesktop.TabIndex = 4;
            // 
            // rdoSourceFile
            // 
            this.rdoSourceFile.AutoSize = true;
            this.rdoSourceFile.Location = new System.Drawing.Point(30, 334);
            this.rdoSourceFile.Margin = new System.Windows.Forms.Padding(6);
            this.rdoSourceFile.Name = "rdoSourceFile";
            this.rdoSourceFile.Size = new System.Drawing.Size(78, 30);
            this.rdoSourceFile.TabIndex = 5;
            this.rdoSourceFile.Text = "File";
            this.rdoSourceFile.UseVisualStyleBackColor = true;
            this.rdoSourceFile.CheckedChanged += new System.EventHandler(this.rdoSourceFile_CheckedChanged);
            // 
            // rdoSourceDesktop
            // 
            this.rdoSourceDesktop.AutoSize = true;
            this.rdoSourceDesktop.Location = new System.Drawing.Point(30, 266);
            this.rdoSourceDesktop.Margin = new System.Windows.Forms.Padding(6);
            this.rdoSourceDesktop.Name = "rdoSourceDesktop";
            this.rdoSourceDesktop.Size = new System.Drawing.Size(218, 30);
            this.rdoSourceDesktop.TabIndex = 3;
            this.rdoSourceDesktop.Text = "Power BI Desktop";
            this.rdoSourceDesktop.UseVisualStyleBackColor = true;
            this.rdoSourceDesktop.CheckedChanged += new System.EventHandler(this.rdoSourceDesktop_CheckedChanged);
            // 
            // rdoSourceDataset
            // 
            this.rdoSourceDataset.AutoSize = true;
            this.rdoSourceDataset.Checked = true;
            this.rdoSourceDataset.Location = new System.Drawing.Point(30, 66);
            this.rdoSourceDataset.Margin = new System.Windows.Forms.Padding(6);
            this.rdoSourceDataset.Name = "rdoSourceDataset";
            this.rdoSourceDataset.Size = new System.Drawing.Size(118, 30);
            this.rdoSourceDataset.TabIndex = 0;
            this.rdoSourceDataset.TabStop = true;
            this.rdoSourceDataset.Text = "Dataset";
            this.rdoSourceDataset.UseVisualStyleBackColor = true;
            this.rdoSourceDataset.CheckedChanged += new System.EventHandler(this.rdoSourceDataset_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(24, 17);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 31);
            this.label5.TabIndex = 3;
            this.label5.Text = "Source";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(744, 19);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(138, 44);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(894, 19);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(142, 44);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSwitch
            // 
            this.btnSwitch.BackgroundImage = global::BismNormalizer.Resources.ButtonSwitch;
            this.btnSwitch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSwitch.Location = new System.Drawing.Point(472, 6);
            this.btnSwitch.Margin = new System.Windows.Forms.Padding(6);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(112, 61);
            this.btnSwitch.TabIndex = 8;
            this.btnSwitch.UseVisualStyleBackColor = true;
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 923);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1064, 86);
            this.panel2.TabIndex = 22;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSwitch);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 406);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1064, 73);
            this.panel1.TabIndex = 25;
            // 
            // grpTarget
            // 
            this.grpTarget.Controls.Add(this.pnlTargetFile);
            this.grpTarget.Controls.Add(this.rdoTargetFile);
            this.grpTarget.Controls.Add(this.pnlTargetDesktop);
            this.grpTarget.Controls.Add(this.rdoTargetDesktop);
            this.grpTarget.Controls.Add(this.rdoTargetDataset);
            this.grpTarget.Controls.Add(this.label6);
            this.grpTarget.Controls.Add(this.label3);
            this.grpTarget.Controls.Add(this.label4);
            this.grpTarget.Controls.Add(this.pnlTargetDataset);
            this.grpTarget.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTarget.Location = new System.Drawing.Point(0, 479);
            this.grpTarget.Margin = new System.Windows.Forms.Padding(6);
            this.grpTarget.Name = "grpTarget";
            this.grpTarget.Padding = new System.Windows.Forms.Padding(6);
            this.grpTarget.Size = new System.Drawing.Size(1064, 419);
            this.grpTarget.TabIndex = 26;
            this.grpTarget.TabStop = false;
            // 
            // pnlTargetFile
            // 
            this.pnlTargetFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTargetFile.Controls.Add(this.btnTargetFileOpen);
            this.pnlTargetFile.Controls.Add(this.txtTargetFile);
            this.pnlTargetFile.Enabled = false;
            this.pnlTargetFile.Location = new System.Drawing.Point(200, 333);
            this.pnlTargetFile.Margin = new System.Windows.Forms.Padding(6);
            this.pnlTargetFile.Name = "pnlTargetFile";
            this.pnlTargetFile.Size = new System.Drawing.Size(852, 59);
            this.pnlTargetFile.TabIndex = 21;
            // 
            // btnTargetFileOpen
            // 
            this.btnTargetFileOpen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTargetFileOpen.Location = new System.Drawing.Point(780, 6);
            this.btnTargetFileOpen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnTargetFileOpen.Name = "btnTargetFileOpen";
            this.btnTargetFileOpen.Size = new System.Drawing.Size(54, 39);
            this.btnTargetFileOpen.TabIndex = 16;
            this.btnTargetFileOpen.Text = "...";
            this.btnTargetFileOpen.UseVisualStyleBackColor = true;
            this.btnTargetFileOpen.Click += new System.EventHandler(this.btnTargetFileOpen_Click);
            // 
            // txtTargetFile
            // 
            this.txtTargetFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetFile.Location = new System.Drawing.Point(22, 6);
            this.txtTargetFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTargetFile.Name = "txtTargetFile";
            this.txtTargetFile.Size = new System.Drawing.Size(745, 31);
            this.txtTargetFile.TabIndex = 15;
            // 
            // rdoTargetFile
            // 
            this.rdoTargetFile.AutoSize = true;
            this.rdoTargetFile.Location = new System.Drawing.Point(30, 342);
            this.rdoTargetFile.Margin = new System.Windows.Forms.Padding(6);
            this.rdoTargetFile.Name = "rdoTargetFile";
            this.rdoTargetFile.Size = new System.Drawing.Size(78, 29);
            this.rdoTargetFile.TabIndex = 14;
            this.rdoTargetFile.Text = "File";
            this.rdoTargetFile.UseVisualStyleBackColor = true;
            this.rdoTargetFile.CheckedChanged += new System.EventHandler(this.rdoTargetFile_CheckedChanged);
            // 
            // pnlTargetDesktop
            // 
            this.pnlTargetDesktop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTargetDesktop.Controls.Add(this.cboTargetDesktop);
            this.pnlTargetDesktop.Enabled = false;
            this.pnlTargetDesktop.Location = new System.Drawing.Point(344, 253);
            this.pnlTargetDesktop.Margin = new System.Windows.Forms.Padding(6);
            this.pnlTargetDesktop.Name = "pnlTargetDesktop";
            this.pnlTargetDesktop.Size = new System.Drawing.Size(708, 59);
            this.pnlTargetDesktop.TabIndex = 19;
            // 
            // cboTargetDesktop
            // 
            this.cboTargetDesktop.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetDesktop.DisplayMember = "Name";
            this.cboTargetDesktop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTargetDesktop.FormattingEnabled = true;
            this.cboTargetDesktop.Location = new System.Drawing.Point(20, 16);
            this.cboTargetDesktop.Margin = new System.Windows.Forms.Padding(6);
            this.cboTargetDesktop.MaxDropDownItems = 11;
            this.cboTargetDesktop.Name = "cboTargetDesktop";
            this.cboTargetDesktop.Size = new System.Drawing.Size(666, 33);
            this.cboTargetDesktop.TabIndex = 13;
            // 
            // rdoTargetDesktop
            // 
            this.rdoTargetDesktop.AutoSize = true;
            this.rdoTargetDesktop.Location = new System.Drawing.Point(30, 272);
            this.rdoTargetDesktop.Margin = new System.Windows.Forms.Padding(6);
            this.rdoTargetDesktop.Name = "rdoTargetDesktop";
            this.rdoTargetDesktop.Size = new System.Drawing.Size(213, 29);
            this.rdoTargetDesktop.TabIndex = 12;
            this.rdoTargetDesktop.Text = "Power BI Desktop";
            this.rdoTargetDesktop.UseVisualStyleBackColor = true;
            this.rdoTargetDesktop.CheckedChanged += new System.EventHandler(this.rdoTargetDesktop_CheckedChanged);
            // 
            // rdoTargetDataset
            // 
            this.rdoTargetDataset.AutoSize = true;
            this.rdoTargetDataset.Checked = true;
            this.rdoTargetDataset.Location = new System.Drawing.Point(30, 72);
            this.rdoTargetDataset.Margin = new System.Windows.Forms.Padding(6);
            this.rdoTargetDataset.Name = "rdoTargetDataset";
            this.rdoTargetDataset.Size = new System.Drawing.Size(117, 29);
            this.rdoTargetDataset.TabIndex = 9;
            this.rdoTargetDataset.TabStop = true;
            this.rdoTargetDataset.Text = "Dataset";
            this.rdoTargetDataset.UseVisualStyleBackColor = true;
            this.rdoTargetDataset.CheckedChanged += new System.EventHandler(this.rdoTargetDataset_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(24, 17);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 31);
            this.label6.TabIndex = 16;
            this.label6.Text = "Target";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(66, 134);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Workspace";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 197);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 25);
            this.label4.TabIndex = 5;
            this.label4.Text = "Dataset";
            // 
            // pnlTargetDataset
            // 
            this.pnlTargetDataset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTargetDataset.Controls.Add(this.cboTargetServer);
            this.pnlTargetDataset.Controls.Add(this.cboTargetDatabase);
            this.pnlTargetDataset.Location = new System.Drawing.Point(200, 116);
            this.pnlTargetDataset.Margin = new System.Windows.Forms.Padding(6);
            this.pnlTargetDataset.Name = "pnlTargetDataset";
            this.pnlTargetDataset.Size = new System.Drawing.Size(852, 127);
            this.pnlTargetDataset.TabIndex = 15;
            // 
            // cboTargetServer
            // 
            this.cboTargetServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetServer.FormattingEnabled = true;
            this.cboTargetServer.Location = new System.Drawing.Point(22, 14);
            this.cboTargetServer.Margin = new System.Windows.Forms.Padding(6);
            this.cboTargetServer.MaxDropDownItems = 11;
            this.cboTargetServer.Name = "cboTargetServer";
            this.cboTargetServer.Size = new System.Drawing.Size(808, 33);
            this.cboTargetServer.TabIndex = 10;
            this.cboTargetServer.TextChanged += new System.EventHandler(this.cboTargetServer_TextChanged);
            // 
            // cboTargetDatabase
            // 
            this.cboTargetDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetDatabase.FormattingEnabled = true;
            this.cboTargetDatabase.Location = new System.Drawing.Point(22, 75);
            this.cboTargetDatabase.Margin = new System.Windows.Forms.Padding(6);
            this.cboTargetDatabase.MaxDropDownItems = 11;
            this.cboTargetDatabase.Name = "cboTargetDatabase";
            this.cboTargetDatabase.Size = new System.Drawing.Size(808, 33);
            this.cboTargetDatabase.TabIndex = 11;
            this.cboTargetDatabase.Enter += new System.EventHandler(this.cboTargetDatabase_Enter);
            // 
            // ConnectionsAlmt
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1064, 1009);
            this.Controls.Add(this.grpTarget);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.grpSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionsAlmt";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Connections";
            this.Load += new System.EventHandler(this.Connections_Load);
            this.pnlSourceDataset.ResumeLayout(false);
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.pnlSourceFile.ResumeLayout(false);
            this.pnlSourceFile.PerformLayout();
            this.pnlSourceDesktop.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
            this.pnlTargetFile.ResumeLayout(false);
            this.pnlTargetFile.PerformLayout();
            this.pnlTargetDesktop.ResumeLayout(false);
            this.pnlTargetDataset.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboSourceDatabase;
        private System.Windows.Forms.ComboBox cboSourceServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlSourceDataset;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSwitch;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox grpTarget;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlTargetDataset;
        private System.Windows.Forms.ComboBox cboTargetServer;
        private System.Windows.Forms.ComboBox cboTargetDatabase;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rdoSourceDataset;
        private System.Windows.Forms.RadioButton rdoSourceDesktop;
        private System.Windows.Forms.RadioButton rdoTargetDesktop;
        private System.Windows.Forms.RadioButton rdoTargetDataset;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rdoSourceFile;
        private System.Windows.Forms.Panel pnlSourceDesktop;
        private System.Windows.Forms.ComboBox cboSourceDesktop;
        private System.Windows.Forms.RadioButton rdoTargetFile;
        private System.Windows.Forms.Panel pnlTargetDesktop;
        private System.Windows.Forms.ComboBox cboTargetDesktop;
        private System.Windows.Forms.Panel pnlTargetFile;
        private System.Windows.Forms.Button btnTargetFileOpen;
        private System.Windows.Forms.TextBox txtTargetFile;
        private System.Windows.Forms.Panel pnlSourceFile;
        private System.Windows.Forms.Button btnSourceFileOpen;
        private System.Windows.Forms.TextBox txtSourceFile;
    }
}