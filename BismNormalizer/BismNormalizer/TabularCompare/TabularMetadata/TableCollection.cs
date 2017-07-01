using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents a collection of Table objects.
    /// </summary>
    public class TableCollection : List<Table>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Table object if found. Null if not found.</returns>
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
        /// Returns a collection of Table objects filtered by the parent DataSource's name.
        /// </summary>
        /// <param name="dataSourceName"></param>
        /// <returns>TableCollection</returns>
        public TableCollection FilterByDataSource(Microsoft.AnalysisServices.Tabular.DataSource dataSource)
        {
            TableCollection returnTables = new TableCollection();
            foreach (Table table in this)
            {
                if (table.DataSourceName == dataSource.Name)
                {
                    returnTables.Add(table);
                }
            }
            return returnTables;
        }

        /// <summary>
        /// Returns a collection of Table objects that do not have a DataSource associated with them. These can be calculated tables or tables with M partitions that do not refer to a DataSource.
        /// </summary>
        /// <returns></returns>
        public TableCollection WithoutDataSource(Model model)
        {
            TableCollection tablesWithDataSource = new TableCollection();
            foreach (Microsoft.AnalysisServices.Tabular.DataSource dataSource in model.DataSources)
            {
                tablesWithDataSource.AddRange(this.FilterByDataSource(dataSource));
            }

            TableCollection tablesWithoutDataSource = new TableCollection();
            foreach (Table table in this)
            {
                if (!tablesWithDataSource.ContainsName(table.Name))
                {
                    tablesWithoutDataSource.Add(table);
                }
            }

            return tablesWithoutDataSource;
        }

        /// <summary>
        /// Removes an object from the collection by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the object was removed, or False if was not found.</returns>
        public bool RemoveByName(string name)
        {
            foreach (Table table in this)
            {
                if (table.Name == name)
                {
                    this.Remove(table);
                    return true;
                }
            }
            return false;
        }
    }
}
