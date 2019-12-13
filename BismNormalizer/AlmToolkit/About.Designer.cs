namespace AlmToolkit
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
            this.linkReleaseHistory = new System.Windows.Forms.LinkLabel();
            this.linkDocumentation = new System.Windows.Forms.LinkLabel();
            this.linkALMTWebsite = new System.Windows.Forms.LinkLabel();
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
            this.okButton.Location = new System.Drawing.Point(203, 204);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 25);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "&OK";
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(12, 9);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(63, 67);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // lblProductVersion
            // 
            this.lblProductVersion.AutoSize = true;
            this.lblProductVersion.Location = new System.Drawing.Point(89, 92);
            this.lblProductVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProductVersion.Name = "lblProductVersion";
            this.lblProductVersion.Size = new System.Drawing.Size(35, 13);
            this.lblProductVersion.TabIndex = 26;
            this.lblProductVersion.Text = "label1";
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductName.Location = new System.Drawing.Point(88, 10);
            this.lblProductName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(64, 22);
            this.lblProductName.TabIndex = 27;
            this.lblProductName.Text = "label1";
            // 
            // linkReleaseHistory
            // 
            this.linkReleaseHistory.AutoSize = true;
            this.linkReleaseHistory.Location = new System.Drawing.Point(89, 140);
            this.linkReleaseHistory.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkReleaseHistory.Name = "linkReleaseHistory";
            this.linkReleaseHistory.Size = new System.Drawing.Size(79, 13);
            this.linkReleaseHistory.TabIndex = 28;
            this.linkReleaseHistory.TabStop = true;
            this.linkReleaseHistory.Text = "Release history";
            this.linkReleaseHistory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkReleaseHistory_LinkClicked);
            // 
            // linkDocumentation
            // 
            this.linkDocumentation.AutoSize = true;
            this.linkDocumentation.Location = new System.Drawing.Point(89, 68);
            this.linkDocumentation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkDocumentation.Name = "linkDocumentation";
            this.linkDocumentation.Size = new System.Drawing.Size(79, 13);
            this.linkDocumentation.TabIndex = 30;
            this.linkDocumentation.TabStop = true;
            this.linkDocumentation.Text = "Documentation";
            this.linkDocumentation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDocumentation_LinkClicked);
            // 
            // linkALMTWebsite
            // 
            this.linkALMTWebsite.AutoSize = true;
            this.linkALMTWebsite.Location = new System.Drawing.Point(89, 44);
            this.linkALMTWebsite.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkALMTWebsite.Name = "linkALMTWebsite";
            this.linkALMTWebsite.Size = new System.Drawing.Size(113, 13);
            this.linkALMTWebsite.TabIndex = 31;
            this.linkALMTWebsite.TabStop = true;
            this.linkALMTWebsite.Text = "http://alm-toolkit.com/";
            this.linkALMTWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkALMTWebsite_LinkClicked);
            // 
            // linkLatestVersion
            // 
            this.linkLatestVersion.AutoSize = true;
            this.linkLatestVersion.Location = new System.Drawing.Point(89, 115);
            this.linkLatestVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLatestVersion.Name = "linkLatestVersion";
            this.linkLatestVersion.Size = new System.Drawing.Size(73, 13);
            this.linkLatestVersion.TabIndex = 32;
            this.linkLatestVersion.TabStop = true;
            this.linkLatestVersion.Text = "Latest version";
            this.linkLatestVersion.Visible = false;
            this.linkLatestVersion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLatestVersion_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 166);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Created by ";
            // 
            // linkTwitter
            // 
            this.linkTwitter.AutoSize = true;
            this.linkTwitter.Location = new System.Drawing.Point(145, 166);
            this.linkTwitter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkTwitter.Name = "linkTwitter";
            this.linkTwitter.Size = new System.Drawing.Size(79, 13);
            this.linkTwitter.TabIndex = 34;
            this.linkTwitter.TabStop = true;
            this.linkTwitter.Text = "Christian Wade";
            this.linkTwitter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTwitter_LinkClicked);
            // 
            // About
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(290, 241);
            this.Controls.Add(this.linkTwitter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLatestVersion);
            this.Controls.Add(this.linkALMTWebsite);
            this.Controls.Add(this.linkDocumentation);
            this.Controls.Add(this.linkReleaseHistory);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.lblProductVersion);
            this.Controls.Add(this.logoPictureBox);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.Padding = new System.Windows.Forms.Padding(9);
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
        private System.Windows.Forms.LinkLabel linkReleaseHistory;
        private System.Windows.Forms.LinkLabel linkDocumentation;
        private System.Windows.Forms.LinkLabel linkALMTWebsite;
        private System.Windows.Forms.LinkLabel linkLatestVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkTwitter;
    }
}
