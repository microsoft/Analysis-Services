using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.AnalysisServices;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Abstraction of a tabular model measure with properties and methods for comparison purposes.
    /// </summary>
    public class Measure : ITabularObject
    {
        private TabularModel _parentTabularModel;
        private string _tableName;
        private string _name;
        private string _expression;
        private string _objectDefinition;
        private bool _IsKpiReferenceMeasure;

        /// <summary>
        /// Initializes a new instance of the Measure class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the measure belongs to.</param>
        /// <param name="tableName">Name of the table that the Measure belongs to.</param>
        /// <param name="measureName">Name of the measure.</param>
        /// <param name="expression">DAX expression</param>
        public Measure(TabularModel parentTabularModel, string tableName, string measureName, string expression)
        {
            _parentTabularModel = parentTabularModel;
            _tableName = tableName;
            _name = measureName;
            _expression = expression;
            _IsKpiReferenceMeasure = false;

            PopulateObjectDefinition();
        }

        private void PopulateObjectDefinition()
        {
            _objectDefinition = "Expression:\n" + _expression + "\n\n";

            if (this.AmoCalculationProperty != null)
            {
                if (this.AmoCalculationProperty.Annotations.Contains("Format") && this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Format"] != null)
                {
                    _objectDefinition += "Format & Visibility:\n";
                    switch (this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Format"].Value)
                    {
                        case "General":
                            _objectDefinition += "Format: General";
                            break;
                        case "NumberDecimal":
                            _objectDefinition += "Format: Decimal Number" +
                                                 (this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Accuracy"] != null ? ", Decimal Places: " + this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Accuracy"].Value : "") +
                                                 (this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["ThousandSeparator"] != null ? ", Show Thousand Separator: " + this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["ThousandSeparator"].Value : "");
                            break;
                        case "NumberWhole":
                            _objectDefinition += "Format: Whole Number" +
                                                 (this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["ThousandSeparator"] != null ? ", Show Thousand Separator: " + this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["ThousandSeparator"].Value : "");
                            break;
                        case "Percentage":
                            _objectDefinition += "Format: Percentage" +
                                                 (this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Accuracy"] != null ? ", Decimal Places: " + this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Accuracy"].Value : "") +
                                                 (this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["ThousandSeparator"] != null ? ", Show Thousand Separator: " + this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["ThousandSeparator"].Value : "");
                            break;
                        case "Scientific":
                            _objectDefinition += "Format: Scientific" +
                                                 (this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Accuracy"] != null ? ", Decimal Places: " + this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Accuracy"].Value : "");
                            break;
                        case "Currency":
                            _objectDefinition += "Format: Currency" +
                                                 (this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Accuracy"] != null ? ", Decimal Places: " + this.AmoCalculationProperty.Annotations["Format"].Value.Attributes["Accuracy"].Value : "") +
                                                 (this.AmoCalculationProperty.Annotations["Format"].Value.HasChildNodes &&
                                                   this.AmoCalculationProperty.Annotations["Format"].Value.ChildNodes[0].Attributes["DisplayName"] != null
                                                   ? ", Currency Symbol: " + this.AmoCalculationProperty.Annotations["Format"].Value.ChildNodes[0].Attributes["DisplayName"].Value : "");
                            break;
                        case "DateTimeCustom":
                            _objectDefinition += "Format: Date" +
                                                 (this.AmoCalculationProperty.Annotations["Format"].Value.HasChildNodes &&
                                                   this.AmoCalculationProperty.Annotations["Format"].Value.ChildNodes[0].HasChildNodes &&
                                                   this.AmoCalculationProperty.Annotations["Format"].Value.ChildNodes[0].ChildNodes[0].Attributes["FormatString"] != null
                                                   ? ", Date Format: " + this.AmoCalculationProperty.Annotations["Format"].Value.ChildNodes[0].ChildNodes[0].Attributes["FormatString"].Value : "");
                            break;
                        case "Boolean":
                            _objectDefinition += "Format: TRUE/FALSE";
                            break;
                        default:
                            break;
                    }
                    if (this.AmoCalculationProperty.Annotations.Contains("IsPrivate") && this.AmoCalculationProperty.Annotations["IsPrivate"].Value != null)
                    {
                        _objectDefinition += ", Hidden: " + this.AmoCalculationProperty.Annotations["IsPrivate"].Value.Value;
                    }
                    //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
                    //{
                    //    _objectDefinition += ", Display Folder: " + (this.AmoCalculationProperty.DisplayFolder == null ? "" : this.AmoCalculationProperty.DisplayFolder);
                    //}
                    //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations)
                    //{
                    //    _objectDefinition += ", Measure Translations: ";
                    //    if (this.AmoCalculationProperty.Translations.Count > 0)
                    //    {
                    //        _objectDefinition += "[";
                    //        foreach (Translation measureTranslation in this.AmoCalculationProperty.Translations)
                    //        {
                    //            _objectDefinition += CultureInfo.GetCultureInfo(measureTranslation.Language).DisplayName + ": " + measureTranslation.Caption + ", ";
                    //        }
                    //        _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
                    //    }

                    //    if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
                    //    {
                    //        _objectDefinition += ", Display Folder Translations: ";
                    //        if (this.AmoCalculationProperty.Translations.Count > 0)
                    //        {
                    //            _objectDefinition += "[";
                    //            foreach (Translation measureDisplayFolderTranslation in this.AmoCalculationProperty.Translations)
                    //            {
                    //                _objectDefinition += CultureInfo.GetCultureInfo(measureDisplayFolderTranslation.Language).DisplayName + ": " + measureDisplayFolderTranslation.DisplayFolder + ", ";
                    //            }
                    //            _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
                    //        }
                    //    }
                    //}
                    _objectDefinition += "\n\n";
                }
            }
        }

        /// <summary>
        /// TabularModel object that the Measure object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Name of the table that the Measure oject belongs to.
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; } 
        }

        /// <summary>
        /// Name of the Measure object.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Long name of the Measure object.
        /// </summary>
        public string LongName => _name;

        /// <summary>
        /// Id of the Measure object.
        /// </summary>
        public string Id => $"'{_tableName}'[{_name}]";

        /// <summary>
        /// Substitute Id of the Measure object.
        /// </summary>
        public string SubstituteId => this.Id;

        /// <summary>
        /// DAX expression of the Measure object.
        /// </summary>
        public string Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        /// <summary>
        /// Boolean indicating if the Measure object is a KPI reference measure.
        /// </summary>
        public bool IsKpiReferenceMeasure
        {
            get { return _IsKpiReferenceMeasure; }
            set { _IsKpiReferenceMeasure = value; }
        }

        /// <summary>
        /// Object definition of the Measure object. This is a simplified list of relevant attribute values for comparison; not the XMLA definition of the abstracted AMO object.
        /// </summary>
        public virtual string ObjectDefinition => _objectDefinition;

        /// <summary>
        /// Calculation reference of the Measure object.
        /// </summary>
        public string CalculationReference => $"[{_name}]";

        /// <summary>
        /// Analysis Management Objects CalculationProperty object for the Measure object.
        /// </summary>
        public CalculationProperty AmoCalculationProperty
        {
            get
            {
                if (_parentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Contains(this.CalculationReference))
                {
                    return _parentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties[this.CalculationReference];
                }
                else
                {
                    return null;
                }
            }
        }

        public override string ToString() => this.GetType().FullName;

        /// <summary>
        /// Find missing dependencies.
        /// </summary>
        /// <returns>List of dependencies.</returns>
        public List<string> FindMissingCalculationDependencies()
        {
            List<string> dependencies = new List<string>();

            using (StringReader lines = new StringReader(_expression))
            {
                string line = string.Empty;
                while ((line = lines.ReadLine()) != null)
                {
                    string whatsLeftOfLine = line;

                    while (whatsLeftOfLine.Contains('[') && whatsLeftOfLine.Contains(']'))
                    {
                        int openSquareBracketPosition = whatsLeftOfLine.IndexOf('[', 0);
                        //brilliant person at microsoft has ]] instead of ]
                        int closeSquareBracketPosition = whatsLeftOfLine.Replace("]]", "  ").IndexOf(']', openSquareBracketPosition + 1);

                        if (openSquareBracketPosition < closeSquareBracketPosition - 1)
                        {
                            string potentialDependency = whatsLeftOfLine.Substring(openSquareBracketPosition + 1, closeSquareBracketPosition - openSquareBracketPosition - 1);
                            if (!potentialDependency.Contains('"') && !dependencies.Contains(potentialDependency))
                            {
                                //unbelievable: some genius at m$ did a replace on ] with ]]
                                dependencies.Add(potentialDependency);
                            }
                        }

                        whatsLeftOfLine = whatsLeftOfLine.Substring(closeSquareBracketPosition + 1);
                    }
                }
            }

            List<string> missingDependencies = new List<string>();
            foreach (string dependency in dependencies)
            {
                bool foundDependency = false;

                //need to check internal measures because of references to KPI goals in measures
                //// check if it is another measure
                //foreach (Measure measure in _parentTabularModel.Measures)
                //{
                //    if (measure.Name == dependency)
                //    {
                //        foundDependency = true;
                //        break;
                //    }
                //}
                foreach (Measure measure in _parentTabularModel.MeasuresFull)
                {
                    if (measure.Name == dependency)
                    {
                        foundDependency = true;
                        break;
                    }
                }

                if (!foundDependency)
                {
                    // check if it is a kpi
                    foreach (Measure kpi in _parentTabularModel.Kpis)
                    {
                        if (kpi.Name == dependency)
                        {
                            foundDependency = true;
                            break;
                        }
                    }
                }

                if (!foundDependency)
                {
                    // check if it is a column
                    foreach (Table table in _parentTabularModel.Tables)
                    {
                        foreach (DimensionAttribute column in table.AmoDimension.Attributes)
                        {
                            if (column.Name == dependency)
                            {
                                foundDependency = true;
                                break;
                            }
                        }
                        if (foundDependency)
                        {
                            break;
                        }
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
