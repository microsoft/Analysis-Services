namespace BismNormalizer.TabularCompare.UI
{
    partial class Deployment
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Deployment));
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStopProcessing = new System.Windows.Forms.Button();
            this.gridProcessing = new System.Windows.Forms.DataGridView();
            this.DeployImageList = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.picStatus = new System.Windows.Forms.PictureBox();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ImageCol = new System.Windows.Forms.DataGridViewImageColumn();
            this.WorkItemCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridProcessing)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Enabled = false;
            this.btnClose.Location = new System.Drawing.Point(366, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStopProcessing
            // 
            this.btnStopProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopProcessing.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnStopProcessing.Enabled = false;
            this.btnStopProcessing.Location = new System.Drawing.Point(246, 12);
            this.btnStopProcessing.Name = "btnStopProcessing";
            this.btnStopProcessing.Size = new System.Drawing.Size(114, 23);
            this.btnStopProcessing.TabIndex = 2;
            this.btnStopProcessing.Text = "Stop Processing";
            this.btnStopProcessing.UseVisualStyleBackColor = true;
            this.btnStopProcessing.Click += new System.EventHandler(this.btnStopProcessing_Click);
            // 
            // gridProcessing
            // 
            this.gridProcessing.AllowUserToAddRows = false;
            this.gridProcessing.AllowUserToDeleteRows = false;
            this.gridProcessing.AllowUserToResizeRows = false;
            this.gridProcessing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridProcessing.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageCol,
            this.WorkItemCol,
            this.StatusCol});
            this.gridProcessing.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridProcessing.Location = new System.Drawing.Point(0, 51);
            this.gridProcessing.MultiSelect = false;
            this.gridProcessing.Name = "gridProcessing";
            this.gridProcessing.ReadOnly = true;
            this.gridProcessing.RowHeadersVisible = false;
            this.gridProcessing.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridProcessing.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridProcessing.Size = new System.Drawing.Size(457, 355);
            this.gridProcessing.TabIndex = 3;
            // 
            // DeployImageList
            // 
            this.DeployImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("DeployImageList.ImageStream")));
            this.DeployImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.DeployImageList.Images.SetKeyName(0, "Processing.png");
            this.DeployImageList.Images.SetKeyName(1, "Check.png");
            this.DeployImageList.Images.SetKeyName(2, "Error.png");
            this.DeployImageList.Images.SetKeyName(3, "Warning.png");
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.picStatus);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(457, 51);
            this.panel1.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(52, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(102, 18);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Deploying ...";
            // 
            // picStatus
            // 
            this.picStatus.Image = global::BismNormalizer.Resources.Progress;
            this.picStatus.Location = new System.Drawing.Point(14, 8);
            this.picStatus.Name = "picStatus";
            this.picStatus.Size = new System.Drawing.Size(32, 32);
            this.picStatus.TabIndex = 0;
            this.picStatus.TabStop = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Work Item";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 120;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Status";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 220;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnStopProcessing);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 409);
            this.panel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(457, 52);
            this.panel2.TabIndex = 5;
            // 
            // ImageCol
            // 
            this.ImageCol.Frozen = true;
            this.ImageCol.HeaderText = "";
            this.ImageCol.Name = "ImageCol";
            this.ImageCol.ReadOnly = true;
            this.ImageCol.Width = 26;
            // 
            // WorkItemCol
            // 
            this.WorkItemCol.HeaderText = "Work Item";
            this.WorkItemCol.Name = "WorkItemCol";
            this.WorkItemCol.ReadOnly = true;
            this.WorkItemCol.Width = 131;
            // 
            // StatusCol
            // 
            this.StatusCol.HeaderText = "Status";
            this.StatusCol.Name = "StatusCol";
            this.StatusCol.ReadOnly = true;
            this.StatusCol.Width = 280;
            // 
            // Deployment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 461);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.gridProcessing);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Deployment";
            this.Text = "Deploy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Deploy_FormClosing);
            this.Load += new System.EventHandler(this.Deploy_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridProcessing)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnStopProcessing;
        private System.Windows.Forms.DataGridView gridProcessing;
        private System.Windows.Forms.ImageList DeployImageList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewImageColumn ImageCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkItemCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusCol;
    }
}