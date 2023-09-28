using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare.UI
{
    /// <summary>
    /// TreeGridView that is used for showing validation messages.
    /// </summary>
    [System.ComponentModel.DesignerCategory("code"),
        //Designer(typeof(System.Windows.Forms.Design.ControlDesigner)),
    ComplexBindingProperties(),
    Docking(DockingBehavior.Ask)]
    public class TreeGridViewValidationOutput : TreeGridView
    {
        public TreeGridViewValidationOutput() : base()
        {
        }

        private bool _informationalMessagesVisible;
        private bool _warningsVisible;

        /// <summary>
        /// Setup columns ready for showing validation messages
        /// </summary>
        public void SetupForValidationOutput()
        {
            //Set up columns

            //0 (tree grid view seems to not always support accessing by name, so using IDs)
            TreeGridColumn typeLabelColumn = new TreeGridColumn();
            typeLabelColumn.Name = "TypeLabel";
            typeLabelColumn.HeaderText = "";
            typeLabelColumn.ReadOnly = true;
            typeLabelColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            typeLabelColumn.Width = 91;
            typeLabelColumn.Resizable = DataGridViewTriState.False;
            this.Columns.Add(typeLabelColumn);

            //1
            DataGridViewTextBoxColumn messageColumn = new DataGridViewTextBoxColumn();
            messageColumn.Name = "Message";
            messageColumn.HeaderText = "Message";
            messageColumn.ReadOnly = true;
            messageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.Columns.Add(messageColumn);

            this.DefaultCellStyle.BackColor = Color.White;

            if (this.Width >= 1400)
            {
                messageColumn.Width = this.Width - 100;
            }
            else
            {
                messageColumn.Width = 1300;
            }

            // Will be shown when there are some messages
            this.Visible = false;
        }

        public void ClearMessages(int windowHandle)
        {
            TreeGridNode topLevelNodeToRemove = null;
            foreach (TreeGridNode topLevelNode in this.Nodes)
            {
                if (topLevelNode.Tag != null && Convert.ToInt32(topLevelNode.Tag) == windowHandle)
                {
                    topLevelNodeToRemove = topLevelNode;
                    break;
                }
            }
            if (topLevelNodeToRemove != null)
            {
                this.Nodes.Remove(topLevelNodeToRemove);
            }
        }

        public void ShowStatusMessage(int windowHandle, string windowName, string message, ValidationMessageType validationMessageType, ValidationMessageStatus validationMessageStatus)
        {
            // There are 3 types of nodes:
            //  1) Top Level (for a comparion window)
            //  2) Type Node (for grouping of messages by type, e.g. DataSources, tables, etc.)
            //  3) Message node (that contains the actual message), which can be either a warning or informational

            //This method will add at least 1 message node and its parent nodes only if necessary.
            //The nodes are differentiated and identified based on their level and the .Tag property


            TreeGridNode messageNode;  //node that will contain the actual message when we are done

            TreeGridNode topLevelNodeForHandle = null;
            foreach (TreeGridNode topLevelNode in this.Nodes)
            {
                if (topLevelNode.Tag != null && Convert.ToInt32(topLevelNode.Tag) == windowHandle)
                {
                    topLevelNodeForHandle = topLevelNode;
                    break;
                }
            }
            if (topLevelNodeForHandle == null)
            {
                //Didn't find match, so need to create new node
                topLevelNodeForHandle = this.Nodes.Add();
                topLevelNodeForHandle.Tag = windowHandle;
                topLevelNodeForHandle.Cells[1].Value = windowName;
                topLevelNodeForHandle.Cells[1].Style.Font = new Font(Font.SystemFontName, 8, FontStyle.Bold);
                topLevelNodeForHandle.ImageIndex = 17;
                //topLevelNodeForHandle.Visible = false;
            }

            TreeGridNode particularTypeNode = null;
            switch (validationMessageType)
            {
                case ValidationMessageType.Model:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Model");
                    particularTypeNode.ImageIndex = 25;
                    break;
                case ValidationMessageType.DataSource:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Data Sources");
                    particularTypeNode.ImageIndex = 0;
                    break;
                case ValidationMessageType.Table:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Tables");
                    particularTypeNode.ImageIndex = 1;
                    break;
                case ValidationMessageType.Relationship:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Relationships");
                    particularTypeNode.ImageIndex = 2;
                    break;
                case ValidationMessageType.Measure:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Measures");
                    particularTypeNode.ImageIndex = 3;
                    break;
                case ValidationMessageType.Kpi:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "KPIs");
                    particularTypeNode.ImageIndex = 4;
                    break;
                case ValidationMessageType.CalculationItem:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Calculation Items");
                    particularTypeNode.ImageIndex = 24;
                    break;
                case ValidationMessageType.CalculationGroup:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Calculation Groups");
                    particularTypeNode.ImageIndex = 23;
                    break;
                case ValidationMessageType.Expression:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Expression");
                    particularTypeNode.ImageIndex = 22;
                    break;
                case ValidationMessageType.Perspective:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Perspectives");
                    particularTypeNode.ImageIndex = 15;
                    break;
                case ValidationMessageType.Culture:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Culture");
                    particularTypeNode.ImageIndex = 21;
                    break;
                case ValidationMessageType.Role:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Roles");
                    particularTypeNode.ImageIndex = 14;
                    break;
                case ValidationMessageType.Action:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Actions");
                    particularTypeNode.ImageIndex = 16;
                    break;
                //case ValidationMessageType.RefreshPolicy:
                //    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Refresh Policy");
                //    particularTypeNode.ImageIndex = 26;
                //    break;
                case ValidationMessageType.MeasureCalculationDependency:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Measure Calculation Dependencies");
                    particularTypeNode.ImageIndex = 3;
                    break;
                case ValidationMessageType.AggregationDependency:
                    particularTypeNode = FindOrCreateTypeNode(topLevelNodeForHandle, "Aggregation Dependencies");
                    particularTypeNode.ImageIndex = 2;
                    break;
                default:
                    //Something is wrong, better get out of here.
                    return;
            }
            messageNode = particularTypeNode.Nodes.Add();
            topLevelNodeForHandle.Expand();
            particularTypeNode.Expand();


            messageNode.Visible = false;
            if (validationMessageStatus == ValidationMessageStatus.Informational)
            {
                messageNode.ImageIndex = 11;
                if (_informationalMessagesVisible)
                {
                    messageNode.Visible = true;
                    messageNode.Parent.Visible = true;
                }
            }
            else
            {
                messageNode.ImageIndex = 12;
                if (_warningsVisible)
                {
                    messageNode.Visible = true;
                    messageNode.Parent.Visible = true;
                }
            }
            messageNode.Tag = validationMessageStatus;
            messageNode.Cells[1].Value = message;

            if (messageNode.Visible && this.Height > 0)
            {
                // autoscroll
                this.FirstDisplayedCell = this.Rows[this.Rows.Count - 1].Cells[0];
                this.Refresh();
            }
        }

        private TreeGridNode FindOrCreateTypeNode(TreeGridNode topLevelNodeForHandle, string particularType)
        {
            TreeGridNode particularTypeNode = null;
            foreach (TreeGridNode typeNode in topLevelNodeForHandle.Nodes)
            {
                if (typeNode.Tag != null && Convert.ToString(typeNode.Tag) == particularType)
                {
                    particularTypeNode = typeNode;
                    break;
                }
            }
            if (particularTypeNode == null)
            {
                //Didn't find match, so need to create new node
                particularTypeNode = topLevelNodeForHandle.Nodes.Add();
                particularTypeNode.Tag = particularType;
                particularTypeNode.Cells[0].Value = new String(' ', 28);
                particularTypeNode.Cells[1].Value = particularType;
                particularTypeNode.Cells[1].Style.Font = new Font(Font.SystemFontName, 8, FontStyle.Bold);
                particularTypeNode.Visible = false;
            }
            return particularTypeNode;
        }

        public bool InformationalMessagesVisible 
        {
            get
            {
                return _informationalMessagesVisible;
            }
            set
            {
                _informationalMessagesVisible = value;
                ShowHideNodes();
            }
        }

        public bool WarningsVisible
        {
            get
            {
                return _warningsVisible;
            }
            set
            {
                _warningsVisible = value;
                ShowHideNodes();
            }
        }

        /// <summary>
        /// Checks values of _informationalMessagesVisible and _warningsVisible to see whether buttons are pressed or not, and whether to display warnings/informational messages
        /// </summary>
        private void ShowHideNodes()
        {
            foreach (TreeGridNode topLevelNode in this.Nodes)
            {
                bool foundVisibleMessage = false;
                foreach (TreeGridNode typeNode in topLevelNode.Nodes)
                {
                    foreach (TreeGridNode messageNode in typeNode.Nodes)
                    {
                        if ((((ValidationMessageStatus)messageNode.Tag) == ValidationMessageStatus.Informational && _informationalMessagesVisible) ||
                            (((ValidationMessageStatus)messageNode.Tag) == ValidationMessageStatus.Warning && _warningsVisible)
                           )
                        {
                            messageNode.Visible = true;
                            foundVisibleMessage = true;
                        }
                        else
                        {
                            messageNode.Visible = false;
                        }
                    }
                    typeNode.Visible = foundVisibleMessage;
                }
                topLevelNode.Visible = foundVisibleMessage;
            }
        }

        internal int MessageCountByStatus(ValidationMessageStatus validationMessageStatus)
        {
            int returnCount = 0;
            foreach (TreeGridNode topLevelNode in this.Nodes)
            {
                foreach (TreeGridNode typeNode in topLevelNode.Nodes)
                {
                    foreach (TreeGridNode messageNode in typeNode.Nodes)
                    {
                        if (((ValidationMessageStatus)messageNode.Tag) == validationMessageStatus)
                        {
                            returnCount += 1;
                        }
                    }
                }
            }
            return returnCount;
        }

    }
}
