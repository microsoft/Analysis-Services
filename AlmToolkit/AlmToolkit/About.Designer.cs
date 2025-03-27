﻿namespace AlmToolkit
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.okButton = new System.Windows.Forms.Button();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.lblProductVersion = new System.Windows.Forms.Label();
            this.lblProductName = new System.Windows.Forms.Label();
            this.linkReportIssue = new System.Windows.Forms.LinkLabel();
            this.linkDocumentation = new System.Windows.Forms.LinkLabel();
            this.linkLatestVersion = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.linkTwitter = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(430, 342);
            this.okButton.Margin = new System.Windows.Forms.Padding(6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(150, 48);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "&OK";
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(24, 17);
            this.logoPictureBox.Margin = new System.Windows.Forms.Padding(6);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(126, 128);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // lblProductVersion
            // 
            this.lblProductVersion.AutoSize = true;
            this.lblProductVersion.Location = new System.Drawing.Point(178, 237);
            this.lblProductVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProductVersion.Name = "lblProductVersion";
            this.lblProductVersion.Size = new System.Drawing.Size(70, 25);
            this.lblProductVersion.TabIndex = 26;
            this.lblProductVersion.Text = "label1";
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductName.Location = new System.Drawing.Point(176, 19);
            this.lblProductName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(115, 39);
            this.lblProductName.TabIndex = 27;
            this.lblProductName.Text = "label1";
            // 
            // linkReportIssue
            // 
            this.linkReportIssue.AutoSize = true;
            this.linkReportIssue.Location = new System.Drawing.Point(178, 140);
            this.linkReportIssue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkReportIssue.Name = "linkReportIssue";
            this.linkReportIssue.Size = new System.Drawing.Size(163, 25);
            this.linkReportIssue.TabIndex = 28;
            this.linkReportIssue.TabStop = true;
            this.linkReportIssue.Text = "Report an issue";
            this.linkReportIssue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkReportIssue_LinkClicked);
            // 
            // linkDocumentation
            // 
            this.linkDocumentation.AutoSize = true;
            this.linkDocumentation.Location = new System.Drawing.Point(178, 90);
            this.linkDocumentation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkDocumentation.Name = "linkDocumentation";
            this.linkDocumentation.Size = new System.Drawing.Size(156, 25);
            this.linkDocumentation.TabIndex = 30;
            this.linkDocumentation.TabStop = true;
            this.linkDocumentation.Text = "Documentation";
            this.linkDocumentation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDocumentation_LinkClicked);
            // 
            // linkLatestVersion
            // 
            this.linkLatestVersion.AutoSize = true;
            this.linkLatestVersion.Location = new System.Drawing.Point(178, 282);
            this.linkLatestVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLatestVersion.Name = "linkLatestVersion";
            this.linkLatestVersion.Size = new System.Drawing.Size(147, 25);
            this.linkLatestVersion.TabIndex = 32;
            this.linkLatestVersion.TabStop = true;
            this.linkLatestVersion.Text = "Latest version";
            this.linkLatestVersion.Visible = false;
            this.linkLatestVersion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLatestVersion_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 190);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 25);
            this.label1.TabIndex = 33;
            this.label1.Text = "Created by ";
            // 
            // linkTwitter
            // 
            this.linkTwitter.AutoSize = true;
            this.linkTwitter.Location = new System.Drawing.Point(290, 190);
            this.linkTwitter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkTwitter.Name = "linkTwitter";
            this.linkTwitter.Size = new System.Drawing.Size(159, 25);
            this.linkTwitter.TabIndex = 34;
            this.linkTwitter.TabStop = true;
            this.linkTwitter.Text = "Christian Wade";
            this.linkTwitter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTwitter_LinkClicked);
            // 
            // About
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(604, 414);
            this.Controls.Add(this.linkTwitter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLatestVersion);
            this.Controls.Add(this.linkDocumentation);
            this.Controls.Add(this.linkReportIssue);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.lblProductVersion);
            this.Controls.Add(this.logoPictureBox);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.Padding = new System.Windows.Forms.Padding(18, 17, 18, 17);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.About_Load);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label lblProductVersion;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.LinkLabel linkReportIssue;
        private System.Windows.Forms.LinkLabel linkDocumentation;
        private System.Windows.Forms.LinkLabel linkLatestVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkTwitter;
    }
}
