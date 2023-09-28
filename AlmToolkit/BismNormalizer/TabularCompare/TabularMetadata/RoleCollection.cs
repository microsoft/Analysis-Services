using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.TabularMetadata
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
                if (role.InternalName == id)
                {
                    return role;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes an object from the collection by its internal name.
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns>True if the object was removed, or False if was not found.</returns>
        public bool Remove(string internalName)
        {
            foreach (Role role in this)
            {
                if (role.InternalName == internalName)
                {
                    this.Remove(role);
                    return true;
                }
            }
            return false;
        }
    }
}
