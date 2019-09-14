//using System;
//using System.Collections.Generic;

//namespace BismNormalizer.TabularCompare.TabularMetadata
//{
//    /// <summary>
//    /// Represents a collection of RefreshPolicy objects.
//    /// </summary>
//    public class RefreshPolicyCollection : List<RefreshPolicy>
//    {
//        /// <summary>
//        /// Find an object in the collection by name.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns>RefreshPolicy object if found. Null if not found.</returns>
//        public RefreshPolicy FindByName(string name)
//        {
//            foreach (RefreshPolicy refreshPolicy in this)
//            {
//                if (refreshPolicy.Name == name)
//                {
//                    return refreshPolicy;
//                }
//            }
//            return null;
//        }

//        /// <summary>
//        /// A Boolean specifying whether the collection contains object by name.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns>True if the object is found, or False if it's not found.</returns>
//        public bool ContainsName(string name)
//        {
//            foreach (RefreshPolicy refreshPolicy in this)
//            {
//                if (refreshPolicy.Name == name)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        /// <summary>
//        /// A Boolean specifying whether the collection contains object by name searching without case sensitivity.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns>True if the object is found, or False if it's not found.</returns>
//        public bool ContainsNameCaseInsensitive(string name)
//        {
//            foreach (RefreshPolicy refreshPolicy in this)
//            {
//                if (refreshPolicy.Name.ToUpper() == name.ToUpper())
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        /// <summary>
//        /// Returns a collection of RefreshPolicy objects filtered by the parent table's name.
//        /// </summary>
//        /// <param name="tableName"></param>
//        /// <returns>RefreshPolicyCollection</returns>
//        public RefreshPolicyCollection FilterByTableName(string tableName)
//        {
//            RefreshPolicyCollection returnRefreshPolicys = new RefreshPolicyCollection();
//            foreach (RefreshPolicy refreshPolicy in this)
//            {
//                if (refreshPolicy.TableName == tableName)
//                {
//                    returnRefreshPolicys.Add(refreshPolicy);
//                }
//            }
//            return returnRefreshPolicys;
//        }

//        /// <summary>
//        /// Removes an object from the collection by its name.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns>True if the object was removed, or False if was not found.</returns>
//        public bool RemoveByName(string name)
//        {
//            foreach (RefreshPolicy refreshPolicy in this)
//            {
//                if (refreshPolicy.Name == name)
//                {
//                    this.Remove(refreshPolicy);
//                    return true;
//                }
//            }
//            return false;
//        }
//    }
//}
