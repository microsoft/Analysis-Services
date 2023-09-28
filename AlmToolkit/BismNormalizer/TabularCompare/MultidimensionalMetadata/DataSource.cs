using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.AnalysisServices;
using Amo=Microsoft.AnalysisServices;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Abstraction of a tabular model data source with properties and methods for comparison purposes.
    /// </summary>
    public class DataSource : ITabularObject
    {
        private TabularModel _parentTabularModel;
        private Microsoft.AnalysisServices.DataSource _amoDataSource;
        private string _substituteId;

        /// <summary>
        /// Initializes a new instance of the DataSource class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the DataSource object belongs to.</param>
        /// <param name="datasource">Analysis Management Objects DataSource object abtstracted by the DataSource object.</param>
        public DataSource(TabularModel parentTabularModel, Microsoft.AnalysisServices.DataSource datasource)
        {
            _parentTabularModel = parentTabularModel;
            _amoDataSource = datasource;
        }

        /// <summary>
        /// TabularModel object that the DataSource object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Analysis Management Objects DataSource object abtstracted by the DataSource object.
        /// </summary>
        public Amo.DataSource AmoDataSource => _amoDataSource;

        /// <summary>
        /// Name of the DataSource object.
        /// </summary>
        public string Name => _amoDataSource.Name;

        /// <summary>
        /// Long name of the DataSource object.
        /// </summary>
        public string LongName => _amoDataSource.Name;

        /// <summary>
        /// Id of the DataSource object.
        /// </summary>
        public string Id => _amoDataSource.ID;

        /// <summary>
        /// Object definition of the DataSource object. This is a simplified list of relevant attribute values for comparison; not the XMLA definition of the abstracted AMO object.
        /// </summary>
        public string ObjectDefinition
        {
            get
            {
                //the order of items in the connection string is not guaranteed to come out in a consistent order ...
                string[] elements = _amoDataSource.ConnectionString.Split(';');
                Array.Sort(elements);
                string returnValue = string.Empty;
                foreach (string element in elements)
                {
                    returnValue += element + ";";
                }
                return returnValue.Substring(0, returnValue.Length - 1) + "\n";
            }
        }

        /// <summary>
        /// Substitute Id of the DataSource object.
        /// </summary>
        public string SubstituteId
        {
            get
            {
                if (string.IsNullOrEmpty(_substituteId))
                {
                    return _amoDataSource.ID;
                }
                else
                {
                    return _substituteId;
                }
            }
            set
            {
                _substituteId = value;
            }
        }

        public override string ToString() => this.GetType().FullName;
    }
}
