using System;
using System.Collections.Generic;

namespace BismNormalizer.TabularCompare.Core
{
    /// <summary>
    /// Represents source and target objects for comparison, their type and status. This class is extended by BismNormalizer.TabularCompare.MultidimensionalMetadata.ComparisonObject and BismNormalizer.TabularCompare.TabularMetadata.ComparisonObject depending on SSAS compatibility level of the comparison.
    /// </summary>
    public abstract class ComparisonObject : IComparable<ComparisonObject>
    {
        #region Protetced/Private Members

        protected ComparisonObjectType _comparisonObjectType;
        protected ComparisonObjectStatus _status;
        protected MergeAction _mergeAction;
        protected List<ComparisonObject> _childComparisonObjects;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ComparisonObject class using multiple parameters.
        /// </summary>
        /// <param name="comparisonObjectType">Type of ComaprisonObject such as Table, Measure, Relationship, etc.</param>
        /// <param name="status">Status of ComaprisonObject such as Same Definition, Different Definitions and Missing In Target.</param>
        /// <param name="mergeAction">Action of ComaprisonObject such as Create, Update, Delete and Skip.</param>
        public ComparisonObject(
            ComparisonObjectType comparisonObjectType,
            ComparisonObjectStatus status,
            MergeAction mergeAction)
        {
            _comparisonObjectType = (comparisonObjectType == ComparisonObjectType.Connection ? ComparisonObjectType.DataSource : comparisonObjectType); //Need to support connection for backwards compatibility when deserializing from xml
            _status = status;
            _mergeAction = mergeAction;
            _childComparisonObjects = new List<ComparisonObject>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The comparison object type such as Table, Measure, Relationship, etc.
        /// </summary>
        public ComparisonObjectType ComparisonObjectType 
        {
            get { return _comparisonObjectType; }
            set
            {
                _comparisonObjectType = (value == ComparisonObjectType.Connection ? ComparisonObjectType.DataSource : value); //Need to support connection for backwards compatibility when deserializing from xml
            }
        }

        /// <summary>
        /// The comparison object status such as Same Definition, Different Definitions, Missing in Target and Missing in Source.
        /// </summary>
        public ComparisonObjectStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// The comparison object merge action such as Create, Update, Delete and Skip.
        /// </summary>
        public MergeAction MergeAction
        {
            get { return _mergeAction; }
            set { _mergeAction = value; }
        }

        /// <summary>
        /// Collection of ComparisonObject. Represents hierarchy shown on the differences grid.
        /// </summary>
        public List<ComparisonObject> ChildComparisonObjects
        {
            get { return _childComparisonObjects; }
            set { _childComparisonObjects = value; }
        }

        #endregion

        #region Abstract Members

        /// <summary>
        /// Gets the name of the source object.
        /// </summary>
        public abstract string SourceObjectName { get; }

        /// <summary>
        /// Gets the internal name of the source object. For objects instantiated in the BismNormalizer.TabularCompare.TabularMetadata namespace, this is the same as the name of the object with the exception of relationships. Relationships will show the internal GUID created by TOM in this property.
        /// </summary>
        public abstract string SourceObjectInternalName { get; }

        /// <summary>
        /// Gets the source object definition. For objects instantiated in the BismNormalizer.TabularCompare.TabularMetadata namespace, this is the JSON representation.
        /// </summary>
        public abstract string SourceObjectDefinition { get; }


        /// <summary>
        /// Gets the name of the target object.
        /// </summary>
        public abstract string TargetObjectName { get; }

        /// <summary>
        /// Gets the internal name of the target object. For objects instantiated in the BismNormalizer.TabularCompare.TabularMetadata namespace, this is the same as the name of the object with the exception of relationships. Relationships will show the internal GUID created by TOM in this property.
        /// </summary>
        public abstract string TargetObjectInternalName { get; }

        /// <summary>
        /// Gets the target object definition. For objects instantiated in the BismNormalizer.TabularCompare.TabularMetadata namespace, this is the JSON representation.
        /// </summary>
        public abstract string TargetObjectDefinition { get; }

        /// <summary>
        /// Provides key for CompareTo method.
        /// </summary>
        public abstract string SortKey();

        public abstract int CompareTo(ComparisonObject other);

        #endregion
    }
}
