namespace AlmToolkit
{
    using BismNormalizer.TabularCompare.Core;
    using BismNormalizer.TabularCompare;
    using Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System;

    public class ComparisonJSInteraction
    {
        #region Private members

        private Comparison _comparison;
        // The form class needs to be changed according to yours
        private static ComparisonForm _instanceMainForm = null;


        // Used to maintain a dictionary with direct access to the Angular node and C# comparison object
        private Dictionary<int, AngularComposite> _directAccessList = new Dictionary<int, AngularComposite>();

        #endregion

        #region Public properties
        // List to be used to populate data in grid control. This needs to be static, since everytime  CEF Sharp invokes the method, it creates a new instance
        // Need to revisit initialization to evaluate removal strategy
        public static List<ComparisonNode> comparisonList = new List<ComparisonNode>();
        public static List<ComparisonNode> selectedNodes = new List<ComparisonNode>();

        public ComparisonJSInteraction(ComparisonForm mainForm)
        {
            _instanceMainForm = mainForm;
        }
        public Comparison Comparison
        {
            get { return _comparison; }
            set { _comparison = value; }
        }
        #endregion

        #region Angular endpoints

        /// <summary>
        /// Method that sends flattened comparison object to Angular control
        /// </summary>
        /// <returns></returns>
        public string GetComparisonList()
        {

            string comparisonData = JsonConvert.SerializeObject(comparisonList);
            return comparisonData;

        }

        /// <summary>
        /// Save or Compare as per the action on UI
        /// </summary>
        /// <param name="action">Action to be performed</param>
        public void SaveOrCompare(string action)
        {
            switch (action.ToLower())
            {
                case "save":
                    _instanceMainForm.SaveNg();
                    break;
                case "compare":
                    _instanceMainForm.InitializeAndCompareTabularModelsNg();
                    break;
            }
        }

        /// <summary>
        /// Update the object as and when selected action is changed on UI
        /// </summary>
        /// <param name="id">Id of the node updated</param>
        /// <param name="newAction">New selected action</param>
        /// <param name="oldAction">Old selected action</param>
        public void ChangeOccurred(int id, string newAction, string oldAction)
        {
            if (_directAccessList.ContainsKey(id))
            {
                AngularComposite currentNode = _directAccessList[id];

                // if set parent to skip/create/delete, MAY need to set all children to skip/create/delete too (only the read only cells)

                switch (newAction)
                {
                    case "Skip":
                        currentNode.dotNetComparison.MergeAction = MergeAction.Skip;
                        currentNode.ngComparison.MergeAction = MergeAction.Skip.ToString();
                        CheckToSkipChildren(currentNode.ngComparison);
                        break;
                    case "Create":
                        currentNode.dotNetComparison.MergeAction = MergeAction.Create;
                        currentNode.ngComparison.MergeAction = MergeAction.Create.ToString();
                        CheckToCreateChildren(currentNode.ngComparison);
                        break;
                    case "Delete":
                        currentNode.dotNetComparison.MergeAction = MergeAction.Delete;
                        currentNode.ngComparison.MergeAction = MergeAction.Delete.ToString();
                        CheckToDeleteChildren(currentNode.ngComparison);
                        break;
                    case "Update":
                        currentNode.dotNetComparison.MergeAction = MergeAction.Update;
                        currentNode.ngComparison.MergeAction = MergeAction.Update.ToString();
                        break;
                    default:
                        break;
                }
                // Disable update menu on comparison change
                _instanceMainForm.HandleComparisonChanged();

                // Refresh the tree control, since grid is maintained here
                _instanceMainForm.refreshGridControl(true);
            }

        }

        /// <summary>
        /// Perform required action on selected nodes
        /// </summary>
        /// <param name="action">Action to be performed: Skip, Update, Create or Delete</param>
        /// <param name="selectedNodesUI">List of Node Ids which are selected on Angular control</param>
        public void PerformActionsOnSelectedActions(string action, List<object> selectedNodesUI)
        {
            selectedNodes.Clear();
            ComparisonNode nodeToAdd;
            for (int nodeCounter = 0; nodeCounter < selectedNodesUI.Count; nodeCounter++)
            {
                if (_directAccessList.ContainsKey(Convert.ToInt32(selectedNodesUI[nodeCounter])))
                {
                    AngularComposite currentNode = _directAccessList[Convert.ToInt32(selectedNodesUI[nodeCounter])];
                    nodeToAdd = currentNode.ngComparison;
                    selectedNodes.Add(nodeToAdd);
                }
            }

            switch (action)
            {
                case "skip":
                    SkipItems(true);
                    break;
                case "create":
                    CreateItems(true);
                    break;
                case "delete":
                    DeleteItems(true);
                    break;
                case "update":
                    UpdateItems(true);
                    break;
            }

            // Disable update menu on comparison change
            _instanceMainForm.HandleComparisonChanged();

            // Refresh the tree control, since grid is maintained here
            _instanceMainForm.refreshGridControl(true);
        }

        #endregion

        #region Data transformation and population

        /// <summary>
        /// Transform comparison object to structure understood by Angular control
        /// </summary>
        public void SetComparisonData()
        {
            if (this._comparison != null)
            {
                comparisonList.Clear();
                _directAccessList.Clear();

                foreach (ComparisonObject comparisonObject in this._comparison.ComparisonObjects)
                {
                    this.PopulateComparisonData(comparisonObject, 0, null);
                }
            }
        }

        /// <summary>
        /// Helper method to transform comparison object to structure understood by Angular control
        /// </summary>
        /// <param name="comparisonObject">Individual node in the tree</param>
        /// <param name="level">Level in the heirarchy to which the object belongs</param>
        /// <param name="parentNode">Reference to the parent node of the current object</param>
        private void PopulateComparisonData(ComparisonObject comparisonObject, int level, ComparisonNode parentNode)
        {
            if (comparisonObject != null)
            {
                string nodeType = "";
                switch (comparisonObject.ComparisonObjectType)
                {
                    case ComparisonObjectType.DataSource:
                        nodeType = "Data Source";
                        break;

                    case ComparisonObjectType.CalculationItem:
                        nodeType = "Calculation Item";
                        break;

                    case ComparisonObjectType.Table:

                        //Check if source table has any calc item children. If yes, it's a calc group.
                        bool isCalcGroup = false;
                        foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                        {
                            if (childComparisonObject.ComparisonObjectType == ComparisonObjectType.CalculationItem && childComparisonObject.Status != ComparisonObjectStatus.MissingInSource)
                            {
                                isCalcGroup = true;
                                break;
                            }
                        }
                        nodeType = isCalcGroup ? "Calculation Group" : "Table";
                        break;

                    default:
                        nodeType = comparisonObject.ComparisonObjectType.ToString();
                        break;
                }

                ComparisonNode currentNode = new ComparisonNode
                {
                    NodeType = nodeType,
                    SourceName = comparisonObject.SourceObjectName,
                    TargetName = comparisonObject.TargetObjectName,
                    SourceInternalName = comparisonObject.SourceObjectInternalName,
                    TargetInternalName = comparisonObject.TargetObjectInternalName,
                    SourceObjectDefinition = comparisonObject.SourceObjectDefinition,
                    TargetObjectDefinition = comparisonObject.TargetObjectDefinition,
                    ShowNode = true,
                    Level = level,
                    MergeAction = comparisonObject.MergeAction.ToString(),
                    DisableMessage = "",
                    DropdownDisabled = false
                };

                if (parentNode != null)
                {
                    currentNode.ParentId = parentNode.Id;
                    parentNode.ChildNodes.Add(currentNode.Id);
                }

                switch (comparisonObject.Status)
                {
                    case ComparisonObjectStatus.MissingInTarget:
                        currentNode.Status = "Missing in Target";
                        break;
                    case ComparisonObjectStatus.MissingInSource:
                        currentNode.Status = "Missing in Source";
                        break;
                    case ComparisonObjectStatus.SameDefinition:
                        currentNode.Status = "Same Definition";
                        break;
                    case ComparisonObjectStatus.DifferentDefinitions:
                        currentNode.Status = "Different Definitions";
                        break;
                    default:
                        break;
                }

                comparisonList.Add(currentNode);

                // Populate helper objects
                AngularComposite angularComposite = new AngularComposite(currentNode, comparisonObject);
                _directAccessList.Add(currentNode.Id, angularComposite);


                // set drop-down to have limited members based on what is available
                switch (comparisonObject.MergeAction)
                {
                    case MergeAction.Create:
                        currentNode.AvailableActions = new List<string> { "Create", "Skip" };

                        if (parentNode != null && string.Equals(parentNode.Status, "Missing in Target") && string.Equals(parentNode.MergeAction, "Skip"))
                        {
                            comparisonObject.MergeAction = MergeAction.Skip;
                            currentNode.MergeAction = MergeAction.Skip.ToString();
                            currentNode.DropdownDisabled = true;
                            SetNodeTooltip(angularComposite, true);
                        }
                        break;
                    case MergeAction.Update:
                        currentNode.AvailableActions = new List<string> { "Update", "Skip" };
                        break;
                    case MergeAction.Delete:
                        currentNode.AvailableActions = new List<string> { "Delete", "Skip" };

                        //check if parent is also set to delete, in which case make this cell readonly
                        if (parentNode != null && string.Equals(parentNode.MergeAction, "Delete"))
                        {
                            currentNode.DropdownDisabled = true;
                            SetNodeTooltip(angularComposite, true);
                        }
                        break;
                    case MergeAction.Skip:

                        switch (comparisonObject.Status)
                        {
                            case ComparisonObjectStatus.MissingInTarget:
                                currentNode.AvailableActions = new List<string> { "Create", "Skip" };

                                //check if parent is also MissingInTarget and Skip, make this cell readonly
                                if (parentNode != null && string.Equals(parentNode.Status, "Missing in Target") && string.Equals(parentNode.MergeAction, "Skip"))
                                {
                                    currentNode.DropdownDisabled = true;
                                    SetNodeTooltip(angularComposite, true);
                                }

                                break;
                            case ComparisonObjectStatus.MissingInSource:
                                currentNode.AvailableActions = new List<string> { "Delete", "Skip" };
                                break;
                            case ComparisonObjectStatus.DifferentDefinitions:
                                currentNode.AvailableActions = new List<string> { "Update", "Skip" };
                                break;
                            default:
                                //default covers ComparisonObjectStatus.SameDefinition: which is most common case (above cases are for saved skip selections from file)
                                currentNode.AvailableActions = new List<string> { "Skip" };
                                currentNode.DropdownDisabled = true;
                                SetNodeTooltip(angularComposite, true);
                                break;
                        }

                        break;
                    default:
                        break;
                };

                // Add child objects if it exists
                if (comparisonObject.ChildComparisonObjects != null && comparisonObject.ChildComparisonObjects.Count > 0)
                {
                    foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                    {
                        PopulateComparisonData(childComparisonObject, level + 1, currentNode);
                    }
                }
            }
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Set visibility of Angular node
        /// </summary>
        /// <param name="IsVisible">Show or hide node</param>
        /// <param name="sourceObjectName">Display name of the node for source</param>
        /// <param name="sourceObjectId">Internal name of the node for source</param>
        /// <param name="targetObjectName">Display name of the node for target</param>
        /// <param name="targetObjectId">Internal name of the node for target</param>
        /// <param name="objType">Object type i.e. Data source, KPI, Measure</param>
        private void SetNodeVisibility(bool IsVisible, AngularComposite node)
        {
            if (node != null)
            {
                //node.IsVisible = IsVisible;
                node.ngComparison.ShowNode = IsVisible;
            }
        }

        private void SetNodeTooltip(AngularComposite node, bool disabledDueToParent)
        {
            node.ngComparison.DisableMessage = (disabledDueToParent ? "This object's action option is disabled due to a parent object selection" : "");

        }
        #endregion

        #region Menu actions

        /// <summary>
        /// Show or Hide skip nodes
        /// </summary>
        /// <param name="hide">Hide Skip nodes</param>
        /// <param name="sameDefinitionFilter">Hide objects only in case of same definition</param>
        public void ShowHideSkipNodes(bool hide, bool sameDefinitionFilter = false)
        {
            if (this._comparison != null)
            {
                foreach (ComparisonNode node in comparisonList)
                {
                    ShowHideSkipNodes(node, hide, sameDefinitionFilter);
                }
            }
        }

        /// <summary>
        /// Show or hide skip nodes
        /// </summary>
        /// <param name="comparisonObject">List of comparison objects for whom children are to be checked</param>
        /// <param name="hide">Show or hide the node</param>
        /// <param name="sameDefinitionFilter">Hide nodes with same definition</param>
        private void ShowHideSkipNodes(ComparisonNode node, bool hide, bool sameDefinitionFilter)
        {
            bool isVisible = true;
            if (node.MergeAction.ToString() == "Skip" && (!sameDefinitionFilter || (sameDefinitionFilter && hide && node.Status.ToString() == "Same Definition")))
            {
                // if currently selected skip item contains Update, Delete or Create children, then need to keep visible - or result in orphans
                bool foundCreateOrDeleteChild = false;
                foreach (int childNodeId in node.ChildNodes)
                {
                    if (_directAccessList.ContainsKey(childNodeId))
                    {
                        AngularComposite childNode = _directAccessList[childNodeId];
                        if (childNode.dotNetComparison.MergeAction == MergeAction.Update || childNode.dotNetComparison.MergeAction == MergeAction.Delete || childNode.dotNetComparison.MergeAction == MergeAction.Create)
                        {
                            foundCreateOrDeleteChild = true;
                            break;
                        }
                    }
                }

                if (hide)
                {
                    if (!foundCreateOrDeleteChild)
                    {
                        isVisible = false;
                    }
                }
                else
                {
                    isVisible = true;
                }
            }
            else
            {
                isVisible = (
                    !(node.MergeAction.ToString() == "Skip " &&
                     (node.ChildNodes.Count == 0 || !NodeContainsEditableChildren(node, hide))));
            }

            if (_directAccessList.ContainsKey(node.Id))
            {
                AngularComposite childNode = _directAccessList[node.Id];
                SetNodeVisibility(isVisible, childNode);
            }

            foreach (int childNodeId in node.ChildNodes)
            {
                if (_directAccessList.ContainsKey(childNodeId))
                {
                    AngularComposite childNode = _directAccessList[childNodeId];
                    ShowHideSkipNodes(childNode.ngComparison, hide, sameDefinitionFilter);
                }
            }
        }

        /// <summary>
        /// Check if node contains editable children
        /// </summary>
        /// <param name="node">Node for which children is to be checked</param>
        /// <param name="hide">Hide or show</param>
        /// <returns></returns>
        private bool NodeContainsEditableChildren(ComparisonNode node, bool hide)
        {
            bool containsChildren = false;

            foreach (int childNodeId in node.ChildNodes)
            {
                if (_directAccessList.ContainsKey(childNodeId))
                {
                    AngularComposite childComposite = _directAccessList[childNodeId];
                    ComparisonNode childNode = childComposite.ngComparison;

                    if ((hide &&
                     childNode.MergeAction != "Skip " &&
                     childNode.MergeAction != "Skip") ||
                    (!hide &&
                     childNode.MergeAction != "Skip "))
                    {
                        containsChildren = true;
                    }
                    else
                    {
                        bool childContainsChildren = NodeContainsEditableChildren(childNode, hide);
                        if (!containsChildren)
                        {
                            containsChildren = childContainsChildren;
                        }
                    }

                    if (childNode.MergeAction.ToString() == "Skip")
                    {
                        SetNodeVisibility(!hide, childComposite);
                    }
                }
            }

            if (node.MergeAction.ToString() != "Skip")
            {
                if (_directAccessList.ContainsKey(node.Id))
                {
                    AngularComposite nodeComposite = _directAccessList[node.Id];
                    SetNodeVisibility(containsChildren, nodeComposite);
                }
            }

            return containsChildren;
        }

        /********** Set node to skip depending on comparison object status ****************/
        /// <summary>
        /// Sets Action property of objects to Skip within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        /// <param name="comparisonStatus"></param>
        public void SkipItems(bool selectedOnly, ComparisonObjectStatus comparisonObjectStatus = ComparisonObjectStatus.Na) //Na because won't take null cos it's an enum
        {
            List<ComparisonNode> listToUse = (selectedOnly ? selectedNodes : comparisonList);
            foreach (ComparisonNode node in listToUse)
            {
                // In case of selected only, check if item is present in selected objects
                SkipItemPrivate(comparisonObjectStatus, node);
            }
        }

        private void SkipItemPrivate(ComparisonObjectStatus comparisonObjectStatus, ComparisonNode row)
        {
            if (comparisonObjectStatus == ComparisonObjectStatus.Na ||
                (comparisonObjectStatus == ComparisonObjectStatus.DifferentDefinitions && row.Status == "Different Definitions") ||
                (comparisonObjectStatus == ComparisonObjectStatus.MissingInSource && row.Status == "Missing in Source") ||
                (comparisonObjectStatus == ComparisonObjectStatus.MissingInTarget && row.Status == "Missing in Target"))
            {
                bool isReadOnly = row.DropdownDisabled;
                if (!isReadOnly &&
                    row.MergeAction != MergeAction.Skip.ToString()
                    //&&
                    //row.Cells[8].Value.ToString() != "Set Parent Node" -- Need to check where is this value set
                    )
                {
                    row.MergeAction = MergeAction.Skip.ToString();
                    if (_directAccessList.ContainsKey(row.Id))
                    {
                        AngularComposite node = _directAccessList[row.Id];
                        node.dotNetComparison.MergeAction = MergeAction.Skip;
                        CheckToSkipChildren(row);
                    }

                }
            }
        }

        private void CheckToSkipChildren(ComparisonNode selectedRow)
        {
            // if Missing in Target (default is create) and user selects skip, definitely can't create child objects, so set them to skip too and disable them
            if (selectedRow.Status == "Missing in Target")
            {
                //TreeGridNode selectedNode = FindNodeByIDs(selectedRow.Cells[0].Value.ToString(), selectedRow.Cells[2].Value.ToString(), selectedRow.Cells[6].Value.ToString());

                foreach (int node in selectedRow.ChildNodes)
                {

                    SetNodeToSkip(node);
                }
            }
            // if Missing in Source (default is delete) and user selects skip, he may still want to delete some child objects, so ensure they are enabled
            else if (selectedRow.Status == "Missing in Source")
            {
                //TreeGridNode selectedNode = FindNodeByIDs(selectedRow.Cells[0].Value.ToString(), selectedRow.Cells[2].Value.ToString(), selectedRow.Cells[6].Value.ToString());

                foreach (int nodeId in selectedRow.ChildNodes)
                {
                    if (_directAccessList.ContainsKey(nodeId))
                    {
                        AngularComposite node = _directAccessList[nodeId];

                        if (node.ngComparison.AvailableActions.Contains("Skip"))
                        {
                            node.ngComparison.DropdownDisabled = false;

                            SetNodeTooltip(node, false);
                        }
                    }
                }
            }
        }
        private void SetNodeToSkip(int nodeId)
        {
            if (_directAccessList.ContainsKey(nodeId))
            {
                AngularComposite node = _directAccessList[nodeId];

                if (node.ngComparison.AvailableActions.Contains("Skip"))
                {
                    node.ngComparison.MergeAction = MergeAction.Skip.ToString();
                    node.ngComparison.DropdownDisabled = true;
                    node.dotNetComparison.MergeAction = MergeAction.Skip;

                    SetNodeTooltip(node, true);
                }

                foreach (int childNode in node.ngComparison.ChildNodes)
                {
                    SetNodeToSkip(childNode);
                }
            }
        }

        /************* End section ****************/

        /********** Set node to update ****************/
        /// <summary>
        /// Set actions for node with different definitions to update
        /// </summary>
        /// <param name="selectedOnly">Set for selected nodes or all nodes</param>
        public void UpdateItems(bool selectedOnly)
        {
            // If selected only, pick items from selected list
            List<ComparisonNode> listToUse = (selectedOnly ? selectedNodes : comparisonList);

            // Not necessary to run twice with internal method because Updates don't impact children
            foreach (ComparisonNode item in listToUse)
            {
                if (item.AvailableActions.Contains("Update"))
                {
                    item.MergeAction = MergeAction.Update.ToString();
                    // Set merge action in corresponding comparison list
                    _directAccessList[item.Id].dotNetComparison.MergeAction = MergeAction.Update;
                }
            }
        }
        /************* End section ****************/

        /********** Set node to create ****************/
        /// <summary>
        /// Sets Action property of objects to Create within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        public void CreateItems(bool selectedOnly)
        {
            List<ComparisonNode> listToUse = (selectedOnly ? selectedNodes : comparisonList);

            foreach (ComparisonNode item in listToUse)
            {
                //DataGridViewRow row = (selectedOnly ? this.SelectedRows[i] : this.Rows[i]);

                bool isReadOnly = item.DropdownDisabled;
                if (!isReadOnly && item.MergeAction != "Skip " // This condition is not working in existing code. Retained for consistency with existing code.
                    && item.AvailableActions.Contains(MergeAction.Create.ToString()))
                {
                    item.MergeAction = MergeAction.Create.ToString();
                    // Set merge action in corresponding comparison list
                    _directAccessList[item.Id].dotNetComparison.MergeAction = MergeAction.Create;

                    // Check status of children
                    CheckToCreateChildren(item);
                }
            }
        }
        private void CheckToCreateChildren(ComparisonNode selectedRow)
        {
            // if Missing in Target (default is create) and user selects create, he may still want to skip some child objects, so ensure they are enabled
            if (selectedRow.Status.ToString() == "Missing in Target")
            {

                foreach (int nodeId in selectedRow.ChildNodes)
                {
                    AngularComposite node = _directAccessList[nodeId];
                    if (node.ngComparison.AvailableActions.Contains(MergeAction.Create.ToString()))
                    {
                        node.ngComparison.DropdownDisabled = false;
                        SetNodeTooltip(node, false);
                    }
                }
            }
        }
        /************* End section ****************/

        /********** Set node to delete ****************/
        /// <summary>
        /// Sets Action property of objects to Delete within given range.
        /// </summary>
        /// <param name="selectedOnly"></param>
        public void DeleteItems(bool selectedOnly)
        {
            List<ComparisonNode> listToUse = (selectedOnly ? selectedNodes : comparisonList);

            foreach (ComparisonNode item in listToUse)
            {
                bool isReadOnly = item.DropdownDisabled;
                if (!isReadOnly
                    && item.MergeAction != "Skip " // This condition is not working in existing code. Retained for consistency with existing code.
                    && item.AvailableActions.Contains(MergeAction.Delete.ToString()))
                {
                    item.MergeAction = MergeAction.Delete.ToString();
                    // Set merge action in corresponding comparison list
                    _directAccessList[item.Id].dotNetComparison.MergeAction = MergeAction.Delete;

                    // Check status of children
                    CheckToDeleteChildren(item);
                }
            }
        }
        private void CheckToDeleteChildren(ComparisonNode selectedRow)
        {
            // if Missing in Source (default is delete) and user selects delete, definitely can't skip child objects, so set them to delete too and disable them
            if (selectedRow.Status == "Missing in Source")
            {

                foreach (int node in selectedRow.ChildNodes)
                {
                    SetNodeToDelete(node);
                }
            }
        }
        private void SetNodeToDelete(int nodeId)
        {
            if (_directAccessList.ContainsKey(nodeId))
            {
                AngularComposite node = _directAccessList[nodeId];

                if (node.ngComparison.AvailableActions.Contains("Delete"))
                {
                    node.ngComparison.MergeAction = MergeAction.Delete.ToString();
                    node.ngComparison.DropdownDisabled = true;
                    node.dotNetComparison.MergeAction = MergeAction.Delete;

                    SetNodeTooltip(node, true);
                }

                foreach (int childNode in node.ngComparison.ChildNodes)
                {
                    SetNodeToDelete(childNode);
                }
            }
        }
        /************* End section ****************/
        #endregion
    }
}
