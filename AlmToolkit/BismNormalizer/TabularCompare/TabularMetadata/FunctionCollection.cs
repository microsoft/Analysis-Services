using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents a collection of Function objects.
    /// </summary>
    public class FunctionCollection : List<Function>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Function object if found. Null if not found.</returns>
        public Function FindByName(string name)
        {
            foreach (Function daxFunction in this)
            {
                if (string.Equals(daxFunction.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return daxFunction;
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
            foreach (Function daxFunction in this)
            {
                if (string.Equals(daxFunction.Name, name, StringComparison.OrdinalIgnoreCase))
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
        /// <returns>DaxFunction object if found. Null if not found.</returns>
        public Function FindById(string id)
        {
            foreach (Function daxFunction in this)
            {
                if (daxFunction.InternalName == id)
                {
                    return daxFunction;
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
            foreach (Function daxFunction in this)
            {
                if (daxFunction.InternalName == internalName)
                {
                    this.Remove(daxFunction);
                    return true;
                }
            }
            return false;
        }
    }
}
