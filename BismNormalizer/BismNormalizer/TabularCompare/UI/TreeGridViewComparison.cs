using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using BismNormalizer.TabularCompare.Core;


//Image Key
//---------
//5: Delete
//6: Update
//7: Create
//8: Skip
//18: Delete Gray
//19: Skip Gray
//20: Create Gray


namespace BismNormalizer.TabularCompare.UI
{
    /// <summary>
    /// TreeGridView that supports binding to a CubeDiff object.
    /// </summary>
    [System.ComponentModel.DesignerCategory("code"),
        //Designer(typeof(System.Windows.Forms.Design.ControlDesigner)),
    ComplexBindingProperties(),
    Docking(DockingBehavior.Ask)]
    public class TreeGridViewComparison : TreeGridView
    {
        public TreeGridViewComparison()
            : base()
        {
            this.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.CellEndEditHandler);
            this.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.RowEnterHandler);
        }

        private Comparison _comparison;

        public Comparison Comparison
        {
            get { return _comparison; }
            set { _comparison = value; }
        }

        #region Callbacks

        public delegate void CellEditDelegate();
        private CellEditDelegate _cellEditCallBack;
        public void SetCellEditCallBack(CellEditDelegate cellEditCallBack)
        {
            _cellEditCallBack = cellEditCallBack;
        }

        public delegate void ObjectDefinitionsDelegate(string objDefSource, string objDefTarget, ComparisonObjectType objType, ComparisonObjectStatus objStatus);
        private ObjectDefinitionsDelegate _objectDefinitionsCallBack;
        public void SetObjectDefinitionsCallBack(ObjectDefinitionsDelegate objectDefinitionsCallBack)
        {
            _objectDefinitionsCallBack = objectDefinitionsCallBack;
        }

        private void RowEnterHandler(object sender, DataGridViewCellEventArgs e)
        {
            InvokeObjectDefinitionsCallBack();
        }
        private void InvokeObjectDefinitionsCallBack()
        {
            // Populate object definition textboxes
            if (this.Rows.GetRowCount(DataGridViewElementStates.Selected) > 0 && this.SelectedRows[0].Cells[10].Value != null)
            {
                ComparisonObjectType selectedObjType = FindComparisonObjectType(this.SelectedRows[0].Cells[0].Value.ToString());
                ComparisonObjectStatus selectedStatus = FindComparisonObjectStatus(this.SelectedRows[0].Cells[4].Value.ToString());
                _objectDefinitionsCallBack.Invoke(this.SelectedRows[0].Cells[10].Value.ToString(), this.SelectedRows[0].Cells[11].Value.ToString(), selectedObjType, selectedStatus);
            }
        }
        private ComparisonObjectType FindComparisonObjectType(string lookupType)
        {
            ComparisonObjectType returnObjType;
            switch (lookupType.TrimStart()) //trim start to remove indent
            {
                case "Data Source":
                    returnObjType = ComparisonObjectType.DataSource;
                    break;
                case "Table":
                    returnObjType = ComparisonObjectType.Table;
                    break;
                case "Relationship":
                    returnObjType = ComparisonObjectType.Relationship;
                    break;
                case "Measure":
                    returnObjType = ComparisonObjectType.Measure;
                    break;
                case "KPI":
                    returnObjType = ComparisonObjectType.Kpi;
                    break;
                case "Expression":
                    returnObjType = ComparisonObjectType.Expression;
                    break;
                case "Perspective":
                    returnObjType = ComparisonObjectType.Perspective;
                    break;
                case "Culture":
                    returnObjType = ComparisonObjectType.Culture;
                    break;
                case "Role":
                    returnObjType = ComparisonObjectType.Role;
                    break;
                case "Action":
                    returnObjType = ComparisonObjectType.Action;
                    break;
                default:
                    returnObjType = ComparisonObjectType.DataSource;
                    break;
            }
            return returnObjType;
        }
        private ComparisonObjectStatus FindComparisonObjectStatus(string status)
        {
            ComparisonObjectStatus returnStatus = ComparisonObjectStatus.Na;
            switch (status)
            {
                case "Different Definitions":
                    returnStatus = ComparisonObjectStatus.DifferentDefinitions;
                    break;
                case "Missing in Source":
                    returnStatus = ComparisonObjectStatus.MissingInSource;
                    break;
                case "Missing in Target":
                    returnStatus = ComparisonObjectStatus.MissingInTarget;
                    break;
                case "NA":
                    returnStatus = ComparisonObjectStatus.Na;
                    break;
                case "Same Definition":
                    returnStatus = ComparisonObjectStatus.SameDefinition;
                    break;
            }
            return returnStatus;
        }

        #endregion

        /// <summary>
        /// Binds to a CubeDiff object
        /// </summary>
        public void DataBindComparison()
        {
            // TreeGridView doesn't support binding to a DataBindingSource

            this.Nodes.Clear();

            if (_comparison != null)
            {
                foreach (ComparisonObject comparisonObject in _comparison.ComparisonObjects)
                {
                    TreeGridNode node = this.Nodes.Add();
                    PopulateNode(node, comparisonObject);
                    //node.Expand();
                }
            }

            //expand all nodes (would prefer to do it when iterating collection whilst populating, but it complains
            foreach (TreeGridNode node in this.Nodes)
            {
                node.Expand();
                foreach (TreeGridNode childNode in node.Nodes)
                {
                    childNode.Expand();
                }
            }

            // show the object properties for the first row
            if (this.Rows.Count > 0)
            {
                InvokeObjectDefinitionsCallBack();

                if (this.Columns.Contains("TypeLabel")) this.AutoResizeColumn(this.Columns["TypeLabel"].Index, DataGridViewAutoSizeColumnMode.AllCells);
                if (this.Columns.Contains("Status")) this.AutoResizeColumn(this.Columns["Status"].Index, DataGridViewAutoSizeColumnMode.AllCells);
                if (this.Columns.Contains("MergeAction")) this.AutoResizeColumn(this.Columns["MergeAction"].Index, DataGridViewAutoSizeColumnMode.AllCells);
            }
        }

        private void PopulateNode(TreeGridNode node, ComparisonObject comparisonObject)
        {
            switch (comparisonObject.Status)
            {
                case ComparisonObjectStatus.MissingInTarget:
                    node.Cells[3].Value = ">";
                    node.Cells[4].Value = "Missing in Target";
                    node.Cells[5].Style.BackColor = Color.LightGray;
                    node.Cells[6].Style.BackColor = Color.LightGray;
                    break;
                case ComparisonObjectStatus.MissingInSource:
                    node.Cells[1].Style.BackColor = Color.LightGray;
                    node.Cells[2].Style.BackColor = Color.LightGray;
                    node.Cells[3].Value = "<";
                    node.Cells[4].Value = "Missing in Source";
                    break;
                case ComparisonObjectStatus.SameDefinition:
                    node.Cells[3].Value = "=";
                    node.Cells[4].Value = "Same Definition";
                    break;
                case ComparisonObjectStatus.DifferentDefinitions:
                    node.Cells[3].Value = "≠";
                    node.Cells[4].Value = "Different Definitions";
                    break;
                default:
                    break;
            }
            node.Cells[1].Value = comparisonObject.SourceObjectName;
            node.Cells[2].Value = comparisonObject.SourceObjectInternalName;
            //node.Cells[8].Value = comparisonObject.MergeAction.ToString();  //set below instead
            node.Cells[5].Value = comparisonObject.TargetObjectName;
            node.Cells[6].Value = comparisonObject.TargetObjectInternalName;
            node.Cells[9].Value = comparisonObject.ComparisonObjectType.ToString();
            node.Cells[10].Value = comparisonObject.SourceObjectDefinition;
            node.Cells[11].Value = comparisonObject.TargetObjectDefinition;

            string treeIndentLevel1 = new String(' ', 13);
            string treeIndentLevel2 = new String(' ', 20);

            switch (comparisonObject.ComparisonObjectType)
            {
                // Tabular objecs
                case ComparisonObjectType.Model:
                    node.ImageIndex = 25;
                    node.Cells[0].Value = treeIndentLevel1 + "Model";
                    break;
                case ComparisonObjectType.DataSource:
                    node.ImageIndex = 0;
                    node.Cells[0].Value = treeIndentLevel1 + "Data Source";
                    break;
                case ComparisonObjectType.Table:
                    node.ImageIndex = 1;
                    node.Cells[0].Value = treeIndentLevel1 + "Table";
                    break;
                case ComparisonObjectType.Relationship:
                    node.ImageIndex = 2;
                    node.Cells[0].Value = treeIndentLevel2 + "Relationship";
                    break;
                case ComparisonObjectType.Measure:
                    node.ImageIndex = 3;
                    node.Cells[0].Value = treeIndentLevel2 + "Measure";
                    break;
                case ComparisonObjectType.Kpi:
                    node.ImageIndex = 4;
                    node.Cells[0].Value = treeIndentLevel2 + "KPI";
                    break;
                case ComparisonObjectType.CalculationItem:
                    node.ImageIndex = 24;
                    node.Cells[0].Value = treeIndentLevel2 + "Calculation Item";
                    break;
                case ComparisonObjectType.Expression:
                    node.ImageIndex = 22;
                    node.Cells[0].Value = treeIndentLevel1 + "Expression";
                    break;
                case ComparisonObjectType.Perspective:
                    node.ImageIndex = 15;
                    node.Cells[0].Value = treeIndentLevel1 + "Perspective";
                    break;
                case ComparisonObjectType.Culture:
                    node.ImageIndex = 21;
                    node.Cells[0].Value = treeIndentLevel1 + "Culture";
                    break;
                case ComparisonObjectType.Role:
                    node.ImageIndex = 14;
                    node.Cells[0].Value = treeIndentLevel1 + "Role";
                    break;
                case ComparisonObjectType.Action:
                    node.ImageIndex = 16;
                    node.Cells[0].Value = treeIndentLevel1 + "Action";
                    break;
                //case ComparisonObjectType.RefreshPolicy:
                //    node.ImageIndex = 26;
                //    node.Cells[0].Value = treeIndentLevel1 + "Refresh Policy";
                //    break;

                default:
                    break;
            };

            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)node.Cells[8];
            DataGridViewCell parentMergeActionCell = node.Parent.Cells[8];
            DataGridViewCell parentStatusCell = node.Parent.Cells[4];

            // set drop-down to have limited members based on what is available
            switch (comparisonObject.MergeAction)
            {
                case MergeAction.Create:
                    node.Cells[7].Value = this.ImageList.Images[7];     //7: Create
                    node.Cells[8].Value = comparisonObject.MergeAction.ToString();
                    comboCell.DataSource = new string[] { "Create", "Skip" };

                    if (parentStatusCell.Value != null && parentMergeActionCell.Value != null &&
                        parentStatusCell.Value.ToString() == "Missing in Target" && parentMergeActionCell.Value.ToString() == "Skip")
                    {
                        // Can only happen if loading from file
                        node.Cells[7].Value = this.ImageList.Images[19];     //19: Skip Gray
                        node.Cells[8].Value = MergeAction.Skip.ToString();
                        node.Cells[8].Style.ForeColor = Color.DimGray;
                        node.Cells[8].ReadOnly = true;
                        SetNodeTooltip(node, true);
                    }
                    break;
                case MergeAction.Update:
                    node.Cells[7].Value = this.ImageList.Images[6];     //6: Update
                    node.Cells[8].Value = comparisonObject.MergeAction.ToString();
                    comboCell.DataSource = new string[] { "Update", "Skip" };
                    break;
                case MergeAction.Delete:
                    node.Cells[7].Value = this.ImageList.Images[5];     //5: Delete
                    node.Cells[8].Value = comparisonObject.MergeAction.ToString();
                    comboCell.DataSource = new string[] { "Delete", "Skip" };

                    //check if parent is also set to delete, in which case make this cell readonly
                    if (parentMergeActionCell.Value != null && parentMergeActionCell.Value.ToString() == "Delete")
                    {
                        node.Cells[7].Value = this.ImageList.Images[18];     //18: Delete Gray
                        node.Cells[8].Style.ForeColor = Color.DimGray;
                        node.Cells[8].ReadOnly = true;
                        SetNodeTooltip(node, true);
                    }
                    break;
                case MergeAction.Skip:
                    node.Cells[7].Value = this.ImageList.Images[8];     //8: Skip
                    node.Cells[8].Value = comparisonObject.MergeAction.ToString();
                    switch (comparisonObject.Status)
                    {
                        case ComparisonObjectStatus.MissingInTarget:
                            comboCell.DataSource = new string[] { "Create", "Skip" };

                            //check if parent is also MissingInTarget and Skip, make this cell readonly
                            if (parentStatusCell.Value != null && parentMergeActionCell.Value != null &&
                                parentStatusCell.Value.ToString() == "Missing in Target" && parentMergeActionCell.Value.ToString() == "Skip")
                            {
                                node.Cells[7].Value = this.ImageList.Images[19];     //19: Skip Gray
                                node.Cells[8].Style.ForeColor = Color.DimGray;
                                node.Cells[8].ReadOnly = true;
                                SetNodeTooltip(node, true);
                            }

                            break;
                        case ComparisonObjectStatus.MissingInSource:
                            comboCell.DataSource = new string[] { "Delete", "Skip" };
                            break;
                        case ComparisonObjectStatus.DifferentDefinitions:
                            comboCell.DataSource = new string[] { "Update", "Skip" };
                            break;
                        default:
                            //default covers ComparisonObjectStatus.SameDefinition: which is most common case (above cases are for saved skip selections from file)
                            node.Cells[7].Value = this.ImageList.Images[19];     //19: Skip Gray
                            comboCell.DataSource = new string[] { "Skip" };
                            node.Cells[8].Style.ForeColor = Color.DimGray;
                            node.Cells[8].ReadOnly = true;
                            break;
                    }

                    break;
                default:
                    break;
            };

            if (comparisonObject.ChildComparisonObjects != null && comparisonObject.ChildComparisonObjects.Count > 0)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    TreeGridNode childNode = node.Nodes.Add();
                    PopulateNode(childNode, childComparisonObject);
                    //node.Expand();  //for inexplicable reason, keeps erroring on this line as though the node doesn't belong to the grid, but it does because its parent does.  So instead will iterate all nodes once fully populate to expand.
                }
            }
        }

        /// <summary>
        /// Setup columns ready for binding to a CubeDiff object
        /// </summary>
        public void SetupForComparison()
        {
            //Set up columns

            //0 (tree grid view seems to no always support accessing by name, so using IDs)
            TreeGridColumn typeLabelColumn = new TreeGridColumn();
            typeLabelColumn.Name = "TypeLabel";
            typeLabelColumn.HeaderText = "Type";
            typeLabelColumn.ReadOnly = true;
            typeLabelColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            typeLabelColumn.Width = this.Width / 601 * 161; //hdpi
            this.Columns.Add(typeLabelColumn);

            //1
            DataGridViewTextBoxColumn sourceNameColumn = new DataGridViewTextBoxColumn();
            sourceNameColumn.Name = "SourceName";
            sourceNameColumn.HeaderText = "Source Name";
            sourceNameColumn.ReadOnly = true;
            sourceNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            sourceNameColumn.Width = this.Width / 601 * 301; //hdpi..
            this.Columns.Add(sourceNameColumn);

            //2
            DataGridViewTextBoxColumn sourceIdColumn = new DataGridViewTextBoxColumn();
            sourceIdColumn.Name = "SourceID";
            sourceIdColumn.HeaderText = "Source ID";
            sourceIdColumn.ReadOnly = true;
            sourceIdColumn.Visible = false;
            sourceIdColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            sourceIdColumn.Width = this.Width / 601 * 200;
            this.Columns.Add(sourceIdColumn);

            //3
            DataGridViewTextBoxColumn equalityColumn = new DataGridViewTextBoxColumn();
            equalityColumn.Name = "Equality";
            equalityColumn.HeaderText = "";
            equalityColumn.ReadOnly = true;
            equalityColumn.Visible = false;
            equalityColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            equalityColumn.Width = this.Width / 601 * 17;
            //equalityColumn.Resizable = DataGridViewTriState.False;
            //equalityColumn.DefaultCellStyle.Font = new Font("Times New Roman", 11);
            this.Columns.Add(equalityColumn);

            //4
            DataGridViewTextBoxColumn statusColumn = new DataGridViewTextBoxColumn();
            statusColumn.Name = "Status";
            statusColumn.HeaderText = "Status";
            statusColumn.ReadOnly = true;
            statusColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            statusColumn.Width = this.Width / 601 * 103;
            this.Columns.Add(statusColumn);

            //5
            DataGridViewTextBoxColumn targetNameColumn = new DataGridViewTextBoxColumn();
            targetNameColumn.Name = "TargetName";
            targetNameColumn.HeaderText = "Target Name";
            targetNameColumn.ReadOnly = true;
            targetNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            targetNameColumn.Width = this.Width / 601 * 301;
            this.Columns.Add(targetNameColumn);

            //6
            DataGridViewTextBoxColumn targetIdColumn = new DataGridViewTextBoxColumn();
            targetIdColumn.Name = "TargetID";
            targetIdColumn.HeaderText = "Target ID";
            targetIdColumn.ReadOnly = true;
            targetIdColumn.Visible = false;
            targetIdColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            targetIdColumn.Width = this.Width / 601 * 200;
            this.Columns.Add(targetIdColumn);

            //7
            DataGridViewImageColumn actionImageColumn = new DataGridViewImageColumn();
            actionImageColumn.Name = "";
            actionImageColumn.HeaderText = "";
            actionImageColumn.ReadOnly = true;
            actionImageColumn.Width = this.Width / 601 * 18;
            actionImageColumn.Resizable = DataGridViewTriState.False;
            actionImageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            actionImageColumn.Visible = true;
            this.Columns.Add(actionImageColumn);

            //8
            DataGridViewComboBoxColumn mergeActionColumn = new DataGridViewComboBoxColumn();
            mergeActionColumn.Name = "MergeAction";
            mergeActionColumn.HeaderText = "Action";
            mergeActionColumn.MaxDropDownItems = 4;
            mergeActionColumn.FlatStyle = FlatStyle.Flat;
            mergeActionColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            mergeActionColumn.Width = this.Width / 601 * 83;
            this.Columns.Add(mergeActionColumn);

            //9
            DataGridViewTextBoxColumn typeColumn = new DataGridViewTextBoxColumn();
            typeColumn.Name = "Type";
            typeColumn.HeaderText = "Type";
            typeColumn.ReadOnly = true;
            typeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            typeColumn.Visible = false;
            this.Columns.Add(typeColumn);

            //10
            DataGridViewTextBoxColumn sourceObjDefColumn = new DataGridViewTextBoxColumn();
            sourceObjDefColumn.Name = "SourceObjDef";
            sourceObjDefColumn.HeaderText = "Source Object Definition";
            sourceObjDefColumn.ReadOnly = true;
            sourceObjDefColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            sourceObjDefColumn.Visible = false;
            this.Columns.Add(sourceObjDefColumn);

            //11
            DataGridViewTextBoxColumn targetObjDefColumn = new DataGridViewTextBoxColumn();
            targetObjDefColumn.Name = "TargetObjDef";
            targetObjDefColumn.HeaderText = "Target Object Definition";
            targetObjDefColumn.ReadOnly = true;
            targetObjDefColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            targetObjDefColumn.Visible = false;
            this.Columns.Add(targetObjDefColumn);

            this.DefaultCellStyle.BackColor = Color.White;
        }

        /// <summary>
        /// Refresh the comparison after user changes to actions in grid
        /// </summary>
        public void RefreshDiffResultsFromGrid()
        {
            //this version of the TreeGridView does not support binding from an object collection (or any built in data binding for that matter), so we have to refresh the CubeDiff with changes done by the user in the UI (they can modify the MergeActions).
            foreach (TreeGridNode node in this.Nodes)
            {
                RefreshComparisonObjFromGrid(node);
            }
        }
        private void RefreshComparisonObjFromGrid(TreeGridNode node)
        {
            ComparisonObjectType nodeCubeOjbectType = (ComparisonObjectType)Enum.Parse(typeof(ComparisonObjectType), node.Cells[9].Value.ToString());

            ComparisonObject comparisonObject = _comparison.FindComparisonObjectByObjectInternalNames(
                node.Cells[1].Value.ToString(),
                node.Cells[2].Value.ToString(),
                node.Cells[5].Value.ToString(),
                node.Cells[6].Value.ToString(),
                nodeCubeOjbectType);

            if (comparisonObject != null)
            {
                comparisonObject.MergeAction = (MergeAction)Enum.Parse(typeof(MergeAction), node.Cells[8].Value.ToString());
            }

            //this version of the TreeGridView does not support binding from an object collection (or any built in data binding for that matter), so we have to refresh the CubeDiff with changes done by the user in the UI (they can modify the MergeActions).
            foreach (TreeGridNode childNode in node.Nodes)
            {
                RefreshComparisonObjFromGrid(childNode);
            }
        }

        /// <summary>
        /// Sets Action property of objects to Skip within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        /// <param name="comparisonStatus"></param>
        public void SkipItems(bool selectedOnly, ComparisonObjectStatus comparisonObjectStatus = ComparisonObjectStatus.Na) //Na because won't take null cos it's an enum
        {
            Int32 selectedRowCount = (selectedOnly ? this.Rows.GetRowCount(DataGridViewElementStates.Selected) : this.Rows.Count);
            if (selectedRowCount > 0)
            {
                // fudge to ensure child objects Missing in Source are skipped (this is because we have to iterate DataGridViewRow object which isn't hierarchical)
                SkipItemsPrivate(selectedOnly, comparisonObjectStatus, selectedRowCount);
                SkipItemsPrivate(selectedOnly, comparisonObjectStatus, selectedRowCount);
                SkipItemsPrivate(selectedOnly, comparisonObjectStatus, selectedRowCount);

                _cellEditCallBack.Invoke();
            }
        }
        private void SkipItemsPrivate(bool selectedOnly, ComparisonObjectStatus comparisonObjectStatus, Int32 selectedRowCount)
        {
            for (int i = 0; i < selectedRowCount; i++)
            {
                DataGridViewRow row = (selectedOnly ? this.SelectedRows[i] : this.Rows[i]);

                SkipItemPrivate(comparisonObjectStatus, row);
            }
        }

        private void SkipItemPrivate(ComparisonObjectStatus comparisonObjectStatus, DataGridViewRow row)
        {
            if (comparisonObjectStatus == ComparisonObjectStatus.Na ||
                (comparisonObjectStatus == ComparisonObjectStatus.DifferentDefinitions && row.Cells[4].Value.ToString() == "Different Definitions") ||
                (comparisonObjectStatus == ComparisonObjectStatus.MissingInSource && row.Cells[4].Value.ToString() == "Missing in Source") ||
                (comparisonObjectStatus == ComparisonObjectStatus.MissingInTarget && row.Cells[4].Value.ToString() == "Missing in Target"))
            {
                if (!row.Cells[8].ReadOnly &&
                    row.Cells[8].Value.ToString() != "Skip " &&
                    row.Cells[8].Value.ToString() != "Set Parent Node")
                {
                    row.Cells[8].Value = "Skip";
                    row.Cells[7].Value = this.ImageList.Images[8];  //8: Skip
                    CheckToSkipChildren(row);
                }
            }
        }

        private void CheckToSkipChildren(DataGridViewRow selectedRow)
        {
            // if Missing in Target (default is create) and user selects skip, definitely can't create child objects, so set them to skip too and disable them
            if (selectedRow.Cells[4].Value.ToString() == "Missing in Target")
            {
                TreeGridNode selectedNode = FindNodeByIDs(selectedRow.Cells[0].Value.ToString(), selectedRow.Cells[2].Value.ToString(), selectedRow.Cells[6].Value.ToString());

                if (selectedNode != null)
                {
                    foreach (TreeGridNode node in selectedNode.Nodes)
                    {
                        SetNodeToSkip(node);
                    }
                }
            }
            // if Missing in Source (default is delete) and user selects skip, he may still want to delete some child objects, so ensure they are enabled
            else if (selectedRow.Cells[4].Value.ToString() == "Missing in Source")
            {
                TreeGridNode selectedNode = FindNodeByIDs(selectedRow.Cells[0].Value.ToString(), selectedRow.Cells[2].Value.ToString(), selectedRow.Cells[6].Value.ToString());

                if (selectedNode != null)
                {
                    foreach (TreeGridNode node in selectedNode.Nodes)
                    {
                        //EnableSkipNode(node);
                        if (CellContainsMember(node.Cells[8], "Skip"))
                        {
                            switch (node.Cells[8].Value.ToString())
                            {
                                case "Delete":
                                    node.Cells[7].Value = this.ImageList.Images[5];     //5: Delete
                                    break;
                                case "Update":
                                    node.Cells[7].Value = this.ImageList.Images[6];     //6: Update
                                    break;
                                case "Create":
                                    node.Cells[7].Value = this.ImageList.Images[7];     //7: Create
                                    break;
                                default:
                                    node.Cells[7].Value = this.ImageList.Images[8];     //8: Skip
                                    break;
                            }
                            node.Cells[8].ReadOnly = false;
                            node.Cells[8].Style.ForeColor = Color.Black;
                            SetNodeTooltip(node, false);
                        }
                    }
                }
            }
        }
        private void SetNodeToSkip(TreeGridNode node)
        {
            if (CellContainsMember(node.Cells[8], "Skip"))
            {
                node.Cells[7].Value = this.ImageList.Images[19];     //19: Skip Gray
                node.Cells[8].Value = "Skip";
                node.Cells[8].ReadOnly = true;
                node.Cells[8].Style.ForeColor = Color.DimGray;
                SetNodeTooltip(node, true);
            }
            foreach (TreeGridNode childNode in node.Nodes)
            {
                SetNodeToSkip(childNode);
            }
        }
        private void SetNodeTooltip(TreeGridNode node, bool disabledDueToParent)
        {
            foreach (DataGridViewCell cell in node.Cells)
            {
                cell.ToolTipText = (disabledDueToParent ? "This object's action option is disabled due to a parent object selection" : "");
            }
        }

        /// <summary>
        /// Sets Action property of objects to Create within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        public void CreateItems(bool selectedOnly)
        {
            Int32 selectedRowCount = (selectedOnly ? this.Rows.GetRowCount(DataGridViewElementStates.Selected) : this.Rows.Count);
            if (selectedRowCount > 0)
            {
                // fudge to ensure child objects are enabled before setting to Create (this is because we have to iterate DataGridViewRow object which isn't hierarchical)
                CreateItemsInternal(selectedOnly, selectedRowCount);
                CreateItemsInternal(selectedOnly, selectedRowCount);
                CreateItemsInternal(selectedOnly, selectedRowCount);

                _cellEditCallBack.Invoke();
            }
        }
        private void CreateItemsInternal(bool selectedOnly, Int32 selectedRowCount)
        {
            for (int i = 0; i < selectedRowCount; i++)
            {
                DataGridViewRow row = (selectedOnly ? this.SelectedRows[i] : this.Rows[i]);

                if (!row.Cells[8].ReadOnly &&
                    row.Cells[8].Value.ToString() != "Skip " &&
                    CellContainsMember(row.Cells[8], "Create"))
                {
                    row.Cells[7].Value = this.ImageList.Images[7];     //7: Create
                    row.Cells[8].Value = "Create";
                    CheckToCreateChildren(row);
                }
            }
        }
        private void CheckToCreateChildren(DataGridViewRow selectedRow)
        {
            // if Missing in Target (default is create) and user selects create, he may still want to skip some child objects, so ensure they are enabled
            if (selectedRow.Cells[4].Value.ToString() == "Missing in Target")
            {
                TreeGridNode selectedNode = FindNodeByIDs(selectedRow.Cells[0].Value.ToString(), selectedRow.Cells[2].Value.ToString(), selectedRow.Cells[6].Value.ToString());

                if (selectedNode != null)
                {
                    foreach (TreeGridNode node in selectedNode.Nodes)
                    {
                        if (CellContainsMember(node.Cells[8], "Create"))
                        {
                            node.Cells[8].ReadOnly = false;
                            node.Cells[8].Style.ForeColor = Color.Black;
                            SetNodeTooltip(node, false);

                            if (node.Cells[8].Value.ToString() == "Skip")
                            {
                                node.Cells[7].Value = this.ImageList.Images[8];     //8: Skip
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets Action property of objects to Delete within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        public void DeleteItems(bool selectedOnly)
        {
            Int32 selectedRowCount = (selectedOnly ? this.Rows.GetRowCount(DataGridViewElementStates.Selected) : this.Rows.Count);
            if (selectedRowCount > 0)
            {
                for (int i = 0; i < selectedRowCount; i++)
                {
                    DataGridViewRow row = (selectedOnly ? this.SelectedRows[i] : this.Rows[i]);

                    if (!row.Cells[8].ReadOnly &&
                        row.Cells[8].Value.ToString() != "Skip " &&
                        CellContainsMember(row.Cells[8], "Delete"))
                    {
                        row.Cells[7].Value = this.ImageList.Images[5];     //5: Delete
                        row.Cells[8].Value = "Delete";
                        CheckToDeleteChildren(row);
                    }
                }

                _cellEditCallBack.Invoke();
            }
        }
        private void CheckToDeleteChildren(DataGridViewRow selectedRow)
        {
            // if Missing in Source (default is delete) and user selects delete, definitely can't skip child objects, so set them to delete too and disable them
            if (selectedRow.Cells[4].Value.ToString() == "Missing in Source")
            {
                TreeGridNode selectedNode = FindNodeByIDs(selectedRow.Cells[0].Value.ToString(), selectedRow.Cells[2].Value.ToString(), selectedRow.Cells[6].Value.ToString());

                if (selectedNode != null)
                {
                    foreach (TreeGridNode node in selectedNode.Nodes)
                    {
                        SetNodeToDelete(node);
                    }
                }
            }
        }
        private void SetNodeToDelete(TreeGridNode node)
        {
            if (CellContainsMember(node.Cells[8], "Delete"))
            {
                node.Cells[7].Value = this.ImageList.Images[18];     //18: Delete Gray
                node.Cells[8].Value = "Delete";
                node.Cells[8].ReadOnly = true;
                node.Cells[8].Style.ForeColor = Color.DimGray;
                SetNodeTooltip(node, true);
            }
            foreach (TreeGridNode childNode in node.Nodes)
            {
                SetNodeToDelete(childNode);
            }
        }

        /// <summary>
        /// Sets Action property of objects to Update within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        public void UpdateItems(bool selectedOnly)
        {
            // Not necessary to run twice with internal method because Updates don't impact children

            Int32 selectedRowCount = (selectedOnly ? this.Rows.GetRowCount(DataGridViewElementStates.Selected) : this.Rows.Count);
            if (selectedRowCount > 0)
            {
                for (int i = 0; i < selectedRowCount; i++)
                {
                    DataGridViewRow row = (selectedOnly ? this.SelectedRows[i] : this.Rows[i]);

                    if (!row.Cells[8].ReadOnly &&
                        row.Cells[8].Value.ToString() != "Skip " &&
                        CellContainsMember(row.Cells[8], "Update"))
                    {
                        row.Cells[7].Value = this.ImageList.Images[6];     //6: Update
                        row.Cells[8].Value = "Update";
                    }
                }

                _cellEditCallBack.Invoke();
            }
        }

        public void ShowHideNodes(bool hide, bool sameDefinitionFilter = false)
        {
            ShowHideNodes(this.Nodes, hide, sameDefinitionFilter);
        }
        private void ShowHideNodes(TreeGridNodeCollection nodes, bool hide, bool sameDefinitionFilter)
        {
            foreach (TreeGridNode node in nodes)
            {
                if (node.Cells[8].Value.ToString() == "Skip" && (!sameDefinitionFilter || (sameDefinitionFilter && hide && node.Cells[4].Value.ToString() == "Same Definition")))
                {
                    // if currently selected skip item contains Update, Delete or Create children, then need to keep visible - or result in orphans
                    bool foundCreateOrDeleteChild = false;
                    foreach (TreeGridNode childNode in node.Nodes)
                    {
                        if (childNode.Cells[8].Value.ToString() == "Update" || childNode.Cells[8].Value.ToString() == "Delete" || childNode.Cells[8].Value.ToString() == "Create")
                        {
                            foundCreateOrDeleteChild = true;
                            break;
                        }
                    }

                    if (hide)
                    {
                        if (!foundCreateOrDeleteChild)
                        {
                            node.Visible = false;
                        }
                    }
                    else
                    {
                        node.Visible = true;
                    }
                }
                else
                {
                    node.Visible = (
                        !(node.Cells[8].Value.ToString() == "Skip " &&
                         (node.Nodes.Count == 0 || !NodeContainsEditableChildren(node, hide))));
                }
                ShowHideNodes(node.Nodes, hide, sameDefinitionFilter);
            }
        }

        private TreeGridNode FindNodeByIDs(string comparisonObjectType, string sourceID, string targetID)
        {
            foreach (TreeGridNode node in this.Nodes)
            {
                if (node.Cells[0].Value.ToString() == comparisonObjectType && node.Cells[2].Value.ToString() == sourceID && node.Cells[6].Value.ToString() == targetID)
                {
                    return node;
                }
                foreach (TreeGridNode childNode in node.Nodes)
                {
                    TreeGridNode returnNode = FindNodeByIDsFromChildNodes(comparisonObjectType, sourceID, targetID, childNode);
                    if (returnNode != null)
                    {
                        return returnNode;
                    }
                }
            }
            return null;
        }
        private static TreeGridNode FindNodeByIDsFromChildNodes(string comparisonObjectType, string sourceID, string targetID, TreeGridNode childNode)
        {
            if (childNode.Cells[0].Value.ToString() == comparisonObjectType && childNode.Cells[2].Value.ToString() == sourceID && childNode.Cells[6].Value.ToString() == targetID)
            {
                return childNode;
            }
            foreach (TreeGridNode grandChildNode in childNode.Nodes)
            {
                TreeGridNode returnNode = FindNodeByIDsFromChildNodes(comparisonObjectType, sourceID, targetID, grandChildNode);
                if (returnNode != null)
                {
                    return returnNode;
                }
            }
            return null;
        }

        private bool NodeContainsEditableChildren(TreeGridNode node, bool hide)
        {
            bool containsChildren = false;

            foreach (TreeGridNode childNode in node.Nodes)
            {
                if ((hide &&
                     childNode.Cells[8].Value.ToString() != "Skip " &&
                     childNode.Cells[8].Value.ToString() != "Skip") ||
                    (!hide &&
                     childNode.Cells[8].Value.ToString() != "Skip "))
                {
                    containsChildren = true;
                }
                else
                {
                    bool childContainsChildren = NodeContainsEditableChildren(childNode, hide);
                    if (!containsChildren)
                        containsChildren = childContainsChildren;
                }

                if (childNode.Cells[8].Value.ToString() == "Skip")
                    childNode.Visible = !hide;
            }

            if (node.Cells[8].Value.ToString() != "Skip")
                node.Visible = containsChildren;

            return containsChildren;
        }

        private bool CellContainsMember(DataGridViewCell cell, string lookupValue)
        {
            foreach (string member in (string[])((DataGridViewComboBoxCell)cell).DataSource)
            {
                if (lookupValue == member)
                {
                    return true;
                }
            }
            return false;
        }

        private void CellEndEditHandler(object sender, DataGridViewCellEventArgs e)
        {
            // if set parent to skip/create/delete, MAY need to set all children to skip/create/delete too (only the read only cells)
            if (this.Rows[e.RowIndex].Cells[8].Value.ToString() == "Skip" && this.ImageList.Images.Count > 0)
            {
                this.Rows[e.RowIndex].Cells[7].Value = this.ImageList.Images[8];     //8: Skip
                CheckToSkipChildren(this.Rows[e.RowIndex]);
            }
            if (this.Rows[e.RowIndex].Cells[8].Value.ToString() == "Create" && this.ImageList.Images.Count > 0)
            {
                this.Rows[e.RowIndex].Cells[7].Value = this.ImageList.Images[7];     //7: Create
                CheckToCreateChildren(this.Rows[e.RowIndex]);
            }
            if (this.Rows[e.RowIndex].Cells[8].Value.ToString() == "Delete" && this.ImageList.Images.Count > 0)
            {
                this.Rows[e.RowIndex].Cells[7].Value = this.ImageList.Images[5];     //5: Delete
                CheckToDeleteChildren(this.Rows[e.RowIndex]);
            }
            if (this.Rows[e.RowIndex].Cells[8].Value.ToString() == "Update" && this.ImageList.Images.Count > 0)
            {
                this.Rows[e.RowIndex].Cells[7].Value = this.ImageList.Images[6];     //6: Update
            }

            _cellEditCallBack.Invoke();
        }

    }
}
