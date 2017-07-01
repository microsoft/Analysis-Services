using System;
using System.Collections.Generic;
using Microsoft.AnalysisServices.Tabular;
using Tom=Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model DataSource with properties and methods for comparison purposes.
    /// </summary>
    public class DataSource : TabularObject
    {
        private TabularModel _parentTabularModel;
        private Microsoft.AnalysisServices.Tabular.DataSource _tomDataSource;

        /// <summary>
        /// Initializes a new instance of the DataSource class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the DataSource object belongs to.</param>
        /// <param name="datasource">Tabular Object Model ProviderDataSource object abtstracted by the DataSource class.</param>
        public DataSource(TabularModel parentTabularModel, Microsoft.AnalysisServices.Tabular.DataSource dataSource) : base(dataSource)
        {
            _parentTabularModel = parentTabularModel;
            _tomDataSource = dataSource;
        }

        /// <summary>
        /// TabularModel object that the DataSource object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Tabular Object Model ProviderDataSource object abtstracted by the DataSource class.
        /// </summary>
        public Tom.DataSource TomDataSource => _tomDataSource;

        public override string ToString() => this.GetType().FullName;
    }
}
