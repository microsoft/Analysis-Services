using System;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents source and target objects for comparison, their type and status. This class is for tabular models that use tabular metadata with SSAS compatibility level 1200 or above.
    /// </summary>
    public class ComparisonObject : Core.ComparisonObject
    {
        private TabularObject _sourceObject;
        private TabularObject _targetObject;

        public ComparisonObject(
            ComparisonObjectType comparisonObjectType,
            ComparisonObjectStatus status,
            TabularObject sourceObject,
            TabularObject targetObject,
            MergeAction mergeAction) : base(comparisonObjectType, status, mergeAction)
        {
            _sourceObject = sourceObject;
            _targetObject = targetObject;
        }

        /// <summary>
        /// Source TabularObject instance for comparison.
        /// </summary>
        public TabularObject SourceObject
        {
            get { return _sourceObject; }
            set { _sourceObject = value; }
        }

        /// <summary>
        /// Name of source TabularObject instance.
        /// </summary>
        public override string SourceObjectName
        {
            get
            {
                if (_sourceObject == null)
                {
                    return "";
                }
                else
                {
                    if (_comparisonObjectType == ComparisonObjectType.Relationship || _comparisonObjectType == ComparisonObjectType.Measure || _comparisonObjectType == ComparisonObjectType.Kpi)
                    {
                        return "      " + _sourceObject.Name;
                    }
                    else
                    {
                        return _sourceObject.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Internal name of source TabularObject instance. This can be different than SourceObjectName for Relationship objects where the internal name is the SSDT assigned GUID.
        /// </summary>
        public override string SourceObjectInternalName
        {
            get
            {
                if (_sourceObject == null)
                {
                    return "";
                }
                else
                {
                    return _sourceObject.InternalName;
                }
            }
        }

        /// <summary>
        /// Definition of source TabularObject instance.
        /// </summary>
        public override string SourceObjectDefinition
        {
            get
            {
                if (_sourceObject == null)
                {
                    return "";
                }
                else
                {
                    return _sourceObject.ObjectDefinition;
                }
            }
        }

        /// <summary>
        /// Target TabularObject instance for comparison.
        /// </summary>
        public TabularObject TargetObject
        {
            get { return _targetObject; }
            set { _targetObject = value; }
        }

        /// <summary>
        /// Name of target TabularObject instance.
        /// </summary>
        public override string TargetObjectName
        {
            get
            {
                if (_targetObject == null)
                {
                    return "";
                }
                else
                {
                    if (_comparisonObjectType == ComparisonObjectType.Relationship || _comparisonObjectType == ComparisonObjectType.Measure || _comparisonObjectType == ComparisonObjectType.Kpi)
                    {
                        return "      " + _targetObject.Name;
                    }
                    else
                    {
                        return _targetObject.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Internal name of target TabularObject instance. This can be different than SourceObjectName for Relationship objects where the internal name is the SSDT assigned GUID.
        /// </summary>
        public override string TargetObjectInternalName
        {
            get
            {
                if (_targetObject == null)
                {
                    return "";
                }
                else
                {
                    return _targetObject.InternalName;
                }
            }
        }

        /// <summary>
        /// Definition of target TabularObject instance.
        /// </summary>
        public override string TargetObjectDefinition
        {
            get
            {
                if (_targetObject == null)
                {
                    return "";
                }
                else
                {
                    return _targetObject.ObjectDefinition;
                }
            }
        }

        /// <summary>
        /// Provides key for CompareTo method.
        /// </summary>
        public override string SortKey()
        {
            string sortKey = "";

            switch (this.ComparisonObjectType)
            {
                //tabular objects
                case ComparisonObjectType.DataSource:
                    sortKey = "A";
                    break;
                case ComparisonObjectType.Expression:
                    sortKey = "B";
                    break;
                case ComparisonObjectType.Table:
                    sortKey = "C";
                    break;
                case ComparisonObjectType.Relationship:
                    sortKey = "D";
                    break;
                case ComparisonObjectType.Measure:
                    sortKey = "E";
                    break;
                case ComparisonObjectType.Kpi:
                    sortKey = "F";
                    break;
                case ComparisonObjectType.Action:
                    sortKey = "G";
                    break;
                case ComparisonObjectType.Perspective:
                    sortKey = "H";
                    break;
                case ComparisonObjectType.Culture:
                    sortKey = "I";
                    break;
                case ComparisonObjectType.Role:
                    sortKey = "J";
                    break;

                default:
                    sortKey = "Z";
                    break;
            }
            sortKey += this.SourceObjectName != "" ? this.SourceObjectName : this.TargetObjectName;
            return sortKey;
        }

        public override int CompareTo(Core.ComparisonObject other) => string.Compare(this.SortKey(), other.SortKey());
    }
}
