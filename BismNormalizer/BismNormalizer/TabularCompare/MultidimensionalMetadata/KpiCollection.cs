using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Represents a collection of Kpi objects.
    /// </summary>
    public class KpiCollection : List<Kpi>
    {
        /// <summary>
        /// Find an object in the collection by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Kpi object if found. Null if not found.</returns>
        public Kpi FindById(string id)
        {
            foreach (Kpi kpi in this)
            {
                if (kpi.Id == id)
                {
                    return kpi;
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
            foreach (Kpi kpi in this)
            {
                if (kpi.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Kpi object if found. Null if not found.</returns>
        public Kpi FindByName(string name)
        {
            foreach (Kpi kpi in this)
            {
                if (kpi.Name == name)
                {
                    return kpi;
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
            foreach (Kpi BismKpi in this)
            {
                if (BismKpi.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a collection of Kpi objects filtered by the parent table's name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>KpiCollection</returns>
        public KpiCollection FilterByTableName(string tableName)
        {
            KpiCollection returnMeasures = new KpiCollection();
            foreach (Kpi kpi in this)
            {
                if (kpi.TableName == tableName)
                {
                    returnMeasures.Add(kpi);
                }
            }
            return returnMeasures;
        }

        /// <summary>
        /// Removes an object from the collection by its Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the object was removed, or False if was not found.</returns>
        public bool RemoveById(string id)
        {
            foreach (Kpi kpi in this)
            {
                if (kpi.Id == id)
                {
                    this.Remove(kpi);
                    return true;
                }
            }
            return false;
        }
    }
}
