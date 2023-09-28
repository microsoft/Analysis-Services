using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents a collection of DataSource objects.
    /// </summary>
    public class DataSourceCollection : List<DataSource>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>DataSource object if found. Null if not found.</returns>
        public DataSource FindByName(string name)
        {
            foreach (DataSource dataSource in this)
            {
                if (dataSource.Name == name)
                {
                    return dataSource;
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
            foreach (DataSource dataSource in this)
            {
                if (dataSource.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes an object from the collection by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the object was removed, or False if was not found.</returns>
        public bool RemoveByName(string name)
        {
            foreach (DataSource dataSource in this)
            {
                if (dataSource.Name == name)
                {
                    this.Remove(dataSource);
                    return true;
                }
            }
            return false;
        }
    }
}
