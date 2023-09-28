using BismNormalizer.TabularCompare;
using BismNormalizer.TabularCompare.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmToolkit
{
    public partial class WarningListForm : Form
    {
        public WarningListForm()
        {
            InitializeComponent();
        }

        private Comparison _comparison;
        private ImageList _treeGridImageList;

        public Comparison Comparison
        {
            get { return _comparison; }
            set { _comparison = value; }
        }

        public ImageList TreeGridImageList
        {
            get { return _treeGridImageList; }
            set { _treeGridImageList = value; }
        }


        private void WarningListForm_Load(object sender, EventArgs e)
        {
            _treeGridImageList = this.TreeGridImageList2;
            validationOutput.ClearMessages(0);
            validationOutput.SetImageList(_treeGridImageList);
            _comparison.ValidationMessage += HandleValidationMessage;
            _comparison.ResizeValidationHeaders += HandleResizeValidationHeaders;
        }

        private void WarningListForm_Shown(object sender, EventArgs e)
        {
            _comparison.ValidateSelection();
        }

        public void HandleValidationMessage(object sender, ValidationMessageEventArgs e)
        {
            validationOutput.ShowStatusMessage(
                0,
                "ALM Toolkit Comparison",
                e.Message,
                e.ValidationMessageType,
                e.ValidationMessageStatus);
        }

        public void HandleResizeValidationHeaders(object sender, EventArgs e)
        {
            validationOutput.ResizeValidationHeaders();
        }

    }
}
