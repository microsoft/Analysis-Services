using System;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Represents source and target objects for comparison, their type and status. This class is for tabular models that use multidimensional metadata with SSAS compatibility level 1100 or 1103.
    /// </summary>
    public class ComparisonObject : Core.ComparisonObject
    {
        private ITabularObject _sourceObject;
        private string _sourceObjectName;
        private string _sourceObjectId;
        private string _sourceObjectSubstituteId;
        private string _sourceObjectDefinition;
        private ITabularObject _targetObject;
        private string _targetObjectName;
        private string _targetObjectId;
        private string _targetObjectDefinition;

        public ComparisonObject(
            ComparisonObjectType comparisonObjectType,
            ComparisonObjectStatus status,
            ITabularObject sourceObject,
            string sourceObjectName,
            string sourceObjectId,
            string sourceObjectDefinition,
            MergeAction mergeAction,
            ITabularObject targetObject,
            string targetObjectName,
            string targetObjectId,
            string targetObjectDefinition) : base(comparisonObjectType, status, mergeAction)
        {
            _sourceObject = sourceObject;
            _sourceObjectName = sourceObjectName;
            _sourceObjectId = sourceObjectId;
            _sourceObjectDefinition = sourceObjectDefinition;
            _targetObject = targetObject;
            _targetObjectName = targetObjectName;
            _targetObjectId = targetObjectId;
            _targetObjectDefinition = targetObjectDefinition;
        }

        /// <summary>
        /// Source ITabularObject instance for comparison.
        /// </summary>
        public ITabularObject SourceObject
        {
            get { return _sourceObject; }
            set { _sourceObject = value; }
        }

        /// <summary>
        /// Name of source ITabularObject instance.
        /// </summary>
        public override string SourceObjectName
        {
            get { return _sourceObjectName; }
        }

        /// <summary>
        /// Id of source ITabularObject instance. This is set by the Id property used by multidimensional metadata.
        /// </summary>
        public string SourceObjectId
        {
            get { return _sourceObjectId; }
            set { _sourceObjectId = value; }
        }

        /// <summary>
        /// Id of source ITabularObject instance. This is a replacement Id to avoid conflict with existing Ids in target model.
        /// </summary>
        public string SourceObjectSubstituteId
        {
            get 
            {
                if (_sourceObjectSubstituteId == null)
                {
                    return _sourceObjectId;
                }
                else
                {
                    return _sourceObjectSubstituteId;
                }
            }
            set 
            { 
                _sourceObjectSubstituteId = value; 
            }
        }

        /// <summary>
        /// Definition of source ITabularObject instance.
        /// </summary>
        public override string SourceObjectDefinition
        {
            get { return _sourceObjectDefinition; }
        }

        /// <summary>
        /// Internal name of source ITabularObject instance.
        /// </summary>
        public override string SourceObjectInternalName => SourceObjectSubstituteId;

        /// <summary>
        /// Target ITabularObject instance for comparison.
        /// </summary>
        public ITabularObject TargetObject
        {
            get { return _targetObject; }
            set { _targetObject = value; }
        }

        /// <summary>
        /// Name of target ITabularObject instance.
        /// </summary>
        public override string TargetObjectName
        {
            get { return _targetObjectName; }
        }

        /// <summary>
        /// Id of target ITabularObject instance. This is set by the Id property used by multidimensional metadata.
        /// </summary>
        public string TargetObjectId
        {
            get { return _targetObjectId; }
            set { _targetObjectId = value; }
        }

        /// <summary>
        /// Definition of target ITabularObject instance.
        /// </summary>
        public override string TargetObjectDefinition
        {
            get { return _targetObjectDefinition; }
        }

        /// <summary>
        /// Internal name of source ITabularObject instance.
        /// </summary>
        public override string TargetObjectInternalName => TargetObjectId;

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
                case ComparisonObjectType.Table:
                    sortKey = "B";
                    break;
                case ComparisonObjectType.Relationship:
                    sortKey = "C";
                    break;
                case ComparisonObjectType.Measure:
                    sortKey = "D";
                    break;
                case ComparisonObjectType.Kpi:
                    sortKey = "E";
                    break;
                case ComparisonObjectType.Action:
                    sortKey = "F";
                    break;
                case ComparisonObjectType.Perspective:
                    sortKey = "G";
                    break;
                case ComparisonObjectType.Role:
                    sortKey = "H";
                    break;

                default:
                    sortKey = "Z";
                    break;
            }
            sortKey += this._sourceObjectName != "" ? this._sourceObjectName : this._targetObjectName;
            return sortKey;
        }
        public override int CompareTo(Core.ComparisonObject other) => string.Compare(this.SortKey(), other.SortKey());
    }
}
