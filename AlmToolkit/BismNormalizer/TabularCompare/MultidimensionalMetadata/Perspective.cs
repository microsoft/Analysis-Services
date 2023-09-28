using System;
using System.Collections.Generic;
using Microsoft.AnalysisServices;
using Amo=Microsoft.AnalysisServices;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Abstraction of a tabular model perspective with properties and methods for comparison purposes.
    /// </summary>
    public class Perspective : ITabularObject
    {
        private TabularModel _parentTabularModel;
        private Microsoft.AnalysisServices.Perspective _amoPerspective;
        private string _objectDefinition;
        private string _substituteId;

        /// <summary>
        /// Initializes a new instance of the Perspective class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the perspective belongs to.</param>
        /// <param name="amoPerspective">Analysis Management Objects Perspective object abtstracted by the Perspective class.</param>
        public Perspective(TabularModel parentTabularModel, Amo.Perspective amoPerspective)
        {
            _parentTabularModel = parentTabularModel;
            _amoPerspective = amoPerspective;
            _objectDefinition = "";

            List<string> perspectiveTableNames = new List<string>();  // put in here to sort
            foreach (PerspectiveDimension perspectiveDimension in _amoPerspective.Dimensions)
            {
                perspectiveTableNames.Add(perspectiveDimension.Dimension.Name);
            }
            // now need to check if there are measures/KPIs where no other table objects are selected in the perspective - because if so won't have a .Dimension value
            foreach (PerspectiveCalculation perspectiveCalculation in _amoPerspective.Calculations)
            {
                string measureName = perspectiveCalculation.Name.Replace("[Measures].[", "").Replace("]", "");
                if (_parentTabularModel.Measures.ContainsName(measureName) &&
                    !perspectiveTableNames.Contains(_parentTabularModel.Measures.FindByName(measureName).TableName))
                {
                    perspectiveTableNames.Add(_parentTabularModel.Measures.FindByName(measureName).TableName);
                }
            }
            foreach (PerspectiveKpi perspectiveKpi in _amoPerspective.Kpis)
            {
                string kpiName = perspectiveKpi.ToString();
                if (_parentTabularModel.Kpis.ContainsName(kpiName) &&
                    !perspectiveTableNames.Contains(_parentTabularModel.Kpis.FindByName(kpiName).TableName))
                {
                    perspectiveTableNames.Add(_parentTabularModel.Kpis.FindByName(kpiName).TableName);
                }
            }
            perspectiveTableNames.Sort();

            foreach (string perspectiveTableName in perspectiveTableNames)
            {
                _objectDefinition += perspectiveTableName + "\n";

                // find the PerspectiveDimension again
                foreach (PerspectiveDimension perspectiveDimension in _amoPerspective.Dimensions)
                {
                    if (perspectiveDimension.Dimension.Name == perspectiveTableName)
                    {
                        //Attributes
                        List<string> perspectiveAttributeNames = new List<string>();  // put in here to sort
                        foreach (PerspectiveAttribute perspectiveAttribute in perspectiveDimension.Attributes)
                        {
                            perspectiveAttributeNames.Add(perspectiveAttribute.Attribute.Name);
                        }
                        perspectiveAttributeNames.Sort();
                        foreach (string perspectiveAttributeName in perspectiveAttributeNames)
                        {
                            _objectDefinition += "   " + perspectiveAttributeName + "\n";
                        }

                        //Hierarchies
                        List<string> perspectiveHierarchyNames = new List<string>();  // put in here to sort
                        foreach (PerspectiveHierarchy perspectiveHierarchy in perspectiveDimension.Hierarchies)
                        {
                            perspectiveHierarchyNames.Add(perspectiveHierarchy.Hierarchy.Name);
                        }
                        perspectiveHierarchyNames.Sort();
                        foreach (string perspectiveHierarchyName in perspectiveHierarchyNames)
                        {
                            _objectDefinition += "   " + perspectiveHierarchyName + "\n";
                        }
                    }
                }

                //Measures
                List<string> perspectiveMeasureNames = new List<string>();  // put in here to sort
                foreach (PerspectiveCalculation perspectiveCalculation in _amoPerspective.Calculations)
                {
                    string measureName = perspectiveCalculation.Name.Replace("[Measures].[", "").Replace("]", "");
                    if (_parentTabularModel.Measures.ContainsName(measureName) &&
                        _parentTabularModel.Measures.FindByName(measureName).TableName == perspectiveTableName)
                    {
                        perspectiveMeasureNames.Add(measureName);
                    }
                }
                perspectiveMeasureNames.Sort();
                foreach (string perspectiveCalculationName in perspectiveMeasureNames)
                {
                    _objectDefinition += "   " + perspectiveCalculationName + "\n";
                }

                //KPIs
                List<string> perspectiveKpiNames = new List<string>();  // put in here to sort
                foreach (PerspectiveKpi perspectiveKpi in _amoPerspective.Kpis)
                {
                    string kpiName = perspectiveKpi.ToString();
                    if (_parentTabularModel.Kpis.ContainsName(kpiName) &&
                        _parentTabularModel.Kpis.FindByName(kpiName).TableName == perspectiveTableName &&
                        !perspectiveMeasureNames.Contains(kpiName))  //last check in case already added as a measure
                    {
                        perspectiveKpiNames.Add(kpiName);
                    }
                }
                perspectiveKpiNames.Sort();
                foreach (string perspectiveCalculationName in perspectiveKpiNames)
                {
                    _objectDefinition += "   " + perspectiveCalculationName + "\n";
                }
            }

            //Actions
            List<string> perspectiveActionNames = new List<string>();  // put in here to sort
            foreach (PerspectiveAction perspectiveAction in _amoPerspective.Actions)
            {
                if (perspectiveAction.ParentCube.Actions.Contains(perspectiveAction.ActionID))  //need this check or .Action returns error
                {
                    string actionName = perspectiveAction.Action.Name;
                    if (_parentTabularModel.Actions.ContainsName(actionName))
                    {
                        perspectiveActionNames.Add(actionName);
                    }
                }
            }
            perspectiveActionNames.Sort();
            foreach (string perspectiveActionName in perspectiveActionNames)
            {
                _objectDefinition += "Action: " + perspectiveActionName + "\n";
            }

            //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations)
            //{
            //    _objectDefinition += "\nFormat & Visibility:\nPerspective Translations: ";
            //    if (_amoPerspective.Translations.Count > 0)
            //    {
            //        _objectDefinition += "[";
            //        foreach (Translation perspectiveTranslation in _amoPerspective.Translations)
            //        {
            //            _objectDefinition += CultureInfo.GetCultureInfo(perspectiveTranslation.Language).DisplayName + ": " + perspectiveTranslation.Caption + ", ";
            //        }
            //        _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
            //    }

            //    if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
            //    {
            //        _objectDefinition += ", Display Folder Translations: ";
            //        if (_amoPerspective.Translations.Count > 0)
            //        {
            //            _objectDefinition += "[";
            //            foreach (Translation perspectiveDisplayFolderTranslation in _amoPerspective.Translations)
            //            {
            //                _objectDefinition += CultureInfo.GetCultureInfo(perspectiveDisplayFolderTranslation.Language).DisplayName + ": " + perspectiveDisplayFolderTranslation.DisplayFolder + ", ";
            //            }
            //            _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
            //        }
            //    }
            //    _objectDefinition += "\n";
            //}

            _objectDefinition += "\n";
        }

        /// <summary>
        /// TabularModel object that the Perspective object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Analysis Management Objects Perspective object abtstracted by the Perspective class.
        /// </summary>
        public Amo.Perspective AmoPerspective => _amoPerspective;

        /// <summary>
        /// Name of the Perspective object.
        /// </summary>
        public string Name => _amoPerspective.Name;

        /// <summary>
        /// Long name of the Perspective object.
        /// </summary>
        public string LongName => _amoPerspective.Name;

        /// <summary>
        /// Id of the Perspective object.
        /// </summary>
        public string Id => _amoPerspective.ID;

        /// <summary>
        /// Object definition of the Perspective object. This is a simplified list of relevant attribute values for comparison; not the XMLA definition of the abstracted AMO object.
        /// </summary>
        public string ObjectDefinition => _objectDefinition;

        /// <summary>
        /// Substitute id of the Perspective object.
        /// </summary>
        public string SubstituteId
        {
            get
            {
                if (string.IsNullOrEmpty(_substituteId))
                {
                    return _amoPerspective.ID;
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

        /// <summary>
        /// Verifies whether another Perspective object's selections are included in this Perspective object.
        /// </summary>
        /// <param name="otherPerspective">The other Perspective object to be verified.</param>
        /// <returns>True if otherPerspective matches. False if does not match.</returns>
        public bool ContainsOtherPerspectiveSelections(Perspective otherPerspective)
        {
            bool everythingMatches = true;

            //Tables
            foreach (PerspectiveDimension otherDimension in otherPerspective.AmoPerspective.Dimensions)
            {
                bool foundDimensionMatch = false;
                foreach (PerspectiveDimension perspectiveDimension in _amoPerspective.Dimensions)
                {
                    if (perspectiveDimension.Dimension.Name == otherDimension.Dimension.Name)
                    {
                        foundDimensionMatch = true;

                        //Columns
                        foreach (PerspectiveAttribute otherAttribute in otherDimension.Attributes)
                        {
                            bool foundAttributeMatch = false;
                            foreach (PerspectiveAttribute perspectiveAttribute in perspectiveDimension.Attributes)
                            {
                                if (perspectiveAttribute.Attribute.Name == otherAttribute.Attribute.Name)
                                {
                                    foundAttributeMatch = true;
                                    break;
                                }
                            }
                            if (!foundAttributeMatch)
                            {
                                everythingMatches = false;
                                break;
                            }
                        }

                        //Hierarchies
                        foreach (PerspectiveHierarchy otherHierarchy in otherDimension.Hierarchies)
                        {
                            bool foundHierarchyMatch = false;
                            foreach (PerspectiveHierarchy perspectiveHierarchy in perspectiveDimension.Hierarchies)
                            {
                                if (perspectiveHierarchy.Hierarchy.Name == otherHierarchy.Hierarchy.Name)
                                {
                                    foundHierarchyMatch = true;
                                    break;
                                }
                            }
                            if (!foundHierarchyMatch)
                            {
                                everythingMatches = false;
                                break;
                            }
                        }

                    }
                    if (!everythingMatches) break;
                }

                if (!foundDimensionMatch)
                {
                    everythingMatches = false;
                    break;
                }
            }

            if (everythingMatches)
            {
                //Measures
                foreach (PerspectiveCalculation otherCalculation in otherPerspective.AmoPerspective.Calculations)
                {
                    string measureName = otherCalculation.Name.Replace("[Measures].[", "").Replace("]", "");
                    if (otherPerspective.ParentTabularModel.Measures.ContainsName(measureName)) // this if clause shouldn't be necessary, but it is
                    {
                        bool foundCalculationMatch = false;
                        foreach (PerspectiveCalculation perspectiveCalculation in _amoPerspective.Calculations)
                        {
                            if (perspectiveCalculation.Name == otherCalculation.Name)
                            {
                                foundCalculationMatch = true;
                                break;
                            }
                        }
                        if (!foundCalculationMatch)
                        {
                            everythingMatches = false;
                            break;
                        }
                    }
                }
            }

            if (everythingMatches)
            {
                //Kpis
                foreach (PerspectiveKpi otherKpi in otherPerspective.AmoPerspective.Kpis)
                {
                    string KpiName = otherKpi.ToString();
                    if (otherPerspective.ParentTabularModel.Kpis.ContainsName(KpiName))
                    {
                        bool foundKpiMatch = false;
                        foreach (PerspectiveKpi perspectiveKpi in _amoPerspective.Kpis)
                        {
                            if (perspectiveKpi.ToString() == otherKpi.ToString())
                            {
                                foundKpiMatch = true;
                                break;
                            }
                        }
                        if (!foundKpiMatch)
                        {
                            everythingMatches = false;
                            break;
                        }
                    }
                }
            }

            if (everythingMatches)
            {
                //Actions
                foreach (PerspectiveAction otherAction in otherPerspective.AmoPerspective.Actions)
                {
                    bool foundActionMatch = false;
                    foreach (PerspectiveAction perspectiveAction in _amoPerspective.Actions)
                    {
                        if (perspectiveAction.ParentCube.Actions.Contains(perspectiveAction.ActionID) && otherAction.ParentCube.Actions.Contains(otherAction.ActionID) &&  //need this check or .Action returns error
                            perspectiveAction.Action.Name == otherAction.Action.Name)
                        {
                            foundActionMatch = true;
                            break;
                        }
                    }
                    if (!foundActionMatch)
                    {
                        everythingMatches = false;
                        break;
                    }
                }
            }

            if (everythingMatches)
            {
                //Translations
                foreach (Translation otherTranslation in otherPerspective.AmoPerspective.Translations)
                {
                    bool foundActionMatch = false;
                    foreach (Translation perspectiveTranslation in _amoPerspective.Translations)
                    {
                        if (perspectiveTranslation.Language == otherTranslation.Language &&
                            perspectiveTranslation.Caption == otherTranslation.Caption)
                        {
                            foundActionMatch = true;
                            break;
                        }
                    }
                    if (!foundActionMatch)
                    {
                        everythingMatches = false;
                        break;
                    }
                }
            }

            return everythingMatches;
        }
    }
}
