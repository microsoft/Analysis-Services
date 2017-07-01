using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Abstraction of a tabular model KPI with properties and methods for comparison purposes.
    /// </summary>
    public class Kpi : Measure
    {
        private Measure _goalMeasure;
        private Measure _statusMeasure;
        private Measure _trendMeasure;
        private string _statusGraphic;
        private string _trendGraphic;

        /// <summary>
        /// Initializes a new instance of the Kpi class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel"></param>
        /// <param name="tableName"></param>
        /// <param name="measureName"></param>
        /// <param name="expression"></param>
        /// <param name="goalMeasure"></param>
        /// <param name="statusMeasure"></param>
        /// <param name="trendMeasure"></param>
        /// <param name="statusGraphic"></param>
        /// <param name="trendGraphic"></param>
        public Kpi(TabularModel parentTabularModel, string tableName, string measureName, string expression,
            Measure goalMeasure, Measure statusMeasure, Measure trendMeasure, string statusGraphic, string trendGraphic) //, Kpi kpi)
            : base(parentTabularModel, tableName, measureName, expression)
        {
            _goalMeasure = goalMeasure;
            _statusMeasure = statusMeasure;
            _trendMeasure = trendMeasure;
            _statusGraphic = statusGraphic;
            _trendGraphic = trendGraphic;
            //_amoKpi = kpi;
        }

        /// <summary>
        /// Goal measure of the Kpi object.
        /// </summary>
        public Measure GoalMeasure
        {
            get { return _goalMeasure; }
            set { _goalMeasure = value; }
        }

        /// <summary>
        /// Goal calculation reference of the Kpi object.
        /// </summary>
        public string GoalCalculationReference => "[" + _goalMeasure.Name + "]";

        /// <summary>
        /// Analysis Management Objects CalculationProperty object for goal abtstracted by the Kpi object.
        /// </summary>
        public CalculationProperty AmoGoalCalculationProperty
        {
            get
            {
                if (this.ParentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Contains(this.GoalCalculationReference))
                {
                    return this.ParentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties[this.GoalCalculationReference];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Status measure of the Kpi object.
        /// </summary>
        public Measure StatusMeasure
        {
            get { return _statusMeasure; }
            set { _statusMeasure = value; }
        }

        /// <summary>
        /// Status calculation reference of the Kpi object.
        /// </summary>
        public string StatusCalculationReference => $"[{_statusMeasure.Name}]";

        /// <summary>
        /// Analysis Management Objects CalculationProperty object for status abtstracted by the Kpi object.
        /// </summary>
        public CalculationProperty AmoStatusCalculationProperty
        {
            get
            {
                if (this.ParentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Contains(this.StatusCalculationReference))
                {
                    return this.ParentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties[this.StatusCalculationReference];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Trend measure of the Kpi object.
        /// </summary>
        public Measure TrendMeasure
        {
            get { return _trendMeasure; }
            set { _trendMeasure = value; }
        }

        /// <summary>
        /// Trend calculation reference of the Kpi object.
        /// </summary>
        public string TrendCalculationReference => $"[{_trendMeasure.Name}]";

        /// <summary>
        /// Analysis Management Objects CalculationProperty object for trend abtstracted by the Kpi object.
        /// </summary>
        public CalculationProperty AmoTrendCalculationProperty
        {
            get
            {
                if (this.ParentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Contains(this.TrendCalculationReference))
                {
                    return this.ParentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties[this.TrendCalculationReference];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Status graphic of the Kpi object.
        /// </summary>
        public string StatusGraphic
        {
            get { return _statusGraphic; }
            set { _statusGraphic = value; }
        }

        /// <summary>
        /// Trend graphic of the Kpi object.
        /// </summary>
        public string TrendGraphic
        {
            get { return _trendGraphic; }
            set { _trendGraphic = value; }
        }

        /// <summary>
        /// Calculation reference of the Kpi object.
        /// </summary>
        public string KpiCalculationReference => $"KPIs.[{this.Name}]";

        /// <summary>
        /// Analysis Management Objects CalculationProperty object for the Kpi object.
        /// </summary>
        public CalculationProperty AmoKpiCalculationProperty
        {
            get
            {
                if (this.ParentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Contains(this.KpiCalculationReference))
                {
                    return this.ParentTabularModel.AmoDatabase.Cubes[0].MdxScripts[0].CalculationProperties[this.KpiCalculationReference];
                }
                else
                {
                    //an old version of Tabular Editor didn't use KPI declarations in the MDX script.  Instead it used the AMO object model.
                    //If the KPI happens to be an AMO object model one, will have an issue, so quickly create a calc ref
                    /*
                        <CalculationProperty>
                          <Annotations>
                            <Annotation>
                              <Name>Type</Name>
                              <Value>SupportKpi</Value>
                            </Annotation>
                            <Annotation>
                              <Name>MainObjectType</Name>
                              <Value>Measure</Value>
                            </Annotation>
                            <Annotation>
                              <Name>MainObjectName</Name>
                              <Value>Total Inventory Value Performance</Value>
                            </Annotation>
                          </Annotations>
                          <CalculationReference>KPIs.[Total Inventory Value Performance]</CalculationReference>
                          <CalculationType>Member</CalculationType>
                        </CalculationProperty>
                     */

                    CalculationProperty amoKpiCalculationProperty = new CalculationProperty(this.KpiCalculationReference, CalculationType.Member);
                    amoKpiCalculationProperty.Annotations.Add("Type", "SupportKpi");
                    amoKpiCalculationProperty.Annotations.Add("MainObjectType", "Measure");
                    amoKpiCalculationProperty.Annotations.Add("MainObjectName", this.Name);
                    return amoKpiCalculationProperty;
                }
            }
        }

        /// <summary>
        /// Object definition of the Kpi object. This is a simplified list of relevant attribute values for comparison; not the XMLA definition of the abstracted AMO object.
        /// </summary>
        public override string ObjectDefinition => base.ObjectDefinition +
           "Goal:\n" + _goalMeasure.Expression + "\n\n" +
           "Status:\n" + _statusMeasure.Expression + "\n\n" +
           //"Trend:\n" + _trendMeasure.Expression + "\n\n" +
           "Status Graphic:\n" + _statusGraphic + "\n"; //\n" +
    }
}
