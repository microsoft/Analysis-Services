using System;
using System.Collections.Generic;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Represents a collection of SkipSelection objects. This is serialized/deserialized to/from the BSMN file.
    /// </summary>
    public class SkipSelectionCollection : List<SkipSelection>
    {
        public SkipSelectionCollection() { }

        /// <summary>
        /// A Boolean specifying whether the collection contains an Core.ComparisonObject .
        /// </summary>
        /// <param name="comparisonObj"></param>
        /// <returns>True if an object of that name is found, or False if it's not found.</returns>
        public bool Contains(Core.ComparisonObject comparisonObj)
        {
            foreach (SkipSelection skipSelection in this)
            {
                if (skipSelection.ComparisonObjectType == comparisonObj.ComparisonObjectType && skipSelection.Status == comparisonObj.Status && (skipSelection.Status == ComparisonObjectStatus.MissingInSource || skipSelection.SourceObjectInternalName == comparisonObj.SourceObjectInternalName) && (skipSelection.Status == ComparisonObjectStatus.MissingInTarget || skipSelection.TargetObjectInternalName == comparisonObj.TargetObjectInternalName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
