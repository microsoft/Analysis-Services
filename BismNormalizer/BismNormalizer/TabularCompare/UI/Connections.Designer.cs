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
            this.pnlSourceProject = new System.Windows.Forms.Panel();
            this.cboSourceProject = new System.Windows.Forms.ComboBox();
            this.rdoSourceProject = new System.Windows.Forms.RadioButton();
            this.rdoSourceDb = new System.Windows.Forms.RadioButton();
            this.grpTarget = new System.Windows.Forms.GroupBox();
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
            this.pnlSourceProject.SuspendLayout();
            this.grpTarget.SuspendLayout();
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
            this.cboSourceDatabase.Location = new System.Drawing.Point(11, 39);
            this.cboSourceDatabase.MaxDropDownItems = 11;
            this.cboSourceDatabase.Name = "cboSourceDatabase";
            this.cboSourceDatabase.Size = new System.Drawing.Size(213, 21);
            this.cboSourceDatabase.TabIndex = 12;
            this.cboSourceDatabase.Enter += new System.EventHandler(this.cboSourceDatabase_Enter);
            // 
            // cboSourceServer
            // 
            this.cboSourceServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSourceServer.FormattingEnabled = true;
            this.cboSourceServer.Location = new System.Drawing.Point(11, 7);
            this.cboSourceServer.MaxDropDownItems = 11;
            this.cboSourceServer.Name = "cboSourceServer";
            this.cboSourceServer.Size = new System.Drawing.Size(213, 21);
            this.cboSourceServer.TabIndex = 9;
            this.cboSourceServer.TextChanged += new System.EventHandler(this.cboSourceServer_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Database";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
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
            this.pnlSourceDb.Location = new System.Drawing.Point(67, 66);
            this.pnlSourceDb.Name = "pnlSourceDb";
            this.pnlSourceDb.Size = new System.Drawing.Size(233, 66);
            this.pnlSourceDb.TabIndex = 1;
            // 
            // grpSource
            // 
            this.grpSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpSource.Controls.Add(this.pnlSourceProject);
            this.grpSource.Controls.Add(this.rdoSourceProject);
            this.grpSource.Controls.Add(this.rdoSourceDb);
            this.grpSource.Controls.Add(this.label1);
            this.grpSource.Controls.Add(this.label2);
            this.grpSource.Controls.Add(this.pnlSourceDb);
            this.grpSource.Location = new System.Drawing.Point(12, 12);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(305, 137);
            this.grpSource.TabIndex = 16;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source";
            // 
            // pnlSourceProject
            // 
            this.pnlSourceProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSourceProject.Controls.Add(this.cboSourceProject);
            this.pnlSourceProject.Location = new System.Drawing.Point(67, 9);
            this.pnlSourceProject.Name = "pnlSourceProject";
            this.pnlSourceProject.Size = new System.Drawing.Size(233, 34);
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
            this.cboSourceProject.Location = new System.Drawing.Point(11, 7);
            this.cboSourceProject.MaxDropDownItems = 11;
            this.cboSourceProject.Name = "cboSourceProject";
            this.cboSourceProject.Size = new System.Drawing.Size(213, 21);
            this.cboSourceProject.TabIndex = 9;
            this.cboSourceProject.SelectedIndexChanged += new System.EventHandler(this.cboSourceProject_SelectedIndexChanged);
            // 
            // rdoSourceProject
            // 
            this.rdoSourceProject.AutoSize = true;
            this.rdoSourceProject.Location = new System.Drawing.Point(7, 19);
            this.rdoSourceProject.Name = "rdoSourceProject";
            this.rdoSourceProject.Size = new System.Drawing.Size(58, 17);
            this.rdoSourceProject.TabIndex = 17;
            this.rdoSourceProject.Text = "Project";
            this.rdoSourceProject.UseVisualStyleBackColor = true;
            this.rdoSourceProject.CheckedChanged += new System.EventHandler(this.rdoSourceProject_CheckedChanged);
            // 
            // rdoSourceDb
            // 
            this.rdoSourceDb.AutoSize = true;
            this.rdoSourceDb.Checked = true;
            this.rdoSourceDb.Location = new System.Drawing.Point(7, 46);
            this.rdoSourceDb.Name = "rdoSourceDb";
            this.rdoSourceDb.Size = new System.Drawing.Size(71, 17);
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
            this.grpTarget.Controls.Add(this.pnlTargetProject);
            this.grpTarget.Controls.Add(this.rdoTargetProject);
            this.grpTarget.Controls.Add(this.rdoTargetDb);
            this.grpTarget.Controls.Add(this.label3);
            this.grpTarget.Controls.Add(this.label4);
            this.grpTarget.Controls.Add(this.pnlTargetDb);
            this.grpTarget.Location = new System.Drawing.Point(365, 12);
            this.grpTarget.Name = "grpTarget";
            this.grpTarget.Size = new System.Drawing.Size(305, 137);
            this.grpTarget.TabIndex = 17;
            this.grpTarget.TabStop = false;
            this.grpTarget.Text = "Target";
            // 
            // pnlTargetProject
            // 
            this.pnlTargetProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTargetProject.Controls.Add(this.cboTargetProject);
            this.pnlTargetProject.Location = new System.Drawing.Point(67, 9);
            this.pnlTargetProject.Name = "pnlTargetProject";
            this.pnlTargetProject.Size = new System.Drawing.Size(232, 34);
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
            this.cboTargetProject.Location = new System.Drawing.Point(11, 7);
            this.cboTargetProject.MaxDropDownItems = 11;
            this.cboTargetProject.Name = "cboTargetProject";
            this.cboTargetProject.Size = new System.Drawing.Size(213, 21);
            this.cboTargetProject.TabIndex = 9;
            // 
            // rdoTargetProject
            // 
            this.rdoTargetProject.AutoSize = true;
            this.rdoTargetProject.Location = new System.Drawing.Point(7, 19);
            this.rdoTargetProject.Name = "rdoTargetProject";
            this.rdoTargetProject.Size = new System.Drawing.Size(58, 17);
            this.rdoTargetProject.TabIndex = 17;
            this.rdoTargetProject.Text = "Project";
            this.rdoTargetProject.UseVisualStyleBackColor = true;
            this.rdoTargetProject.CheckedChanged += new System.EventHandler(this.rdoTargetProject_CheckedChanged);
            // 
            // rdoTargetDb
            // 
            this.rdoTargetDb.AutoSize = true;
            this.rdoTargetDb.Checked = true;
            this.rdoTargetDb.Location = new System.Drawing.Point(7, 46);
            this.rdoTargetDb.Name = "rdoTargetDb";
            this.rdoTargetDb.Size = new System.Drawing.Size(71, 17);
            this.rdoTargetDb.TabIndex = 16;
            this.rdoTargetDb.TabStop = true;
            this.rdoTargetDb.Text = "Database";
            this.rdoTargetDb.UseVisualStyleBackColor = true;
            this.rdoTargetDb.CheckedChanged += new System.EventHandler(this.rdoTargetDb_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Server";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
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
            this.pnlTargetDb.Location = new System.Drawing.Point(67, 66);
            this.pnlTargetDb.Name = "pnlTargetDb";
            this.pnlTargetDb.Size = new System.Drawing.Size(232, 66);
            this.pnlTargetDb.TabIndex = 15;
            // 
            // cboTargetServer
            // 
            this.cboTargetServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetServer.FormattingEnabled = true;
            this.cboTargetServer.Location = new System.Drawing.Point(11, 7);
            this.cboTargetServer.MaxDropDownItems = 11;
            this.cboTargetServer.Name = "cboTargetServer";
            this.cboTargetServer.Size = new System.Drawing.Size(213, 21);
            this.cboTargetServer.TabIndex = 9;
            this.cboTargetServer.TextChanged += new System.EventHandler(this.cboTargetServer_TextChanged);
            // 
            // cboTargetDatabase
            // 
            this.cboTargetDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetDatabase.FormattingEnabled = true;
            this.cboTargetDatabase.Location = new System.Drawing.Point(11, 39);
            this.cboTargetDatabase.MaxDropDownItems = 11;
            this.cboTargetDatabase.Name = "cboTargetDatabase";
            this.cboTargetDatabase.Size = new System.Drawing.Size(213, 21);
            this.cboTargetDatabase.TabIndex = 12;
            this.cboTargetDatabase.Enter += new System.EventHandler(this.cboTargetDatabase_Enter);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(506, 159);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(587, 159);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSwitch
            // 
            this.btnSwitch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSwitch.BackgroundImage = global::BismNormalizer.Resources.ButtonSwitch;
            this.btnSwitch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSwitch.Location = new System.Drawing.Point(323, 50);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(36, 32);
            this.btnSwitch.TabIndex = 20;
            this.btnSwitch.UseVisualStyleBackColor = true;
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            // 
            // Connections
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(682, 194);
            this.Controls.Add(this.btnSwitch);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpTarget);
            this.Controls.Add(this.grpSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
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
            this.pnlSourceProject.ResumeLayout(false);
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
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
    }
}