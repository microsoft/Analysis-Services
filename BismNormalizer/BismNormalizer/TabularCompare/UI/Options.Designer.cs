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
            this.chkMergeCultures = new System.Windows.Forms.CheckBox();
            this.chkCultures = new System.Windows.Forms.CheckBox();
            this.chkMergePerspectives = new System.Windows.Forms.CheckBox();
            this.chkRetainPartitions = new System.Windows.Forms.CheckBox();
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
            this.btnCancel.Location = new System.Drawing.Point(274, 393);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(193, 393);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
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
            this.chkRoles.Location = new System.Drawing.Point(13, 124);
            this.chkRoles.Name = "chkRoles";
            this.chkRoles.Size = new System.Drawing.Size(86, 17);
            this.chkRoles.TabIndex = 5;
            this.chkRoles.Text = "Include roles";
            this.chkRoles.UseVisualStyleBackColor = true;
            // 
            // chkPartitions
            // 
            this.chkPartitions.AutoSize = true;
            this.chkPartitions.Location = new System.Drawing.Point(13, 152);
            this.chkPartitions.Name = "chkPartitions";
            this.chkPartitions.Size = new System.Drawing.Size(224, 17);
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
            this.chkMeasureDependencies.Location = new System.Drawing.Point(13, 208);
            this.chkMeasureDependencies.Name = "chkMeasureDependencies";
            this.chkMeasureDependencies.Size = new System.Drawing.Size(47, 17);
            this.chkMeasureDependencies.TabIndex = 7;
            this.chkMeasureDependencies.Text = "XXX";
            this.chkMeasureDependencies.UseVisualStyleBackColor = true;
            // 
            // chkPerspectives
            // 
            this.chkPerspectives.AutoSize = true;
            this.chkPerspectives.Location = new System.Drawing.Point(13, 25);
            this.chkPerspectives.Name = "chkPerspectives";
            this.chkPerspectives.Size = new System.Drawing.Size(124, 17);
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
            this.groupBox1.Controls.Add(this.chkRetainPartitions);
            this.groupBox1.Controls.Add(this.chkMergeCultures);
            this.groupBox1.Controls.Add(this.chkCultures);
            this.groupBox1.Controls.Add(this.chkMergePerspectives);
            this.groupBox1.Controls.Add(this.chkPerspectives);
            this.groupBox1.Controls.Add(this.chkMeasureDependencies);
            this.groupBox1.Controls.Add(this.chkPartitions);
            this.groupBox1.Controls.Add(this.chkRoles);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 256);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comparison Options";
            // 
            // chkMergeCultures
            // 
            this.chkMergeCultures.AutoSize = true;
            this.chkMergeCultures.Enabled = false;
            this.chkMergeCultures.Location = new System.Drawing.Point(37, 98);
            this.chkMergeCultures.Name = "chkMergeCultures";
            this.chkMergeCultures.Size = new System.Drawing.Size(270, 17);
            this.chkMergeCultures.TabIndex = 9;
            this.chkMergeCultures.Text = "For culture updates, merge translations (not replace)";
            this.chkMergeCultures.UseVisualStyleBackColor = true;
            // 
            // chkCultures
            // 
            this.chkCultures.AutoSize = true;
            this.chkCultures.Location = new System.Drawing.Point(15, 75);
            this.chkCultures.Name = "chkCultures";
            this.chkCultures.Size = new System.Drawing.Size(101, 17);
            this.chkCultures.TabIndex = 8;
            this.chkCultures.Text = "Include cultures";
            this.chkCultures.UseVisualStyleBackColor = true;
            this.chkCultures.CheckedChanged += new System.EventHandler(this.chkCultures_CheckedChanged);
            // 
            // chkMergePerspectives
            // 
            this.chkMergePerspectives.AutoSize = true;
            this.chkMergePerspectives.Enabled = false;
            this.chkMergePerspectives.Location = new System.Drawing.Point(35, 48);
            this.chkMergePerspectives.Name = "chkMergePerspectives";
            this.chkMergePerspectives.Size = new System.Drawing.Size(287, 17);
            this.chkMergePerspectives.TabIndex = 4;
            this.chkMergePerspectives.Text = "For perspective updates, merge selections (not replace)";
            this.chkMergePerspectives.UseVisualStyleBackColor = true;
            // 
            // chkRetainPartitions
            // 
            this.chkRetainPartitions.AutoSize = true;
            this.chkRetainPartitions.Location = new System.Drawing.Point(13, 181);
            this.chkRetainPartitions.Name = "chkRetainPartitions";
            this.chkRetainPartitions.Size = new System.Drawing.Size(247, 17);
            this.chkRetainPartitions.TabIndex = 10;
            this.chkRetainPartitions.Text = "For table updates, retain partitions (not replace)";
            this.chkRetainPartitions.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkAffectedTables);
            this.groupBox2.Controls.Add(this.cboProcessingOption);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 274);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(337, 101);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database Deployment";
            // 
            // chkAffectedTables
            // 
            this.chkAffectedTables.AutoSize = true;
            this.chkAffectedTables.Checked = true;
            this.chkAffectedTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAffectedTables.Location = new System.Drawing.Point(13, 64);
            this.chkAffectedTables.Name = "chkAffectedTables";
            this.chkAffectedTables.Size = new System.Drawing.Size(159, 17);
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
            "Default",
            "Do Not Process",
            "Full"});
            this.cboProcessingOption.Location = new System.Drawing.Point(114, 25);
            this.cboProcessingOption.Name = "cboProcessingOption";
            this.cboProcessingOption.Size = new System.Drawing.Size(131, 21);
            this.cboProcessingOption.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Processing Option:";
            // 
            // Options
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(361, 428);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "BISM Normalizer Options";
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
    }
}