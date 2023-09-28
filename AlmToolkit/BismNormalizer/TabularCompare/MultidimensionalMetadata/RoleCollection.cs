using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Represents a collection of Role objects.
    /// </summary>
    public class RoleCollection : List<Role>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Role object if found. Null if not found.</returns>
        public Role FindByName(string name)
        {
            foreach (Role role in this)
            {
                if (role.Name == name)
                {
                    return role;
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
            foreach (Role role in this)
            {
                if (role.Name == name)
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
        /// <returns>Role object if found. Null if not found.</returns>
        public Role FindById(string id)
        {
            foreach (Role role in this)
            {
                if (role.Id == id)
                {
                    return role;
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
            foreach (Role role in this)
            {
                if (role.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes an object from the collection by its Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the object was removed, or False if was not found.</returns>
        public bool RemoveById(string id)
        {
            foreach (Role role in this)
            {
                if (role.Id == id)
                {
                    this.Remove(role);
                    return true;
                }
            }
            return false;
        }
    }
}
