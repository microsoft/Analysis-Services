using System;
using System.Collections.Generic;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Represents a collection of ProcessingTable objects.
    /// </summary>
    public class ProcessingTableCollection : List<ProcessingTable>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>ProcessingTable object if found. Null if not found.</returns>
        public ProcessingTable FindByName(string name)
        {
            foreach (ProcessingTable table in this)
            {
                if (table.Name == name)
                {
                    return table;
                }
            }
            return null;
        }

        /// <summary>
        /// Find an object in the collection by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ProcessingTable object if found. Null if not found.</returns>
        public ProcessingTable FindById(string id)
        {
            foreach (ProcessingTable table in this)
            {
                if (table.Id == id)
                {
                    return table;
                }
            }
            return null;
        }

        /// <summary>
        /// A Boolean specifying whether the collection contains object by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if an object of that name is found, or False if it's not found.</returns>
        public bool ContainsName(string name)
        {
            foreach (ProcessingTable table in this)
            {
                if (table.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A Boolean specifying whether the collection contains object by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if an object of that name is found, or False if it's not found.</returns>
        public bool ContainsId(string id)
        {
            foreach (ProcessingTable table in this)
            {
                if (table.Id == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
