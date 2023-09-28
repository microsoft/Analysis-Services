using System;
using System.Collections.Generic;
using Microsoft.AnalysisServices;
using Excel = Microsoft.Office.Interop.Excel;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Represents a source vs. target comparison of an SSAS tabular model. This class is for tabular models that use multidimensional metadata with SSAS compatibility level 1100 or 1103.
    /// </summary>
    public class Comparison : Core.Comparison
    {
        #region Private Variables

        private TabularModel _sourceTabularModel;
        private TabularModel _targetTabularModel;
        private bool _uncommitedChanges = false;
        private DateTime _lastSourceSchemaUpdate = DateTime.MinValue;
        private DateTime _lastTargetSchemaUpdate = DateTime.MinValue;
        private bool _disposed = false;

        #endregion

        #region Properties

        /// <summary>
        /// TabularModel object being used as the source for comparison.
        /// </summary>
        public TabularModel SourceTabularModel
        {
            get { return _sourceTabularModel; }
            set { _sourceTabularModel = value; }
        }

        /// <summary>
        /// TabularModel object being used as the target for comparison.
        /// </summary>
        public TabularModel TargetTabularModel
        {
            get { return _targetTabularModel; }
            set { _targetTabularModel = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Comparison class using a ComparisonInfo object.
        /// </summary>
        /// <param name="comparisonInfo">ComparisonInfo object typically deserialized from a BSMN file.</param>
        public Comparison(ComparisonInfo comparisonInfo)
            : base(comparisonInfo)
        {
            _sourceTabularModel = new TabularModel(this, comparisonInfo.ConnectionInfoSource, comparisonInfo);
            _targetTabularModel = new TabularModel(this, comparisonInfo.ConnectionInfoTarget, comparisonInfo);
        }

        #endregion

        /// <summary>
        /// Connect to source and target tabular models, and instantiate their properties.
        /// </summary>
        public override void Connect()
        {
            _sourceTabularModel.Connect();
            _targetTabularModel.Connect();
        }

        /// <summary>
        /// Disconnect from source and target tabular models.
        /// </summary>
        public override void Disconnect()
        {
            _sourceTabularModel.Disconnect();
            _targetTabularModel.Disconnect();
        }

        public override void CompareTabularModels()
        {
            _comparisonObjectCount = 0;

            #region Data Sources

            foreach (DataSource dataSourceSource in _sourceTabularModel.DataSources)
            {
                // check if source is not in target
                if (!_targetTabularModel.DataSources.ContainsName(dataSourceSource.Name))
                {
                    ComparisonObject comparisonObjectDataSource = new ComparisonObject(ComparisonObjectType.DataSource, ComparisonObjectStatus.MissingInTarget, dataSourceSource, dataSourceSource.Name, dataSourceSource.Id, dataSourceSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                    _comparisonObjects.Add(comparisonObjectDataSource);
                    _comparisonObjectCount += 1;

                    #region Tables for DataSource that is Missing in Target

                    foreach (Table tblSource in _sourceTabularModel.Tables.FilterByDataSourceId(dataSourceSource.Id))
                    {
                        ComparisonObject comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.MissingInTarget, tblSource, tblSource.Name, tblSource.Id, tblSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                        comparisonObjectDataSource.ChildComparisonObjects.Add(comparisonObjectTable);
                        _comparisonObjectCount += 1;

                        #region Relationships for Table that is Missing in Target

                        foreach (Relationship relSource in tblSource.Relationships)
                        {
                            ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInTarget, relSource, "        " + relSource.Name, relSource.Id, relSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                            _comparisonObjectCount += 1;
                        }

                        #endregion

                        #region Measures for Table that is Missing in Target

                        foreach (Measure measureSource in _sourceTabularModel.Measures.FilterByTableName(tblSource.Name))
                        {
                            ComparisonObject comparisonObjectMeasure = new ComparisonObject(ComparisonObjectType.Measure, ComparisonObjectStatus.MissingInTarget, measureSource, "        " + measureSource.Name, measureSource.Id, measureSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                            _comparisonObjectCount += 1;
                        }

                        #endregion

                        #region KPIs for Table that is Missing in Target

                        foreach (Kpi kpiSource in _sourceTabularModel.Kpis.FilterByTableName(tblSource.Name))
                        {
                            ComparisonObject comparisonObjectKpi = new ComparisonObject(ComparisonObjectType.Kpi, ComparisonObjectStatus.MissingInTarget, kpiSource, "        " + kpiSource.Name, kpiSource.Id, kpiSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectKpi);
                            _comparisonObjectCount += 1;
                        }

                        #endregion
                    }

                    #endregion
                }
                else
                {
                    // there is a datasource in the target with the same name at least
                    DataSource dataSourceTarget = _targetTabularModel.DataSources.FindByName(dataSourceSource.Name);
                    if (dataSourceSource.Id != dataSourceTarget.Id)
                    {
                        dataSourceSource.SubstituteId = dataSourceTarget.Id;
                    }
                    ComparisonObject comparisonObjectDataSource;
                    
                    // check if datasource object definition is different
                    if (dataSourceSource.ObjectDefinition != dataSourceTarget.ObjectDefinition)
                    {
                        comparisonObjectDataSource = new ComparisonObject(ComparisonObjectType.DataSource, ComparisonObjectStatus.DifferentDefinitions, dataSourceSource, dataSourceSource.Name, dataSourceSource.Id, dataSourceSource.ObjectDefinition, MergeAction.Update, dataSourceTarget, dataSourceTarget.Name, dataSourceTarget.Id, dataSourceTarget.ObjectDefinition);
                        _comparisonObjects.Add(comparisonObjectDataSource);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        // they are equal, ...
                        comparisonObjectDataSource = new ComparisonObject(ComparisonObjectType.DataSource, ComparisonObjectStatus.SameDefinition, dataSourceSource, dataSourceSource.Name, dataSourceSource.Id, dataSourceSource.ObjectDefinition, MergeAction.Skip, dataSourceTarget, dataSourceTarget.Name, dataSourceTarget.Id, dataSourceTarget.ObjectDefinition);
                        _comparisonObjects.Add(comparisonObjectDataSource);
                        _comparisonObjectCount += 1;
                    }

                    #region Tables where source/target datasources exist

                    foreach (Table tblSource in _sourceTabularModel.Tables.FilterByDataSourceId(dataSourceSource.Id))
                    {
                        // check if source is not in target
                        if (!_targetTabularModel.Tables.FilterByDataSourceId(dataSourceTarget.Id).ContainsName(tblSource.Name))
                        {
                            ComparisonObject comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.MissingInTarget, tblSource, tblSource.Name, tblSource.Id, tblSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                            comparisonObjectDataSource.ChildComparisonObjects.Add(comparisonObjectTable);
                            _comparisonObjectCount += 1;

                            #region Relationships for table Missing in Target

                            // all relationships in source are not in target (the target table doesn't even exist)
                            foreach (Relationship relSource in tblSource.Relationships)
                            {
                                ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInTarget, relSource, "        " + relSource.Name, relSource.Id, relSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                                _comparisonObjectCount += 1;
                            }

                            #endregion

                            #region Measures for Table that is Missing in Target

                            foreach (Measure measureSource in _sourceTabularModel.Measures.FilterByTableName(tblSource.Name))
                            {
                                ComparisonObject comparisonObjectMeasure = new ComparisonObject(ComparisonObjectType.Measure, ComparisonObjectStatus.MissingInTarget, measureSource, "        " + measureSource.Name, measureSource.Id, measureSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                                _comparisonObjectCount += 1;
                            }

                            #endregion

                            #region Kpis for Table that is Missing in Target

                            foreach (Kpi kpiSource in _sourceTabularModel.Kpis.FilterByTableName(tblSource.Name))
                            {
                                ComparisonObject comparisonObjectKpi = new ComparisonObject(ComparisonObjectType.Kpi, ComparisonObjectStatus.MissingInTarget, kpiSource, "        " + kpiSource.Name, kpiSource.Id, kpiSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectKpi);
                                _comparisonObjectCount += 1;
                            }

                            #endregion
                        }
                        else
                        {
                            //table name is in source and target

                            Table tblTarget = _targetTabularModel.Tables.FindByName(tblSource.Name);
                            if (tblSource.Id != tblTarget.Id)
                            {
                                tblSource.SubstituteId = tblTarget.Id;
                            }
                            ComparisonObject comparisonObjectTable;

                            if (tblSource.ObjectDefinition == tblTarget.ObjectDefinition)
                            {
                                comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.SameDefinition, tblSource, tblSource.Name, tblSource.Id, tblSource.ObjectDefinition, MergeAction.Skip, tblTarget, tblTarget.Name, tblTarget.Id, tblTarget.ObjectDefinition);
                                comparisonObjectDataSource.ChildComparisonObjects.Add(comparisonObjectTable);
                                _comparisonObjectCount += 1;
                            }
                            else
                            {
                                comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.DifferentDefinitions, tblSource, tblSource.Name, tblSource.Id, tblSource.ObjectDefinition, MergeAction.Update, tblTarget, tblTarget.Name, tblTarget.Id, tblTarget.ObjectDefinition);
                                comparisonObjectDataSource.ChildComparisonObjects.Add(comparisonObjectTable);
                                _comparisonObjectCount += 1;
                            }

                            #region Relationships (table in source and target)

                            // see if matching relationhip in source and target
                            foreach (Relationship relSource in tblSource.Relationships)
                            {
                                bool foundMatch = false;
                                foreach (Relationship relTarget in tblTarget.Relationships)
                                {
                                    if (relSource.ObjectDefinition == relTarget.ObjectDefinition)
                                    {
                                        ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.SameDefinition, relSource, "        " + relSource.Name, relSource.Id, relSource.ObjectDefinition, MergeAction.Skip, relTarget, "        " + relTarget.Name, relTarget.Id, relTarget.ObjectDefinition);
                                        comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                                        _comparisonObjectCount += 1;
                                        foundMatch = true;
                                        break;
                                    }
                                }
                                //the relationship in the source table doesnt' exist in the target table
                                if (!foundMatch)
                                {
                                    ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInTarget, relSource, "        " + relSource.Name, relSource.Id, relSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                                    comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                                    _comparisonObjectCount += 1;
                                }
                            }

                            // see if relationships in target table that don't exist in source table
                            foreach (Relationship relTarget in tblTarget.Relationships)
                            {
                                bool foundMatch = false;
                                foreach (Relationship relSource in tblSource.Relationships)
                                {
                                    if (relSource.ObjectDefinition == relTarget.ObjectDefinition)
                                    {
                                        foundMatch = true;
                                        break;
                                    }
                                }
                                if (!foundMatch)
                                {
                                    ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, relTarget, "        " + relTarget.Name, relTarget.Id, relTarget.ObjectDefinition);
                                    comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                                    _comparisonObjectCount += 1;
                                }
                            }

                            #endregion

                            #region Measures (table in source and target)

                            // see if matching measure in source and target
                            foreach (Measure measureSource in _sourceTabularModel.Measures.FilterByTableName(tblSource.Name))
                            {
                                if (_targetTabularModel.Measures.FilterByTableName(tblTarget.Name).ContainsName(measureSource.Name))
                                {
                                    //Measure in source and target, so check definition
                                    Measure measureTarget = _targetTabularModel.Measures.FilterByTableName(tblTarget.Name).FindByName(measureSource.Name);
                                    if (measureSource.ObjectDefinition == measureTarget.ObjectDefinition)
                                    {
                                        //Measure has same definition
                                        ComparisonObject comparisonObjectMeasure = new ComparisonObject(ComparisonObjectType.Measure, ComparisonObjectStatus.SameDefinition, measureSource, "        " + measureSource.Name, measureSource.Id, measureSource.ObjectDefinition, MergeAction.Skip, measureTarget, "        " + measureTarget.Name, measureTarget.Id, measureTarget.ObjectDefinition);
                                        comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                                        _comparisonObjectCount += 1;
                                    }
                                    else
                                    {
                                        //Measure has different definition
                                        ComparisonObject comparisonObjectMeasure = new ComparisonObject(ComparisonObjectType.Measure, ComparisonObjectStatus.DifferentDefinitions, measureSource, "        " + measureSource.Name, measureSource.Id, measureSource.ObjectDefinition, MergeAction.Update, measureTarget, "        " + measureTarget.Name, measureTarget.Id, measureTarget.ObjectDefinition);
                                        comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                                        _comparisonObjectCount += 1;
                                    }
                                }
                                else
                                {
                                    ComparisonObject comparisonObjectMeasure = new ComparisonObject(ComparisonObjectType.Measure, ComparisonObjectStatus.MissingInTarget, measureSource, "        " + measureSource.Name, measureSource.Id, measureSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                                    comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                                    _comparisonObjectCount += 1;
                                }
                            }
                            //now check if target contains measures Missing in Source
                            foreach (Measure measureTarget in _targetTabularModel.Measures.FilterByTableName(tblTarget.Name))
                            {
                                if (!_sourceTabularModel.Measures.FilterByTableName(tblSource.Name).ContainsName(measureTarget.Name))
                                {
                                    ComparisonObject comparisonObjectMeasure = new ComparisonObject(ComparisonObjectType.Measure, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, measureTarget, "        " + measureTarget.Name, measureTarget.Id, measureTarget.ObjectDefinition);
                                    comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                                    _comparisonObjectCount += 1;
                                }
                            }

                            #endregion

                            #region Kpis (table in source and target)

                            // see if matching kpi in source and target
                            foreach (Kpi kpiSource in _sourceTabularModel.Kpis.FilterByTableName(tblSource.Name))
                            {
                                if (_targetTabularModel.Kpis.FilterByTableName(tblTarget.Name).ContainsName(kpiSource.Name))
                                {
                                    //Kpi in source and target, so check definition
                                    Kpi kpiTarget = _targetTabularModel.Kpis.FilterByTableName(tblTarget.Name).FindByName(kpiSource.Name);
                                    if (kpiSource.ObjectDefinition == kpiTarget.ObjectDefinition)
                                    {
                                        //Kpi has same definition
                                        ComparisonObject comparisonObjectKpi = new ComparisonObject(ComparisonObjectType.Kpi, ComparisonObjectStatus.SameDefinition, kpiSource, "        " + kpiSource.Name, kpiSource.Id, kpiSource.ObjectDefinition, MergeAction.Skip, kpiTarget, "        " + kpiTarget.Name, kpiTarget.Id, kpiTarget.ObjectDefinition);
                                        comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectKpi);
                                        _comparisonObjectCount += 1;
                                    }
                                    else
                                    {
                                        //Kpi has different definition
                                        ComparisonObject comparisonObjectKpi = new ComparisonObject(ComparisonObjectType.Kpi, ComparisonObjectStatus.DifferentDefinitions, kpiSource, "        " + kpiSource.Name, kpiSource.Id, kpiSource.ObjectDefinition, MergeAction.Update, kpiTarget, "        " + kpiTarget.Name, kpiTarget.Id, kpiTarget.ObjectDefinition);
                                        comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectKpi);
                                        _comparisonObjectCount += 1;
                                    }
                                }
                                else
                                {
                                    ComparisonObject comparisonObjectKpi = new ComparisonObject(ComparisonObjectType.Kpi, ComparisonObjectStatus.MissingInTarget, kpiSource, "        " + kpiSource.Name, kpiSource.Id, kpiSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                                    comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectKpi);
                                    _comparisonObjectCount += 1;
                                }
                            }
                            //now check if target contains kpis Missing in Source
                            foreach (Kpi kpiTarget in _targetTabularModel.Kpis.FilterByTableName(tblTarget.Name))
                            {
                                if (!_sourceTabularModel.Kpis.FilterByTableName(tblSource.Name).ContainsName(kpiTarget.Name))
                                {
                                    ComparisonObject comparisonObjectKpi = new ComparisonObject(ComparisonObjectType.Kpi, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, kpiTarget, "        " + kpiTarget.Name, kpiTarget.Id, kpiTarget.ObjectDefinition);
                                    comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectKpi);
                                    _comparisonObjectCount += 1;
                                }
                            }

                            #endregion
                        }
                    }

                    foreach (Table tblTarget in _targetTabularModel.Tables.FilterByDataSourceId(dataSourceTarget.Id))
                    {
                        // check if target is not in source
                        if (!_sourceTabularModel.Tables.FilterByDataSourceId(dataSourceSource.Id).ContainsName(tblTarget.Name))
                        {
                            ComparisonObject comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, tblTarget, tblTarget.Name, tblTarget.Id, tblTarget.ObjectDefinition);
                            comparisonObjectDataSource.ChildComparisonObjects.Add(comparisonObjectTable);
                            _comparisonObjectCount += 1;

                            #region Relationships for table Missing in Source

                            // all relationships in target are not in source (the source table doesn't even exist)
                            foreach (Relationship relTarget in tblTarget.Relationships)
                            {
                                ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, relTarget, "        " + relTarget.Name, relTarget.Id, relTarget.ObjectDefinition);
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                                _comparisonObjectCount += 1;
                            }

                            #endregion

                            #region Measures for Table that is Missing in Source

                            foreach (Measure measureTarget in _targetTabularModel.Measures.FilterByTableName(tblTarget.Name))
                            {
                                ComparisonObject comparisonObjectMeasure = new ComparisonObject(ComparisonObjectType.Measure, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, measureTarget, "        " + measureTarget.Name, measureTarget.Id, measureTarget.ObjectDefinition);
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                                _comparisonObjectCount += 1;
                            }

                            #endregion

                            #region Kpis for Table that is Missing in Source

                            foreach (Kpi kpiTarget in _targetTabularModel.Kpis.FilterByTableName(tblTarget.Name))
                            {
                                ComparisonObject comparisonObjectKpi = new ComparisonObject(ComparisonObjectType.Kpi, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, kpiTarget, "        " + kpiTarget.Name, kpiTarget.Id, kpiTarget.ObjectDefinition);
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectKpi);
                                _comparisonObjectCount += 1;
                            }

                            #endregion
                        }
                    }
                    #endregion
                }
            }

            foreach (DataSource dataSourceTarget in _targetTabularModel.DataSources)
            {
                // if target datasource is Missing in Source, offer deletion
                if (!_sourceTabularModel.DataSources.ContainsName(dataSourceTarget.Name))
                {
                    ComparisonObject comparisonObjectDataSource = new ComparisonObject(ComparisonObjectType.DataSource, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, dataSourceTarget, dataSourceTarget.Name, dataSourceTarget.Id, dataSourceTarget.ObjectDefinition);
                    _comparisonObjects.Add(comparisonObjectDataSource);
                    _comparisonObjectCount += 1;

                    #region Tables for DataSource that is Missing in Source

                    foreach (Table tblTarget in _targetTabularModel.Tables.FilterByDataSourceId(dataSourceTarget.Id))
                    {
                        ComparisonObject comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, tblTarget, tblTarget.Name, tblTarget.Id, tblTarget.ObjectDefinition);
                        comparisonObjectDataSource.ChildComparisonObjects.Add(comparisonObjectTable);
                        _comparisonObjectCount += 1;

                        #region Relationships for Table that is Missing in Source

                        foreach (Relationship relTarget in tblTarget.Relationships)
                        {
                            ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, relTarget, "        " + relTarget.Name, relTarget.Id, relTarget.ObjectDefinition);
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                            _comparisonObjectCount += 1;
                        }

                        #endregion

                        #region Measures for Table that is Missing in Source

                        foreach (Measure measureTarget in _targetTabularModel.Measures.FilterByTableName(tblTarget.Name))
                        {
                            ComparisonObject comparisonObjectMeasure = new ComparisonObject(ComparisonObjectType.Measure, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, measureTarget, "        " + measureTarget.Name, measureTarget.Id, measureTarget.ObjectDefinition);
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                            _comparisonObjectCount += 1;
                        }

                        #endregion

                        #region Kpis for Table that is Missing in Source

                        foreach (Kpi kpiTarget in _targetTabularModel.Kpis.FilterByTableName(tblTarget.Name))
                        {
                            ComparisonObject comparisonObjectKpi = new ComparisonObject(ComparisonObjectType.Kpi, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, kpiTarget, "        " + kpiTarget.Name, kpiTarget.Id, kpiTarget.ObjectDefinition);
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectKpi);
                            _comparisonObjectCount += 1;
                        }

                        #endregion
                    }

                    #endregion
                }
            }

            #endregion

            #region Actions

            if (_comparisonInfo.OptionsInfo.OptionActions)
            {
                foreach (Action actionSource in _sourceTabularModel.Actions)
                {
                    // check if source is not in target
                    if (!_targetTabularModel.Actions.ContainsName(actionSource.Name))
                    {
                        ComparisonObject comparisonObjectAction = new ComparisonObject(ComparisonObjectType.Action, ComparisonObjectStatus.MissingInTarget, actionSource, actionSource.Name, actionSource.Id, actionSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                        _comparisonObjects.Add(comparisonObjectAction);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        // there is a Action in the target with the same name at least
                        Action actionTarget = _targetTabularModel.Actions.FindByName(actionSource.Name);
                        if (actionSource.Id != actionTarget.Id)
                        {
                            actionSource.SubstituteId = actionTarget.Id;
                        }
                        ComparisonObject comparisonObjectAction;

                        // check if Action object definition is different
                        if (actionSource.ObjectDefinition != actionTarget.ObjectDefinition)
                        {
                            comparisonObjectAction = new ComparisonObject(ComparisonObjectType.Action, ComparisonObjectStatus.DifferentDefinitions, actionSource, actionSource.Name, actionSource.Id, actionSource.ObjectDefinition, MergeAction.Update, actionTarget, actionTarget.Name, actionTarget.Id, actionTarget.ObjectDefinition);
                            _comparisonObjects.Add(comparisonObjectAction);
                            _comparisonObjectCount += 1;
                        }
                        else
                        {
                            // they are equal, ...
                            comparisonObjectAction = new ComparisonObject(ComparisonObjectType.Action, ComparisonObjectStatus.SameDefinition, actionSource, actionSource.Name, actionSource.Id, actionSource.ObjectDefinition, MergeAction.Skip, actionTarget, actionTarget.Name, actionTarget.Id, actionTarget.ObjectDefinition);
                            _comparisonObjects.Add(comparisonObjectAction);
                            _comparisonObjectCount += 1;
                        }
                    }
                }

                foreach (Action actionTarget in _targetTabularModel.Actions)
                {
                    // if target Action is Missing in Source, offer deletion
                    if (!_sourceTabularModel.Actions.ContainsName(actionTarget.Name))
                    {
                        ComparisonObject comparisonObjectAction = new ComparisonObject(ComparisonObjectType.Action, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, actionTarget, actionTarget.Name, actionTarget.Id, actionTarget.ObjectDefinition);
                        _comparisonObjects.Add(comparisonObjectAction);
                        _comparisonObjectCount += 1;
                    }
                }
            }

            #endregion

            #region Perspectives

            if (_comparisonInfo.OptionsInfo.OptionPerspectives)
            {
                foreach (Perspective perspectiveSource in _sourceTabularModel.Perspectives)
                {
                    // check if source is not in target
                    if (!_targetTabularModel.Perspectives.ContainsName(perspectiveSource.Name))
                    {
                        ComparisonObject comparisonObjectPerspective = new ComparisonObject(ComparisonObjectType.Perspective, ComparisonObjectStatus.MissingInTarget, perspectiveSource, perspectiveSource.Name, perspectiveSource.Id, perspectiveSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                        _comparisonObjects.Add(comparisonObjectPerspective);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        // there is a perspective in the target with the same name at least
                        Perspective perspectiveTarget = _targetTabularModel.Perspectives.FindByName(perspectiveSource.Name);
                        if (perspectiveSource.Id != perspectiveTarget.Id)
                        {
                            perspectiveSource.SubstituteId = perspectiveTarget.Id;
                        }
                        ComparisonObject comparisonObjectPerspective;

                        // check if perspective object definition is different
                        //if (perspectiveSource.ObjectDefinition != perspectiveTarget.ObjectDefinition)
                        if ( (_comparisonInfo.OptionsInfo.OptionMergePerspectives && perspectiveTarget.ContainsOtherPerspectiveSelections(perspectiveSource)) ||
                             (!_comparisonInfo.OptionsInfo.OptionMergePerspectives && perspectiveTarget.ContainsOtherPerspectiveSelections(perspectiveSource) && perspectiveSource.ContainsOtherPerspectiveSelections(perspectiveTarget)) )
                        {
                            // they are equal, ...
                            comparisonObjectPerspective = new ComparisonObject(ComparisonObjectType.Perspective, ComparisonObjectStatus.SameDefinition, perspectiveSource, perspectiveSource.Name, perspectiveSource.Id, perspectiveSource.ObjectDefinition, MergeAction.Skip, perspectiveTarget, perspectiveTarget.Name, perspectiveTarget.Id, perspectiveTarget.ObjectDefinition);
                            _comparisonObjects.Add(comparisonObjectPerspective);
                            _comparisonObjectCount += 1;
                        }
                        else
                        {
                            comparisonObjectPerspective = new ComparisonObject(ComparisonObjectType.Perspective, ComparisonObjectStatus.DifferentDefinitions, perspectiveSource, perspectiveSource.Name, perspectiveSource.Id, perspectiveSource.ObjectDefinition, MergeAction.Update, perspectiveTarget, perspectiveTarget.Name, perspectiveTarget.Id, perspectiveTarget.ObjectDefinition);
                            _comparisonObjects.Add(comparisonObjectPerspective);
                            _comparisonObjectCount += 1;
                        }
                    }
                }

                foreach (Perspective perspectiveTarget in _targetTabularModel.Perspectives)
                {
                    // if target perspective is Missing in Source, offer deletion
                    if (!_sourceTabularModel.Perspectives.ContainsName(perspectiveTarget.Name))
                    {
                        ComparisonObject comparisonObjectPerspective = new ComparisonObject(ComparisonObjectType.Perspective, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, perspectiveTarget, perspectiveTarget.Name, perspectiveTarget.Id, perspectiveTarget.ObjectDefinition);
                        _comparisonObjects.Add(comparisonObjectPerspective);
                        _comparisonObjectCount += 1;
                    }
                }
            }

            #endregion

            #region Roles

            if (_comparisonInfo.OptionsInfo.OptionRoles)
            {
                foreach (Role roleSource in _sourceTabularModel.Roles)
                {
                    // check if source is not in target
                    if (!_targetTabularModel.Roles.ContainsName(roleSource.Name))
                    {
                        ComparisonObject comparisonObjectRole = new ComparisonObject(ComparisonObjectType.Role, ComparisonObjectStatus.MissingInTarget, roleSource, roleSource.Name, roleSource.Id, roleSource.ObjectDefinition, MergeAction.Create, null, "", "", "");
                        _comparisonObjects.Add(comparisonObjectRole);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        // there is a role in the target with the same name at least
                        Role roleTarget = _targetTabularModel.Roles.FindByName(roleSource.Name);
                        if (roleSource.Id != roleTarget.Id)
                        {
                            roleSource.SubstituteId = roleTarget.Id;
                        }
                        ComparisonObject comparisonObjectRole;

                        // check if role object definition is different
                        if (roleSource.ObjectDefinition != roleTarget.ObjectDefinition)
                        {
                            comparisonObjectRole = new ComparisonObject(ComparisonObjectType.Role, ComparisonObjectStatus.DifferentDefinitions, roleSource, roleSource.Name, roleSource.Id, roleSource.ObjectDefinition, MergeAction.Update, roleTarget, roleTarget.Name, roleTarget.Id, roleTarget.ObjectDefinition);
                            _comparisonObjects.Add(comparisonObjectRole);
                            _comparisonObjectCount += 1;
                        }
                        else
                        {
                            // they are equal, ...
                            comparisonObjectRole = new ComparisonObject(ComparisonObjectType.Role, ComparisonObjectStatus.SameDefinition, roleSource, roleSource.Name, roleSource.Id, roleSource.ObjectDefinition, MergeAction.Skip, roleTarget, roleTarget.Name, roleTarget.Id, roleTarget.ObjectDefinition);
                            _comparisonObjects.Add(comparisonObjectRole);
                            _comparisonObjectCount += 1;
                        }
                    }
                }

                foreach (Role roleTarget in _targetTabularModel.Roles)
                {
                    // if target role is Missing in Source, offer deletion
                    if (!_sourceTabularModel.Roles.ContainsName(roleTarget.Name))
                    {
                        ComparisonObject comparisonObjectRole = new ComparisonObject(ComparisonObjectType.Role, ComparisonObjectStatus.MissingInSource, null, "", "", "", MergeAction.Delete, roleTarget, roleTarget.Name, roleTarget.Id, roleTarget.ObjectDefinition);
                        _comparisonObjects.Add(comparisonObjectRole);
                        _comparisonObjectCount += 1;
                    }
                }
            }

            #endregion

            #region Sorting

            _comparisonObjects.Sort();
            foreach (ComparisonObject childComparisonObject in _comparisonObjects)
            {
                childComparisonObject.ChildComparisonObjects.Sort();
                foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                {
                    grandChildComparisonObject.ChildComparisonObjects.Sort();
                }
            }

            #endregion

            this.RefreshComparisonObjectsFromSkipSelections();

            _uncommitedChanges = false;
            _lastSourceSchemaUpdate = _sourceTabularModel.AmoDatabase.LastSchemaUpdate;
            _lastTargetSchemaUpdate = _targetTabularModel.AmoDatabase.LastSchemaUpdate;
        }

        /// <summary>
        /// Validate selection of actions to perform on target tabular model. Warnings and informational messages are provided by invoking ShowStatusMessageCallBack.
        /// </summary>
        public override void ValidateSelection()
        {
            #region Refresh/reconnect source and target dbs to check if server definition has changed

            if (_uncommitedChanges)
            {
                // Reconnect to re-initialize
                _sourceTabularModel = new TabularModel(this, _comparisonInfo.ConnectionInfoSource, _comparisonInfo);
                _sourceTabularModel.Connect();
                _targetTabularModel = new TabularModel(this, _comparisonInfo.ConnectionInfoTarget, _comparisonInfo);
                _targetTabularModel.Connect();
                InitializeSubstituteIds();
            }
            else
            {
                _sourceTabularModel.AmoDatabase.Refresh();
                _targetTabularModel.AmoDatabase.Refresh();
            }

            if (!_sourceTabularModel.ConnectionInfo.UseProject && _sourceTabularModel.AmoDatabase.LastSchemaUpdate > _lastSourceSchemaUpdate)
            {
                throw new Exception("The definition of the source database has changed since the comparison was run.  Please re-run the comparison.");
            }
            if (!_targetTabularModel.ConnectionInfo.UseProject && _targetTabularModel.AmoDatabase.LastSchemaUpdate > _lastTargetSchemaUpdate)
            {
                throw new Exception("The definition of the target database has changed since the comparison was run.  Please re-run the comparison.");
            }

            _uncommitedChanges = true;

            #endregion

            #region DataSources

            // do deletions first to minimize chance of conflict
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.DataSource && comparisonObject.MergeAction == MergeAction.Delete)
                {
                    _targetTabularModel.DeleteDataSource(comparisonObject.TargetObjectId);
                    OnValidationMessage(new ValidationMessageEventArgs("Delete Data Source [" + comparisonObject.TargetObjectName + "].", ValidationMessageType.DataSource, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.DataSource && comparisonObject.MergeAction == MergeAction.Create)
                {
                    _targetTabularModel.CreateDataSource(_sourceTabularModel.DataSources.FindById(comparisonObject.SourceObjectId));
                    OnValidationMessage(new ValidationMessageEventArgs("Create Data Source [" + comparisonObject.SourceObjectName + "].", ValidationMessageType.DataSource, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.DataSource && comparisonObject.MergeAction == MergeAction.Update)
                {
                    _targetTabularModel.UpdateDataSource(_sourceTabularModel.DataSources.FindById(comparisonObject.SourceObjectId), _targetTabularModel.DataSources.FindById(comparisonObject.TargetObjectId));
                    OnValidationMessage(new ValidationMessageEventArgs("Update Data Source [" + comparisonObject.TargetObjectName + "].", ValidationMessageType.DataSource, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.DataSource && 
                    (comparisonObject.MergeAction == MergeAction.Skip || comparisonObject.MergeAction == MergeAction.Update) &&
                    (comparisonObject.Status == ComparisonObjectStatus.DifferentDefinitions || comparisonObject.Status == ComparisonObjectStatus.SameDefinition) &&
                    comparisonObject.SourceObjectId != comparisonObject.TargetObjectId)
                {
                    comparisonObject.SourceObjectSubstituteId = comparisonObject.TargetObjectId;
                    _sourceTabularModel.DataSources.FindById(comparisonObject.SourceObjectId).SubstituteId = comparisonObject.TargetObjectId;
                }
            }

            #endregion

            #region Flush reference dims

            _sourceTabularModel.FlushReferenceDimensions();
            _targetTabularModel.FlushReferenceDimensions();

            #endregion

            #region Tables

            //retain partitions depending on option
            if (_comparisonInfo.OptionsInfo.OptionRetainPartitions)
            {
                OnValidationMessage(new ValidationMessageEventArgs("Option to retain partitions is set, but it is not supported for models with compatibility level 1100 or 1103. It will be ignored.", ValidationMessageType.Table, ValidationMessageStatus.Warning));
            }

            // do deletions first to minimize chance of conflict
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    if (childComparisonObject.ComparisonObjectType == ComparisonObjectType.Table && childComparisonObject.MergeAction == MergeAction.Delete)
                    {
                        _targetTabularModel.DeleteTable(childComparisonObject.TargetObjectId);
                        OnValidationMessage(new ValidationMessageEventArgs("Delete Table '" + childComparisonObject.TargetObjectName + "'.", ValidationMessageType.Table, ValidationMessageStatus.Informational));
                    }
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    if (childComparisonObject.ComparisonObjectType == ComparisonObjectType.Table && childComparisonObject.MergeAction == MergeAction.Create)
                    {
                        Table tableTarget = _targetTabularModel.Tables.FindByName(childComparisonObject.SourceObjectName);
                        if (tableTarget == null)
                        {
                            string sourceObjectSubstituteId = childComparisonObject.SourceObjectSubstituteId;
                            bool useSubstituteId = false;
                            _targetTabularModel.CreateTable(_sourceTabularModel.Tables.FindById(childComparisonObject.SourceObjectId), ref sourceObjectSubstituteId, ref useSubstituteId);
                            if (useSubstituteId)
                            {
                                _sourceTabularModel.UpdateRelationshipsWithSubstituteTableIds(childComparisonObject.SourceObjectSubstituteId, sourceObjectSubstituteId);
                                childComparisonObject.SourceObjectSubstituteId = sourceObjectSubstituteId;
                            }
                            OnValidationMessage(new ValidationMessageEventArgs("Create Table '" + childComparisonObject.SourceObjectName + "'.", ValidationMessageType.Table, ValidationMessageStatus.Informational));
                        }
                        else
                        {
                            OnValidationMessage(new ValidationMessageEventArgs("Unable to create Table " + childComparisonObject.SourceObjectName + " because another table with the same name (under a different data source) already exists in target model.", ValidationMessageType.Table, ValidationMessageStatus.Warning));
                        }
                    }
                }
            }

            // before update tables, we need to check tables with Skip action that exist in source and target, to set substitute ids equal to ids from the target table
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    if (childComparisonObject.ComparisonObjectType == ComparisonObjectType.Table && childComparisonObject.MergeAction == MergeAction.Skip &&
                        (childComparisonObject.Status == ComparisonObjectStatus.DifferentDefinitions || childComparisonObject.Status == ComparisonObjectStatus.SameDefinition) &&
                        childComparisonObject.SourceObjectId != childComparisonObject.TargetObjectId)
                    {
                        childComparisonObject.SourceObjectSubstituteId = childComparisonObject.TargetObjectId;
                        _sourceTabularModel.Tables.FindById(childComparisonObject.SourceObjectId).SubstituteId = childComparisonObject.TargetObjectId;
                        _sourceTabularModel.UpdateRelationshipsWithSubstituteTableIds(childComparisonObject.SourceObjectId, childComparisonObject.TargetObjectId);
                    }
                }
            }

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    if (childComparisonObject.ComparisonObjectType == ComparisonObjectType.Table && childComparisonObject.MergeAction == MergeAction.Update)
                    {
                        string sourceObjectSubstituteId = childComparisonObject.SourceObjectSubstituteId;
                        bool useSubstituteId = false;
                        _targetTabularModel.UpdateTable(_sourceTabularModel.Tables.FindById(childComparisonObject.SourceObjectId), _targetTabularModel.Tables.FindById(childComparisonObject.TargetObjectId), ref sourceObjectSubstituteId, ref useSubstituteId);
                        if (useSubstituteId)
                        {
                            _sourceTabularModel.UpdateRelationshipsWithSubstituteTableIds(childComparisonObject.SourceObjectSubstituteId, sourceObjectSubstituteId);
                            childComparisonObject.SourceObjectSubstituteId = sourceObjectSubstituteId;
                        }
                        OnValidationMessage(new ValidationMessageEventArgs("Update Table '" + childComparisonObject.TargetObjectName + "'.", ValidationMessageType.Table, ValidationMessageStatus.Informational));
                    }
                }
            }

            // now that we've done table updates, we need to check the child relationships that referred to the updated tables.  We have to do this here
            // rather than in UpdateTable because need to ensure all the child tables have been updated too (if they happened to have an update action too)
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    if (childComparisonObject.ComparisonObjectType == ComparisonObjectType.Table && childComparisonObject.MergeAction == MergeAction.Update)
                    {
                        _targetTabularModel.UpdateRelationshipsForChildrenOfUpdatedTables(_targetTabularModel.Tables.FindById(childComparisonObject.SourceObjectSubstituteId));
                    }
                }
            }

            #endregion

            #region Relationships

            // do deletions first to minimize chance of conflict
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                    {
                        if (grandChildComparisonObject.ComparisonObjectType == ComparisonObjectType.Relationship && grandChildComparisonObject.MergeAction == MergeAction.Delete)
                        {
                            Table tableTarget = _targetTabularModel.Tables.FindById(childComparisonObject.TargetObjectId);
                            if (tableTarget != null)
                            {
                                // Relationship may have already been deleted if parent table was deleted
                                tableTarget.DeleteRelationship(grandChildComparisonObject.TargetObjectId);
                            }
                            OnValidationMessage(new ValidationMessageEventArgs("Delete Relationship " + grandChildComparisonObject.TargetObjectName.Trim() + ".", ValidationMessageType.Relationship, ValidationMessageStatus.Informational));
                        }
                    }
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                    {
                        if (grandChildComparisonObject.ComparisonObjectType == ComparisonObjectType.Relationship && grandChildComparisonObject.MergeAction == MergeAction.Create)
                        {
                            Table tableSource = _sourceTabularModel.Tables.FindById(childComparisonObject.SourceObjectId);
                            Table tableTarget = _targetTabularModel.Tables.FindByName(childComparisonObject.SourceObjectName);
                            Relationship relationshipSource = tableSource.Relationships.FindById(grandChildComparisonObject.SourceObjectId);
                            Table parentTableSource = _sourceTabularModel.Tables.FindByName(relationshipSource.ParentTableName);

                            string warningMessage = "Unable to create Relationship " + grandChildComparisonObject.SourceObjectName.Trim() + " because (considering changes) necessary table/column(s) not found in target model.";
                            if (tableTarget != null && tableTarget.CreateRelationship(relationshipSource, parentTableSource.AmoDimension, grandChildComparisonObject.SourceObjectName.Trim(), ref warningMessage))
                            {
                                OnValidationMessage(new ValidationMessageEventArgs("Create Relationship " + grandChildComparisonObject.SourceObjectName.Trim() + ".", ValidationMessageType.Relationship, ValidationMessageStatus.Informational));
                            }
                            else
                            {
                                OnValidationMessage(new ValidationMessageEventArgs(warningMessage, ValidationMessageType.Relationship, ValidationMessageStatus.Warning));
                            }
                        }
                    }
                }
            }

            #endregion

            _targetTabularModel.CheckRelationshipValidity();
            _targetTabularModel.PopulateReferenceDimensions();

            #region Measures / KPIs

            // delete measures
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                    {
                        if (grandChildComparisonObject.ComparisonObjectType == ComparisonObjectType.Measure && grandChildComparisonObject.MergeAction == MergeAction.Delete)
                        {
                            _targetTabularModel.DeleteMeasure(grandChildComparisonObject.TargetObjectId);
                            OnValidationMessage(new ValidationMessageEventArgs("Delete Measure '" + grandChildComparisonObject.TargetObjectId + "'.", ValidationMessageType.Measure, ValidationMessageStatus.Informational));
                        }
                    }
                }
            }
            // need to delete KPIs now to minimize chance of conflict - but show the message later
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                    {
                        if (grandChildComparisonObject.ComparisonObjectType == ComparisonObjectType.Kpi && grandChildComparisonObject.MergeAction == MergeAction.Delete)
                        {
                            _targetTabularModel.DeleteKpi(grandChildComparisonObject.TargetObjectId);
                            //OnValidationMessage(new ValidationMessageEventArgs("Delete KPI '" + grandChildComparisonObject.TargetObjectId + "'.", ValidationMessageType.Kpi, ValidationMessageStatus.MergeActionSuccessful);
                        }
                    }
                }
            }

            // now finish off measures
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                    {
                        if (grandChildComparisonObject.ComparisonObjectType == ComparisonObjectType.Measure && grandChildComparisonObject.MergeAction == MergeAction.Create)
                        {
                            if (_targetTabularModel.Measures.ContainsName(_sourceTabularModel.Measures.FindById(grandChildComparisonObject.SourceObjectId).Name) ||
                                _targetTabularModel.Kpis.ContainsName(_sourceTabularModel.Measures.FindById(grandChildComparisonObject.SourceObjectId).Name))
                            {
                                OnValidationMessage(new ValidationMessageEventArgs("Unable to create Measure " + grandChildComparisonObject.SourceObjectId + " because measure name already exists in target model.", ValidationMessageType.Measure, ValidationMessageStatus.Warning));
                            }
                            else
                            {
                                _targetTabularModel.CreateMeasure(_sourceTabularModel.Measures.FindById(grandChildComparisonObject.SourceObjectId));
                                OnValidationMessage(new ValidationMessageEventArgs("Create Measure " + grandChildComparisonObject.SourceObjectId + ".", ValidationMessageType.Measure, ValidationMessageStatus.Informational));
                            }
                        }
                    }
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                    {
                        if (grandChildComparisonObject.ComparisonObjectType == ComparisonObjectType.Measure && grandChildComparisonObject.MergeAction == MergeAction.Update)
                        {
                            _targetTabularModel.UpdateMeasure(_sourceTabularModel.Measures.FindById(grandChildComparisonObject.SourceObjectId), _targetTabularModel.Measures.FindById(grandChildComparisonObject.TargetObjectId));
                            OnValidationMessage(new ValidationMessageEventArgs("Update Measure '" + grandChildComparisonObject.TargetObjectId + "'.", ValidationMessageType.Measure, ValidationMessageStatus.Informational));
                        }
                    }
                }
            }

            // now finish off KPIs
            // start by showing the messages we didn't when deleted above
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                    {
                        if (grandChildComparisonObject.ComparisonObjectType == ComparisonObjectType.Kpi && grandChildComparisonObject.MergeAction == MergeAction.Delete)
                        {
                            OnValidationMessage(new ValidationMessageEventArgs("Delete KPI '" + grandChildComparisonObject.TargetObjectId + "'.", ValidationMessageType.Kpi, ValidationMessageStatus.Informational));
                        }
                    }
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                    {
                        if (grandChildComparisonObject.ComparisonObjectType == ComparisonObjectType.Kpi && grandChildComparisonObject.MergeAction == MergeAction.Create)
                        {
                            if (_targetTabularModel.Kpis.ContainsName(_sourceTabularModel.Kpis.FindById(grandChildComparisonObject.SourceObjectId).Name) ||
                                _targetTabularModel.Measures.ContainsName(_sourceTabularModel.Kpis.FindById(grandChildComparisonObject.SourceObjectId).Name))
                            {
                                OnValidationMessage(new ValidationMessageEventArgs("Unable to create KPI " + grandChildComparisonObject.SourceObjectId + " because name already exists in target model as a KPI or measure.", ValidationMessageType.Kpi, ValidationMessageStatus.Warning));
                            }
                            else
                            {
                                _targetTabularModel.CreateKpi(_sourceTabularModel.Kpis.FindById(grandChildComparisonObject.SourceObjectId));
                                OnValidationMessage(new ValidationMessageEventArgs("Create KPI " + grandChildComparisonObject.SourceObjectId + ".", ValidationMessageType.Kpi, ValidationMessageStatus.Informational));
                            }
                        }
                    }
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    foreach (ComparisonObject grandChildComparisonObject in childComparisonObject.ChildComparisonObjects)
                    {
                        if (grandChildComparisonObject.ComparisonObjectType == ComparisonObjectType.Kpi && grandChildComparisonObject.MergeAction == MergeAction.Update)
                        {
                            _targetTabularModel.UpdateKpi(_sourceTabularModel.Kpis.FindById(grandChildComparisonObject.SourceObjectId), _targetTabularModel.Kpis.FindById(grandChildComparisonObject.TargetObjectId));
                            OnValidationMessage(new ValidationMessageEventArgs("Update KPI '" + grandChildComparisonObject.TargetObjectId + "'.", ValidationMessageType.Kpi, ValidationMessageStatus.Informational));
                        }
                    }
                }
            }

            #endregion

            _targetTabularModel.PopulateMdxScript();
            CheckCalcPropsAnnotations();

            #region Actions

            // do deletions first to minimize chance of conflict
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Action && comparisonObject.MergeAction == MergeAction.Delete)
                {
                    _targetTabularModel.DeleteAction(comparisonObject.TargetObjectId);
                    OnValidationMessage(new ValidationMessageEventArgs("Delete Action [" + comparisonObject.TargetObjectName + "].", ValidationMessageType.Action, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Action && comparisonObject.MergeAction == MergeAction.Create)
                {
                    if (_targetTabularModel.AmoDatabase.Cubes.Count > 0)
                    {
                        _targetTabularModel.CreateAction(_sourceTabularModel.Actions.FindById(comparisonObject.SourceObjectId));
                        OnValidationMessage(new ValidationMessageEventArgs("Create Action [" + comparisonObject.SourceObjectName + "].", ValidationMessageType.Action, ValidationMessageStatus.Informational));
                    }
                    else
                    {
                        OnValidationMessage(new ValidationMessageEventArgs("Unable to create Action " + comparisonObject.SourceObjectName + " because public \"cube\" not found in target.  There must be at least one data source/table in target, for there to be an public \"cube\".", ValidationMessageType.Action, ValidationMessageStatus.Warning));
                    }
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Action && comparisonObject.MergeAction == MergeAction.Update)
                {
                    _targetTabularModel.MergeAction(_sourceTabularModel.Actions.FindById(comparisonObject.SourceObjectId), _targetTabularModel.Actions.FindById(comparisonObject.TargetObjectId));
                    OnValidationMessage(new ValidationMessageEventArgs("Update Action [" + comparisonObject.TargetObjectName + "].", ValidationMessageType.Action, ValidationMessageStatus.Informational));
                }
            }

            #endregion

            #region Perspectives

            // do deletions first to minimize chance of conflict
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Perspective && comparisonObject.MergeAction == MergeAction.Delete)
                {
                    _targetTabularModel.DeletePerspective(comparisonObject.TargetObjectId);
                    OnValidationMessage(new ValidationMessageEventArgs("Delete Perspective [" + comparisonObject.TargetObjectName + "].", ValidationMessageType.Perspective, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Perspective && comparisonObject.MergeAction == MergeAction.Create)
                {
                    if (_targetTabularModel.AmoDatabase.Cubes.Count > 0)
                    {
                        _targetTabularModel.CreatePerspective(_sourceTabularModel.Perspectives.FindById(comparisonObject.SourceObjectId));
                        OnValidationMessage(new ValidationMessageEventArgs("Create Perspective [" + comparisonObject.SourceObjectName + "].", ValidationMessageType.Perspective, ValidationMessageStatus.Informational));
                    }
                    else
                    {
                        OnValidationMessage(new ValidationMessageEventArgs("Unable to create Perspective " + comparisonObject.SourceObjectName + " because public \"cube\" not found in target.  There must be at least one data source/table in target, for there to be an public \"cube\".", ValidationMessageType.Perspective, ValidationMessageStatus.Warning));
                    }
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Perspective && comparisonObject.MergeAction == MergeAction.Update)
                {
                    _targetTabularModel.UpdatePerspective(_sourceTabularModel.Perspectives.FindById(comparisonObject.SourceObjectId), _targetTabularModel.Perspectives.FindById(comparisonObject.TargetObjectId));
                    OnValidationMessage(new ValidationMessageEventArgs("Update Perspective [" + comparisonObject.TargetObjectName + "].", ValidationMessageType.Perspective, ValidationMessageStatus.Informational));
                }
            }

            #endregion

            #region Roles

            // do deletions first to minimize chance of conflict
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Role && comparisonObject.MergeAction == MergeAction.Delete)
                {
                    _targetTabularModel.DeleteRole(comparisonObject.TargetObjectId);
                    OnValidationMessage(new ValidationMessageEventArgs("Delete Role [" + comparisonObject.TargetObjectName + "].", ValidationMessageType.Role, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Role && comparisonObject.MergeAction == MergeAction.Create)
                {
                    if (_targetTabularModel.AmoDatabase.Cubes.Count > 0)
                    {
                        _targetTabularModel.CreateRole(_sourceTabularModel.Roles.FindById(comparisonObject.SourceObjectId));
                        OnValidationMessage(new ValidationMessageEventArgs("Create Role [" + comparisonObject.SourceObjectName + "].", ValidationMessageType.Role, ValidationMessageStatus.Informational));
                    }
                    else
                    {
                        OnValidationMessage(new ValidationMessageEventArgs("Unable to create Role " + comparisonObject.SourceObjectName + " because public \"cube\" not found in target.  There must be at least one data source/table in target, for there to be an public \"cube\".", ValidationMessageType.Role, ValidationMessageStatus.Warning));
                    }
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Role && comparisonObject.MergeAction == MergeAction.Update)
                {
                    _targetTabularModel.UpdateRole(_sourceTabularModel.Roles.FindById(comparisonObject.SourceObjectId), _targetTabularModel.Roles.FindById(comparisonObject.TargetObjectId));
                    OnValidationMessage(new ValidationMessageEventArgs("Update Role [" + comparisonObject.TargetObjectName + "].", ValidationMessageType.Role, ValidationMessageStatus.Informational));
                }
            }

            #endregion

            #region Missing calculation dependencies

            if (_comparisonInfo.OptionsInfo.OptionMeasureDependencies)
            {
                foreach (Measure measure in _targetTabularModel.Measures)
                {
                    foreach (string missingDependency in measure.FindMissingCalculationDependencies())
                    {
                        OnValidationMessage(new ValidationMessageEventArgs("Measure " + measure.Id + " contains dependency on measure/column [" + missingDependency + "], which (considering changes to target) cannot be found in target model.", ValidationMessageType.MeasureCalculationDependency, ValidationMessageStatus.Warning));
                    }
                }

                foreach (Kpi kpi in _targetTabularModel.Kpis)
                {
                    foreach (string missingDependency in kpi.FindMissingCalculationDependencies())
                    {
                        OnValidationMessage(new ValidationMessageEventArgs("KPI " + kpi.Id + " contains dependency on measure/column [" + missingDependency + "], which (considering changes to target) cannot be found in target model.", ValidationMessageType.MeasureCalculationDependency, ValidationMessageStatus.Warning));
                    }
                }
            }

            #endregion

            _targetTabularModel.FinalCleanup();

            OnResizeValidationHeaders(new EventArgs());
        }

        #region Private methods for validation

        private void CheckCalcPropsAnnotations()
        {
            if (_sourceTabularModel.AmoDatabase.Cubes.Count > 0 && _targetTabularModel.AmoDatabase.Cubes.Count > 0)
            {
                if ( (_sourceTabularModel.AmoDatabase.Cubes[0].MdxScripts.Contains("MdxScript") && _sourceTabularModel.AmoDatabase.Cubes[0].MdxScripts["MdxScript"].CalculationProperties.Contains("Measures.[__No measures defined]")) &&
                     (_targetTabularModel.AmoDatabase.Cubes[0].MdxScripts.Contains("MdxScript") && !_targetTabularModel.AmoDatabase.Cubes[0].MdxScripts["MdxScript"].CalculationProperties.Contains("Measures.[__No measures defined]"))
                   )
                {
                    _targetTabularModel.CreateCalculationProperty(_sourceTabularModel.AmoDatabase.Cubes[0].MdxScripts["MdxScript"].CalculationProperties["Measures.[__No measures defined]"], "Measures.[__No measures defined]");
                }
                if (_sourceTabularModel.AmoDatabase.Cubes[0].Annotations.Contains("DefaultMeasure") && !_targetTabularModel.AmoDatabase.Cubes[0].Annotations.Contains("DefaultMeasure"))
                {
                    _targetTabularModel.AmoDatabase.Cubes[0].Annotations.Add(_sourceTabularModel.AmoDatabase.Cubes[0].Annotations["DefaultMeasure"].Clone());
                }
                if (_sourceTabularModel.AmoDatabase.Cubes[0].ProactiveCaching != null && _targetTabularModel.AmoDatabase.Cubes[0].ProactiveCaching == null)
                {
                    _targetTabularModel.AmoDatabase.Cubes[0].ProactiveCaching = _sourceTabularModel.AmoDatabase.Cubes[0].ProactiveCaching.Clone();
                }
            }
        }

        private void InitializeSubstituteIds()
        {
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    childComparisonObject.SourceObjectSubstituteId = null;
                }
            }
        }

        #endregion

        /// <summary>
        /// Update target tabular model with changes defined by actions in ComparisonObject instances.
        /// </summary>
        /// <returns>Flag to indicate whether update was successful.</returns>
        public override bool Update() => _targetTabularModel.Update();

        /// <summary>
        /// Gets a collection of ProcessingTable objects depending on Process Affected Tables option.
        /// </summary>
        /// <returns>Collection of ProcessingTable objects.</returns>
        public override ProcessingTableCollection GetTablesToProcess()
        {
            ProcessingTableCollection tablesToProcess = new ProcessingTableCollection();

            if (_comparisonInfo.OptionsInfo.OptionProcessingOption != ProcessingOption.DoNotProcess)
            {
                if (_comparisonInfo.OptionsInfo.OptionAffectedTables)
                {
                    foreach (Core.ComparisonObject comparisonObject in _comparisonObjects)
                    {
                        ProcessAffectedTables(comparisonObject, tablesToProcess);
                    }
                }
                else
                {
                    foreach (Table table in _targetTabularModel.Tables)
                    {
                        tablesToProcess.Add(new ProcessingTable(table.Name, table.Id));
                    }
                }
            }

            tablesToProcess.Sort();
            return tablesToProcess;
        }

        private void ProcessAffectedTables(Core.ComparisonObject comparisonObject, ProcessingTableCollection tablesToProcess)
        {
            //Recursively call for multiple levels to ensure catch calculated tables or those child of data source

            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Table &&
                (comparisonObject.MergeAction == MergeAction.Create || comparisonObject.MergeAction == MergeAction.Update)
               )
            {
                tablesToProcess.Add(new ProcessingTable(comparisonObject.SourceObjectName, comparisonObject.SourceObjectInternalName));
            }

            foreach (Core.ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
            {
                ProcessAffectedTables(childComparisonObject, tablesToProcess);
            }
        }

        /// <summary>
        /// Deploy database to target server and perform processing if required.
        /// </summary>
        /// <param name="tablesToProcess"></param>
        public override void DatabaseDeployAndProcess(ProcessingTableCollection tablesToProcess)
        {
            _targetTabularModel.DatabaseDeployAndProcess(tablesToProcess);
        }

        /// <summary>
        /// Stop processing of deployed database.
        /// </summary>
        public override void StopProcessing()
        {
            _targetTabularModel.StopProcessing();
        }

        /// <summary>
        /// Generate script of target database including changes.
        /// </summary>
        /// <returns>Script.</returns>
        public override string ScriptDatabase() => _targetTabularModel.ScriptDatabase();

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_sourceTabularModel != null)
                    {
                        _sourceTabularModel.Dispose();
                    }
                    if (_targetTabularModel != null)
                    {
                        _targetTabularModel.Dispose();
                    }
                }

                _disposed = true;
            }
        }

    }
}
