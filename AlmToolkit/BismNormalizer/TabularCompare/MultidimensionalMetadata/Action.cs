using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using Microsoft.AnalysisServices;
using Amo=Microsoft.AnalysisServices;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    //[Obsolete("This class is obsolete. Left over from BISM Normalizer 2, which supported BIDS Helper actions.")]

    /// <summary>
    /// Abstraction of a tabular model action with properties and methods for comparison purposes.
    /// </summary>
    public class Action : ITabularObject
    {
        private TabularModel _parentTabularModel;
        private Amo.Action _amoAction;
        private string _objectDefinition;
        private string _substituteId;

        public Action(TabularModel parentTabularModel, Amo.Action Action)
        {
            _parentTabularModel = parentTabularModel;
            _amoAction = Action;
            _objectDefinition = "";

            if (_amoAction.Caption != null) _objectDefinition += "Caption: " + _amoAction.Caption + "\n";
            _objectDefinition += "Caption is MDX: " + _amoAction.CaptionIsMdx.ToString() + "\n";
            if (_amoAction.Description != null) _objectDefinition += "Description: " + _amoAction.Description + "\n";
            _objectDefinition += "Action Type: " + _amoAction.Type.ToString() + "\n";
            if (_amoAction.Target != null) _objectDefinition += "Target: " + _amoAction.Target + "\n";
            if (_amoAction.Condition != null) _objectDefinition += "Condition: " + _amoAction.Condition + "\n";
            _objectDefinition += "Invocation: " + _amoAction.Invocation.ToString() + "\n\n";

            switch (_amoAction.Type)
            {
                case ActionType.DrillThrough:
                    if (_amoAction is DrillThroughAction)
                    {
                        DrillThroughAction drillThroughAction = (DrillThroughAction)_amoAction;
                        _objectDefinition += "Drillthrough Columns:\n";
                        foreach (CubeAttributeBinding column in drillThroughAction.Columns)
                        {
                            if (drillThroughAction.Parent.Dimensions.Contains(column.CubeDimensionID) && drillThroughAction.Parent.Dimensions[column.CubeDimensionID].Attributes.Contains(column.AttributeID))
                            {
                                _objectDefinition += "Table: " + drillThroughAction.Parent.Dimensions[column.CubeDimensionID].Name + ", Column: " + drillThroughAction.Parent.Dimensions[column.CubeDimensionID].Attributes[column.AttributeID].Attribute.Name + "\n";
                            }
                        }
                        if (drillThroughAction.Columns.Count > 0) _objectDefinition += "\n";
                        _objectDefinition += "Default: " + drillThroughAction.Default.ToString() + "\n";
                        _objectDefinition += "Maximum Rows: " + drillThroughAction.MaximumRows.ToString() + "\n";
                    }
                    break;
                case ActionType.Report:
                    if (_amoAction is ReportAction)
                    {
                        ReportAction reportAction = (ReportAction)_amoAction;
                        _objectDefinition += "Report Parameters:\n";
                        foreach (ReportParameter reportParameter in reportAction.ReportParameters)
                        {
                            if (reportParameter.Name != null && reportParameter.Value != null)
                            {
                                _objectDefinition += "Name: " + reportParameter.Name + ", Value: " + reportParameter.Value + "\n";
                            }
                        }
                        if (reportAction.ReportParameters.Count > 0 || reportAction.ReportFormatParameters.Count > 0) _objectDefinition += "\n";
                        if (reportAction.ReportServer != null) _objectDefinition += "Report Server: " + reportAction.ReportServer + "\n";
                        if (reportAction.Path != null) _objectDefinition += "Maximum Path: " + reportAction.Path + "\n";
                    }
                    break;
                default:
                    if (_amoAction is StandardAction)
                    {
                        StandardAction standardAction = (StandardAction)_amoAction;
                        if (standardAction.Expression != null) _objectDefinition += "Expression:\n" + standardAction.Expression + "\n";
                    }
                    break;
            }

            //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations)
            //{
            //    _objectDefinition += "\nFormat & Visibility:\n";

            //    _objectDefinition += "Action Translations: ";
            //    if (_amoAction.Translations.Count > 0)
            //    {
            //        _objectDefinition += "[";
            //        foreach (Translation actionTranslation in _amoAction.Translations)
            //        {
            //            _objectDefinition += CultureInfo.GetCultureInfo(actionTranslation.Language).DisplayName + ": " + actionTranslation.Caption + ", ";
            //        }
            //        _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
            //    }

            //    if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
            //    {
            //        _objectDefinition += ", Display Folder Translations: ";
            //        if (_amoAction.Translations.Count > 0)
            //        {
            //            _objectDefinition += "[";
            //            foreach (Translation actionDisplayFolderTranslation in _amoAction.Translations)
            //            {
            //                _objectDefinition += CultureInfo.GetCultureInfo(actionDisplayFolderTranslation.Language).DisplayName + ": " + actionDisplayFolderTranslation.DisplayFolder + ", ";
            //            }
            //            _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
            //        }
            //    }
            //    _objectDefinition += "\n";
            //}
        }

        public TabularModel ParentTabularModel => _parentTabularModel;

        public Amo.Action AmoAction => _amoAction;

        public string Name => _amoAction.Name;

        public string LongName => _amoAction.Name;

        public string Id => _amoAction.ID;

        public string ObjectDefinition => _objectDefinition;

        public string SubstituteId
        {
            get
            {
                if (string.IsNullOrEmpty(_substituteId))
                {
                    return _amoAction.ID;
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
