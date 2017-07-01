using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents a link in a relationship chain, used to detect ambiguity in relationship paths.
    /// </summary>
    public class RelationshipLink
    {
        private Table _beginTable;
        private Table _endTable;
        private bool _root;
        private string _tablePath;
        private Relationship _filteringRelationship;

        /// <summary>
        /// Initializes a new instance of the RelationshipLink class using multiple parameters.
        /// </summary>
        /// <param name="beginTable">Table object representing begin table.</param>
        /// <param name="endTable">Table object representing end table.</param>
        /// <param name="root">Boolean indicating if the root link in a the relationship chain.</param>
        /// <param name="precedingTablePath">Recursive path to the preceding table in the relationship chain.</param>
        /// <param name="filteringRelationship">Relationship object for the relationship link.</param>
        public RelationshipLink(Table beginTable, Table endTable, bool root, string precedingTablePath, Relationship filteringRelationship)
        {
            _beginTable = beginTable;
            _endTable = endTable;
            _root = root;
            if (root)
            {
                _tablePath = $"'{beginTable.Name}'->'{endTable.Name}'";
            }
            else
            {
                _tablePath = $"{precedingTablePath}->'{endTable.Name}'";
            }
            _filteringRelationship = filteringRelationship;
        }

        /// <summary>
        /// BeginTable might be the From or the To table in TOM Relationship object; it depends on CrossFilterDirection and specific relationship direction.
        /// </summary>
        public Table BeginTable => _beginTable;

        /// <summary>
        /// EndTable might be the From or the To table in TOM Relationship object; it depends on CrossFilterDirection and specific relationship direction.
        /// </summary>
        public Table EndTable => _endTable;

        /// <summary>
        /// Boolean indicating if the root link in a the relationship chain.
        /// </summary>
        public bool Root => _root;

        /// <summary>
        /// Recursive path to the preceding table in the relationship chain.
        /// </summary>
        public string TablePath => _tablePath;

        /// <summary>
        /// Relationship object for the relationship link.
        /// </summary>
        public Relationship FilteringRelationship => _filteringRelationship;

        public override string ToString() => _tablePath;
    }
}
