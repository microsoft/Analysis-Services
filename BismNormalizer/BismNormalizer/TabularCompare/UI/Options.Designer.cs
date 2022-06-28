namespace BismNormalizer.TabularCompare.UI
{
    partial class Options
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
            this.chkRoles = new System.Windows.Forms.CheckBox();
            this.chkPartitions = new System.Windows.Forms.CheckBox();
            this.chkMeasureDependencies = new System.Windows.Forms.CheckBox();
            this.chkPerspectives = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkRetainRefreshPolicy = new System.Windows.Forms.CheckBox();
            this.chkRetainRoleMembers = new System.Windows.Forms.CheckBox();
            this.chkLineageTag = new System.Windows.Forms.CheckBox();
            this.chkRetainStorageMode = new System.Windows.Forms.CheckBox();
            this.chkRetainPolicyPartitions = new System.Windows.Forms.CheckBox();
            this.chkRetainPartitions = new System.Windows.Forms.CheckBox();
            this.chkMergeCultures = new System.Windows.Forms.CheckBox();
            this.chkCultures = new System.Windows.Forms.CheckBox();
            this.chkMergePerspectives = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkAffectedTables = new System.Windows.Forms.CheckBox();
            this.cboProcessingOption = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblProcessingWarning = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkRoles
            // 
            this.chkRoles.AutoSize = true;
            this.chkRoles.Checked = true;
            this.chkRoles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRoles.Location = new System.Drawing.Point(38, 233);
            this.chkRoles.Margin = new System.Windows.Forms.Padding(6);
            this.chkRoles.Name = "chkRoles";
            this.chkRoles.Size = new System.Drawing.Size(166, 29);
            this.chkRoles.TabIndex = 7;
            this.chkRoles.Text = "Include roles";
            this.chkRoles.UseVisualStyleBackColor = true;
            this.chkRoles.CheckedChanged += new System.EventHandler(this.chkRoles_CheckedChanged);
            // 
            // chkPartitions
            // 
            this.chkPartitions.AutoSize = true;
            this.chkPartitions.Location = new System.Drawing.Point(38, 331);
            this.chkPartitions.Margin = new System.Windows.Forms.Padding(6);
            this.chkPartitions.Name = "chkPartitions";
            this.chkPartitions.Size = new System.Drawing.Size(451, 29);
            this.chkPartitions.TabIndex = 9;
            this.chkPartitions.Text = "Consider partitions when comparing tables";
            this.chkPartitions.UseVisualStyleBackColor = true;
            // 
            // chkMeasureDependencies
            // 
            this.chkMeasureDependencies.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMeasureDependencies.AutoSize = true;
            this.chkMeasureDependencies.Checked = true;
            this.chkMeasureDependencies.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMeasureDependencies.Location = new System.Drawing.Point(38, 655);
            this.chkMeasureDependencies.Margin = new System.Windows.Forms.Padding(6);
            this.chkMeasureDependencies.Name = "chkMeasureDependencies";
            this.chkMeasureDependencies.Size = new System.Drawing.Size(86, 29);
            this.chkMeasureDependencies.TabIndex = 15;
            this.chkMeasureDependencies.Text = "XXX";
            this.chkMeasureDependencies.UseVisualStyleBackColor = true;
            // 
            // chkPerspectives
            // 
            this.chkPerspectives.AutoSize = true;
            this.chkPerspectives.Location = new System.Drawing.Point(38, 46);
            this.chkPerspectives.Margin = new System.Windows.Forms.Padding(6);
            this.chkPerspectives.Name = "chkPerspectives";
            this.chkPerspectives.Size = new System.Drawing.Size(241, 29);
            this.chkPerspectives.TabIndex = 3;
            this.chkPerspectives.Text = "Include perspectives";
            this.chkPerspectives.UseVisualStyleBackColor = true;
            this.chkPerspectives.CheckedChanged += new System.EventHandler(this.chkPerspectives_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkRetainRefreshPolicy);
            this.groupBox1.Controls.Add(this.chkRetainRoleMembers);
            this.groupBox1.Controls.Add(this.chkLineageTag);
            this.groupBox1.Controls.Add(this.chkRetainStorageMode);
            this.groupBox1.Controls.Add(this.chkRetainPolicyPartitions);
            this.groupBox1.Controls.Add(this.chkRetainPartitions);
            this.groupBox1.Controls.Add(this.chkMergeCultures);
            this.groupBox1.Controls.Add(this.chkCultures);
            this.groupBox1.Controls.Add(this.chkMergePerspectives);
            this.groupBox1.Controls.Add(this.chkPerspectives);
            this.groupBox1.Controls.Add(this.chkMeasureDependencies);
            this.groupBox1.Controls.Add(this.chkPartitions);
            this.groupBox1.Controls.Add(this.chkRoles);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox1.Size = new System.Drawing.Size(794, 756);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comparison Options";
            // 
            // chkRetainRefreshPolicy
            // 
            this.chkRetainRefreshPolicy.AutoSize = true;
            this.chkRetainRefreshPolicy.Location = new System.Drawing.Point(38, 552);
            this.chkRetainRefreshPolicy.Margin = new System.Windows.Forms.Padding(6);
            this.chkRetainRefreshPolicy.Name = "chkRetainRefreshPolicy";
            this.chkRetainRefreshPolicy.Size = new System.Drawing.Size(413, 29);
            this.chkRetainRefreshPolicy.TabIndex = 13;
            this.chkRetainRefreshPolicy.Text = "For table updates, retain refresh policy";
            this.chkRetainRefreshPolicy.UseVisualStyleBackColor = true;
            // 
            // chkRetainRoleMembers
            // 
            this.chkRetainRoleMembers.AutoSize = true;
            this.chkRetainRoleMembers.Checked = true;
            this.chkRetainRoleMembers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRetainRoleMembers.Location = new System.Drawing.Point(78, 277);
            this.chkRetainRoleMembers.Margin = new System.Windows.Forms.Padding(6);
            this.chkRetainRoleMembers.Name = "chkRetainRoleMembers";
            this.chkRetainRoleMembers.Size = new System.Drawing.Size(361, 29);
            this.chkRetainRoleMembers.TabIndex = 8;
            this.chkRetainRoleMembers.Text = "For role updates, retain members";
            this.chkRetainRoleMembers.UseVisualStyleBackColor = true;
            // 
            // chkLineageTag
            // 
            this.chkLineageTag.AutoSize = true;
            this.chkLineageTag.Location = new System.Drawing.Point(38, 390);
            this.chkLineageTag.Margin = new System.Windows.Forms.Padding(6);
            this.chkLineageTag.Name = "chkLineageTag";
            this.chkLineageTag.Size = new System.Drawing.Size(413, 29);
            this.chkLineageTag.TabIndex = 10;
            this.chkLineageTag.Text = "Consider LineageTag when comparing";
            this.chkLineageTag.UseVisualStyleBackColor = true;
            // 
            // chkRetainStorageMode
            // 
            this.chkRetainStorageMode.AutoSize = true;
            this.chkRetainStorageMode.Location = new System.Drawing.Point(38, 605);
            this.chkRetainStorageMode.Margin = new System.Windows.Forms.Padding(6);
            this.chkRetainStorageMode.Name = "chkRetainStorageMode";
            this.chkRetainStorageMode.Size = new System.Drawing.Size(415, 29);
            this.chkRetainStorageMode.TabIndex = 14;
            this.chkRetainStorageMode.Text = "For table updates, retain storage mode";
            this.chkRetainStorageMode.UseVisualStyleBackColor = true;
            // 
            // chkRetainPolicyPartitions
            // 
            this.chkRetainPolicyPartitions.AutoSize = true;
            this.chkRetainPolicyPartitions.Enabled = false;
            this.chkRetainPolicyPartitions.Location = new System.Drawing.Point(78, 492);
            this.chkRetainPolicyPartitions.Margin = new System.Windows.Forms.Padding(6);
            this.chkRetainPolicyPartitions.Name = "chkRetainPolicyPartitions";
            this.chkRetainPolicyPartitions.Size = new System.Drawing.Size(447, 29);
            this.chkRetainPolicyPartitions.TabIndex = 12;
            this.chkRetainPolicyPartitions.Text = "Retain only refresh-policy based partitions";
            this.chkRetainPolicyPartitions.UseVisualStyleBackColor = true;
            // 
            // chkRetainPartitions
            // 
            this.chkRetainPartitions.AutoSize = true;
            this.chkRetainPartitions.Location = new System.Drawing.Point(38, 452);
            this.chkRetainPartitions.Margin = new System.Windows.Forms.Padding(6);
            this.chkRetainPartitions.Name = "chkRetainPartitions";
            this.chkRetainPartitions.Size = new System.Drawing.Size(372, 29);
            this.chkRetainPartitions.TabIndex = 11;
            this.chkRetainPartitions.Text = "For table updates, retain partitions";
            this.chkRetainPartitions.UseVisualStyleBackColor = true;
            this.chkRetainPartitions.CheckedChanged += new System.EventHandler(this.ChkRetainPartitions_CheckedChanged);
            // 
            // chkMergeCultures
            // 
            this.chkMergeCultures.AutoSize = true;
            this.chkMergeCultures.Enabled = false;
            this.chkMergeCultures.Location = new System.Drawing.Point(78, 185);
            this.chkMergeCultures.Margin = new System.Windows.Forms.Padding(6);
            this.chkMergeCultures.Name = "chkMergeCultures";
            this.chkMergeCultures.Size = new System.Drawing.Size(546, 29);
            this.chkMergeCultures.TabIndex = 6;
            this.chkMergeCultures.Text = "For culture updates, merge translations (not replace)";
            this.chkMergeCultures.UseVisualStyleBackColor = true;
            // 
            // chkCultures
            // 
            this.chkCultures.AutoSize = true;
            this.chkCultures.Location = new System.Drawing.Point(38, 144);
            this.chkCultures.Margin = new System.Windows.Forms.Padding(6);
            this.chkCultures.Name = "chkCultures";
            this.chkCultures.Size = new System.Drawing.Size(195, 29);
            this.chkCultures.TabIndex = 5;
            this.chkCultures.Text = "Include cultures";
            this.chkCultures.UseVisualStyleBackColor = true;
            this.chkCultures.CheckedChanged += new System.EventHandler(this.chkCultures_CheckedChanged);
            // 
            // chkMergePerspectives
            // 
            this.chkMergePerspectives.AutoSize = true;
            this.chkMergePerspectives.Enabled = false;
            this.chkMergePerspectives.Location = new System.Drawing.Point(78, 88);
            this.chkMergePerspectives.Margin = new System.Windows.Forms.Padding(6);
            this.chkMergePerspectives.Name = "chkMergePerspectives";
            this.chkMergePerspectives.Size = new System.Drawing.Size(578, 29);
            this.chkMergePerspectives.TabIndex = 4;
            this.chkMergePerspectives.Text = "For perspective updates, merge selections (not replace)";
            this.chkMergePerspectives.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.lblProcessingWarning);
            this.groupBox2.Controls.Add(this.chkAffectedTables);
            this.groupBox2.Controls.Add(this.cboProcessingOption);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 756);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox2.Size = new System.Drawing.Size(794, 255);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database Deployment";
            // 
            // chkAffectedTables
            // 
            this.chkAffectedTables.AutoSize = true;
            this.chkAffectedTables.Checked = true;
            this.chkAffectedTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAffectedTables.Location = new System.Drawing.Point(38, 103);
            this.chkAffectedTables.Margin = new System.Windows.Forms.Padding(6);
            this.chkAffectedTables.Name = "chkAffectedTables";
            this.chkAffectedTables.Size = new System.Drawing.Size(315, 29);
            this.chkAffectedTables.TabIndex = 31;
            this.chkAffectedTables.Text = "Process only affected tables";
            this.chkAffectedTables.UseVisualStyleBackColor = true;
            // 
            // cboProcessingOption
            // 
            this.cboProcessingOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboProcessingOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProcessingOption.Items.AddRange(new object[] {
            "Recalc",
            "Default",
            "Do Not Process",
            "Full"});
            this.cboProcessingOption.Location = new System.Drawing.Point(238, 46);
            this.cboProcessingOption.Margin = new System.Windows.Forms.Padding(6);
            this.cboProcessingOption.Name = "cboProcessingOption";
            this.cboProcessingOption.Size = new System.Drawing.Size(376, 33);
            this.cboProcessingOption.TabIndex = 30;
            this.cboProcessingOption.SelectedIndexChanged += new System.EventHandler(this.cboProcessingOption_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Processing Option:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 1015);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(794, 86);
            this.panel2.TabIndex = 24;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(624, 19);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(142, 44);
            this.btnCancel.TabIndex = 36;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(474, 19);
            this.btnOK.Margin = new System.Windows.Forms.Padding(6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(138, 44);
            this.btnOK.TabIndex = 35;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblProcessingWarning
            // 
            this.lblProcessingWarning.AutoSize = true;
            this.lblProcessingWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessingWarning.Location = new System.Drawing.Point(73, 150);
            this.lblProcessingWarning.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProcessingWarning.Name = "lblProcessingWarning";
            this.lblProcessingWarning.Size = new System.Drawing.Size(612, 75);
            this.lblProcessingWarning.TabIndex = 32;
            this.lblProcessingWarning.Text = "⚠️ Warning: if your dataset requires long-running refreshes, it\'s\r\nrecommended to" +
    " select Recalc or Do Not Process and refresh\r\nfrom SSMS after deployment for bet" +
    "ter visibility of progress.";
            // 
            // Options
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(794, 1101);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Options_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkRoles;
        private System.Windows.Forms.CheckBox chkPartitions;
        private System.Windows.Forms.CheckBox chkMeasureDependencies;
        private System.Windows.Forms.CheckBox chkPerspectives;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkMergePerspectives;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboProcessingOption;
        private System.Windows.Forms.CheckBox chkAffectedTables;
        private System.Windows.Forms.CheckBox chkMergeCultures;
        private System.Windows.Forms.CheckBox chkCultures;
        private System.Windows.Forms.CheckBox chkRetainPartitions;
        private System.Windows.Forms.CheckBox chkRetainPolicyPartitions;
        private System.Windows.Forms.CheckBox chkRetainStorageMode;
        private System.Windows.Forms.CheckBox chkLineageTag;
        private System.Windows.Forms.CheckBox chkRetainRoleMembers;
        private System.Windows.Forms.CheckBox chkRetainRefreshPolicy;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblProcessingWarning;
    }
}