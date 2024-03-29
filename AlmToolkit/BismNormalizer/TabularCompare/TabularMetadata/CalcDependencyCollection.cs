﻿using System;
using System.Collections.Generic;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    [Serializable]
    public class CalcDependenciesInfiniteRecursionException : Exception
    {
        public CalcDependenciesInfiniteRecursionException() { }

        public CalcDependenciesInfiniteRecursionException(string message)
            : base(message) { }

        public CalcDependenciesInfiniteRecursionException(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>
    /// Represents a collection of CalcDependency objects.
    /// </summary>
    public class CalcDependencyCollection : List<CalcDependency>
    {
        /// <summary>
        /// Returns collection of calc dependencies that the object identified by the params references (directly or indirectly).
        /// </summary>
        /// <param name="objectType">Type of the object to look up dependencies.</param>
        /// <param name="objectName">Name of the object to look up dependencies.</param>
        /// <returns></returns>
        public CalcDependencyCollection DependenciesReferenceFrom(CalcDependencyObjectType objectType, string tableName, string objectName)
        {
            CalcDependencyCollection returnVal = new CalcDependencyCollection();
            recursionCounter = 0;
            try
            { 
                LookUpDependenciesReferenceFrom(objectType, tableName, objectName, returnVal);
            }
            catch (CalcDependenciesInfiniteRecursionException)
            {   //Some M is not parsable
            }
            return returnVal;
        }

        private const int RecursionCounterMax = 1000;
        static int recursionCounter = 0;
        private void LookUpDependenciesReferenceFrom(CalcDependencyObjectType objectType, string tableName, string objectName, CalcDependencyCollection returnVal)
        {
            recursionCounter++;
            if (recursionCounter >= RecursionCounterMax)
            {
                Exception infRecursion = new CalcDependenciesInfiniteRecursionException($"Calc dependencies infinite recursion detected for type \"{objectType}\", table \"{tableName}\", name \"{objectName}\".");
                //Telemetry.TrackException(infRecursion);
                throw infRecursion;
            }

            foreach (CalcDependency calcDependency in this)
            {
                if (calcDependency.ObjectType == objectType && calcDependency.TableName == tableName && calcDependency.ObjectName == objectName)
                {
                    LookUpDependenciesReferenceFrom(calcDependency.ReferencedObjectType, calcDependency.ReferencedTableName, calcDependency.ReferencedObjectName, returnVal);
                    returnVal.Add(calcDependency);
                }
            }
        }

        /// <summary>
        /// Returns collection of M dependency objects that hold references to the object identified by the param values (directly or chained).
        /// </summary>
        /// <param name="objectType">Type of the object to look up dependencies.</param>
        /// <param name="objectName">Name of the object to look up dependencies.</param>
        /// <returns></returns>
        public CalcDependencyCollection DependenciesReferenceTo(CalcDependencyObjectType referencedObjectType, string referencedObjectName, string referencedTableName)
        {
            CalcDependencyCollection returnVal = new CalcDependencyCollection();
            LookUpDependenciesReferenceTo(referencedObjectType, referencedObjectName, referencedTableName, returnVal);
            return returnVal;
        }

        private void LookUpDependenciesReferenceTo(CalcDependencyObjectType referencedObjectType, string referencedObjectName, string referencedTableName, CalcDependencyCollection returnVal)
        {
            foreach (CalcDependency calcDependency in this)
            {
                if (
                      (calcDependency.ReferencedObjectType == referencedObjectType && referencedObjectType != CalcDependencyObjectType.Partition && calcDependency.ReferencedObjectName == referencedObjectName) ||
                      (calcDependency.ReferencedObjectType == referencedObjectType && referencedObjectType == CalcDependencyObjectType.Partition && calcDependency.ReferencedTableName == referencedTableName) //References to table-partition expressions are by table name, not partition name
                   )
                {
                    LookUpDependenciesReferenceTo(calcDependency.ObjectType, calcDependency.ObjectName, calcDependency.TableName, returnVal);
                    returnVal.Add(calcDependency);
                }
            }
        }

        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>CalcDependency object if found. Null if not found.</returns>
        public CalcDependency FindByName(string name)
        {
            foreach (CalcDependency calcDependency in this)
            {
                if (calcDependency.ObjectName == name)
                {
                    return calcDependency;
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
            foreach (CalcDependency calcDependency in this)
            {
                if (calcDependency.ObjectName == name)
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
        public bool Remove(string name)
        {
            foreach (CalcDependency calcDependency in this)
            {
                if (calcDependency.ObjectName == name)
                {
                    this.Remove(calcDependency);
                    return true;
                }
            }
            return false;
        }
    }
}
