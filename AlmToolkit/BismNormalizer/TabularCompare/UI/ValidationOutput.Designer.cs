using BismNormalizer.TabularCompare.UI;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare.UI
{
    partial class ValidationOutput
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
            this.validationOutputButtons = new BismNormalizer.TabularCompare.UI.ValidationOutputButtons();
            this.treeGridViewValidationOutput = new BismNormalizer.TabularCompare.UI.TreeGridViewValidationOutput();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridViewValidationOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // validationOutputButtons
            // 
            this.validationOutputButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.validationOutputButtons.HpiScaleFactor = 1F;
            this.validationOutputButtons.InformationalMessageCount = 0;
            this.validationOutputButtons.InformationalMessagesVisible = true;
            this.validationOutputButtons.Location = new System.Drawing.Point(0, 0);
            this.validationOutputButtons.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.validationOutputButtons.Name = "validationOutputButtons";
            this.validationOutputButtons.Size = new System.Drawing.Size(494, 24);
            this.validationOutputButtons.TabIndex = 1;
            this.validationOutputButtons.WarningCount = 0;
            this.validationOutputButtons.WarningsVisible = true;
            // 
            // treeGridViewValidationOutput
            // 
            this.treeGridViewValidationOutput.AllowUserToAddRows = false;
            this.treeGridViewValidationOutput.AllowUserToDeleteRows = false;
            this.treeGridViewValidationOutput.AllowUserToResizeRows = false;
            this.treeGridViewValidationOutput.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.treeGridViewValidationOutput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.treeGridViewValidationOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGridViewValidationOutput.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.treeGridViewValidationOutput.ImageList = null;
            this.treeGridViewValidationOutput.InformationalMessagesVisible = false;
            this.treeGridViewValidationOutput.Location = new System.Drawing.Point(0, 24);
            this.treeGridViewValidationOutput.Name = "treeGridViewValidationOutput";
            this.treeGridViewValidationOutput.ReadOnly = true;
            this.treeGridViewValidationOutput.RowHeadersVisible = false;
            this.treeGridViewValidationOutput.Size = new System.Drawing.Size(494, 341);
            this.treeGridViewValidationOutput.TabIndex = 0;
            this.treeGridViewValidationOutput.Unloading = false;
            this.treeGridViewValidationOutput.WarningsVisible = false;
            // 
            // ValidationOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeGridViewValidationOutput);
            this.Controls.Add(this.validationOutputButtons);
            this.Name = "ValidationOutput";
            this.Size = new System.Drawing.Size(494, 365);
            this.Load += new System.EventHandler(this.ValidationOutput_Load);
            ((System.ComponentModel.ISupportInitialize)(this.treeGridViewValidationOutput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TreeGridViewValidationOutput treeGridViewValidationOutput;
        private ValidationOutputButtons validationOutputButtons;

    }
}
