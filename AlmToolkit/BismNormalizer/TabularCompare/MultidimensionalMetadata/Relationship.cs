using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices;
using Amo=Microsoft.AnalysisServices;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Abstraction of a tabular model relationship with properties and methods for comparison purposes.
    /// </summary>
    public class Relationship : ITabularObject
    {
        private Table _table;
        private Amo.Relationship _amoRelationship;
        private string _name;
        private string _longName;
        private string _childTableName;
        private string _childColumnName;
        private string _parentTableName;
        private string _parentColumnName;
        private string _objectDefinition;
        private bool _copiedFromSource;

        /// <summary>
        /// Initializes a new instance of the Relationship class using multiple parameters.
        /// </summary>
        /// <param name="table">Table object that the Relationship belongs to.</param>
        /// <param name="amoRelationship">Analysis Management Objects Relationship object abtstracted by the Relationship class.</param>
        /// <param name="copiedFromSource">Boolean indicating whether the relationship was copied from the source TabularModel object.</param>
        public Relationship(Table table, Amo.Relationship amoRelationship, bool copiedFromSource = false)
        {
            _table = table;
            _amoRelationship = amoRelationship;

            // parentTable is actually the FK (child) table in the relationship
            Dimension dimPK = table.TabularModel.AmoDatabase.Dimensions.Find(amoRelationship.ToRelationshipEnd.DimensionID);

            _childTableName = table.Name;
            _childColumnName = table.AmoDimension.Attributes.Find(amoRelationship.FromRelationshipEnd.Attributes[0].AttributeID).Name;
            _parentTableName = dimPK.Name;
            _parentColumnName = dimPK.Attributes.Find(amoRelationship.ToRelationshipEnd.Attributes[0].AttributeID).Name;

            //_name = "'" + _childTableName + "'[" + _childColumnName + "]" + "' -> '" + _parentTableName + "'[" + _parentColumnName + "]";
            _name = "'" + _childTableName + "' -> '" + _parentTableName + "'";
            _longName = _childTableName + "'[" + _childColumnName + "] => '" + _parentTableName + "'[" + _parentColumnName + "]";
            _objectDefinition = "Foreign Key Column: '" + _childTableName + "'[" + _childColumnName + "]\n" +
                                "Primary Key Column: '" + _parentTableName + "'[" + _parentColumnName + "]\n";

            _copiedFromSource = copiedFromSource;
        }

        /// <summary>
        /// Table object that the Relationship oject belongs to.
        /// </summary>
        public Table Table => _table;

        /// <summary>
        /// Analysis Management Objects Relationship object abtstracted by the Relationship class.
        /// </summary>
        public Amo.Relationship AmoRelationship
        {
            get { return _amoRelationship; }
            set { _amoRelationship = value; }
        }

        /// <summary>
        /// Name of the Relationship object.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Long name of the Relationship object.
        /// </summary>
        public string LongName => _longName;

        /// <summary>
        /// Id of the Relationship object.
        /// </summary>
        public string Id => _amoRelationship.ID;

        /// <summary>
        /// Substitute Id of the Relationship object.
        /// </summary>
        public string SubstituteId => _amoRelationship.ID;

        /// <summary>
        /// Name of the child table for the Relationship object.
        /// </summary>
        public string ChildTableName => _childTableName;

        /// <summary>
        /// Name of the child column for the Relationship object.
        /// </summary>
        public string ChildColumnName => _childColumnName;

        /// <summary>
        /// Name of the parent table for the Relationship object.
        /// </summary>
        public string ParentTableName => _parentTableName;

        /// <summary>
        /// Name of the parent column for the Relationship object.
        /// </summary>
        public string ParentColumnName => _parentColumnName;

        /// <summary>
        /// Indicates whether the Relationship object is active in the tabular model.
        /// </summary>
        public bool IsActive => _table.TabularModel.ActiveRelationshipIds.Contains(_amoRelationship.ID);

        /// <summary>
        /// Indicates whether the relationship was copied from the source tabular model.
        /// </summary>
        public bool CopiedFromSource => _copiedFromSource;

        /// <summary>
        /// Object definition of the Relationship object. This is a simplified list of relevant attribute values for comparison; not the XMLA definition of the abstracted AMO object.
        /// </summary>
        public string ObjectDefinition => _objectDefinition;

        public override string ToString() => this.GetType().FullName;
    }
}
