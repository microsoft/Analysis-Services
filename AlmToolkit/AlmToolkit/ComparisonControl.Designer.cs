namespace AlmToolkit
{
    partial class ComparisonControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComparisonControl));
            this.scDifferenceResults = new System.Windows.Forms.SplitContainer();
            this.treeGridComparisonResults = new BismNormalizer.TabularCompare.UI.TreeGridViewComparison();
            this.TreeGridImageList = new System.Windows.Forms.ImageList(this.components);
            this.scObjectDefinitions = new System.Windows.Forms.SplitContainer();
            this.txtSourceObjectDefinition = new BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTargetObjectDefinition = new BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.scDifferenceResults)).BeginInit();
            this.scDifferenceResults.Panel1.SuspendLayout();
            this.scDifferenceResults.Panel2.SuspendLayout();
            this.scDifferenceResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridComparisonResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scObjectDefinitions)).BeginInit();
            this.scObjectDefinitions.Panel1.SuspendLayout();
            this.scObjectDefinitions.Panel2.SuspendLayout();
            this.scObjectDefinitions.SuspendLayout();
            this.SuspendLayout();
            // 
            // scDifferenceResults
            // 
            this.scDifferenceResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scDifferenceResults.Location = new System.Drawing.Point(0, 0);
            this.scDifferenceResults.Name = "scDifferenceResults";
            this.scDifferenceResults.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scDifferenceResults.Panel1
            // 
            this.scDifferenceResults.Panel1.Controls.Add(this.treeGridComparisonResults);
            // 
            // scDifferenceResults.Panel2
            // 
            this.scDifferenceResults.Panel2.Controls.Add(this.scObjectDefinitions);
            this.scDifferenceResults.Size = new System.Drawing.Size(653, 565);
            this.scDifferenceResults.SplitterDistance = 411;
            this.scDifferenceResults.TabIndex = 2;
            // 
            // treeGridComparisonResults
            // 
            this.treeGridComparisonResults.AllowUserToAddRows = false;
            this.treeGridComparisonResults.AllowUserToDeleteRows = false;
            this.treeGridComparisonResults.AllowUserToResizeRows = false;
            this.treeGridComparisonResults.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.treeGridComparisonResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.treeGridComparisonResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.treeGridComparisonResults.Comparison = null;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.treeGridComparisonResults.DefaultCellStyle = dataGridViewCellStyle2;
            this.treeGridComparisonResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGridComparisonResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.treeGridComparisonResults.ImageList = this.TreeGridImageList;
            this.treeGridComparisonResults.Location = new System.Drawing.Point(0, 0);
            this.treeGridComparisonResults.Name = "treeGridComparisonResults";
            this.treeGridComparisonResults.RowHeadersVisible = false;
            this.treeGridComparisonResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.treeGridComparisonResults.Size = new System.Drawing.Size(653, 411);
            this.treeGridComparisonResults.TabIndex = 0;
            this.treeGridComparisonResults.Unloading = false;
            this.treeGridComparisonResults.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.treeGridComparisonResults_DataError);
            this.treeGridComparisonResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeGridComparisonResults_MouseUp);
            // 
            // TreeGridImageList
            // 
            this.TreeGridImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeGridImageList.ImageStream")));
            this.TreeGridImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.TreeGridImageList.Images.SetKeyName(0, "Connection.png");
            this.TreeGridImageList.Images.SetKeyName(1, "Table.png");
            this.TreeGridImageList.Images.SetKeyName(2, "Relationship.png");
            this.TreeGridImageList.Images.SetKeyName(3, "BismMeasure.png");
            this.TreeGridImageList.Images.SetKeyName(4, "KPI.png");
            this.TreeGridImageList.Images.SetKeyName(5, "DeleteAction.png");
            this.TreeGridImageList.Images.SetKeyName(6, "UpdateAction.png");
            this.TreeGridImageList.Images.SetKeyName(7, "CreateAction.png");
            this.TreeGridImageList.Images.SetKeyName(8, "SkipAction.png");
            this.TreeGridImageList.Images.SetKeyName(9, "Plus.png");
            this.TreeGridImageList.Images.SetKeyName(10, "Minus.png");
            this.TreeGridImageList.Images.SetKeyName(11, "Informational.png");
            this.TreeGridImageList.Images.SetKeyName(12, "Warning.png");
            this.TreeGridImageList.Images.SetKeyName(13, "WarningToolWindow.png");
            this.TreeGridImageList.Images.SetKeyName(14, "Role.png");
            this.TreeGridImageList.Images.SetKeyName(15, "Perspective.png");
            this.TreeGridImageList.Images.SetKeyName(16, "Action.png");
            this.TreeGridImageList.Images.SetKeyName(17, "CompareBismModels_Small.png");
            this.TreeGridImageList.Images.SetKeyName(18, "DeleteActionGrey.png");
            this.TreeGridImageList.Images.SetKeyName(19, "SkipActionGrey.png");
            this.TreeGridImageList.Images.SetKeyName(20, "CreateActionGrey.png");
            this.TreeGridImageList.Images.SetKeyName(21, "Culture.png");
            this.TreeGridImageList.Images.SetKeyName(22, "Expression.png");
            this.TreeGridImageList.Images.SetKeyName(23, "CalculationGroup.png");
            this.TreeGridImageList.Images.SetKeyName(24, "CalculationItem.png");
            this.TreeGridImageList.Images.SetKeyName(25, "Model.png");
            this.TreeGridImageList.Images.SetKeyName(26, "RefreshPolicy.png");
            // 
            // scObjectDefinitions
            // 
            this.scObjectDefinitions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scObjectDefinitions.Location = new System.Drawing.Point(0, 0);
            this.scObjectDefinitions.Name = "scObjectDefinitions";
            // 
            // scObjectDefinitions.Panel1
            // 
            this.scObjectDefinitions.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.scObjectDefinitions.Panel1.Controls.Add(this.txtSourceObjectDefinition);
            this.scObjectDefinitions.Panel1.Controls.Add(this.label4);
            // 
            // scObjectDefinitions.Panel2
            // 
            this.scObjectDefinitions.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.scObjectDefinitions.Panel2.Controls.Add(this.txtTargetObjectDefinition);
            this.scObjectDefinitions.Panel2.Controls.Add(this.label5);
            this.scObjectDefinitions.Size = new System.Drawing.Size(653, 150);
            this.scObjectDefinitions.SplitterDistance = 331;
            this.scObjectDefinitions.TabIndex = 0;
            // 
            // txtSourceObjectDefinition
            // 
            this.txtSourceObjectDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceObjectDefinition.BackColor = System.Drawing.Color.White;
            this.txtSourceObjectDefinition.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSourceObjectDefinition.Location = new System.Drawing.Point(0, 16);
            this.txtSourceObjectDefinition.Name = "txtSourceObjectDefinition";
            this.txtSourceObjectDefinition.ReadOnly = true;
            this.txtSourceObjectDefinition.Size = new System.Drawing.Size(331, 134);
            this.txtSourceObjectDefinition.TabIndex = 1;
            this.txtSourceObjectDefinition.Text = "";
            this.txtSourceObjectDefinition.WordWrap = false;
            this.txtSourceObjectDefinition.vScroll += new BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox.vScrollEventHandler(this.txtSourceObjectDefinition_vScroll);
            this.txtSourceObjectDefinition.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSourceObjectDefinition_KeyUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Source Object Definition";
            // 
            // txtTargetObjectDefinition
            // 
            this.txtTargetObjectDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetObjectDefinition.BackColor = System.Drawing.Color.White;
            this.txtTargetObjectDefinition.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetObjectDefinition.Location = new System.Drawing.Point(0, 16);
            this.txtTargetObjectDefinition.Name = "txtTargetObjectDefinition";
            this.txtTargetObjectDefinition.ReadOnly = true;
            this.txtTargetObjectDefinition.Size = new System.Drawing.Size(313, 134);
            this.txtTargetObjectDefinition.TabIndex = 2;
            this.txtTargetObjectDefinition.Text = "";
            this.txtTargetObjectDefinition.WordWrap = false;
            this.txtTargetObjectDefinition.vScroll += new BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox.vScrollEventHandler(this.txtTargetObjectDefinition_vScroll);
            this.txtTargetObjectDefinition.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtTargetObjectDefinition_KeyUp);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Target Object Definition";
            // 
            // ComparisonControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scDifferenceResults);
            this.Name = "ComparisonControl";
            this.Size = new System.Drawing.Size(653, 565);
            this.Load += new System.EventHandler(this.ComparisonControl_Load);
            this.scDifferenceResults.Panel1.ResumeLayout(false);
            this.scDifferenceResults.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDifferenceResults)).EndInit();
            this.scDifferenceResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeGridComparisonResults)).EndInit();
            this.scObjectDefinitions.Panel1.ResumeLayout(false);
            this.scObjectDefinitions.Panel1.PerformLayout();
            this.scObjectDefinitions.Panel2.ResumeLayout(false);
            this.scObjectDefinitions.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scObjectDefinitions)).EndInit();
            this.scObjectDefinitions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BismNormalizer.TabularCompare.UI.TreeGridViewComparison treeGridComparisonResults;
        private System.Windows.Forms.SplitContainer scDifferenceResults;
        private System.Windows.Forms.SplitContainer scObjectDefinitions;
        private BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox txtSourceObjectDefinition;
        private System.Windows.Forms.Label label4;
        private BismNormalizer.TabularCompare.UI.SynchronizedScrollRichTextBox txtTargetObjectDefinition;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.ImageList TreeGridImageList;
    }
}
