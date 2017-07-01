using System;
using System.Windows.Forms;
using BismNormalizer.TabularCompare.Core;
using System.Drawing;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class ValidationOutput : UserControl
    {
        private ValidationOutputButtons _validationOutputButtons = new ValidationOutputButtons();

        public ValidationOutput()
        {
            InitializeComponent();
        }

        private void ValidationOutput_Load(object sender, EventArgs e)
        {
            treeGridViewValidationOutput.SetupForValidationOutput();
            validationOutputButtons.WarningsVisible = Settings.Default.WarningsVisible;
            validationOutputButtons.InformationalMessagesVisible = Settings.Default.InformationalMessagesVisible;
            treeGridViewValidationOutput.InformationalMessagesVisible = validationOutputButtons.InformationalMessagesVisible;
            treeGridViewValidationOutput.WarningsVisible = validationOutputButtons.WarningsVisible;

            validationOutputButtons.InformationalMessageValueChangedHandler += new EventHandler(validationOutputButtons_InformationalMessageValueChangedHandler);
            validationOutputButtons.WarningValueChangedHandler += new EventHandler(validationOutputButtons_WarningValueChangedHandler);
        }

        void validationOutputButtons_WarningValueChangedHandler(object sender, EventArgs e)
        {
            treeGridViewValidationOutput.WarningsVisible = validationOutputButtons.WarningsVisible;
            Settings.Default.WarningsVisible = validationOutputButtons.WarningsVisible;
            Settings.Default.Save();
        }

        void validationOutputButtons_InformationalMessageValueChangedHandler(object sender, EventArgs e)
        {
            treeGridViewValidationOutput.InformationalMessagesVisible = validationOutputButtons.InformationalMessagesVisible;
            Settings.Default.InformationalMessagesVisible = validationOutputButtons.InformationalMessagesVisible;
            Settings.Default.Save();
        }

        public void SetImageList(ImageList treeGridImageList)
        {
            treeGridViewValidationOutput.ImageList = treeGridImageList;
        }

        public void ClearMessages(int windowHandle)
        {
            treeGridViewValidationOutput.ClearMessages(windowHandle);

            validationOutputButtons.InformationalMessageCount = treeGridViewValidationOutput.MessageCountByStatus(ValidationMessageStatus.Informational);
            validationOutputButtons.WarningCount = treeGridViewValidationOutput.MessageCountByStatus(ValidationMessageStatus.Warning);
        }

        public void ShowStatusMessage(int windowHandle, string windowName, string message, ValidationMessageType validationMessageType, ValidationMessageStatus validationMessageStatus)
        {
            validationOutputButtons.Visible = true; 
            treeGridViewValidationOutput.Visible = true;

            treeGridViewValidationOutput.ShowStatusMessage(windowHandle, windowName, message, validationMessageType, validationMessageStatus);

            if (validationMessageStatus == ValidationMessageStatus.Informational)
            {
                validationOutputButtons.InformationalMessageCount += 1;
            }
            else
            {
                validationOutputButtons.WarningCount += 1;
            }

            this.Refresh();
        }

        public void Rescale(float scaleFactor)
        {
            validationOutputButtons.HpiScaleFactor = (scaleFactor > 1 ? scaleFactor * 0.918f : 1); //complete fudge. todo: replace with a toolbar instead of tabstrip - or use the VS error list
            treeGridViewValidationOutput.Scale(new SizeF(scaleFactor, scaleFactor));
            treeGridViewValidationOutput.ResetColumnWidths(scaleFactor);
        }

        public void ResizeValidationHeaders()
        {
            if (treeGridViewValidationOutput.Rows.Count > 0)
            {
                treeGridViewValidationOutput.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }
    }
}
