using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Represents a collection of Measure objects.
    /// </summary>
    public class MeasureCollection : List<Measure>
    {
        /// <summary>
        /// Find an object in the collection by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Measure object if found. Null if not found.</returns>
        public Measure FindById(string id)
        {
            foreach (Measure measure in this)
            {
                if (measure.Id == id)
                {
                    return measure;
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
            foreach (Measure measure in this)
            {
                if (measure.Id == id)
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
        /// <returns>Measure object if found. Null if not found.</returns>
        public Measure FindByName(string name)
        {
            foreach (Measure measure in this)
            {
                if (measure.Name == name)
                {
                    return measure;
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
            foreach (Measure measure in this)
            {
                if (measure.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a collection of Measure objects filtered by the parent table's name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>MeasureCollection</returns>
        public MeasureCollection FilterByTableName(string tableName)
        {
            MeasureCollection returnMeasures = new MeasureCollection();
            foreach (Measure measure in this)
            {
                if (measure.TableName == tableName)
                {
                    returnMeasures.Add(measure);
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
            foreach (Measure measure in this)
            {
                if (measure.Id == id)
                {
                    this.Remove(measure);
                    return true;
                }
            }
            return false;
        }
    }
}
