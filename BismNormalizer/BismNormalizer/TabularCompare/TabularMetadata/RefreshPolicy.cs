//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Globalization;
//using Microsoft.AnalysisServices.Tabular;
//using Tom=Microsoft.AnalysisServices.Tabular;

//namespace BismNormalizer.TabularCompare.TabularMetadata
//{
//    /// <summary>
//    /// Abstraction of a tabular model refreshPolicy with properties and methods for comparison purposes.
//    /// </summary>
//    public class RefreshPolicy : TabularObject
//    {
//        private Table _parentTable;
//        private Tom.RefreshPolicy _tomRefreshPolicy;

//        /// <summary>
//        /// Initializes a new instance of the RefreshPolicy class using multiple parameters.
//        /// </summary>
//        /// <param name="parentTable">Table object that the refreshPolicy belongs to.</param>
//        /// <param name="tomRefreshPolicy">Tabular Object Model RefreshPolicy object abtstracted by the RefreshPolicy class.</param>
//        public RefreshPolicy(Table parentTable, Tom.RefreshPolicy tomRefreshPolicy) : base(tomRefreshPolicy)
//        {
//            _parentTable = parentTable;
//            _tomRefreshPolicy = tomRefreshPolicy;
//        }

//        /// <summary>
//        /// Table object that the Relationship oject belongs to.
//        /// </summary>
//        public Table ParentTable => _parentTable;

//        /// <summary>
//        /// Tabular Object Model RefreshPolicy object abtstracted by the RefreshPolicy class.
//        /// </summary>
//        public Tom.RefreshPolicy TomRefreshPolicy => _tomRefreshPolicy;

//        /// <summary>
//        /// Name of the table that the RefreshPolicy oject belongs to.
//        /// </summary>
//        public string TableName => _tomRefreshPolicy.Table.Name;

//        public override string ToString() => this.GetType().FullName;
//    }
//}
