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
    /// Abstraction of a tabular model calculationItem with properties and methods for comparison purposes.
    /// </summary>
    public class CalculationItem : TabularObject
    {
        private Table _parentTable;
        private Tom.CalculationItem _tomCalculationItem;

        /// <summary>
        /// Initializes a new instance of the CalculationItem class using multiple parameters.
        /// </summary>
        /// <param name="parentTable">Table object that the calculationItem belongs to.</param>
        /// <param name="tomCalculationItem">Tabular Object Model CalculationItem object abtstracted by the CalculationItem class.</param>
        /// <param name="isKpi">Indicates whether the calculationItem is a KPI.</param>
        public CalculationItem(Table parentTable, Tom.CalculationItem tomCalculationItem) : base(tomCalculationItem)
        {
            _parentTable = parentTable;
            _tomCalculationItem = tomCalculationItem;
        }

        /// <summary>
        /// Table object that the Relationship oject belongs to.
        /// </summary>
        public Table ParentTable => _parentTable;

        /// <summary>
        /// Tabular Object Model CalculationItem object abtstracted by the CalculationItem class.
        /// </summary>
        public Tom.CalculationItem TomCalculationItem => _tomCalculationItem;

        /// <summary>
        /// Name of the table that the CalculationItem oject belongs to.
        /// </summary>
        public string TableName => _tomCalculationItem.CalculationGroup.Table.Name;

        public override string ToString() => this.GetType().FullName;

        /// <summary>
        /// Find missing calculation dependencies by inspecting the DAX expression for the calculationItem and iterating columns and other calculationItems in the tabular model for validity of the expression.
        /// </summary>
        /// <returns>List of missing dependencies to be displayed or logged as warnings.</returns>
        public List<string> FindMissingCalculationItemDependencies()
        {
            List<string> dependencies = new List<string>();

            using (StringReader lines = new StringReader(_tomCalculationItem.Expression))
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
                                    !_tomCalculationItem.Expression.Contains($"\"{potentialDependency}\"") && //it's possible the calculationItem itself is deriving the column name from an ADDCOLUMNS for example
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
                    //Check if another calculationItem or column has same name
                    if (table.CalculationItems.ContainsNameCaseInsensitive(dependency) || table.ColumnsContainsNameCaseInsensitive(dependency))
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
