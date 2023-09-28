using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Represents a collection of Perspective objects.
    /// </summary>
    public class PerspectiveCollection : List<Perspective>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Perspective object if found. Null if not found.</returns>
        public Perspective FindByName(string name)
        {
            foreach (Perspective perspective in this)
            {
                if (perspective.Name == name)
                {
                    return perspective;
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
            foreach (Perspective perspective in this)
            {
                if (perspective.Name == name)
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
        /// <returns>Perspective object if found. Null if not found.</returns>
        public Perspective FindById(string id)
        {
            foreach (Perspective perspective in this)
            {
                if (perspective.Id == id)
                {
                    return perspective;
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
            foreach (Perspective perspective in this)
            {
                if (perspective.Id == id)
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
            foreach (Perspective perspective in this)
            {
                if (perspective.Id == id)
                {
                    this.Remove(perspective);
                    return true;
                }
            }
            return false;
        }
    }
}
