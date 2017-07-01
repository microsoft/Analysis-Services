using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model relationship with properties and methods for comparison purposes.
    /// </summary>
    public class Relationship : TabularObject
    {
        private Table _table;
        private SingleColumnRelationship _tomRelationship;
        private string _relationshipName;
        private bool _copiedFromSource;
        private bool _modifiedInternalName;
        private string _oldInternalName;

        /// <summary>
        /// Initializes a new instance of the Relationship class using multiple parameters.
        /// </summary>
        /// <param name="table">Table object that the Relationship belongs to.</param>
        /// <param name="tomRelationship">Tabular Object Model SingleColumnRelationship object abtstracted by the Relationship class.</param>
        /// <param name="copiedFromSource">Boolean indicating whether the relationship was copied from the source TabularModel object.</param>
        /// <param name="modifiedInternalName">Boolean indicating whether the TOM Relationship object Name property was changed to avoid name conflict.</param>
        /// <param name="oldInternalName">If the TOM Relationship object Name property was changed, this parameter shows the old value.</param>
        public Relationship(Table table, SingleColumnRelationship tomRelationship, bool copiedFromSource = false, bool modifiedInternalName = false, string oldInternalName = "") 
            : base(tomRelationship)
        {
            _table = table;
            _tomRelationship = tomRelationship;

            //_relationshipName = $"'{_relationship.FromTable.Name}'->'{_relationship.ToTable.Name}'";
            //_relationshipName = $"[{_relationship.FromColumn.Name}]->'{_relationship.ToTable.Name}'[{_relationship.ToColumn.Name}]";
            _relationshipName = $"'{_tomRelationship.FromTable.Name}'[{_tomRelationship.FromColumn.Name}]->'{_tomRelationship.ToTable.Name}'[{_tomRelationship.ToColumn.Name}]";

            _copiedFromSource = copiedFromSource;
            _modifiedInternalName = modifiedInternalName;
            _oldInternalName = oldInternalName;
        }

        /// <summary>
        /// Table object that the Relationship oject belongs to.
        /// </summary>
        public Table Table => _table;

        /// <summary>
        /// Tabular Object Model SingleColumnRelationship object abtstracted by the Relationship class.
        /// </summary>
        public SingleColumnRelationship TomRelationship
        {
            get { return _tomRelationship; }
            set { _tomRelationship = value; }
        }

        /// <summary>
        /// Name of the Relationship object. Uses a friendly format.
        /// </summary>
        public override string Name => _relationshipName;

        /// <summary>
        /// The TOM Relationship object Name property, which is not displayed to users as its of GUID format.
        /// </summary>
        public override string InternalName => _tomRelationship.Name;

        /// <summary>
        /// Name of the from table for the Relationship object.
        /// </summary>
        public string FromTableName => _tomRelationship.FromTable.Name;

        /// <summary>
        /// Name of the to table for the Relationship object.
        /// </summary>
        public string ToTableName => _tomRelationship.ToTable.Name;

        /// <summary>
        /// Boolean indicating whether the relationship was copied from the source TabularModel object.
        /// </summary>
        public bool CopiedFromSource => _copiedFromSource;

        /// <summary>
        /// Boolean indicating whether the TOM Relationship object Name property was changed to avoid name conflict.
        /// </summary>
        public bool ModifiedInternalName => _modifiedInternalName;

        /// <summary>
        /// If the TOM Relationship object Name property was changed, this parameter shows the old value.
        /// </summary>
        public string OldInternalName => _oldInternalName;

        public override string ToString() => this.GetType().FullName;
    }
}
