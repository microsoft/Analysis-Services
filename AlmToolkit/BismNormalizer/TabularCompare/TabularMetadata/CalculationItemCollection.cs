using System;
using System.Collections.Generic;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents a collection of CalculationItem objects.
    /// </summary>
    public class CalculationItemCollection : List<CalculationItem>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>CalculationItem object if found. Null if not found.</returns>
        public CalculationItem FindByName(string name)
        {
            foreach (CalculationItem calculationItem in this)
            {
                if (calculationItem.Name == name)
                {
                    return calculationItem;
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
            foreach (CalculationItem calculationItem in this)
            {
                if (calculationItem.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A Boolean specifying whether the collection contains object by name searching without case sensitivity.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the object is found, or False if it's not found.</returns>
        public bool ContainsNameCaseInsensitive(string name)
        {
            foreach (CalculationItem calculationItem in this)
            {
                if (calculationItem.Name.ToUpper() == name.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a collection of CalculationItem objects filtered by the parent table's name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>CalculationItemCollection</returns>
        public CalculationItemCollection FilterByTableName(string tableName)
        {
            CalculationItemCollection returnCalculationItems = new CalculationItemCollection();
            foreach (CalculationItem calculationItem in this)
            {
                if (calculationItem.TableName == tableName)
                {
                    returnCalculationItems.Add(calculationItem);
                }
            }
            return returnCalculationItems;
        }

        /// <summary>
        /// Removes an object from the collection by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the object was removed, or False if was not found.</returns>
        public bool RemoveByName(string name)
        {
            foreach (CalculationItem calculationItem in this)
            {
                if (calculationItem.Name == name)
                {
                    this.Remove(calculationItem);
                    return true;
                }
            }
            return false;
        }
    }
}
