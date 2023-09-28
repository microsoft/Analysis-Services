using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.TabularMetadata
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
        /// Find an object in the collection by internal name.
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns>Relationship object if found. Null if not found.</returns>
        public Relationship FindByInternalName(string internalName)
        {
            foreach (Relationship relationship in this)
            {
                if (relationship.InternalName == internalName)
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
                if (relationship.Name == name.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A Boolean specifying whether the collection contains object by internal name.
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns>True if the object is found, or False if it's not found.</returns>
        public bool ContainsInternalName(string internalName)
        {
            foreach (Relationship relationship in this)
            {
                if (relationship.InternalName == internalName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes an object from the collection by its internal name.
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns>True if the object was removed, or False if was not found.</returns>
        public bool RemoveByInternalName(string internalName)
        {
            foreach (Relationship relationship in this)
            {
                if (relationship.InternalName == internalName)
                {
                    this.Remove(relationship);
                    return true;
                }
            }
            return false;
        }
    }
}
