namespace AlmToolkit
{
    partial class WarningListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarningListForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.validationOutput = new BismNormalizer.TabularCompare.UI.ValidationOutput();
            this.panelOk = new System.Windows.Forms.Panel();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.TreeGridImageList2 = new System.Windows.Forms.ImageList(this.components);
            this.panelOk.SuspendLayout();
            this.panelGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(609, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 20;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // validationOutput
            // 
            this.validationOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.validationOutput.Location = new System.Drawing.Point(0, 0);
            this.validationOutput.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.validationOutput.Name = "validationOutput";
            this.validationOutput.Size = new System.Drawing.Size(696, 342);
            this.validationOutput.TabIndex = 22;
            // 
            // panelOk
            // 
            this.panelOk.Controls.Add(this.btnOK);
            this.panelOk.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelOk.Location = new System.Drawing.Point(0, 342);
            this.panelOk.Name = "panelOk";
            this.panelOk.Size = new System.Drawing.Size(696, 47);
            this.panelOk.TabIndex = 23;
            // 
            // panelGrid
            // 
            this.panelGrid.Controls.Add(this.validationOutput);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(0, 0);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Size = new System.Drawing.Size(696, 342);
            this.panelGrid.TabIndex = 24;
            // 
            // TreeGridImageList2
            // 
            this.TreeGridImageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeGridImageList2.ImageStream")));
            this.TreeGridImageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.TreeGridImageList2.Images.SetKeyName(0, "Connection.png");
            this.TreeGridImageList2.Images.SetKeyName(1, "Table.png");
            this.TreeGridImageList2.Images.SetKeyName(2, "Relationship.png");
            this.TreeGridImageList2.Images.SetKeyName(3, "BismMeasure.png");
            this.TreeGridImageList2.Images.SetKeyName(4, "KPI.png");
            this.TreeGridImageList2.Images.SetKeyName(5, "DeleteAction.png");
            this.TreeGridImageList2.Images.SetKeyName(6, "UpdateAction.png");
            this.TreeGridImageList2.Images.SetKeyName(7, "CreateAction.png");
            this.TreeGridImageList2.Images.SetKeyName(8, "SkipAction.png");
            this.TreeGridImageList2.Images.SetKeyName(9, "Plus.png");
            this.TreeGridImageList2.Images.SetKeyName(10, "Minus.png");
            this.TreeGridImageList2.Images.SetKeyName(11, "Informational.png");
            this.TreeGridImageList2.Images.SetKeyName(12, "Warning.png");
            this.TreeGridImageList2.Images.SetKeyName(13, "WarningToolWindow.png");
            this.TreeGridImageList2.Images.SetKeyName(14, "Role.png");
            this.TreeGridImageList2.Images.SetKeyName(15, "Perspective.png");
            this.TreeGridImageList2.Images.SetKeyName(16, "Action.png");
            this.TreeGridImageList2.Images.SetKeyName(17, "CompareBismModels_Small.png");
            this.TreeGridImageList2.Images.SetKeyName(18, "DeleteActionGrey.png");
            this.TreeGridImageList2.Images.SetKeyName(19, "SkipActionGrey.png");
            this.TreeGridImageList2.Images.SetKeyName(20, "CreateActionGrey.png");
            this.TreeGridImageList2.Images.SetKeyName(21, "Culture.png");
            this.TreeGridImageList2.Images.SetKeyName(22, "Expression.png");
            this.TreeGridImageList2.Images.SetKeyName(23, "CalculationGroup.png");
            this.TreeGridImageList2.Images.SetKeyName(24, "CalculationItem.png");
            this.TreeGridImageList2.Images.SetKeyName(25, "Model.png");
            this.TreeGridImageList2.Images.SetKeyName(26, "RefreshPolicy.png");
            // 
            // WarningListForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(696, 389);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "WarningListForm";
            this.Text = "Warning List";
            this.Load += new System.EventHandler(this.WarningListForm_Load);
            this.Shown += new System.EventHandler(this.WarningListForm_Shown);
            this.panelOk.ResumeLayout(false);
            this.panelGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private BismNormalizer.TabularCompare.UI.ValidationOutput validationOutput;
        private System.Windows.Forms.Panel panelOk;
        private System.Windows.Forms.Panel panelGrid;
        public System.Windows.Forms.ImageList TreeGridImageList2;
    }
}