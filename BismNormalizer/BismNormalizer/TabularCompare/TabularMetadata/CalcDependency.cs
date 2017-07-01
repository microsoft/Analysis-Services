using System;
using System.Collections.Generic;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Dependency between partitions, M expressions and data sources
    /// </summary>
    public class CalcDependency
    {
        private TabularModel _parentTabularModel;
        private CalcDependencyObjectType _objectType;
        private string _tableName;
        private string _objectName;
        private string _expression;
        private CalcDependencyObjectType _referencedObjectType;
        private string _referencedTableName;
        private string _referencedObjectName;
        private string _referencedExpression;

        /// <summary>
        /// Initializes a new instance of an CalcDependency class using multiple parameters    .
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the CalcDependency object belongs to.</param>
        public CalcDependency(TabularModel parentTabularModel, string objectType, string tableName, string objectName, string expression, string referencedObjectType, string referencedTableName, string referencedObjectName, string referencedExpression)
        {
            _parentTabularModel = parentTabularModel;
            switch (objectType)
            {
                case "PARTITION":
                    _objectType = CalcDependencyObjectType.Partition;
                    break;
                case "M_EXPRESSION":
                    _objectType = CalcDependencyObjectType.Expression;
                    break;
                default:
                    break;
            }
            _tableName = tableName;
            _objectName = objectName;
            _expression = expression;
            switch (referencedObjectType)
            {
                case "PARTITION":
                    _referencedObjectType = CalcDependencyObjectType.Partition;
                    break;
                case "M_EXPRESSION":
                    _referencedObjectType = CalcDependencyObjectType.Expression;
                    break;
                case "DATA_SOURCE":
                    _referencedObjectType = CalcDependencyObjectType.DataSource;
                    break;
                default:
                    break;
            }
            _referencedTableName = referencedTableName;
            _referencedObjectName = referencedObjectName;
            _referencedExpression = referencedExpression;
        }

        /// <summary>
        /// The object type of the dependency.
        /// </summary>
        public CalcDependencyObjectType ObjectType => _objectType;
        
        /// <summary>
        /// The table name of the dependency.
        /// </summary>
        public string TableName => _tableName;
        
        /// <summary>
        /// The object name of the dependency.
        /// </summary>
        public string ObjectName => _objectName;

        /// <summary>
        /// The expression of the dependency.
        /// </summary>
        public string Expression => _expression;
        
        /// <summary>
        /// The referenced object type of the dependency.
        /// </summary>
        public CalcDependencyObjectType ReferencedObjectType => _referencedObjectType;

        /// <summary>
        /// The referenced object name of the dependency.
        /// </summary>
        public string ReferencedTableName => _referencedTableName;

        /// <summary>
        /// The referenced object name of the dependency.
        /// </summary>
        public string ReferencedObjectName => _referencedObjectName;

        /// <summary>
        /// The referenced expression of the dependency.
        /// </summary>
        public string ReferencedExpression => _referencedExpression;

        public override string ToString() => this.GetType().FullName;
    }
}
