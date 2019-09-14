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
            this.pnlSourceDb = new System.Windows.Forms.Panel();
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSwitch = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grpTarget = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlTargetDb = new System.Windows.Forms.Panel();
            this.cboTargetServer = new System.Windows.Forms.ComboBox();
            this.cboTargetDatabase = new System.Windows.Forms.ComboBox();
            this.pnlSourceDb.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.pnlTargetDb.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboSourceDatabase
            // 
            this.cboSourceDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSourceDatabase.FormattingEnabled = true;
            this.cboSourceDatabase.Location = new System.Drawing.Point(26, 87);
            this.cboSourceDatabase.Margin = new System.Windows.Forms.Padding(7);
            this.cboSourceDatabase.MaxDropDownItems = 11;
            this.cboSourceDatabase.Name = "cboSourceDatabase";
            this.cboSourceDatabase.Size = new System.Drawing.Size(858, 37);
            this.cboSourceDatabase.TabIndex = 2;
            this.cboSourceDatabase.Enter += new System.EventHandler(this.cboSourceDatabase_Enter);
            // 
            // cboSourceServer
            // 
            this.cboSourceServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSourceServer.FormattingEnabled = true;
            this.cboSourceServer.Location = new System.Drawing.Point(26, 16);
            this.cboSourceServer.Margin = new System.Windows.Forms.Padding(7);
            this.cboSourceServer.MaxDropDownItems = 11;
            this.cboSourceServer.Name = "cboSourceServer";
            this.cboSourceServer.Size = new System.Drawing.Size(858, 37);
            this.cboSourceServer.TabIndex = 1;
            this.cboSourceServer.TextChanged += new System.EventHandler(this.cboSourceServer_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 154);
            this.label2.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "Dataset";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 83);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "Workspace";
            // 
            // pnlSourceDb
            // 
            this.pnlSourceDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSourceDb.Controls.Add(this.cboSourceServer);
            this.pnlSourceDb.Controls.Add(this.cboSourceDatabase);
            this.pnlSourceDb.Location = new System.Drawing.Point(147, 60);
            this.pnlSourceDb.Margin = new System.Windows.Forms.Padding(7);
            this.pnlSourceDb.Name = "pnlSourceDb";
            this.pnlSourceDb.Size = new System.Drawing.Size(910, 147);
            this.pnlSourceDb.TabIndex = 1;
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.label1);
            this.grpSource.Controls.Add(this.label2);
            this.grpSource.Controls.Add(this.pnlSourceDb);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSource.Location = new System.Drawing.Point(0, 0);
            this.grpSource.Margin = new System.Windows.Forms.Padding(7);
            this.grpSource.Name = "grpSource";
            this.grpSource.Padding = new System.Windows.Forms.Padding(7);
            this.grpSource.Size = new System.Drawing.Size(1069, 263);
            this.grpSource.TabIndex = 16;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(696, 23);
            this.btnOK.Margin = new System.Windows.Forms.Padding(7);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(161, 51);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(871, 23);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(166, 51);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSwitch
            // 
            this.btnSwitch.BackgroundImage = global::BismNormalizer.Resources.ButtonSwitch;
            this.btnSwitch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSwitch.Location = new System.Drawing.Point(467, 6);
            this.btnSwitch.Margin = new System.Windows.Forms.Padding(7);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(130, 71);
            this.btnSwitch.TabIndex = 3;
            this.btnSwitch.UseVisualStyleBackColor = true;
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 627);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1069, 100);
            this.panel2.TabIndex = 22;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSwitch);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 263);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1069, 84);
            this.panel1.TabIndex = 25;
            // 
            // grpTarget
            // 
            this.grpTarget.Controls.Add(this.label3);
            this.grpTarget.Controls.Add(this.label4);
            this.grpTarget.Controls.Add(this.pnlTargetDb);
            this.grpTarget.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTarget.Location = new System.Drawing.Point(0, 347);
            this.grpTarget.Margin = new System.Windows.Forms.Padding(7);
            this.grpTarget.Name = "grpTarget";
            this.grpTarget.Padding = new System.Windows.Forms.Padding(7);
            this.grpTarget.Size = new System.Drawing.Size(1069, 268);
            this.grpTarget.TabIndex = 26;
            this.grpTarget.TabStop = false;
            this.grpTarget.Text = "Target";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 83);
            this.label3.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 29);
            this.label3.TabIndex = 4;
            this.label3.Text = "Workspace";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 154);
            this.label4.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 29);
            this.label4.TabIndex = 5;
            this.label4.Text = "Dataset";
            // 
            // pnlTargetDb
            // 
            this.pnlTargetDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTargetDb.Controls.Add(this.cboTargetServer);
            this.pnlTargetDb.Controls.Add(this.cboTargetDatabase);
            this.pnlTargetDb.Location = new System.Drawing.Point(144, 60);
            this.pnlTargetDb.Margin = new System.Windows.Forms.Padding(7);
            this.pnlTargetDb.Name = "pnlTargetDb";
            this.pnlTargetDb.Size = new System.Drawing.Size(911, 147);
            this.pnlTargetDb.TabIndex = 15;
            // 
            // cboTargetServer
            // 
            this.cboTargetServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetServer.FormattingEnabled = true;
            this.cboTargetServer.Location = new System.Drawing.Point(26, 16);
            this.cboTargetServer.Margin = new System.Windows.Forms.Padding(7);
            this.cboTargetServer.MaxDropDownItems = 11;
            this.cboTargetServer.Name = "cboTargetServer";
            this.cboTargetServer.Size = new System.Drawing.Size(862, 37);
            this.cboTargetServer.TabIndex = 4;
            this.cboTargetServer.TextChanged += new System.EventHandler(this.cboTargetServer_TextChanged);
            // 
            // cboTargetDatabase
            // 
            this.cboTargetDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTargetDatabase.FormattingEnabled = true;
            this.cboTargetDatabase.Location = new System.Drawing.Point(26, 87);
            this.cboTargetDatabase.Margin = new System.Windows.Forms.Padding(7);
            this.cboTargetDatabase.MaxDropDownItems = 11;
            this.cboTargetDatabase.Name = "cboTargetDatabase";
            this.cboTargetDatabase.Size = new System.Drawing.Size(862, 37);
            this.cboTargetDatabase.TabIndex = 5;
            this.cboTargetDatabase.Enter += new System.EventHandler(this.cboTargetDatabase_Enter);
            // 
            // Connections
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1069, 727);
            this.Controls.Add(this.grpTarget);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.grpSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(7);
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
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
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
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSwitch;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox grpTarget;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlTargetDb;
        private System.Windows.Forms.ComboBox cboTargetServer;
        private System.Windows.Forms.ComboBox cboTargetDatabase;
    }
}