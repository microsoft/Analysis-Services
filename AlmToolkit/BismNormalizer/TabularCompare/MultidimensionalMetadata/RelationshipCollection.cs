using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Represents a collection of Relationship objects.
    /// </summary>
    public class RelationshipCollection : List<Relationship>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Relationship object if found. Null if not found.</returns>
        public Relationship FindByName(string name)
        {
            foreach (Relationship relationship in this)
            {
                if (relationship.Name == name)
                {
                    return relationship;
                }
            }
            return null;
        }

        /// <summary>
        /// A Boolean specifying whether the collection contains object by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the object is found, or False if it's not found.</returns>
        public bool ContainsName(string name)
        {
            foreach (Relationship relationship in this)
            {
                if (relationship.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find an object in the collection by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Relationship object if found. Null if not found.</returns>
        public Relationship FindById(string id)
        {
            foreach (Relationship relationship in this)
            {
                if (relationship.Id == id)
                {
                    return relationship;
                }
            }
            return null;
        }

        /// <summary>
        /// A Boolean specifying whether the collection contains object by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the object is found, or False if it's not found.</returns>
        public bool ContainsId(string id)
        {
            foreach (Relationship relationship in this)
            {
                if (relationship.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a collection of Relationship objects filtered by the parent table's name.
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns>RelationshipCollection</returns>
        public RelationshipCollection FilterByTableId(string tableId)
        {
            RelationshipCollection returnTables = new RelationshipCollection();
            foreach (Relationship relationship in this)
            {
                if (relationship.Table.Id == tableId)
                {
                    returnTables.Add(relationship);
                }
            }
            return returnTables;
        }

        /// <summary>
        /// Removes an object from the collection by its Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the object was removed, or False if was not found.</returns>
        public bool RemoveById(string id)
        {
            foreach (Relationship relationship in this)
            {
                if (relationship.Id == id)
                {
                    this.Remove(relationship);
                    return true;
                }
            }
            return false;
        }
    }
}
