namespace BismNormalizer.TabularCompare.UI
{
    partial class BlobCredentials
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboPrivacyLevel = new System.Windows.Forms.ComboBox();
            this.lblPrivacyLevel = new System.Windows.Forms.Label();
            this.txtAccountKey = new System.Windows.Forms.TextBox();
            this.txtConnectionName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(346, 142);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(265, 142);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cboPrivacyLevel);
            this.groupBox1.Controls.Add(this.lblPrivacyLevel);
            this.groupBox1.Controls.Add(this.txtAccountKey);
            this.groupBox1.Controls.Add(this.txtConnectionName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(409, 123);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Credentials";
            // 
            // cboPrivacyLevel
            // 
            this.cboPrivacyLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPrivacyLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPrivacyLevel.Items.AddRange(new object[] {
            "None",
            "Public",
            "Organizational",
            "Private"});
            this.cboPrivacyLevel.Location = new System.Drawing.Point(118, 76);
            this.cboPrivacyLevel.Name = "cboPrivacyLevel";
            this.cboPrivacyLevel.Size = new System.Drawing.Size(285, 21);
            this.cboPrivacyLevel.TabIndex = 6;
            // 
            // lblPrivacyLevel
            // 
            this.lblPrivacyLevel.AutoSize = true;
            this.lblPrivacyLevel.Location = new System.Drawing.Point(7, 79);
            this.lblPrivacyLevel.Name = "lblPrivacyLevel";
            this.lblPrivacyLevel.Size = new System.Drawing.Size(74, 13);
            this.lblPrivacyLevel.TabIndex = 7;
            this.lblPrivacyLevel.Text = "Privacy Level:";
            // 
            // txtAccountKey
            // 
            this.txtAccountKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccountKey.Location = new System.Drawing.Point(118, 50);
            this.txtAccountKey.Name = "txtAccountKey";
            this.txtAccountKey.Size = new System.Drawing.Size(285, 20);
            this.txtAccountKey.TabIndex = 2;
            this.txtAccountKey.UseSystemPasswordChar = true;
            // 
            // txtConnectionName
            // 
            this.txtConnectionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnectionName.Location = new System.Drawing.Point(118, 24);
            this.txtConnectionName.Name = "txtConnectionName";
            this.txtConnectionName.ReadOnly = true;
            this.txtConnectionName.Size = new System.Drawing.Size(285, 20);
            this.txtConnectionName.TabIndex = 3;
            this.txtConnectionName.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Account Key:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Data Source Name:";
            // 
            // BlobCredentials
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(433, 177);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BlobCredentials";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Azure Blob Storage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImpersonationCredentials_FormClosing);
            this.Load += new System.EventHandler(this.BlobCredentials_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAccountKey;
        private System.Windows.Forms.TextBox txtConnectionName;
        private System.Windows.Forms.ComboBox cboPrivacyLevel;
        private System.Windows.Forms.Label lblPrivacyLevel;
    }
}