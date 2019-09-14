namespace AlmToolkit.Model
{
    using System.Collections.Generic;

    public class ComparisonNode
    {
        private static int objectCount = 1;

        /// <summary>
        /// Id of the object
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Node type of the object
        /// Example: Data Source, Table, Relationship, KPI
        /// </summary>
        public string NodeType { get; set; }
        /// <summary>
        /// Id of this object's parent
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// Name of this object in source schema
        /// </summary>
        public string SourceName { get; set; }
        /// <summary>
        /// Name of this object in target schema
        /// </summary>
        public string TargetName { get; set; }
        /// <summary>
        /// Internal name of this object in source schema
        /// </summary>
        public string SourceInternalName { get; set; }
        /// <summary>
        /// Internal Name of this object in target schema
        /// </summary>
        public string TargetInternalName { get; set; }
        /// <summary>
        /// Indentation level of the object
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// Status of the object compared to the source and target
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Current action to be performed for this object
        /// </summary>
        public string MergeAction { get; set; }
        /// <summary>
        /// Code at source
        /// </summary>
        public string SourceObjectDefinition { get; set; }
        /// <summary>
        /// Code at target
        /// </summary>
        public string TargetObjectDefinition { get; set; }
        /// <summary>
        /// Ids of the children nodes
        /// </summary>
        public List<int> ChildNodes { get; set; }
        /// <summary>
        /// Actions that can be performed for this object
        /// </summary>
        public List<string> AvailableActions { get; set; }
        /// <summary>
        /// To maintain if the object is to be shown on UI or not
        /// </summary>
        public bool ShowNode { get; set; }
        /// <summary>
        /// To maintain if the dropdown is disabled on the UI
        /// </summary>
        public bool DropdownDisabled { get; set; }
        /// <summary>
        /// Text mentioning why the dropdown is disabled
        /// </summary>
        public string DisableMessage { get; set; }


        public ComparisonNode()
        {
            Id = objectCount;
            objectCount = objectCount + 1;
            ChildNodes = new List<int>();
        }

    }
}
