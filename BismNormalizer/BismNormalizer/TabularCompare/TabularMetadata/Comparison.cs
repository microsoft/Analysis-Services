using System;
using System.Collections.Generic;
using BismNormalizer.TabularCompare.Core;
using Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents a source vs. target comparison of an SSAS tabular model. This class is for tabular models that use tabular metadata with SSAS compatibility level 1200 or above.
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

        public Comparison(ComparisonInfo comparisonInfo)
            : base(comparisonInfo)
        {
            _sourceTabularModel = new TabularModel(this, comparisonInfo.ConnectionInfoSource, comparisonInfo);
            _targetTabularModel = new TabularModel(this, comparisonInfo.ConnectionInfoTarget, comparisonInfo);
        }

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

            #region DataSources

            foreach (DataSource dataSourceSource in _sourceTabularModel.DataSources)
            {
                // check if source is not in target
                if (!_targetTabularModel.DataSources.ContainsName(dataSourceSource.Name))
                {
                    ComparisonObject comparisonObjectDataSource = new ComparisonObject(ComparisonObjectType.DataSource, ComparisonObjectStatus.MissingInTarget, dataSourceSource, null, MergeAction.Create);
                    _comparisonObjects.Add(comparisonObjectDataSource);
                    _comparisonObjectCount += 1;
                }
                else
                {
                    // there is a DataSource in the target with the same name at least
                    DataSource dataSourceTarget = _targetTabularModel.DataSources.FindByName(dataSourceSource.Name);
                    ComparisonObject comparisonObjectDataSource;

                    // check if DataSource object definition is different
                    if (dataSourceSource.ObjectDefinition != dataSourceTarget.ObjectDefinition)
                    {
                        comparisonObjectDataSource = new ComparisonObject(ComparisonObjectType.DataSource, ComparisonObjectStatus.DifferentDefinitions, dataSourceSource, dataSourceTarget, MergeAction.Update);
                        _comparisonObjects.Add(comparisonObjectDataSource);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        // they are equal, ...
                        comparisonObjectDataSource = new ComparisonObject(ComparisonObjectType.DataSource, ComparisonObjectStatus.SameDefinition, dataSourceSource, dataSourceTarget, MergeAction.Skip);
                        _comparisonObjects.Add(comparisonObjectDataSource);
                        _comparisonObjectCount += 1;
                    }
                }
            }

            foreach (DataSource dataSourceTarget in _targetTabularModel.DataSources)
            {
                // if target DataSource is Missing in Source, offer deletion
                if (!_sourceTabularModel.DataSources.ContainsName(dataSourceTarget.Name))
                {
                    ComparisonObject comparisonObjectDataSource = new ComparisonObject(ComparisonObjectType.DataSource, ComparisonObjectStatus.MissingInSource, null, dataSourceTarget, MergeAction.Delete);
                    _comparisonObjects.Add(comparisonObjectDataSource);
                    _comparisonObjectCount += 1;
                }
            }

            #endregion

            #region Tables

            foreach (Table tblSource in _sourceTabularModel.Tables)
            {
                // check if source is not in target
                TableCollection targetTablesForComparison = _targetTabularModel.Tables;

                if (!targetTablesForComparison.ContainsName(tblSource.Name))
                {
                    ComparisonObject comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.MissingInTarget, tblSource, null, MergeAction.Create);
                    _comparisonObjects.Add(comparisonObjectTable);
                    _comparisonObjectCount += 1;

                    #region Relationships for table Missing in Target

                    // all relationships in source are not in target (the target table doesn't even exist)
                    foreach (Relationship relSource in tblSource.Relationships)
                    {
                        ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInTarget, relSource, null, MergeAction.Create);
                        comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                        _comparisonObjectCount += 1;
                    }

                    #endregion

                    #region Measures / KPIs for Table that is Missing in Target

                    foreach (Measure measureSource in tblSource.Measures.FilterByTableName(tblSource.Name))
                    {
                        ComparisonObjectType comparisonObjectType = measureSource.IsKpi ? ComparisonObjectType.Kpi : ComparisonObjectType.Measure;
                        ComparisonObject comparisonObjectMeasure = new ComparisonObject(comparisonObjectType, ComparisonObjectStatus.MissingInTarget, measureSource, null, MergeAction.Create);
                        comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                        _comparisonObjectCount += 1;
                    }

                    #endregion
                }
                else
                {
                    //table name is in source and target

                    Table tblTarget = _targetTabularModel.Tables.FindByName(tblSource.Name);
                    ComparisonObject comparisonObjectTable;

                    if (tblSource.ObjectDefinition == tblTarget.ObjectDefinition)
                    {
                        comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.SameDefinition, tblSource, tblTarget, MergeAction.Skip);
                        _comparisonObjects.Add(comparisonObjectTable);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.DifferentDefinitions, tblSource, tblTarget, MergeAction.Update);
                        _comparisonObjects.Add(comparisonObjectTable);
                        _comparisonObjectCount += 1;
                    }

                    #region Relationships source/target tables exist

                    foreach (Relationship relSource in tblSource.Relationships)
                    {
                        // check if source is not in target
                        if (!tblTarget.Relationships.ContainsName(relSource.Name)) //Using Name, not InternalName in case internal name is different
                        {
                            ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInTarget, relSource, null, MergeAction.Create);
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                            _comparisonObjectCount += 1;
                        }
                        else
                        {
                            //relationship is in source and target

                            Relationship relTarget = tblTarget.Relationships.FindByName(relSource.Name);
                            ComparisonObject comparisonObjectRelationship;

                            if (relSource.ObjectDefinition == relTarget.ObjectDefinition)
                            {
                                comparisonObjectRelationship = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.SameDefinition, relSource, relTarget, MergeAction.Skip);
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelationship);
                                _comparisonObjectCount += 1;
                            }
                            else
                            {
                                comparisonObjectRelationship = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.DifferentDefinitions, relSource, relTarget, MergeAction.Update);
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelationship);
                                _comparisonObjectCount += 1;
                            }
                        }
                    }

                    // see if relationships in target table that don't exist in source table
                    foreach (Relationship relTarget in tblTarget.Relationships)
                    {
                        // check if source is not in target
                        if (!tblSource.Relationships.ContainsName(relTarget.Name)) //Using Name, not InternalName in case internal name is different
                        {
                            ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInSource, null, relTarget, MergeAction.Delete);
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                            _comparisonObjectCount += 1;
                        }
                    }

                    #endregion

                    #region Measures / KPIs (table in source and target)

                    // see if matching measure in source and target
                    foreach (Measure measureSource in tblSource.Measures.FilterByTableName(tblSource.Name))
                    {
                        ComparisonObjectType comparisonObjectType = measureSource.IsKpi ? ComparisonObjectType.Kpi : ComparisonObjectType.Measure;

                        if (tblTarget.Measures.FilterByTableName(tblTarget.Name).ContainsName(measureSource.Name))
                        {
                            //Measure in source and target, so check definition
                            Measure measureTarget = tblTarget.Measures.FilterByTableName(tblTarget.Name).FindByName(measureSource.Name);
                            if (measureSource.ObjectDefinition == measureTarget.ObjectDefinition)
                            {
                                //Measure has same definition
                                ComparisonObject comparisonObjectMeasure = new ComparisonObject(comparisonObjectType, ComparisonObjectStatus.SameDefinition, measureSource, measureTarget, MergeAction.Skip);
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                                _comparisonObjectCount += 1;
                            }
                            else
                            {
                                //Measure has different definition
                                ComparisonObject comparisonObjectMeasure = new ComparisonObject(comparisonObjectType, ComparisonObjectStatus.DifferentDefinitions, measureSource, measureTarget, MergeAction.Update);
                                comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                                _comparisonObjectCount += 1;
                            }
                        }
                        else
                        {
                            ComparisonObject comparisonObjectMeasure = new ComparisonObject(comparisonObjectType, ComparisonObjectStatus.MissingInTarget, measureSource, null, MergeAction.Create);
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                            _comparisonObjectCount += 1;
                        }
                    }
                    //now check if target contains measures Missing in Source
                    foreach (Measure measureTarget in tblTarget.Measures.FilterByTableName(tblTarget.Name))
                    {
                        ComparisonObjectType comparisonObjectType = measureTarget.IsKpi ? ComparisonObjectType.Kpi : ComparisonObjectType.Measure;
                        if (!tblSource.Measures.FilterByTableName(tblSource.Name).ContainsName(measureTarget.Name))
                        {
                            ComparisonObject comparisonObjectMeasure = new ComparisonObject(comparisonObjectType, ComparisonObjectStatus.MissingInSource, null, measureTarget, MergeAction.Delete);
                            comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                            _comparisonObjectCount += 1;
                        }
                    }

                    #endregion
                }
            }

            foreach (Table tblTarget in _targetTabularModel.Tables)
            {
                // check if target is not in source
                if (!_sourceTabularModel.Tables.ContainsName(tblTarget.Name))
                {
                    ComparisonObject comparisonObjectTable = new ComparisonObject(ComparisonObjectType.Table, ComparisonObjectStatus.MissingInSource, null, tblTarget, MergeAction.Delete);
                    _comparisonObjects.Add(comparisonObjectTable);
                    _comparisonObjectCount += 1;

                    #region Relationships for table Missing in Source

                    // all relationships in target are not in source (the source table doesn't even exist)
                    foreach (Relationship relTarget in tblTarget.Relationships)
                    {
                        ComparisonObject comparisonObjectRelation = new ComparisonObject(ComparisonObjectType.Relationship, ComparisonObjectStatus.MissingInSource, null, relTarget, MergeAction.Delete);
                        comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectRelation);
                        _comparisonObjectCount += 1;
                    }

                    #endregion

                    #region Measures for Table that is Missing in Source

                    foreach (Measure measureTarget in tblTarget.Measures.FilterByTableName(tblTarget.Name))
                    {
                        ComparisonObjectType comparisonObjectType = measureTarget.IsKpi ? ComparisonObjectType.Kpi : ComparisonObjectType.Measure;
                        ComparisonObject comparisonObjectMeasure = new ComparisonObject(ComparisonObjectType.Measure, ComparisonObjectStatus.MissingInSource, null, measureTarget, MergeAction.Delete);
                        comparisonObjectTable.ChildComparisonObjects.Add(comparisonObjectMeasure);
                        _comparisonObjectCount += 1;
                    }

                    #endregion
                }
            }
            
            #endregion

            #region Expressions

            foreach (Expression expressionSource in _sourceTabularModel.Expressions)
            {
                // check if source is not in target
                if (!_targetTabularModel.Expressions.ContainsName(expressionSource.Name))
                {
                    ComparisonObject comparisonObjectExpression = new ComparisonObject(ComparisonObjectType.Expression, ComparisonObjectStatus.MissingInTarget, expressionSource, null, MergeAction.Create);
                    _comparisonObjects.Add(comparisonObjectExpression);
                    _comparisonObjectCount += 1;
                }
                else
                {
                    // there is a expression in the target with the same name at least
                    Expression expressionTarget = _targetTabularModel.Expressions.FindByName(expressionSource.Name);
                    ComparisonObject comparisonObjectExpression;

                    // check if expression object definition is different
                    if (expressionSource.ObjectDefinition != expressionTarget.ObjectDefinition)
                    {
                        comparisonObjectExpression = new ComparisonObject(ComparisonObjectType.Expression, ComparisonObjectStatus.DifferentDefinitions, expressionSource, expressionTarget, MergeAction.Update);
                        _comparisonObjects.Add(comparisonObjectExpression);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        // they are equal, ...
                        comparisonObjectExpression = new ComparisonObject(ComparisonObjectType.Expression, ComparisonObjectStatus.SameDefinition, expressionSource, expressionTarget, MergeAction.Skip);
                        _comparisonObjects.Add(comparisonObjectExpression);
                        _comparisonObjectCount += 1;
                    }
                }
            }

            foreach (Expression expressionTarget in _targetTabularModel.Expressions)
            {
                // if target expression is Missing in Source, offer deletion
                if (!_sourceTabularModel.Expressions.ContainsName(expressionTarget.Name))
                {
                    ComparisonObject comparisonObjectExpression = new ComparisonObject(ComparisonObjectType.Expression, ComparisonObjectStatus.MissingInSource, null, expressionTarget, MergeAction.Delete);
                    _comparisonObjects.Add(comparisonObjectExpression);
                    _comparisonObjectCount += 1;
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
                        ComparisonObject comparisonObjectPerspective = new ComparisonObject(ComparisonObjectType.Perspective, ComparisonObjectStatus.MissingInTarget, perspectiveSource, null, MergeAction.Create);
                        _comparisonObjects.Add(comparisonObjectPerspective);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        // there is a perspective in the target with the same name at least
                        Perspective perspectiveTarget = _targetTabularModel.Perspectives.FindByName(perspectiveSource.Name);
                        ComparisonObject comparisonObjectPerspective;

                        // check if perspective object definition is different
                        //if (perspectiveSource.ObjectDefinition != perspectiveTarget.ObjectDefinition)
                        if ( (_comparisonInfo.OptionsInfo.OptionMergePerspectives && perspectiveTarget.ContainsOtherPerspectiveSelections(perspectiveSource)) ||
                             (!_comparisonInfo.OptionsInfo.OptionMergePerspectives && perspectiveTarget.ContainsOtherPerspectiveSelections(perspectiveSource) && perspectiveSource.ContainsOtherPerspectiveSelections(perspectiveTarget)) )
                        {
                            // they are equal, ...
                            comparisonObjectPerspective = new ComparisonObject(ComparisonObjectType.Perspective, ComparisonObjectStatus.SameDefinition, perspectiveSource, perspectiveTarget, MergeAction.Skip);
                            _comparisonObjects.Add(comparisonObjectPerspective);
                            _comparisonObjectCount += 1;
                        }
                        else
                        {
                            comparisonObjectPerspective = new ComparisonObject(ComparisonObjectType.Perspective, ComparisonObjectStatus.DifferentDefinitions, perspectiveSource, perspectiveTarget, MergeAction.Update);
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
                        ComparisonObject comparisonObjectPerspective = new ComparisonObject(ComparisonObjectType.Perspective, ComparisonObjectStatus.MissingInSource, null, perspectiveTarget, MergeAction.Delete);
                        _comparisonObjects.Add(comparisonObjectPerspective);
                        _comparisonObjectCount += 1;
                    }
                }
            }

            #endregion

            #region Cultures

            if (_comparisonInfo.OptionsInfo.OptionCultures)
            {
                foreach (Culture cultureSource in _sourceTabularModel.Cultures)
                {
                    // check if source is not in target
                    if (!_targetTabularModel.Cultures.ContainsName(cultureSource.Name))
                    {
                        ComparisonObject comparisonObjectCulture = new ComparisonObject(ComparisonObjectType.Culture, ComparisonObjectStatus.MissingInTarget, cultureSource, null, MergeAction.Create);
                        _comparisonObjects.Add(comparisonObjectCulture);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        // there is a culture in the target with the same name at least
                        Culture cultureTarget = _targetTabularModel.Cultures.FindByName(cultureSource.Name);
                        ComparisonObject comparisonObjectCulture;

                        // check if culture object definition is different
                        //if (cultureSource.ObjectDefinition != cultureTarget.ObjectDefinition)
                        if ((_comparisonInfo.OptionsInfo.OptionMergeCultures && cultureTarget.ContainsOtherCultureTranslations(cultureSource)) ||
                             (!_comparisonInfo.OptionsInfo.OptionMergeCultures && cultureTarget.ContainsOtherCultureTranslations(cultureSource) && cultureSource.ContainsOtherCultureTranslations(cultureTarget)))
                        {
                            // they are equal, ...
                            comparisonObjectCulture = new ComparisonObject(ComparisonObjectType.Culture, ComparisonObjectStatus.SameDefinition, cultureSource, cultureTarget, MergeAction.Skip);
                            _comparisonObjects.Add(comparisonObjectCulture);
                            _comparisonObjectCount += 1;
                        }
                        else
                        {
                            comparisonObjectCulture = new ComparisonObject(ComparisonObjectType.Culture, ComparisonObjectStatus.DifferentDefinitions, cultureSource, cultureTarget, MergeAction.Update);
                            _comparisonObjects.Add(comparisonObjectCulture);
                            _comparisonObjectCount += 1;
                        }
                    }
                }

                foreach (Culture cultureTarget in _targetTabularModel.Cultures)
                {
                    // if target culture is Missing in Source, offer deletion
                    if (!_sourceTabularModel.Cultures.ContainsName(cultureTarget.Name))
                    {
                        ComparisonObject comparisonObjectCulture = new ComparisonObject(ComparisonObjectType.Culture, ComparisonObjectStatus.MissingInSource, null, cultureTarget, MergeAction.Delete);
                        _comparisonObjects.Add(comparisonObjectCulture);
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
                        ComparisonObject comparisonObjectRole = new ComparisonObject(ComparisonObjectType.Role, ComparisonObjectStatus.MissingInTarget, roleSource, null, MergeAction.Create);
                        _comparisonObjects.Add(comparisonObjectRole);
                        _comparisonObjectCount += 1;
                    }
                    else
                    {
                        // there is a role in the target with the same name at least
                        Role roleTarget = _targetTabularModel.Roles.FindByName(roleSource.Name);
                        ComparisonObject comparisonObjectRole;

                        // check if role object definition is different
                        if (roleSource.ObjectDefinition != roleTarget.ObjectDefinition)
                        {
                            comparisonObjectRole = new ComparisonObject(ComparisonObjectType.Role, ComparisonObjectStatus.DifferentDefinitions, roleSource, roleTarget, MergeAction.Update);
                            _comparisonObjects.Add(comparisonObjectRole);
                            _comparisonObjectCount += 1;
                        }
                        else
                        {
                            // they are equal, ...
                            comparisonObjectRole = new ComparisonObject(ComparisonObjectType.Role, ComparisonObjectStatus.SameDefinition, roleSource, roleTarget, MergeAction.Skip);
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
                        ComparisonObject comparisonObjectRole = new ComparisonObject(ComparisonObjectType.Role, ComparisonObjectStatus.MissingInSource, null, roleTarget, MergeAction.Delete);
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
            _lastSourceSchemaUpdate = _sourceTabularModel.TomDatabase.LastSchemaUpdate;
            _lastTargetSchemaUpdate = _targetTabularModel.TomDatabase.LastSchemaUpdate;
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
            }
            else
            {
                _sourceTabularModel.TomDatabase.Refresh();
                _targetTabularModel.TomDatabase.Refresh();
            }

            if (!_sourceTabularModel.ConnectionInfo.UseProject && _sourceTabularModel.TomDatabase.LastSchemaUpdate > _lastSourceSchemaUpdate)
            {
                throw new Exception("The definition of the source database has changed since the comparison was run.  Please re-run the comparison.");
            }
            if (!_targetTabularModel.ConnectionInfo.UseProject && _targetTabularModel.TomDatabase.LastSchemaUpdate > _lastTargetSchemaUpdate)
            {
                throw new Exception("The definition of the target database has changed since the comparison was run.  Please re-run the comparison.");
            }

            _uncommitedChanges = true;

            #endregion

            #region Iterate of objects for delete/create/updates

            #region Backup perspectives, cultures and roles

            /*It's easier to take a backup of perspectives, cultures and roles now and add back after table changes, rather than every
              time update a table, take a temp backup to add back columns/measures. Also would need to remove deleted tables/meausures, ... 
              Gets pretty hairy.
            */

            _targetTabularModel.BackupAffectedObjects();

            #endregion

            #region DataSources

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                DeleteDataSource(comparisonObject);
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                CreateDataSource(comparisonObject);
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                UpdateDataSource(comparisonObject);
            }

            #endregion

            #region Expressions

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                DeleteExpression(comparisonObject);
            }

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                CreateExpression(comparisonObject);
            }

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                UpdateExpression(comparisonObject);
            }

            #endregion

            #region Tables

            // do deletions first to minimize chance of conflict
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                DeleteTable(comparisonObject);
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                CreateTable(comparisonObject);
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                UpdateTable(comparisonObject);
            }

            #endregion

            #region Relationships

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    DeleteRelationship(childComparisonObject);                                    //Relationship
                }
            }

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    CreateRelationship(childComparisonObject, comparisonObject.SourceObjectName); //Relationship, Table
                }
            }

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    UpdateRelationship(childComparisonObject, comparisonObject.SourceObjectName); //Relationship, Table
                }
            }

            _targetTabularModel.ValidateRelationships();

            #endregion

            _targetTabularModel.CleanUpVariations();

            #region Measures / KPIs

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    DeleteMeasure(childComparisonObject);                                    //Measure
                }
            }

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    CreateMeasure(childComparisonObject, comparisonObject.SourceObjectName); //Measure, Table
                }
            }

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                foreach (ComparisonObject childComparisonObject in comparisonObject.ChildComparisonObjects)
                {
                    UpdateMeasure(childComparisonObject, comparisonObject.SourceObjectName); //Measure, Table
                }
            }

            #endregion

            #region Perspectives

            //Restore perspectives that were backed up earlier. Having done this there won't be any dependency issues, so can start comparison changes.
            _targetTabularModel.RestorePerspectives();

            // Do separate loops of _comparisonObjectects for Delete, Create, Update to ensure informational logging order is consistent with other object types. This also ensures deletions are done first to minimize chance of conflict.
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Perspective && comparisonObject.MergeAction == MergeAction.Delete)
                {
                    _targetTabularModel.DeletePerspective(comparisonObject.TargetObjectInternalName);
                    OnValidationMessage(new ValidationMessageEventArgs($"Delete perspective [{comparisonObject.TargetObjectName}].", ValidationMessageType.Perspective, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Perspective && comparisonObject.MergeAction == MergeAction.Create)
                {
                    _targetTabularModel.CreatePerspective(_sourceTabularModel.Perspectives.FindById(comparisonObject.SourceObjectInternalName).TomPerspective);
                    OnValidationMessage(new ValidationMessageEventArgs($"Create perspective [{comparisonObject.SourceObjectName}].", ValidationMessageType.Perspective, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Perspective && comparisonObject.MergeAction == MergeAction.Update)
                {
                    _targetTabularModel.UpdatePerspective(_sourceTabularModel.Perspectives.FindById(comparisonObject.SourceObjectInternalName).TomPerspective, _targetTabularModel.Perspectives.FindById(comparisonObject.TargetObjectInternalName).TomPerspective);
                    OnValidationMessage(new ValidationMessageEventArgs($"Update perspective [{comparisonObject.TargetObjectName}].", ValidationMessageType.Perspective, ValidationMessageStatus.Informational));
                }
            }

            #endregion

            #region Roles

            //Restore roles that were backed up earlier. Having done this there won't be any dependency issues, so can start comparison changes.
            _targetTabularModel.RestoreRoles();

            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Role && comparisonObject.MergeAction == MergeAction.Delete)
                {
                    _targetTabularModel.DeleteRole(comparisonObject.TargetObjectInternalName);
                    OnValidationMessage(new ValidationMessageEventArgs($"Delete role [{comparisonObject.TargetObjectName}].", ValidationMessageType.Role, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Role && comparisonObject.MergeAction == MergeAction.Create)
                {
                    _targetTabularModel.CreateRole(_sourceTabularModel.Roles.FindById(comparisonObject.SourceObjectInternalName).TomRole);
                    OnValidationMessage(new ValidationMessageEventArgs($"Create role [{comparisonObject.SourceObjectName}].", ValidationMessageType.Role, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Role && comparisonObject.MergeAction == MergeAction.Update)
                {
                    _targetTabularModel.UpdateRole(_sourceTabularModel.Roles.FindById(comparisonObject.SourceObjectInternalName), _targetTabularModel.Roles.FindById(comparisonObject.TargetObjectInternalName));
                    OnValidationMessage(new ValidationMessageEventArgs($"Update role [{comparisonObject.TargetObjectName}].", ValidationMessageType.Role, ValidationMessageStatus.Informational));
                }
            }

            _targetTabularModel.RolesCleanup();

            #endregion

            #region Cultures

            //Restore cultures that were backed up earlier. Having done this there won't be any dependency issues, so can start comparison changes.
            //Note that cannot restore cultures before finished perspective comparison changes above, because cultures can have dependencies on perspectives.
            _targetTabularModel.RestoreCultues();

            // Do separate loops of _comparisonObjectects for Delete, Create, Update to ensure informational logging order is consistent with other object types. This also ensures deletions are done first to minimize chance of conflict.
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Culture && comparisonObject.MergeAction == MergeAction.Delete)
                {
                    _targetTabularModel.DeleteCulture(comparisonObject.TargetObjectInternalName);
                    OnValidationMessage(new ValidationMessageEventArgs($"Delete culture [{comparisonObject.TargetObjectName}].", ValidationMessageType.Culture, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Culture && comparisonObject.MergeAction == MergeAction.Create)
                {
                    _targetTabularModel.CreateCulture(_sourceTabularModel.Cultures.FindById(comparisonObject.SourceObjectInternalName).TomCulture);
                    OnValidationMessage(new ValidationMessageEventArgs($"Create culture [{comparisonObject.SourceObjectName}].", ValidationMessageType.Culture, ValidationMessageStatus.Informational));
                }
            }
            foreach (ComparisonObject comparisonObject in _comparisonObjects)
            {
                if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Culture && comparisonObject.MergeAction == MergeAction.Update)
                {
                    _targetTabularModel.UpdateCulture(_sourceTabularModel.Cultures.FindById(comparisonObject.SourceObjectInternalName).TomCulture, _targetTabularModel.Cultures.FindById(comparisonObject.TargetObjectInternalName).TomCulture);
                    OnValidationMessage(new ValidationMessageEventArgs($"Update culture [{comparisonObject.TargetObjectName}].", ValidationMessageType.Culture, ValidationMessageStatus.Informational));
                }
            }

            #endregion

            #endregion

            #region Missing measure dependencies

            if (_comparisonInfo.OptionsInfo.OptionMeasureDependencies)
            {
                foreach (Table table in _targetTabularModel.Tables)
                {
                    foreach (Measure measure in table.Measures)
                    {
                        foreach (string missingDependency in measure.FindMissingMeasureDependencies())
                        {
                            OnValidationMessage(new ValidationMessageEventArgs($"Measure [{measure.InternalName}] in table '{table.Name}' contains dependency on measure/column [{missingDependency}], which (considering changes to target) cannot be found in target model.", ValidationMessageType.MeasureCalculationDependency, ValidationMessageStatus.Warning));
                        }
                    }
                }
            }

            #endregion

            OnResizeValidationHeaders(new EventArgs());
        }

        #region Private methods for validation

        #region Calc dependencies validation

        private bool HasBlockingToDependenciesInTarget(string targetObjectName, CalcDependencyObjectType targetObjectType, ref List<string> warningObjectList)
        {
            //For deletion.
            //Check any objects in target that depend on this object are also going to be deleted or updated.

            bool returnVal = false;
            CalcDependencyCollection targetToDepdendencies = _targetTabularModel.MDependencies.DependenciesReferenceTo(targetObjectType, targetObjectName);
            foreach (CalcDependency targetToDependency in targetToDepdendencies)
            {
                foreach (ComparisonObject comparisonObjectToCheck in _comparisonObjects)
                {
                    switch (targetToDependency.ObjectType)
                    {
                        case CalcDependencyObjectType.Expression:
                            //Does this expression (comparisonObjectToCheck) have a dependency on the object about to be deleted (targetObjectName)?

                            if (comparisonObjectToCheck.ComparisonObjectType == ComparisonObjectType.Expression &&
                                comparisonObjectToCheck.TargetObjectName == targetToDependency.ObjectName &&
                                (
                                    comparisonObjectToCheck.MergeAction == MergeAction.Skip ||       //Skip covers if this expression is for deletion and being skipped, or if same defintion and not being touched in target (in either case, dependency will remain).
                                    (
                                        comparisonObjectToCheck.MergeAction == MergeAction.Update && //Updates (if successful) are fine because covered by source dependency checking. So need to check if the update will be unsuccessful (and therefore dependency will remain).
                                        HasBlockingFromDependenciesInSource(
                                            "", //Expressions don't have table value
                                            comparisonObjectToCheck.TargetObjectName, 
                                            CalcDependencyObjectType.Expression)
                                    )                                                                //Create expression is not possible to have a dependency on this object about to be deleted. Delete expression is fine.
                                )
                            )
                            {
                                string warningObject = $"Expression {comparisonObjectToCheck.TargetObjectName}";
                                if (!warningObjectList.Contains(warningObject))
                                {
                                    warningObjectList.Add(warningObject);
                                }
                                returnVal = true;
                            }
                            break;
                        case CalcDependencyObjectType.Partition:
                            //Does this table (comparisonObjectToCheck) have a dependency on the object about to be deleted (targetObjectName)?

                            if (comparisonObjectToCheck.ComparisonObjectType == ComparisonObjectType.Table &&
                                comparisonObjectToCheck.TargetObjectName == targetToDependency.TableName &&
                                (
                                    comparisonObjectToCheck.MergeAction == MergeAction.Skip ||       //Skip covers if this table is for deletion and being skipped, or if same defintion and not being touched in target (in either case, dependency will remain).
                                    (
                                        comparisonObjectToCheck.MergeAction == MergeAction.Update && //Updates (if successful) are fine because covered by source dependency checking. So need to check if the update will be unsuccessful (and therefore dependency will remain).
                                        (
                                            HasBlockingFromDependenciesInSourceForTable(_sourceTabularModel.Tables.FindByName(comparisonObjectToCheck.TargetObjectName)) ||
                                            _targetTabularModel.CanRetainPartitions(                 //But also check if doing retain partitions on this table (if so, dependency will remain).
                                                _sourceTabularModel.Tables.FindByName(comparisonObjectToCheck.TargetObjectName),
                                                _targetTabularModel.Tables.FindByName(comparisonObjectToCheck.TargetObjectName),
                                                out string retainPartitionsMessage)
                                        )
                                    )                                                                //Create table is not possible to have a dependency on this object about to be deleted. Delete table is fine.
                                )
                            )
                            {
                                string warningObject = $"Table {comparisonObjectToCheck.TargetObjectName}/Partition {targetToDependency.ObjectName}";
                                if (!warningObjectList.Contains(warningObject))
                                {
                                    warningObjectList.Add(warningObject);
                                }
                                returnVal = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return returnVal;
        }

        private bool HasBlockingFromDependenciesInSource(string sourceTableName, string sourceObjectName, CalcDependencyObjectType sourceObjectType, ref List<string> warningObjectList, out bool nonStructuredDataSource)
        {
            //For creation and updates.
            //Check any objects in source that this object depends on are also going to be created OR updated (if not already in target).

            bool returnVal = false;
            nonStructuredDataSource = false;

            CalcDependencyCollection sourceFromDepdendencies = _sourceTabularModel.MDependencies.DependenciesReferenceFrom(sourceObjectType, sourceTableName, sourceObjectName);
            foreach (CalcDependency sourceFromDependency in sourceFromDepdendencies)
            {
                foreach (ComparisonObject comparisonObjectToCheck in _comparisonObjects)
                {
                    switch (sourceFromDependency.ReferencedObjectType)
                    {
                        case CalcDependencyObjectType.Expression:
                            //Does the object about to be created/updated (sourceObjectName) have a source dependency on this expression (comparisonObjectToCheck)?

                            if (!_targetTabularModel.Expressions.ContainsName(sourceFromDependency.ReferencedObjectName) &&
                                comparisonObjectToCheck.ComparisonObjectType == ComparisonObjectType.Expression &&
                                comparisonObjectToCheck.SourceObjectName == sourceFromDependency.ReferencedObjectName &&
                                comparisonObjectToCheck.Status == ComparisonObjectStatus.MissingInTarget &&  //Creates being skipped (dependency will be missing).
                                comparisonObjectToCheck.MergeAction == MergeAction.Skip)
                                //Deletes are impossible for this object to depend on, so don't need to detect. Other Skips can assume are fine, so don't need to detect.
                            {
                                string warningObject = $"Expression {comparisonObjectToCheck.SourceObjectName}";
                                if (!warningObjectList.Contains(warningObject))
                                {
                                    warningObjectList.Add(warningObject);
                                }
                                returnVal = true;
                            }

                            break;
                        case CalcDependencyObjectType.DataSource:
                            //Does the object about to be created/updated (sourceObjectName) have a source dependency on this data source (comparisonObjectToCheck)?

                            if (!_targetTabularModel.DataSources.ContainsName(sourceFromDependency.ReferencedObjectName) &&
                                comparisonObjectToCheck.ComparisonObjectType == ComparisonObjectType.DataSource &&
                                comparisonObjectToCheck.SourceObjectName == sourceFromDependency.ReferencedObjectName &&
                                comparisonObjectToCheck.Status == ComparisonObjectStatus.MissingInTarget &&  //Creates being skipped (dependency will be missing).
                                comparisonObjectToCheck.MergeAction == MergeAction.Skip)
                                //Deletes are impossible for this object to depend on, so don't need to detect. Other Skips can assume are fine, so don't need to detect.
                            {
                                string warningObject = $"Data Source {comparisonObjectToCheck.SourceObjectName}";
                                if (!warningObjectList.Contains(warningObject))
                                {
                                    warningObjectList.Add(warningObject);
                                }
                                returnVal = true;
                            }

                            //Check if target data source type is provider and source is structured. Won't be updated.
                            if (comparisonObjectToCheck.ComparisonObjectType == ComparisonObjectType.DataSource &&
                                comparisonObjectToCheck.Status == ComparisonObjectStatus.DifferentDefinitions &&
                                comparisonObjectToCheck.SourceObjectName == sourceFromDependency.ReferencedObjectName &&
                                _targetTabularModel.DataSources.ContainsName(sourceFromDependency.ReferencedObjectName) &&
                                _sourceTabularModel.DataSources.ContainsName(sourceFromDependency.ReferencedObjectName) &&
                                _targetTabularModel.DataSources.FindByName(sourceFromDependency.ReferencedObjectName).TomDataSource.Type == DataSourceType.Provider &&
                                _sourceTabularModel.DataSources.FindByName(sourceFromDependency.ReferencedObjectName).TomDataSource.Type == DataSourceType.Structured) //Don't need to check if Skip or not because can't update if different data source types anyway
                            {
                                string warningObject = $"Data Source {comparisonObjectToCheck.SourceObjectName}";
                                if (!warningObjectList.Contains(warningObject))
                                {
                                    warningObjectList.Add(warningObject);
                                }
                                returnVal = true;
                                nonStructuredDataSource = true;
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
            return returnVal;
        }

        private bool HasBlockingFromDependenciesInSource(string sourceTableName, string sourceObjectName, CalcDependencyObjectType sourceObjectType)
        {
            List<string> warningObjectList = new List<string>();
            return HasBlockingFromDependenciesInSource(sourceTableName, sourceObjectName, sourceObjectType, ref warningObjectList, out bool nonStructuredDataSource);
        }

        private bool HasBlockingFromDependenciesInSourceForTable(Table sourceTable)
        {
            bool returnVal = false;
            foreach (Partition partition in sourceTable.TomTable.Partitions)
            {
                if (HasBlockingFromDependenciesInSource(sourceTable.Name, partition.Name, CalcDependencyObjectType.Partition))
                {
                    returnVal = true;
                    break;
                }
            }
            return returnVal;
        }

        private bool HasBlockingOldPartitionDependency(Partition partition, ref List<string> warningObjectList)
        {
            //Only for old partition types

            bool returnVal = false;

            if (partition.SourceType == PartitionSourceType.Query &&
                !_targetTabularModel.DataSources.ContainsName(((QueryPartitionSource)partition.Source).DataSource.Name))
            {
                string dataSourceName = ((QueryPartitionSource)partition.Source).DataSource.Name;

                //For old non-M partitions, check if data source references exist
                foreach (ComparisonObject comparisonObjectToCheck in _comparisonObjects)
                {
                    if (comparisonObjectToCheck.ComparisonObjectType == ComparisonObjectType.DataSource &&
                        comparisonObjectToCheck.SourceObjectName == dataSourceName &&
                        comparisonObjectToCheck.Status == ComparisonObjectStatus.MissingInTarget &&
                        comparisonObjectToCheck.MergeAction == MergeAction.Skip)
                    {
                        string warningObject = $"Data Source {dataSourceName}";
                        if (!warningObjectList.Contains(warningObject))
                        {
                            warningObjectList.Add(warningObject);
                        }
                        returnVal = true;
                    }
                }
            }
            return returnVal;
        }

        #endregion

        //DataSources

        private void DeleteDataSource(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.DataSource && comparisonObject.MergeAction == MergeAction.Delete)
            {
                //Check any objects in target that depend on the DataSource are also going to be deleted
                List<string> warningObjectList = new List<string>();
                bool toDependencies = HasBlockingToDependenciesInTarget(comparisonObject.TargetObjectName, CalcDependencyObjectType.DataSource, ref warningObjectList);

                //For old non-M partitions, check if any such tables have reference to this DataSource, and will not be deleted
                foreach (Table table in _targetTabularModel.Tables)
                {
                    foreach (Partition partition in table.TomTable.Partitions)
                    {
                        if (partition.SourceType == PartitionSourceType.Query && 
                            table.DataSourceName == comparisonObject.TargetObjectName)
                        {
                            foreach (ComparisonObject comparisonObjectToCheck in _comparisonObjects)
                            {
                                if (
                                       (
                                            comparisonObjectToCheck.ComparisonObjectType == ComparisonObjectType.Table &&
                                            comparisonObjectToCheck.TargetObjectName == table.Name
                                       ) &&
                                       (
                                           (    //Skipped deletes, dependency will remain
                                                comparisonObjectToCheck.Status == ComparisonObjectStatus.MissingInSource &&
                                                comparisonObjectToCheck.MergeAction == MergeAction.Skip
                                           ) ||
                                           (    //Same definition, dependency will remain
                                                comparisonObjectToCheck.Status == ComparisonObjectStatus.SameDefinition
                                           ) ||
                                           (    //Different definition, and skip update (source already dependencies covered), dependency will remain
                                                comparisonObjectToCheck.Status == ComparisonObjectStatus.DifferentDefinitions &&
                                                comparisonObjectToCheck.MergeAction == MergeAction.Skip
                                           )
                                       )
                                )
                                {
                                    string warningObject = $"Table {table.Name}/Partition {partition.Name}";
                                    if (!warningObjectList.Contains(warningObject))
                                    {
                                        warningObjectList.Add(warningObject);
                                    }
                                    toDependencies = true;
                                }
                            }
                        }
                    }
                }

                if (!toDependencies)
                {
                    _targetTabularModel.DeleteDataSource(comparisonObject.TargetObjectName);
                    OnValidationMessage(new ValidationMessageEventArgs($"Delete data source [{comparisonObject.TargetObjectName}].", ValidationMessageType.DataSource, ValidationMessageStatus.Informational));
                }
                else
                {
                    OnValidationMessage(new ValidationMessageEventArgs($"Unable to delete data source {comparisonObject.TargetObjectName} because the following object(s) depend on it: {String.Join(", ", warningObjectList)}.", ValidationMessageType.DataSource, ValidationMessageStatus.Warning));
                }
            }
        }

        private void CreateDataSource(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.DataSource && comparisonObject.MergeAction == MergeAction.Create)
            {
                _targetTabularModel.CreateDataSource(_sourceTabularModel.DataSources.FindByName(comparisonObject.SourceObjectName));
                OnValidationMessage(new ValidationMessageEventArgs($"Create data source [{comparisonObject.SourceObjectName}].", ValidationMessageType.DataSource, ValidationMessageStatus.Informational));
            }
        }

        private void UpdateDataSource(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.DataSource && comparisonObject.MergeAction == MergeAction.Update)
            {
                DataSource sourceDataSource = _sourceTabularModel.DataSources.FindByName(comparisonObject.SourceObjectName);
                DataSource targetDataSource = _targetTabularModel.DataSources.FindByName(comparisonObject.TargetObjectName);

                if (sourceDataSource.TomDataSource.Type != targetDataSource.TomDataSource.Type)
                {
                    OnValidationMessage(new ValidationMessageEventArgs($"Unable to update data source {comparisonObject.TargetObjectName} because the source/target types (provider/structured) don't match, which is not supported.", ValidationMessageType.DataSource, ValidationMessageStatus.Warning));
                }
                else
                {
                    _targetTabularModel.UpdateDataSource(sourceDataSource, targetDataSource);
                    OnValidationMessage(new ValidationMessageEventArgs($"Update data source [{comparisonObject.TargetObjectName}].", ValidationMessageType.DataSource, ValidationMessageStatus.Informational));
                }
            }
        }

        //Expressions

        private void DeleteExpression(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Expression && comparisonObject.MergeAction == MergeAction.Delete)
            {
                //Check any objects in target that depend on the expression are also going to be deleted
                List<string> warningObjectList = new List<string>();
                if (!HasBlockingToDependenciesInTarget(comparisonObject.TargetObjectName, CalcDependencyObjectType.Expression, ref warningObjectList))
                {
                    _targetTabularModel.DeleteExpression(comparisonObject.TargetObjectName);
                    OnValidationMessage(new ValidationMessageEventArgs($"Delete expression [{comparisonObject.TargetObjectName}].", ValidationMessageType.Expression, ValidationMessageStatus.Informational));
                }
                else
                {
                    string message = $"Unable to delete expression {comparisonObject.TargetObjectName} because the following objects depend on it: {String.Join(", ", warningObjectList)}.";
                    if (_comparisonInfo.OptionsInfo.OptionRetainPartitions)
                    {
                        message += " Note: the option to retain partitions is on, which may be affecting this.";
                    }
                    OnValidationMessage(new ValidationMessageEventArgs(message, ValidationMessageType.Expression, ValidationMessageStatus.Warning));
                }
            }
        }

        private void CreateExpression(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Expression && comparisonObject.MergeAction == MergeAction.Create)
            {
                //Check any objects in source that this expression depends on are also going to be created if not already in target
                List<string> warningObjectList = new List<string>();
                if (!HasBlockingFromDependenciesInSource(
                    "", //can assume blank table for an expression
                    comparisonObject.SourceObjectName, 
                    CalcDependencyObjectType.Expression, 
                    ref warningObjectList, 
                    out bool nonStructuredDataSource))
                {
                    _targetTabularModel.CreateExpression(_sourceTabularModel.Expressions.FindByName(comparisonObject.SourceObjectName).TomExpression);
                    OnValidationMessage(new ValidationMessageEventArgs($"Create expression [{comparisonObject.SourceObjectName}].", ValidationMessageType.Expression, ValidationMessageStatus.Informational));
                }
                else
                {
                    if (!nonStructuredDataSource)
                    {
                        OnValidationMessage(new ValidationMessageEventArgs($"Unable to create expression {comparisonObject.SourceObjectName} because it depends on the following objects, which (considering changes) are missing from target: {String.Join(", ", warningObjectList)}.", ValidationMessageType.Expression, ValidationMessageStatus.Warning));
                    }
                    else
                    {
                        OnValidationMessage(new ValidationMessageEventArgs($"Unable to create expression {comparisonObject.SourceObjectName} because it depends on the following objects, which (considering changes) are missing from target and/or depend on a structured data source that is provider in the target: {String.Join(", ", warningObjectList)}.", ValidationMessageType.Expression, ValidationMessageStatus.Warning));
                    }
                }
            }
        }

        private void UpdateExpression(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Expression && comparisonObject.MergeAction == MergeAction.Update)
            {
                //Check any objects in source that this expression depends on are also going to be created if not already in target
                List<string> warningObjectList = new List<string>();
                if (!HasBlockingFromDependenciesInSource(
                    "", //Can assume blank table for expression
                    comparisonObject.SourceObjectName, 
                    CalcDependencyObjectType.Expression, 
                    ref warningObjectList, 
                    out bool nonStructuredDataSource))
                {
                    _targetTabularModel.UpdateExpression(_sourceTabularModel.Expressions.FindByName(comparisonObject.SourceObjectName), _targetTabularModel.Expressions.FindByName(comparisonObject.TargetObjectName));
                    OnValidationMessage(new ValidationMessageEventArgs($"Update expression [{comparisonObject.TargetObjectName}].", ValidationMessageType.Expression, ValidationMessageStatus.Informational));
                }
                else
                {
                    if (!nonStructuredDataSource)
                    {
                        OnValidationMessage(new ValidationMessageEventArgs($"Unable to update expression {comparisonObject.TargetObjectName} because version from the source depends on the following objects, which (considering changes) are missing from target: {String.Join(", ", warningObjectList)}.", ValidationMessageType.Expression, ValidationMessageStatus.Warning));
                    }
                    else
                    {
                        OnValidationMessage(new ValidationMessageEventArgs($"Unable to update expression {comparisonObject.TargetObjectName} because version from the source depends on the following objects, which (considering changes) are missing from target and/or depend on a structured data source that is provider in the target: {String.Join(", ", warningObjectList)}.", ValidationMessageType.Expression, ValidationMessageStatus.Warning));
                    }
                }
            }
        }

        //Tables

        private void DeleteTable(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Table && comparisonObject.MergeAction == MergeAction.Delete)
            {
                _targetTabularModel.DeleteTable(comparisonObject.TargetObjectName);
                OnValidationMessage(new ValidationMessageEventArgs($"Delete table '{comparisonObject.TargetObjectName}'.", ValidationMessageType.Table, ValidationMessageStatus.Informational));
            }
        }

        private void CreateTable(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Table && comparisonObject.MergeAction == MergeAction.Create)
            {
                Table sourceTable = _sourceTabularModel.Tables.FindByName(comparisonObject.SourceObjectName);
                List<string> warningObjectList = new List<string>();
                bool fromDependencies = false;
                bool nonStructuredDataSourceLocal = false;

                foreach (Partition partition in sourceTable.TomTable.Partitions)
                {
                    //Check any objects in source that this partition depends on are also going to be created if not already in target
                    if (HasBlockingFromDependenciesInSource(sourceTable.Name, partition.Name, CalcDependencyObjectType.Partition, ref warningObjectList, out bool nonStructuredDataSource))
                    {
                        fromDependencies = true;
                        if (nonStructuredDataSource)
                            nonStructuredDataSourceLocal = true;
                    }

                    //For old non-M partitions, check if data source references exist
                    if (HasBlockingOldPartitionDependency(partition, ref warningObjectList))
                        fromDependencies = true;  //Need if clause in case last of n partitions has no dependencies and sets back to true
                }

                if (!fromDependencies)
                {
                    _targetTabularModel.CreateTable(sourceTable);
                    OnValidationMessage(new ValidationMessageEventArgs($"Create table '{comparisonObject.SourceObjectName}'.", ValidationMessageType.Table, ValidationMessageStatus.Informational));
                }
                else
                {
                    if (!nonStructuredDataSourceLocal)
                    {
                        OnValidationMessage(new ValidationMessageEventArgs($"Unable to create table {comparisonObject.SourceObjectName} because it depends on the following objects, which (considering changes) are missing from target: {String.Join(", ", warningObjectList)}.", ValidationMessageType.Table, ValidationMessageStatus.Warning));
                    }
                    else
                    {
                        OnValidationMessage(new ValidationMessageEventArgs($"Unable to create table {comparisonObject.SourceObjectName} because it depends on the following objects, which (considering changes) are missing from target and/or depend on a structured data source that is provider in the target: {String.Join(", ", warningObjectList)}.", ValidationMessageType.Table, ValidationMessageStatus.Warning));
                    }
                }
            }
        }

        private void UpdateTable(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Table && comparisonObject.MergeAction == MergeAction.Update)
            {
                Table tableSource = _sourceTabularModel.Tables.FindByName(comparisonObject.SourceObjectName);
                Table tableTarget = _targetTabularModel.Tables.FindByName(comparisonObject.TargetObjectName);
                List<string> warningObjectList = new List<string>();
                bool fromDependencies = false;
                bool nonStructuredDataSourceLocal = false;

                //Will this table retain partitions? If yes, don't need to bother with source dependency (target dependency checking will cover for deletes).
                if (!_targetTabularModel.CanRetainPartitions(tableSource, tableTarget, out string retainPartitionsMessageTemp))
                {
                    //Check any objects in source that this table depends on are also going to be created/updated if not already in target
                    foreach (Partition partition in tableSource.TomTable.Partitions)
                    {
                        if (HasBlockingFromDependenciesInSource(tableSource.Name, partition.Name, CalcDependencyObjectType.Partition, ref warningObjectList, out bool nonStructuredDataSource))
                        {
                            fromDependencies = true;
                            if (nonStructuredDataSource)
                                nonStructuredDataSourceLocal = true;
                        }

                        //For old non-M partitions, check if data source references exist
                        if (HasBlockingOldPartitionDependency(partition, ref warningObjectList))
                            fromDependencies = true;  //Need if clause in case last of n partitions has no dependencies and sets back to true
                    }
                }

                if (!fromDependencies)
                {
                    _targetTabularModel.UpdateTable(tableSource, tableTarget, out string retainPartitionsMessage);
                    OnValidationMessage(new ValidationMessageEventArgs($"Update table '{comparisonObject.TargetObjectName}'. {retainPartitionsMessage}", ValidationMessageType.Table, ValidationMessageStatus.Informational));
                }
                else
                {
                    if (!nonStructuredDataSourceLocal)
                    {
                        OnValidationMessage(new ValidationMessageEventArgs($"Unable to update table {comparisonObject.TargetObjectName} because version from the source depends on the following objects, which (considering changes) are missing from target: {String.Join(", ", warningObjectList)}.", ValidationMessageType.Table, ValidationMessageStatus.Warning));
                    }
                    else
                    {
                        OnValidationMessage(new ValidationMessageEventArgs($"Unable to update table {comparisonObject.TargetObjectName} because version from the source depends on the following objects, which (considering changes) are missing from target and/or depend on a structured data source that is provider in the target: {String.Join(", ", warningObjectList)}.", ValidationMessageType.Table, ValidationMessageStatus.Warning));
                    }
                }
            }
        }

        //Relationships

        private void DeleteRelationship(ComparisonObject comparisonObject)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Relationship && comparisonObject.MergeAction == MergeAction.Delete)
            {
                foreach (Table tableTarget in _targetTabularModel.Tables)
                {
                    Relationship relationshipTarget = tableTarget.Relationships.FindByName(comparisonObject.TargetObjectName.Trim());

                    if (relationshipTarget != null)
                    {
                        // Relationship may have already been deleted if parent table was deleted
                        tableTarget.DeleteRelationship(comparisonObject.TargetObjectInternalName);
                        break;
                    }
                }

                OnValidationMessage(new ValidationMessageEventArgs($"Delete relationship {comparisonObject.TargetObjectName.Trim()}.", ValidationMessageType.Relationship, ValidationMessageStatus.Informational));
            }
        }

        private void CreateRelationship(ComparisonObject comparisonObject, string tableName)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Relationship && comparisonObject.MergeAction == MergeAction.Create)
            {
                Table tableSource = _sourceTabularModel.Tables.FindByName(tableName);
                Table tableTarget = _targetTabularModel.Tables.FindByName(tableName);
                Relationship relationshipSource = tableSource.Relationships.FindByInternalName(comparisonObject.SourceObjectInternalName);
                Table parentTableSource = _sourceTabularModel.Tables.FindByName(relationshipSource.ToTableName);

                string warningMessage = $"Unable to create relationship {comparisonObject.SourceObjectName.Trim()} because (considering changes) necessary table/column(s) not found in target model.";
                if (tableTarget != null && tableTarget.CreateRelationshipWithValidation(relationshipSource, parentTableSource.TomTable, comparisonObject.SourceObjectName.Trim(), ref warningMessage))
                {
                    OnValidationMessage(new ValidationMessageEventArgs($"Create relationship {comparisonObject.SourceObjectName.Trim()}.", ValidationMessageType.Relationship, ValidationMessageStatus.Informational));
                }
                else
                {
                    OnValidationMessage(new ValidationMessageEventArgs(warningMessage, ValidationMessageType.Relationship, ValidationMessageStatus.Warning));
                }
            }
        }

        private void UpdateRelationship(ComparisonObject comparisonObject, string tableName)
        {
            if (comparisonObject.ComparisonObjectType == ComparisonObjectType.Relationship && comparisonObject.MergeAction == MergeAction.Update)
            {
                Table tableSource = _sourceTabularModel.Tables.FindByName(tableName);
                Table tableTarget = _targetTabularModel.Tables.FindByName(tableName);
                Relationship relationshipSource = tableSource.Relationships.FindByInternalName(comparisonObject.SourceObjectInternalName);
                Table parentTableSource = _sourceTabularModel.Tables.FindByName(relationshipSource.ToTableName);

                string warningMessage = "";
                if (tableTarget.UpdateRelationship(relationshipSource, parentTableSource.TomTable, comparisonObject.SourceObjectName.Trim(), ref warningMessage))
                {
                    OnValidationMessage(new ValidationMessageEventArgs($"Update relationship {comparisonObject.SourceObjectName.Trim()}.", ValidationMessageType.Relationship, ValidationMessageStatus.Informational));
                }
                else
                {
                    OnValidationMessage(new ValidationMessageEventArgs(warningMessage, ValidationMessageType.Relationship, ValidationMessageStatus.Warning));
                }
            }
        }

        //Measures / KPIs

        private void DeleteMeasure(ComparisonObject comparisonObject)
        {
            if ((comparisonObject.ComparisonObjectType == ComparisonObjectType.Measure || comparisonObject.ComparisonObjectType == ComparisonObjectType.Kpi) &&
                    comparisonObject.MergeAction == MergeAction.Delete)
            {
                foreach (Table tableTarget in _targetTabularModel.Tables)
                {
                    Measure measureTarget = tableTarget.Measures.FindByName(comparisonObject.TargetObjectInternalName);

                    if (measureTarget != null)
                    {
                        // Measure may have already been deleted if parent table was deleted
                        tableTarget.DeleteMeasure(comparisonObject.TargetObjectInternalName);
                        break;
                    }
                }

                OnValidationMessage(new ValidationMessageEventArgs($"Delete measure / KPI {comparisonObject.TargetObjectInternalName}.", ValidationMessageType.Measure, ValidationMessageStatus.Informational));
            }
        }

        private void CreateMeasure(ComparisonObject comparisonObject, string tableName)
        {
            if ((comparisonObject.ComparisonObjectType == ComparisonObjectType.Measure || comparisonObject.ComparisonObjectType == ComparisonObjectType.Kpi) &&
                    comparisonObject.MergeAction == MergeAction.Create)
            {
                foreach (Table tableInTarget in _targetTabularModel.Tables)
                {
                    Measure measureInTarget = tableInTarget.Measures.FindByName(comparisonObject.SourceObjectInternalName);

                    if (measureInTarget != null)
                    {
                        OnValidationMessage(new ValidationMessageEventArgs($"Unable to create measure / KPI {comparisonObject.SourceObjectInternalName} because name already exists in target model.", ValidationMessageType.Measure, ValidationMessageStatus.Warning));
                        return;
                    }
                }

                Table tableSource = _sourceTabularModel.Tables.FindByName(tableName);
                Table tableTarget = _targetTabularModel.Tables.FindByName(tableName);

                if (tableTarget == null)
                {
                    OnValidationMessage(new ValidationMessageEventArgs($"Unable to create measure / KPI {comparisonObject.SourceObjectInternalName} because (considering changes) target table does not exist.", ValidationMessageType.Measure, ValidationMessageStatus.Warning));
                    return;
                }

                //If we get here, can create measure/kpi
                Measure measureSource = tableSource.Measures.FindByName(comparisonObject.SourceObjectInternalName);
                tableTarget.CreateMeasure(measureSource.TomMeasure);
                OnValidationMessage(new ValidationMessageEventArgs($"Create measure / KPI {comparisonObject.SourceObjectInternalName}.", ValidationMessageType.Measure, ValidationMessageStatus.Informational));
            }
        }

        private void UpdateMeasure(ComparisonObject comparisonObject, string tableName)
        {
            if ((comparisonObject.ComparisonObjectType == ComparisonObjectType.Measure || comparisonObject.ComparisonObjectType == ComparisonObjectType.Kpi) &&
                    comparisonObject.MergeAction == MergeAction.Update)
            {
                Table tableSource = _sourceTabularModel.Tables.FindByName(tableName);
                Table tableTarget = _targetTabularModel.Tables.FindByName(tableName);
                Measure measureSource = tableSource.Measures.FindByName(comparisonObject.SourceObjectInternalName);

                tableTarget.UpdateMeasure(measureSource.TomMeasure);
                OnValidationMessage(new ValidationMessageEventArgs($"Update measure / KPI {comparisonObject.SourceObjectInternalName}.", ValidationMessageType.Measure, ValidationMessageStatus.Informational));
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
                        tablesToProcess.Add(new ProcessingTable(table.Name, table.InternalName));
                    }
                }
            }

            tablesToProcess.Sort();
            return tablesToProcess;
        }

        private void ProcessAffectedTables(Core.ComparisonObject comparisonObject, ProcessingTableCollection tablesToProcess)
        {
            //Recursively call for multiple levels to ensure catch calculated tables or those child of DataSource

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
