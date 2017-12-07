using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.AnalysisServices.Tabular;
using Tom=Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model measure with properties and methods for comparison purposes.
    /// </summary>
    public class Measure : TabularObject
    {
        private Table _parentTable;
        private Tom.Measure _tomMeasure;
        private bool _IsKpi;

        /// <summary>
        /// Initializes a new instance of the Measure class using multiple parameters.
        /// </summary>
        /// <param name="parentTable">Table object that the measure belongs to.</param>
        /// <param name="tomMeasure">Tabular Object Model Measure object abtstracted by the Measure class.</param>
        /// <param name="isKpi">Indicates whether the measure is a KPI.</param>
        public Measure(Table parentTable, Tom.Measure tomMeasure, bool isKpi) : base(tomMeasure)
        {
            _parentTable = parentTable;
            _tomMeasure = tomMeasure;
            _IsKpi = isKpi;
        }

        /// <summary>
        /// Table object that the Relationship oject belongs to.
        /// </summary>
        public Table ParentTable => _parentTable;

        /// <summary>
        /// Tabular Object Model Measure object abtstracted by the Measure class.
        /// </summary>
        public Tom.Measure TomMeasure => _tomMeasure;

        /// <summary>
        /// Name of the table that the Measure oject belongs to.
        /// </summary>
        public string TableName => _tomMeasure.Table.Name;

        /// <summary>
        /// Boolean indicating if the Measure object is a KPI.
        /// </summary>
        public bool IsKpi
        {
            get { return _IsKpi; }
            set { _IsKpi = value; }
        }

        public override string ToString() => this.GetType().FullName;

        /// <summary>
        /// Find missing calculation dependencies by inspecting the DAX expression for the measure and iterating columns and other measures in the tabular model for validity of the expression.
        /// </summary>
        /// <returns>List of missing dependencies to be displayed or logged as warnings.</returns>
        public List<string> FindMissingMeasureDependencies()
        {
            List<string> dependencies = new List<string>();

            using (StringReader lines = new StringReader(_tomMeasure.Expression))
            {
                string line = string.Empty;
                while ((line = lines.ReadLine()) != null)
                {
                    if (line.TrimStart().Length > 1 && line.TrimStart().Substring(0, 2) != "--")  //Ignore comments
                    {
                        //Todo2: still need to parse for /* blah */ type comments.  Currently can show missing dependency that doesn't apply if within a comment

                        string whatsRemainingOfLine = line;

                        while (whatsRemainingOfLine.Contains('[') && whatsRemainingOfLine.Contains(']'))
                        {
                            int openSquareBracketPosition = whatsRemainingOfLine.IndexOf('[', 0);
                            //brilliant person at microsoft has ]] instead of ]
                            int closeSquareBracketPosition = whatsRemainingOfLine.Replace("]]", "  ").IndexOf(']', openSquareBracketPosition + 1);

                            if (openSquareBracketPosition < closeSquareBracketPosition - 1)
                            {
                                string potentialDependency = whatsRemainingOfLine.Substring(openSquareBracketPosition + 1, closeSquareBracketPosition - openSquareBracketPosition - 1);
                                if (!potentialDependency.Contains('"') &&
                                    !_tomMeasure.Expression.Contains($"\"{potentialDependency}\"") && //it's possible the measure itself is deriving the column name from an ADDCOLUMNS for example
                                    !dependencies.Contains(potentialDependency))
                                {
                                    //unbelievable: some genius at m$ did a replace on ] with ]]
                                    dependencies.Add(potentialDependency);
                                }
                            }

                            whatsRemainingOfLine = whatsRemainingOfLine.Substring(closeSquareBracketPosition + 1);
                        }
                    }
                }
            }

            List<string> missingDependencies = new List<string>();
            foreach (string dependency in dependencies)
            {
                bool foundDependency = false;

                foreach (Table table in _parentTable.ParentTabularModel.Tables)
                {
                    //Check if another measure or column has same name
                    if (table.Measures.ContainsNameCaseInsensitive(dependency) || table.ColumnsContainsNameCaseInsensitive(dependency))
                    {
                        foundDependency = true;
                        break;
                    }
                }

                if (!foundDependency)
                {
                    missingDependencies.Add(dependency);
                }
            }

            return missingDependencies;
        }
    }
}
