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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkRoles = new System.Windows.Forms.CheckBox();
            this.chkPartitions = new System.Windows.Forms.CheckBox();
            this.chkMeasureDependencies = new System.Windows.Forms.CheckBox();
            this.chkPerspectives = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
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
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(365, 538);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(258, 538);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 20;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkRoles
            // 
            this.chkRoles.AutoSize = true;
            this.chkRoles.Checked = true;
            this.chkRoles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRoles.Location = new System.Drawing.Point(18, 149);
            this.chkRoles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkRoles.Name = "chkRoles";
            this.chkRoles.Size = new System.Drawing.Size(103, 20);
            this.chkRoles.TabIndex = 5;
            this.chkRoles.Text = "Include roles";
            this.chkRoles.UseVisualStyleBackColor = true;
            // 
            // chkPartitions
            // 
            this.chkPartitions.AutoSize = true;
            this.chkPartitions.Location = new System.Drawing.Point(18, 186);
            this.chkPartitions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPartitions.Name = "chkPartitions";
            this.chkPartitions.Size = new System.Drawing.Size(279, 20);
            this.chkPartitions.TabIndex = 6;
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
            this.chkMeasureDependencies.Location = new System.Drawing.Point(18, 318);
            this.chkMeasureDependencies.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkMeasureDependencies.Name = "chkMeasureDependencies";
            this.chkMeasureDependencies.Size = new System.Drawing.Size(51, 20);
            this.chkMeasureDependencies.TabIndex = 7;
            this.chkMeasureDependencies.Text = "XXX";
            this.chkMeasureDependencies.UseVisualStyleBackColor = true;
            // 
            // chkPerspectives
            // 
            this.chkPerspectives.AutoSize = true;
            this.chkPerspectives.Location = new System.Drawing.Point(18, 30);
            this.chkPerspectives.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPerspectives.Name = "chkPerspectives";
            this.chkPerspectives.Size = new System.Drawing.Size(151, 20);
            this.chkPerspectives.TabIndex = 3;
            this.chkPerspectives.Text = "Include perspectives";
            this.chkPerspectives.UseVisualStyleBackColor = true;
            this.chkPerspectives.CheckedChanged += new System.EventHandler(this.chkPerspectives_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.groupBox1.Location = new System.Drawing.Point(16, 14);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(450, 377);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comparison Options";
            // 
            // chkRetainStorageMode
            // 
            this.chkRetainStorageMode.AutoSize = true;
            this.chkRetainStorageMode.Location = new System.Drawing.Point(18, 286);
            this.chkRetainStorageMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkRetainStorageMode.Name = "chkRetainStorageMode";
            this.chkRetainStorageMode.Size = new System.Drawing.Size(258, 20);
            this.chkRetainStorageMode.TabIndex = 12;
            this.chkRetainStorageMode.Text = "For table updates, retain storage mode";
            this.chkRetainStorageMode.UseVisualStyleBackColor = true;
            // 
            // chkRetainPolicyPartitions
            // 
            this.chkRetainPolicyPartitions.AutoSize = true;
            this.chkRetainPolicyPartitions.Enabled = false;
            this.chkRetainPolicyPartitions.Location = new System.Drawing.Point(46, 250);
            this.chkRetainPolicyPartitions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkRetainPolicyPartitions.Name = "chkRetainPolicyPartitions";
            this.chkRetainPolicyPartitions.Size = new System.Drawing.Size(277, 20);
            this.chkRetainPolicyPartitions.TabIndex = 11;
            this.chkRetainPolicyPartitions.Text = "Retain only refresh-policy based partitions";
            this.chkRetainPolicyPartitions.UseVisualStyleBackColor = true;
            // 
            // chkRetainPartitions
            // 
            this.chkRetainPartitions.AutoSize = true;
            this.chkRetainPartitions.Location = new System.Drawing.Point(18, 224);
            this.chkRetainPartitions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkRetainPartitions.Name = "chkRetainPartitions";
            this.chkRetainPartitions.Size = new System.Drawing.Size(228, 20);
            this.chkRetainPartitions.TabIndex = 10;
            this.chkRetainPartitions.Text = "For table updates, retain partitions";
            this.chkRetainPartitions.UseVisualStyleBackColor = true;
            this.chkRetainPartitions.CheckedChanged += new System.EventHandler(this.ChkRetainPartitions_CheckedChanged);
            // 
            // chkMergeCultures
            // 
            this.chkMergeCultures.AutoSize = true;
            this.chkMergeCultures.Enabled = false;
            this.chkMergeCultures.Location = new System.Drawing.Point(46, 118);
            this.chkMergeCultures.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkMergeCultures.Name = "chkMergeCultures";
            this.chkMergeCultures.Size = new System.Drawing.Size(335, 20);
            this.chkMergeCultures.TabIndex = 9;
            this.chkMergeCultures.Text = "For culture updates, merge translations (not replace)";
            this.chkMergeCultures.UseVisualStyleBackColor = true;
            // 
            // chkCultures
            // 
            this.chkCultures.AutoSize = true;
            this.chkCultures.Location = new System.Drawing.Point(18, 92);
            this.chkCultures.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkCultures.Name = "chkCultures";
            this.chkCultures.Size = new System.Drawing.Size(119, 20);
            this.chkCultures.TabIndex = 8;
            this.chkCultures.Text = "Include cultures";
            this.chkCultures.UseVisualStyleBackColor = true;
            this.chkCultures.CheckedChanged += new System.EventHandler(this.chkCultures_CheckedChanged);
            // 
            // chkMergePerspectives
            // 
            this.chkMergePerspectives.AutoSize = true;
            this.chkMergePerspectives.Enabled = false;
            this.chkMergePerspectives.Location = new System.Drawing.Point(46, 57);
            this.chkMergePerspectives.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkMergePerspectives.Name = "chkMergePerspectives";
            this.chkMergePerspectives.Size = new System.Drawing.Size(360, 20);
            this.chkMergePerspectives.TabIndex = 4;
            this.chkMergePerspectives.Text = "For perspective updates, merge selections (not replace)";
            this.chkMergePerspectives.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkAffectedTables);
            this.groupBox2.Controls.Add(this.cboProcessingOption);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(16, 399);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(450, 124);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database Deployment";
            // 
            // chkAffectedTables
            // 
            this.chkAffectedTables.AutoSize = true;
            this.chkAffectedTables.Checked = true;
            this.chkAffectedTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAffectedTables.Location = new System.Drawing.Point(18, 78);
            this.chkAffectedTables.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkAffectedTables.Name = "chkAffectedTables";
            this.chkAffectedTables.Size = new System.Drawing.Size(196, 20);
            this.chkAffectedTables.TabIndex = 9;
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
            this.cboProcessingOption.Location = new System.Drawing.Point(152, 30);
            this.cboProcessingOption.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboProcessingOption.Name = "cboProcessingOption";
            this.cboProcessingOption.Size = new System.Drawing.Size(173, 24);
            this.cboProcessingOption.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Processing Option:";
            // 
            // Options
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(482, 586);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
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
    }
}