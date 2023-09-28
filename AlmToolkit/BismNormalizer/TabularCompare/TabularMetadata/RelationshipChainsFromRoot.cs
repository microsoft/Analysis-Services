using System;
using System.Collections;
using System.Collections.Generic;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents a chain of RelationshipLink objects, used to detect ambiguity in relationship paths.
    /// </summary>
    public class RelationshipChainsFromRoot : List<RelationshipLink>
    {
        /// <summary>
        /// Find end table by name.
        /// </summary>
        /// <param name="endTableName">Name of the end table.</param>
        /// <returns>RelationshipLink object if found; null if not found.</returns>
        public RelationshipLink FindByEndTableName(string endTableName)
        {
            foreach (RelationshipLink ReferencedTable in this)
            {
                if (ReferencedTable.EndTable.Name == endTableName)
                {
                    return ReferencedTable;
                }
            }
            return null;
        }

        /// <summary>
        /// Find the root RelationshipLink in the chain.
        /// </summary>
        /// <returns>RelationshipLink object if found; null if not found.</returns>
        public RelationshipLink FindRoot()
        {
            foreach (RelationshipLink ReferencedTable in this)
            {
                if (ReferencedTable.Root)
                {
                    return ReferencedTable;
                }
            }
            return null;
        }

        /// <summary>
        /// Check if chain of RelationshipLink objects contains an end table with specified name.
        /// </summary>
        /// <param name="endTableName">Name of the end table.</param>
        /// <returns>Boolean indicating if the end table was found.</returns>
        public bool ContainsEndTableName(string endTableName)
        {
            foreach (RelationshipLink link in this)
            {
                if (link.EndTable.Name == endTableName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if chain of RelationshipLink objects contains an end table with specified name that is BiDi invoked.
        /// </summary>
        /// <param name="endTableName">Name of the end table.</param>
        /// <returns>Boolean indicating if the BiDi invoked end table was found.</returns>
        public bool ContainsBidiToEndTable(string endTableName)
        {
            foreach (RelationshipLink link in this)
            {
                if (link.EndTable.Name == endTableName && link.PrecedingPathBiDiInvoked)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove end table from chain of RelationshipLink objects.
        /// </summary>
        /// <param name="endTableName">Name of end table to remvoe.</param>
        /// <returns>Boolean indicating if the end table was successfully removed.</returns>
        public bool RemoveByEndTableName(string endTableName)
        {
            foreach (RelationshipLink ReferencedTable in this)
            {
                if (ReferencedTable.EndTable.Name == endTableName)
                {
                    this.Remove(ReferencedTable);
                    return true;
                }
            }
            return false;
        }
    }
}
