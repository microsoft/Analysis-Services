using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Represents a collection of DataSource objects.
    /// </summary>
    public class TableCollection : List<Table>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>DataSource object if found. Null if not found.</returns>
        public Table FindByName(string name)
        {
            foreach (Table table in this)
            {
                if (table.Name == name)
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
        /// <returns>True if the object is found, or False if it's not found.</returns>
        public bool ContainsName(string name)
        {
            foreach (Table table in this)
            {
                if (table.Name == name)
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
        /// <returns>DataSource object if found. Null if not found.</returns>
        public Table FindById(string id)
        {
            foreach (Table table in this)
            {
                if (table.Id == id)
                {
                    return table;
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
            foreach (Table table in this)
            {
                if (table.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a collection of Table objects filtered by the parent datasource's Id.
        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <returns>TableCollection</returns>
        public TableCollection FilterByDataSourceId(string dataSourceId)
        {
            TableCollection returnTables = new TableCollection();
            foreach (Table table in this)
            {
                if (table.DataSourceID == dataSourceId)
                {
                    returnTables.Add(table);
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
            foreach (Table table in this)
            {
                if (table.Id == id)
                {
                    this.Remove(table);
                    return true;
                }
            }
            return false;
        }
    }
}
