using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Represents a skipped ComparisonObject.
    /// </summary>
    public class SkipSelection
    {
        private ComparisonObjectType _comparisonObjectType;
        private ComparisonObjectStatus _comparisonObjectStatus;
        private string _sourceObjectInternalName;
        private string _targetObjectInternalName;

        /// <summary>
        /// Initializes a new instance of the SkipSelection class.
        /// </summary>
        public SkipSelection() { }

        /// <summary>
        /// Initializes a new instance of the SkipSelection class using a ComparisonInfo object.
        /// </summary>
        /// <param name="comparisonInfo">ComparisonInfo object typically deserialized from a BSMN file.</param>
        public SkipSelection(Core.ComparisonObject comparisonObject)
        {
            _comparisonObjectType = comparisonObject.ComparisonObjectType;
            _comparisonObjectStatus = comparisonObject.Status;
            _sourceObjectInternalName = comparisonObject.SourceObjectInternalName;
            _targetObjectInternalName = comparisonObject.TargetObjectInternalName;
        }

        /// <summary>
        /// The comparison object type such as Table, Measure, Relationship, etc.
        /// </summary>
        public ComparisonObjectType ComparisonObjectType
        {
            get { return _comparisonObjectType; }
            set { _comparisonObjectType = value; }
        }

        /// <summary>
        /// The comparison object status such as Same Definition, Different Definitions, Missing in Target and Missing in Source.
        /// </summary>
        public ComparisonObjectStatus Status
        {
            get { return _comparisonObjectStatus; }
            set { _comparisonObjectStatus = value; }
        }

        /// <summary>
        /// The source object internal name.
        /// </summary>
        public string SourceObjectInternalName
        {
            get { return _sourceObjectInternalName; }
            set { _sourceObjectInternalName = value; }
        }

        /// <summary>
        /// The target object internal name.
        /// </summary>
        public string TargetObjectInternalName
        {
            get { return _targetObjectInternalName; }
            set { _targetObjectInternalName = value; }
        }

    }
}
