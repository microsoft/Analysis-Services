﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.AnalysisServices.Tabular;
using Tom = Microsoft.AnalysisServices.Tabular;
using Amo = Microsoft.AnalysisServices;
using Newtonsoft.Json.Linq;
using BismNormalizer.TabularCompare.Core;
//using System.Web.UI.WebControls.WebParts;
using System.Text.RegularExpressions;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model table with properties and methods for comparison purposes. This class can represent a database on a server, or a project in Visual Studio.
    /// </summary>
    public class TabularModel : IDisposable
    {
        #region Private Members

        private Comparison _parentComparison;
        private ConnectionInfo _connectionInfo;
        private ComparisonInfo _comparisonInfo;
        private Server _server;
        private Database _database;
        private Model _model;
        private DataSourceCollection _dataSources = new DataSourceCollection();
        private TableCollection _tables = new TableCollection();
        private ExpressionCollection _expressions = new ExpressionCollection();
        private PerspectiveCollection _perspectives = new PerspectiveCollection();
        private CultureCollection _cultures = new CultureCollection();
        private RoleCollection _roles = new RoleCollection();
        private CalcDependencyCollection _calcDependencies = new CalcDependencyCollection();
        private List<Tom.Perspective> _tomPerspectivesBackup;
        private List<Tom.Culture> _tomCulturesBackup;
        private List<Tom.ModelRole> _tomRolesBackup;
        private bool _disposed = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the TabularModel class using multiple parameters.
        /// </summary>
        /// <param name="parentComparison">Comparison object that the tabular model belongs to.</param>
        /// <param name="connectionInfo">ConnectionInfo object for the tabular model.</param>
        /// <param name="comparisonInfo">ComparisonInfo object for the tabular model.</param>
        public TabularModel(Comparison parentComparison, ConnectionInfo connectionInfo, ComparisonInfo comparisonInfo)
        {
            _parentComparison = parentComparison;
            _connectionInfo = connectionInfo;
            _comparisonInfo = comparisonInfo;
        }

        /// <summary>
        /// Connect to AS server and instantiate properties of the TabularModel object.
        /// </summary>
        public void Connect()
        {
            TomConnect();

            //Shell model
            _model = new Model(this, _database.Model);
            foreach (Tom.DataSource dataSource in _database.Model.DataSources)
            {
                if (dataSource.Type == DataSourceType.Provider || dataSource.Type == DataSourceType.Structured)
                {
                    _dataSources.Add(new DataSource(this, dataSource));
                }
            }
            foreach (Tom.Table table in _database.Model.Tables)
            {
                _tables.Add(new Table(this, table));
            }
            foreach (Tom.NamedExpression expression in _database.Model.Expressions)
            {
                _expressions.Add(new Expression(this, expression));
            }
            foreach (ModelRole role in _database.Model.Roles)
            {
                //Workaround for AAD role members - todo delete if changed in Azure AS
                if (_parentComparison?.TargetTabularModel?.ConnectionInfo?.ServerName?.Length > 7 &&
                    _parentComparison?.TargetTabularModel?.ConnectionInfo?.ServerName?.Substring(0, 7) == "asazure")
                {
                    List<ExternalModelRoleMember> membersToAdd = new List<ExternalModelRoleMember>();
                    foreach (ModelRoleMember member in role.Members)
                    {
                        if (member is ExternalModelRoleMember && ((ExternalModelRoleMember)member).IdentityProvider == "AzureAD" && member.MemberID != null)
                        {
                            //AAD member from SSAS to Azure AS
                            ExternalModelRoleMember externalMemberOld = (ExternalModelRoleMember)member;
                            ExternalModelRoleMember externalMemberToAdd = new ExternalModelRoleMember();
                            externalMemberOld.CopyTo(externalMemberToAdd);
                            externalMemberToAdd.MemberID = null;
                            membersToAdd.Add(externalMemberToAdd);
                        }
                    }
                    foreach (ExternalModelRoleMember memberToAdd in membersToAdd)
                    {
                        role.Members.Remove(memberToAdd.Name);
                        role.Members.Add(memberToAdd);
                    }
                }
                else
                {
                    List<ExternalModelRoleMember> membersToAdd = new List<ExternalModelRoleMember>();
                    foreach (ModelRoleMember member in role.Members)
                    {
                        if (member is ExternalModelRoleMember && ((ExternalModelRoleMember)member).IdentityProvider == "AzureAD" && String.IsNullOrEmpty(member.MemberID))
                        {
                            //AAD member from Azure AS to SSAS
                            ExternalModelRoleMember externalMemberOld = (ExternalModelRoleMember)member;
                            ExternalModelRoleMember externalMemberToAdd = new ExternalModelRoleMember();
                            externalMemberOld.CopyTo(externalMemberToAdd);
                            externalMemberToAdd.MemberID = member.MemberName; //***
                            membersToAdd.Add(externalMemberToAdd);
                        }
                    }
                    foreach (ExternalModelRoleMember memberToAdd in membersToAdd)
                    {
                        role.Members.Remove(memberToAdd.Name);
                        role.Members.Add(memberToAdd);
                    }
                }

                _roles.Add(new Role(this, role));
            }
            foreach (Tom.Perspective perspective in _database.Model.Perspectives)
            {
                _perspectives.Add(new Perspective(this, perspective));
            }
            foreach (Tom.Culture culture in _database.Model.Cultures)
            {
                _cultures.Add(new Culture(this, culture));
            }

            if (_connectionInfo.UseBimFile || _connectionInfo.UseTmdlFolder)
            {
                InitializeCalcDependenciesFromM();
            }
            else
            {
                InitializeCalcDependenciesFromServer();
            }
        }

        /// <summary>
        /// Connect only to AS server. Does not instantiate properties of the TabularModel object.
        /// </summary>
        public void TomConnect()
        {
            this.Disconnect();

            if (_connectionInfo.UseTmdlFolder)
            {
                _database = _connectionInfo.OpenDatabaseFromFolder();
            }
            else if (_connectionInfo.UseBimFile)
            {
                _database = _connectionInfo.OpenDatabaseFromFile();
            }
            else
            {
                _server = new Server();
                string connectionString = _connectionInfo.BuildConnectionString();
                _server.Connect(connectionString);

                _database = _server.Databases.FindByName(_connectionInfo.DatabaseName);
                if (_database == null)
                {
                    //Don't need try to load from project here as will already be done before instantiated Comparison
                    throw new Amo.ConnectionException($"Could not connect to database {_connectionInfo.DatabaseName}");
                }
            }
        }

        private void InitializeCalcDependenciesFromServer()
        {
            _calcDependencies.Clear();
            string command =
                "SELECT * FROM $System.DISCOVER_CALC_DEPENDENCY " +
                "WHERE (OBJECT_TYPE = 'PARTITION' OR OBJECT_TYPE = 'M_EXPRESSION') AND " +
                "NOT (OBJECT_TYPE = REFERENCED_OBJECT_TYPE AND " +
                "     [TABLE] = REFERENCED_TABLE AND" +
                "     OBJECT = REFERENCED_OBJECT);"; //Ignore recursive M expression dependencies
            bool foundFault = false;
            XmlNodeList rows = Core.Comparison.ExecuteXmlaCommand(_server, _connectionInfo.DatabaseName, command, ref foundFault);
            
            if (foundFault)
            {
                InitializeCalcDependenciesFromM();
            }
            else
            {
                foreach (XmlNode row in rows)
                {
                    string objectType = "";
                    string tableName = "";
                    string objectName = "";
                    string expression = "";
                    string referencedObjectType = "";
                    string referencedTableName = "";
                    string referencedObjectName = "";
                    string referencedExpression = "";

                    foreach (XmlNode col in row.ChildNodes)
                    {
                        if (col.Name == "OBJECT_TYPE") objectType = col.InnerText;
                        if (col.Name == "TABLE") tableName = col.InnerText;
                        if (col.Name == "OBJECT") objectName = col.InnerText;
                        if (col.Name == "EXPRESSION") expression = col.InnerText;
                        if (col.Name == "REFERENCED_OBJECT_TYPE") referencedObjectType = col.InnerText;
                        if (col.Name == "REFERENCED_TABLE") referencedTableName = col.InnerText;
                        if (col.Name == "REFERENCED_OBJECT") referencedObjectName = col.InnerText;
                        if (col.Name == "REFERENCED_EXPRESSION") referencedExpression = col.InnerText;
                    }

                    _calcDependencies.Add(new CalcDependency(this,
                        objectType,
                        tableName,
                        objectName,
                        expression,
                        referencedObjectType,
                        referencedTableName,
                        referencedObjectName,
                        referencedExpression
                        )
                    );
                }
            }
        }

        private struct MObject
        {
            public string ObjectType;
            public string TableName;
            public string ObjectName;
            public string Expression;

            public MObject(string objectType, string tableName, string objectName, string expression)
            {
                ObjectType = objectType;
                TableName = tableName;
                ObjectName = objectName;
                Expression = expression;
            }
        }

        private void InitializeCalcDependenciesFromM()
        {
            _calcDependencies.Clear();
            List<MObject> mObjects = new List<MObject>();

            #region Add M-dependent objects to collection

            foreach (Table table in _tables)
            {
                foreach (Partition partition in table.TomTable.Partitions)
                {
                    if (partition.SourceType == PartitionSourceType.M)
                    {
                        mObjects.Add(
                            new MObject(
                                objectType: "PARTITION",
                                tableName: table.Name,
                                objectName: partition.Name,
                                expression: ((MPartitionSource)partition.Source).Expression
                            )
                        );
                    }
                }
            }

            foreach (Expression expression in _expressions)
            {
                mObjects.Add(
                    new MObject(
                        objectType: "M_EXPRESSION",
                        tableName: "",
                        objectName: expression.Name,
                        expression: expression.TomExpression.Expression
                    )
                );
            }

            foreach (DataSource dataSource in _dataSources)
            {
                if (dataSource.TomDataSource.Type == DataSourceType.Structured)
                {
                    mObjects.Add(
                        new MObject(
                            objectType: "DATA_SOURCE",
                            tableName: "",
                            objectName: dataSource.Name,
                            expression: ""
                        )
                    );
                }
            }

            #endregion

            char[] delimiterChars = { ' ', ',', ':', '=', '\t', '\n', '[', ']', '(', ')', '{', '}' };

            //9/20/2020: stack overflow exception fix added to list of keywords: "Data", "Item", "Source",
            //9/28/2020: stack overflow exception fix added to list of keywords: "Severity", "SeverityID",
            //9/29/2020: stack overflow exception fix added to list of keywords: "SQL"
            List<string> keywords = new List<string>() { "and", "as", "Data", "each", "else", "error", "false", "if", "in", "is", "Item", "let", "meta", "not", "otherwise", "or", "section", "Severity", "SeverityID", "shared", "Source", "SQL", "then", "true", "try", "type", "#binary", "#date", "#datetime", "#datetimezone", "#duration", "#infinity", "#nan", "#sections", "#shared", "#table", "#time" };
                                                                                                     
            foreach (MObject mObject in mObjects)
            {
                string regex = "(#\"(.*?)\")";
                //Expression with double quote references like #"Query" removed
                string expressionRegex = Regex.Replace(mObject.Expression, regex, "");
                string[] words = expressionRegex.Split(delimiterChars);

                foreach (MObject referencedMObject in mObjects)
                {
                    bool foundDependency = false;

                    if (!(  //Ignore circular dependencies
                            mObject.ObjectName == referencedMObject.ObjectName &&
                            mObject.ObjectType == referencedMObject.ObjectType &&
                            mObject.TableName == referencedMObject.TableName
                         ))
                    {
                        if (  //if M_EXPRESSION or DATA_SOURCE, check for occurrence like #"My Query" or #"let"
                            (referencedMObject.ObjectType == "M_EXPRESSION" || referencedMObject.ObjectType == "DATA_SOURCE") &&
                            (mObject.Expression.Contains("#\"" + referencedMObject.ObjectName + "\""))
                        )
                        {
                            foundDependency = true;
                        }
                        else if ( //if table name, check for occurrence like #"My Query" or #"let"
                            referencedMObject.ObjectType == "PARTITION" &&
                            (mObject.Expression.Contains("#\"" + referencedMObject.TableName + "\""))
                        )
                        {
                            foundDependency = true;
                        }
                        else
                        {
                            foreach (string word in words)
                            {
                                if (
                                        (
                                            (referencedMObject.ObjectType == "M_EXPRESSION" || referencedMObject.ObjectType == "DATA_SOURCE") &&
                                            word == referencedMObject.ObjectName && !keywords.Contains(referencedMObject.ObjectName)
                                        ) ||
                                        (
                                            referencedMObject.ObjectType == "PARTITION" &&
                                            word == referencedMObject.TableName && !keywords.Contains(referencedMObject.TableName)
                                        )
                                   )
                                {
                                    foundDependency = true;
                                }
                            }
                        }

                        if (foundDependency)
                        {
                            _calcDependencies.Add(
                                new CalcDependency(
                                    this,
                                    objectType: mObject.ObjectType,
                                    tableName: mObject.TableName,
                                    objectName: mObject.ObjectName,
                                    expression: mObject.Expression,
                                    referencedObjectType: referencedMObject.ObjectType,
                                    referencedTableName: referencedMObject.TableName,
                                    referencedObjectName: referencedMObject.ObjectName,
                                    referencedExpression: referencedMObject.Expression
                                )
                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disconnect from the SSAS server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_server != null) _server.Disconnect();
            }
            catch { }
        }

        #region Properties

        /// <summary>
        /// Tabular Object Model Database object abtstracted by the TabularModel class.
        /// </summary>
        public Database TomDatabase
        {
            get { return _database; }
            set { _database = value; }
        }

        /// <summary>
        /// Model object.
        /// </summary>
        public Model Model
        {
            get { return _model; }
            set { _model = value; }
        }

        /// <summary>
        /// Collection of DataSources for the TabularModel object.
        /// </summary>
        public DataSourceCollection DataSources => _dataSources;

        /// <summary>
        /// Collection of tables for the TabularModel object.
        /// </summary>
        public TableCollection Tables => _tables;

        /// <summary>
        /// Collection of expressions for the TabularModel object.
        /// </summary>
        public ExpressionCollection Expressions => _expressions;

        /// <summary>
        /// Collection of perspectives for the TabularModel object.
        /// </summary>
        public PerspectiveCollection Perspectives => _perspectives;

        /// <summary>
        /// Collection of cultures for the TabularModel object.
        /// </summary>
        public CultureCollection Cultures => _cultures;

        /// <summary>
        /// Collection of roles for the TabularModel object.
        /// </summary>
        public RoleCollection Roles => _roles;

        /// <summary>
        /// Collection of M dependencies for the TabularModel object.
        /// </summary>
        public CalcDependencyCollection MDependencies => _calcDependencies;

        /// <summary>
        /// ConnectionInfo object for the tabular model.
        /// </summary>
        public ConnectionInfo ConnectionInfo => _connectionInfo;

        /// <summary>
        /// ComparisonInfo object for the tabular model.
        /// </summary>
        public ComparisonInfo ComparisonInfo => _comparisonInfo;

        #endregion

        #region Relationships

        /// <summary>
        /// Check whether the TabularModel object contains a relationship.
        /// </summary>
        /// <param name="relationshipInternalName">The internal name of the relationship.</param>
        /// <returns>True if found; false if not.</returns>
        public bool ContainsRelationshipByInternalName(string relationshipInternalName)
        {
            bool foundRelationship = false;

            foreach (Table table in _tables)
            {
                foreach (Relationship relationship in table.Relationships)
                {
                    if (relationship.InternalName == relationshipInternalName)
                    {
                        foundRelationship = true;
                        break;
                    }
                }
                if (foundRelationship)
                {
                    break;
                }
            }

            return foundRelationship;
        }

        #endregion

        #region Model

        /// <summary>
        /// Update Model associated with the TabularModel object.
        /// </summary>
        /// <param name="dataSourceSource">Model object from the source tabular model to be updated in the target.</param>
        /// <param name="dataSourceTarget">Model object in the target tabular model to be updated.</param>
        public void UpdateModel(Model modelSource, Model modelTarget)
        {
            modelTarget.TomModel.Description = modelSource.TomModel.Description;
            if (!_comparisonInfo.OptionsInfo.OptionRetainStorageMode)
            {
                modelTarget.TomModel.DefaultMode = modelSource.TomModel.DefaultMode;
            }
            modelTarget.TomModel.DiscourageImplicitMeasures = modelSource.TomModel.DiscourageImplicitMeasures;
        }

        #endregion

        #region DataSources

        /// <summary>
        /// Delete DataSource associated with the TabularModel object.
        /// </summary>
        /// <param name="name">The name of the DataSource to be deleted.</param>
        public void DeleteDataSource(string name)
        {
            if (_database.Model.DataSources.ContainsName(name))
            {
                _database.Model.DataSources.Remove(name);
            }

            // shell model
            if (_dataSources.ContainsName(name))
            {
                _dataSources.RemoveByName(name);
            }
        }

        /// <summary>
        /// Create DataSource associated with the TabularModel object.
        /// </summary>
        /// <param name="dataSourceSource">DataSource object from the source tabular model to be created in the target.</param>
        public void CreateDataSource(DataSource dataSourceSource)
        {
            if (dataSourceSource.TomDataSource is ProviderDataSource)
            {
                ProviderDataSource providerTarget = new ProviderDataSource();
                dataSourceSource.TomDataSource.CopyTo(providerTarget);

                _database.Model.DataSources.Add(providerTarget);

                // shell model
                _dataSources.Add(new DataSource(this, providerTarget));
            }
            else
            {
                StructuredDataSource structuredTarget = new StructuredDataSource();
                dataSourceSource.TomDataSource.CopyTo(structuredTarget);

                _database.Model.DataSources.Add(structuredTarget);

                // shell model
                _dataSources.Add(new DataSource(this, structuredTarget));
            }
        }

        /// <summary>
        /// Update DataSource associated with the TabularModel object.
        /// </summary>
        /// <param name="dataSourceSource">DataSource object from the source tabular model to be updated in the target.</param>
        /// <param name="dataSourceTarget">DataSource object in the target tabular model to be updated.</param>
        public void UpdateDataSource(DataSource dataSourceSource, DataSource dataSourceTarget)
        {
            if (dataSourceSource.TomDataSource is ProviderDataSource && dataSourceTarget.TomDataSource is ProviderDataSource)
            {
                ProviderDataSource providerSource = (ProviderDataSource)dataSourceSource.TomDataSource;
                ProviderDataSource providerTarget = (ProviderDataSource)dataSourceTarget.TomDataSource;

                providerTarget.Description = providerSource.Description;
                providerTarget.ConnectionString = providerSource.ConnectionString;
                providerTarget.ImpersonationMode = providerSource.ImpersonationMode;
                providerTarget.Account = providerSource.Account;
            }
            else if (dataSourceSource.TomDataSource is StructuredDataSource) //(can replace a provider with a structured, but not vice versa) && dataSourceTarget.TomDataSource is StructuredDataSource)
            {
                DeleteDataSource(dataSourceTarget.Name);
                CreateDataSource(dataSourceSource);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Tables

        /// <summary>
        /// Delete Table object, it's table in the underlying model, and all associated relationships.
        /// </summary>
        /// <param name="name">Name of the table to be deleted.</param>
        /// <returns>Collection of all associated relationships that had to be deleted. Useful if updating tables as then need to add back.</returns>
        public List<RelationshipToRetain> DeleteTable(string name)
        {
            List<RelationshipToRetain> deletedRelationships = new List<RelationshipToRetain>();

            // shell model
            if (_tables.ContainsName(name))
            {
                deletedRelationships = _tables.FindByName(name).DeleteAllAssociatedRelationships();
                _tables.RemoveByName(name);
            }

            if (_database.Model.Tables.Contains(name))
            {
                _database.Model.Tables.Remove(name);
            }

            return deletedRelationships;
        }

        /// <summary>
        /// Create table associated with the TabularModel object.
        /// </summary>
        /// <param name="tableSource">Table object from the source tabular model to be created in the target.</param>
        public void CreateTable(Table tableSource)
        {
            Tom.Table tomTableTarget = new Tom.Table();
            tableSource.TomTable.CopyTo(tomTableTarget);

            //decouple from original model to the current one
            for (int i = 0; i < tomTableTarget.Partitions.Count; i++)
            {
                if (tomTableTarget.Partitions[i].QueryGroup != null)
                {
                    CreateQueryGroup(tomTableTarget.Partitions[i].QueryGroup);
                }

                if (tomTableTarget.Partitions[i].SourceType == PartitionSourceType.Query)
                {
                    QueryPartitionSource queryPartition = ((QueryPartitionSource)tomTableTarget.Partitions[i].Source);
                    //string dataSourceName = queryPartition.DataSource.Name;
                    //11/19/2024 commented above line due to another instance of Oren's change where CopyTo loses object references.
                    //However, we know there is at least 1 partition in tableSource and all partitions in a table with SourceType=Query have only 1 DataSource, so can reliably do this instead:
                    string dataSourceName = ((QueryPartitionSource)tableSource.TomTable.Partitions[i].Source).DataSource.Name;
                    queryPartition.DataSource = _dataSources.FindByName(dataSourceName).TomDataSource;
                }
            }

            tomTableTarget.Measures.Clear();  //Measures will be added separately later
            tomTableTarget.CalculationGroup?.CalculationItems.Clear();  //Calculation items will be added separately later

            _database.Model.Tables.Add(tomTableTarget);
            _tables.Add(new Table(this, tomTableTarget));
        }

        /// <summary>
        /// Update table associated with the TabularModel object.
        /// </summary>
        /// <param name="tableSource">Table object from the source tabular model to be updated in the target.</param>
        /// <param name="tableTarget">Table object in the target tabular model to be updated.</param>
        public void UpdateTable(Table tableSource, Table tableTarget, out string retainPartitionsMessage)
        {
            bool canRetainPartitions = CanRetainPartitions(
                tableSource, 
                tableTarget, 
                out retainPartitionsMessage,
                out PartitionSourceType partitionSourceTypeSource,
                out PartitionSourceType partitionSourceTypeTarget);
            Tom.Table tomTableTargetOrig = tableTarget.TomTable.Clone();
            ModeType tableTargetModeType = tableTarget.TableModeType;
            List<RelationshipToRetain> relationshipsToAddBack = DeleteTable(tableTarget.Name);
            CreateTable(tableSource);

            //get back the newly created table
            tableTarget = _tables.FindByName(tableSource.Name);

            //retain partitions if possible
            if (canRetainPartitions)
                RetainPartitions(tableTarget, tomTableTargetOrig, out retainPartitionsMessage);

            //add back deleted relationships where possible
            foreach (RelationshipToRetain relationshipToAddBack in relationshipsToAddBack)
            {
                Table fromTable = _tables.FindByName(relationshipToAddBack.FromTableName);
                Table toTable = _tables.FindByName(relationshipToAddBack.ToTableName);

                if (fromTable != null && fromTable.TomTable.Columns.ContainsName(relationshipToAddBack.FromColumnName) &&
                    toTable != null & toTable.TomTable.Columns.ContainsName(relationshipToAddBack.ToColumnName))
                {
                    //decouple from original table to the current one
                    relationshipToAddBack.TomRelationship.FromColumn = fromTable.TomTable.Columns.Find(relationshipToAddBack.FromColumnName);
                    relationshipToAddBack.TomRelationship.ToColumn = toTable.TomTable.Columns.Find(relationshipToAddBack.ToColumnName);

                    fromTable.CreateRelationship(relationshipToAddBack.TomRelationship);
                }
            }

            //add back measures
            foreach (Tom.Measure tomMeasureToAddBack in tomTableTargetOrig.Measures)
            {
                tableTarget.CreateMeasure(tomMeasureToAddBack);
            }

            //add back calculationItems
            if (tomTableTargetOrig.CalculationGroup != null)
            {
                foreach (Tom.CalculationItem tomCalculationItemToAddBack in tomTableTargetOrig.CalculationGroup.CalculationItems)
                {
                    tableTarget.CreateCalculationItem(tomCalculationItemToAddBack);
                }
            }
            else
            {
                //add back storage mode if option selected
                if (_comparisonInfo.OptionsInfo.OptionRetainStorageMode)
                {
                    tableTarget.ResetStorageMode(tableTargetModeType);
                }

                //add back refresh policy if option selected
                if (_comparisonInfo.OptionsInfo.OptionRetainRefreshPolicy)
                {
                    if (tomTableTargetOrig.RefreshPolicy == null)
                    {
                        tableTarget.TomTable.RefreshPolicy = null;
                    }
                    else
                    {
                        tableTarget.TomTable.RefreshPolicy = tomTableTargetOrig.RefreshPolicy.Clone();
                        retainPartitionsMessage = "Retain refresh policy option applied. " + retainPartitionsMessage;
                    }
                }
            }
        }

        public bool CanRetainPartitions(Table tableSource, Table tableTarget, out string retainPartitionsMessage, out PartitionSourceType partitionSourceTypeSource, out PartitionSourceType partitionSourceTypeTarget)
        {
            //Initialize variables
            retainPartitionsMessage = "";
            partitionSourceTypeSource = PartitionSourceType.None;
            foreach (Partition partition in tableSource.TomTable.Partitions)
            {
                partitionSourceTypeSource = partition.SourceType;
                break;
            }
            partitionSourceTypeTarget = PartitionSourceType.None;
            foreach (Partition partitionTarget in tableTarget.TomTable.Partitions)
            {
                partitionSourceTypeTarget = partitionTarget.SourceType;
                break;
            }

            //Verify necessary options are checked
            if (!_comparisonInfo.OptionsInfo.OptionRetainPartitions)
                return false;
            if (_comparisonInfo.OptionsInfo.OptionRetainPolicyPartitions && partitionSourceTypeTarget != PartitionSourceType.PolicyRange)
                return false;

            //both tables need to have M or query partitions, or target can be policy partitions. Also type needs to match (won't copy query partition to M table). If a table has no partitions, do nothing.
            if (!(partitionSourceTypeSource == PartitionSourceType.M || partitionSourceTypeSource == PartitionSourceType.Query || partitionSourceTypeSource == PartitionSourceType.PolicyRange))
            {
                retainPartitionsMessage = $"Retain partitions not applicable to partition types.";
                return false;
            }

            if (!(partitionSourceTypeTarget == PartitionSourceType.M || partitionSourceTypeTarget == PartitionSourceType.Query || partitionSourceTypeTarget == PartitionSourceType.PolicyRange))
            {
                retainPartitionsMessage = $"Retain partitions not applicable to partition types.";
                return false;
            }

            if (partitionSourceTypeTarget != partitionSourceTypeSource && !(partitionSourceTypeTarget == PartitionSourceType.M && partitionSourceTypeSource == PartitionSourceType.PolicyRange) && partitionSourceTypeTarget != PartitionSourceType.PolicyRange)
            {
                retainPartitionsMessage = $"Retain partitions not applied because source partition type is {partitionSourceTypeSource.ToString()} and target partition type is {partitionSourceTypeTarget.ToString()}.";
                return false;
            }

            if (tableSource.PartitionsDefinition == tableTarget.PartitionsDefinition)
            {
                retainPartitionsMessage = "Source & target partition definitions already match, so retain partitions not necessary.";
                return false;
            }

            return true;
        }

        private void RetainPartitions(Table modifiedTableTarget, Tom.Table tomTableTargetOrig, out string retainPartitionsMessage)
        {
            //Actually do the retain partitions
            retainPartitionsMessage = "Retain target partitions applied: ";
            modifiedTableTarget.TomTable.Partitions.Clear();
            foreach (Partition partitionOrig in tomTableTargetOrig.Partitions)
            {
                Partition partitionTarget = partitionOrig.Clone();
                if (partitionTarget.SourceType == PartitionSourceType.Query)
                {
                    QueryPartitionSource querySource = (QueryPartitionSource)partitionTarget.Source;
                    querySource.DataSource = _dataSources.FindByName(querySource.DataSource?.Name).TomDataSource;
                }
                modifiedTableTarget.TomTable.Partitions.Add(partitionTarget);
                retainPartitionsMessage += $"{partitionTarget.Name}, ";
            }
            retainPartitionsMessage = retainPartitionsMessage.Substring(0, retainPartitionsMessage.Length - 2) + ".";
        }

        #endregion

        #region Expressions

        /// <summary>
        /// Delete expression associated with the TabularModel object.
        /// </summary>
        /// <param name="name">Name of the expression to be deleted.</param>
        public void DeleteExpression(string name)
        {
            if (_database.Model.Expressions.Contains(name))
            {
                _database.Model.Expressions.Remove(name);
            }

            // shell model
            if (_expressions.ContainsName(name))
            {
                _expressions.Remove(name);
            }
        }

        /// <summary>
        /// Create expression associated with the TabularModel object.
        /// </summary>
        /// <param name="tomExpressionSource">Tabular Object Model NamedExpression object from the source tabular model to be abstracted in the target.</param>
        public void CreateExpression(NamedExpression tomExpressionSource)
        {
            NamedExpression tomExpressionTarget = new NamedExpression();
            tomExpressionSource.CopyTo(tomExpressionTarget);

            if (tomExpressionTarget.QueryGroup != null)
            {
                CreateQueryGroup(tomExpressionTarget.QueryGroup);
            }

            _database.Model.Expressions.Add(tomExpressionTarget);

            // shell model
            _expressions.Add(new Expression(this, tomExpressionTarget));
        }

        private void CreateQueryGroup(QueryGroup queryGroupToCreate)
        {
            List<string> foldersToCreate = new List<string>();
            string currentPath = "";
            string[] folders = queryGroupToCreate.Folder.Split('\\');
            for (int i = 0; i < folders.Length; ++i)
            {
                if (i==0)
                    currentPath += folders[i];
                else
                    currentPath += '\\' + folders[i];

                foldersToCreate.Add(currentPath);
            }
            foreach (string folderToCreate in foldersToCreate)
            {
                if (!_database.Model.QueryGroups.ContainsName(folderToCreate))
                {
                    QueryGroup queryGroup = new QueryGroup();
                    queryGroup.Folder = folderToCreate;
                    _database.Model.QueryGroups.Add(queryGroup);
                }
            }
        }

        /// <summary>
        /// Update expression associated with the TabularModel object.
        /// </summary>
        /// <param name="expressionSource">Expression object from the source tabular model to be updated in the target.</param>
        /// <param name="expressionTarget">Expression object in the target tabular model to be updated.</param>
        public void UpdateExpression(Expression expressionSource, Expression expressionTarget)
        {
            DeleteExpression(expressionTarget.Name);
            CreateExpression(expressionSource.TomExpression);
        }

        #endregion

        #region Validation of Relationships

        /// <summary>
        /// Set relationships copied from source to inactive if cause ambigious paths.
        /// </summary>
        public void ValidateRelationships()
        {
            foreach (Table beginTable in _tables)
            {
                //beginTable might be the From or the To table in TOM Relationship object; it depends on CrossFilterDirection.

                RelationshipChainsFromRoot referencedTableCollection = new RelationshipChainsFromRoot();
                foreach (Relationship filteringRelationship in beginTable.FindFilteredRelationships())
                {
                    // EndTable can be either the From or the To table of a Relationship object depending on CrossFilteringBehavior
                    string endTableName = GetEndTableName(beginTable, filteringRelationship, out bool biDi);
                    RelationshipLink rootLink = new RelationshipLink(beginTable, _tables.FindByName(endTableName), true, "", false, filteringRelationship, biDi);
                    ValidateLink(rootLink, referencedTableCollection);
                }
            }

            //also issue a warning for any table that had it's guid name modified
            foreach (Table table in _tables)
            {
                foreach (Relationship relationship in table.Relationships)
                {
                    if (relationship.ModifiedInternalName)
                    {
                        _parentComparison.OnValidationMessage(new ValidationMessageEventArgs(
                            $"Relationship {relationship.Name.Trim()} has been created/updated, but its Name property was changed from \"{relationship.OldInternalName}\" to \"{relationship.InternalName}\" to avoid conflict with an existing relationship.",
                            ValidationMessageType.Relationship,
                            ValidationMessageStatus.Warning));
                    }
                }
            }
        }

        private void ValidateLink(RelationshipLink link, RelationshipChainsFromRoot chainsFromRoot)
        {
            if (
                 chainsFromRoot.ContainsEndTableName(link.EndTable.Name)
                 //&& !(link.PrecedingPathBiDiInvoked && !chainsFromRoot.ContainsBidiToEndTable(link.EndTable.Name))
                 //Fix 12/1/2017: we allow 1 ambiguous relationship path as long as only one of the paths is bidi invoked (2 bidis to the same table counts as ambiguous)
               )
            {
                // If we are here, we have identified 2 active paths to get to the same table.
                // So, the one that was already there in the target should win.

                RelationshipLink otherLink = chainsFromRoot.FindByEndTableName(link.EndTable.Name);
                string rootTableName = chainsFromRoot.FindRoot().BeginTable.Name;

                if (link.FilteringRelationship.CopiedFromSource)
                {
                    link.FilteringRelationship.TomRelationship.IsActive = false;

                    _parentComparison.OnValidationMessage(new ValidationMessageEventArgs(
                        $"Relationship {link.FilteringRelationship.Name.Trim()} (which is active in the source) has been created/updated, but is set to inactive in the target because it introduces ambiguous paths between '{rootTableName}' and '{link.EndTable.Name}': '{link.TablePath} and {otherLink.TablePath}'.",
                        ValidationMessageType.Relationship,
                        ValidationMessageStatus.Warning));
                }
                else
                {
                    // link.FilteringRelationship is the one that was already in the target.  So need to remove the other one that leads to the same table (which must have been copied from source)
                    otherLink.FilteringRelationship.TomRelationship.IsActive = false;

                    _parentComparison.OnValidationMessage(new ValidationMessageEventArgs(
                        $"Relationship {otherLink.FilteringRelationship.Name.Trim()} (which is active in the source) has been created/updated, but is set to inactive in the target because it introduces ambiguous paths between '{rootTableName}' and '{otherLink.EndTable.Name}': '{otherLink.TablePath} and {link.TablePath}'.",
                        ValidationMessageType.Relationship,
                        ValidationMessageStatus.Warning));

                    //remove OTHER ONE from collection as no longer in filtering chain
                    chainsFromRoot.RemoveByEndTableName(otherLink.EndTable.Name);
                }
            }

            if (link.FilteringRelationship.TomRelationship.IsActive)  //If not, we must have just set it to false above
            {
                //Add the link to the chain and re-iterate ...
                chainsFromRoot.Add(link);

                Table beginTable = link.EndTable; //EndTable is now the begin table as iterating next level ...
                foreach (Relationship filteringRelationship in beginTable.FindFilteredRelationships())
                {
                    // EndTable can be either the From or the To table of a Relationship object depending on direction of CrossFilteringBehavior
                    string endTableName = GetEndTableName(beginTable, filteringRelationship, out bool biDi);

                    //Need to check if endTableName has already been covered by TablePath to avoid CrossFilteringBehavior leading both ways and never ending loop
                    if (!link.TablePath.Contains("'" + endTableName + "'"))
                    {
                        RelationshipLink newLink = new RelationshipLink(beginTable, _tables.FindByName(endTableName), false, link.TablePath, link.PrecedingPathBiDiInvoked, filteringRelationship, biDi);
                        ValidateLink(newLink, chainsFromRoot);
                    }
                }
            }
        }

        private string GetEndTableName(Table beginTable, Relationship filteringRelationship, out bool biDi)
        {
            string endTableName;
            biDi = false;
            if (filteringRelationship.TomRelationship.FromTable.Name == beginTable.Name)
            {
                endTableName = filteringRelationship.TomRelationship.ToTable.Name;
            }
            else
            {
                endTableName = filteringRelationship.TomRelationship.FromTable.Name;
                biDi = true;
            }
            return endTableName;
        }

        #endregion

        #region Variation / Aggregations Cleanup

        /// <summary>
        /// Remove variations referring to objects that don't exist.
        /// </summary>
        public void CleanUpVariations()
        {
            List<string> targetVariationTablesRemaining = new List<string>();

            foreach (Table table in _tables)
            {
                foreach (Column column in table.TomTable.Columns)
                {
                    List<string> variationsToRemove = new List<string>();

                    foreach (Variation variation in column.Variations)
                    {
                        if (!_database.Model.Relationships.ContainsName(variation.Relationship.Name))
                        {
                            variationsToRemove.Add(variation.Name);
                            break;
                        }

                        if (variation.DefaultColumn != null)
                        {
                            if (_database.Model.Tables.ContainsName(variation.DefaultColumn.Table?.Name))
                            {
                                //the referenced table is there, how about the referenced column?
                                if (!_database.Model.Tables.Find(variation.DefaultColumn.Table.Name).Columns.ContainsName(variation.DefaultColumn.Name))
                                {
                                    variationsToRemove.Add(variation.Name);
                                    break;
                                }
                            }
                            else
                            {
                                variationsToRemove.Add(variation.Name);
                                break;
                            }

                            //If we get here, the variation is valid
                            targetVariationTablesRemaining.Add(variation.DefaultColumn.Table?.Name);
                            break;
                        }

                        if (variation.DefaultHierarchy != null)
                        {
                            if (_database.Model.Tables.ContainsName(variation.DefaultHierarchy.Table?.Name))
                            {
                                //the referenced table is there, how about the referenced hierarchy?
                                if (!_database.Model.Tables.Find(variation.DefaultHierarchy.Table.Name).Hierarchies.ContainsName(variation.DefaultHierarchy.Name))
                                {
                                    variationsToRemove.Add(variation.Name);
                                    break;
                                }
                            }
                            else
                            {
                                variationsToRemove.Add(variation.Name);
                                break;
                            }

                            //If we get here, the variation is valid
                            targetVariationTablesRemaining.Add(variation.DefaultHierarchy.Table?.Name);
                            break;
                        }
                    }

                    foreach (string variationToRemove in variationsToRemove)
                    {
                        column.Variations.Remove(variationToRemove);
                    }
                }
            }

            //Check if any tables that have ShowAsVariationsOnly = true really have variations pointing at them
            foreach (Table table in _tables)
            {
                if (table.TomTable.ShowAsVariationsOnly == true && !targetVariationTablesRemaining.Contains(table.Name))
                {
                    table.TomTable.ShowAsVariationsOnly = false;
                }
            }
        }

        /// <summary>
        /// Remove aggregations referring to objects that don't exist.
        /// </summary>
        public void CleanUpAggregations()
        {
            //modelTablesWithRls to be used for Rule 11 below:
            List<string> modelTablesWithRls = new List<string>();
            foreach (Role role in _roles)
            {
                foreach (TablePermission tablePermission in role.TomRole.TablePermissions)
                {
                    if (!String.IsNullOrEmpty(tablePermission.FilterExpression))
                    {
                        modelTablesWithRls.Add(tablePermission.Name);
                    }
                }
            }

            foreach (Table table in _tables)
            {
                bool foundViolation = false;
                string warningMessage = "";

                foreach (Column column in table.TomTable.Columns)
                {
                    if (!foundViolation)
                    {
                        /* Check aggs refer to valid base tables/columns
                         */

                        if (column.AlternateOf?.BaseTable != null)
                        {
                            if (!_database.Model.Tables.ContainsName(column.AlternateOf.BaseTable.Name))
                            {
                                //Base table doesn't exist
                                foundViolation = true;
                                warningMessage = $"Removed aggregations on table {table.Name} because summarization {column.AlternateOf.Summarization.ToString()} on column {column.Name} (considering changes) refers to detail table that does not exist [table:{column.AlternateOf.BaseTable.Name}].\n";
                                break;
                            }
                        }
                        else if (column.AlternateOf?.BaseColumn != null)
                        {
                            if (_database.Model.Tables.ContainsName(column.AlternateOf.BaseColumn.Table?.Name))
                            {
                                //the referenced table is there, how about the referenced column?
                                if (!_database.Model.Tables.Find(column.AlternateOf.BaseColumn.Table.Name).Columns.ContainsName(column.AlternateOf.BaseColumn.Name))
                                {
                                    //Base column does not exist
                                    foundViolation = true;
                                    warningMessage = $"Removed aggregations on table {table.Name} because summarization {column.AlternateOf.Summarization.ToString()} on column {column.Name} (considering changes) refers to detail column that does not exist [table:{column.AlternateOf.BaseColumn.Table.Name}/column:{column.AlternateOf.BaseColumn.Name}].\n";
                                    break;
                                }
                            }
                            else
                            {
                                //Base table does not exist
                                foundViolation = true;
                                warningMessage = $"Removed aggregations on table {table.Name} because summarization {column.AlternateOf.Summarization.ToString()} on column {column.Name} (considering changes) refers to detail table that does not exist [table:{column.AlternateOf.BaseColumn.Table.Name}].\n";
                                break;
                            }
                        }

                        string detailTableName = null;
                        if (!foundViolation && column.AlternateOf != null)
                        {
                            detailTableName = (column.AlternateOf.BaseTable != null ? column.AlternateOf.BaseTable.Name : column.AlternateOf.BaseColumn.Table.Name);
                        }
                        Table detailTable = _tables.FindByName(detailTableName);

                        if (!foundViolation && column.AlternateOf != null && column.AlternateOf.Summarization != SummarizationType.GroupBy && modelTablesWithRls.Count > 0 && detailTable != null)
                        {
                            /* Rule 11: RLS expressions that can filter the agg table, must also be able to filter the detail table(s) using an active relationship
                             */

                            //Get list of filtering RLS tables that filter the agg table
                            List<string> rlsTablesFilteringAgg = new List<string>(); //RLS tables that filter the agg table

                            //beginTable might be the From or the To table in TOM Relationship object; it depends on CrossFilterDirection.
                            RelationshipChainsFromRoot referencedTableCollection = new RelationshipChainsFromRoot();
                            foreach (Relationship filteringRelationship in table.FindFilteredRelationships(checkSecurityBehavior: true))
                            {
                                // EndTable can be either the From or the To table of a Relationship object depending on CrossFilteringBehavior/SecurityBehavior
                                string endTableName = GetEndTableName(table, filteringRelationship, out bool biDi);
                                RelationshipLink rootLink = new RelationshipLink(table, _tables.FindByName(endTableName), true, "", false, filteringRelationship, biDi);
                                ValidateLinkForAggsRls(rootLink, referencedTableCollection, modelTablesWithRls, rlsTablesFilteringAgg);
                            }

                            //If the agg table itself has RLS on it, then consider it a table that is filtering the agg too
                            if (modelTablesWithRls.Contains(table.Name))
                            {
                                rlsTablesFilteringAgg.Add(table.Name);
                            }

                            if (rlsTablesFilteringAgg.Count > 0)
                            {
                                //Get list of filtering RLS tables on the detail table
                                List<string> rlsTablesFilteringDetail = new List<string>(); //RLS tables that filter the detail table

                                //beginTable might be the From or the To table in TOM Relationship object; it depends on CrossFilterDirection.
                                referencedTableCollection = new RelationshipChainsFromRoot();
                                foreach (Relationship filteringRelationship in detailTable.FindFilteredRelationships(checkSecurityBehavior: true))
                                {
                                    // EndTable can be either the From or the To table of a Relationship object depending on CrossFilteringBehavior/SecurityBehavior
                                    string endTableName = GetEndTableName(detailTable, filteringRelationship, out bool biDi);
                                    RelationshipLink rootLink = new RelationshipLink(detailTable, _tables.FindByName(endTableName), true, "", false, filteringRelationship, biDi);
                                    ValidateLinkForAggsRls(rootLink, referencedTableCollection, modelTablesWithRls, rlsTablesFilteringDetail);
                                }

                                //For each agg table, check any RLS filter tables also covers the detail table
                                foreach (string rlsTableFilteringAgg in rlsTablesFilteringAgg)
                                {
                                    if (!rlsTablesFilteringDetail.Contains(rlsTableFilteringAgg))
                                    {
                                        foundViolation = true;
                                        warningMessage = $"Removed aggregations on table {table.Name} because summarization {column.AlternateOf.Summarization.ToString()} on column {column.Name} (considering changes) RLS filter on table {rlsTableFilteringAgg} that filters the agg, but does not filter detail table {detailTableName}.\n";
                                        break;
                                    }
                                }
                            }
                        }

                        if (!foundViolation && column.AlternateOf != null)
                        {
                            /* Rule 10: Relationships between aggregation tables and other (non-aggregation) tables are not allowed if the aggregation table is on the filtering side of a relationship (active or inactive relationships).
                               This rule applies whether relationships are weak or strong, whether BiDi or not [including to-many BiDi, not just to-one]
                             */

                            //beginTable might be the From or the To table in TOM Relationship object; it depends on CrossFilterDirection.
                            RelationshipChainsFromRoot referencedTableCollection = new RelationshipChainsFromRoot();
                            foreach (Relationship filteringRelationship in table.FindFilteringRelationships())
                            {
                                // EndTable can be either the From or the To table of a Relationship object depending on CrossFilteringBehavior/SecurityBehavior
                                string endTableName = GetEndTableName(table, filteringRelationship, out bool biDi);
                                Table endTable = _tables.FindByName(endTableName);
                                if (endTable != null)
                                {
                                    bool endTableContainsAggs = false;
                                    foreach (Column col in endTable.TomTable.Columns)
                                    {
                                        if (col.AlternateOf != null)
                                        {
                                            //End table has at least 1 agg so we are good
                                            endTableContainsAggs = true;
                                            break;
                                        }
                                    }

                                    if (!endTableContainsAggs)
                                    {
                                        foundViolation = true;
                                        warningMessage = $"Removed aggregations on table {table.Name} because summarization {column.AlternateOf.Summarization.ToString()} on column {column.Name} (considering changes) the agg table is on the filtering side of a relationship to a table ({endTable.Name}) that does not contain aggregations, which is not allowed.\n";
                                        break;
                                    }
                                }
                            }
                        }

                        if (!foundViolation && column.AlternateOf != null && detailTable != null)
                        {
                            /* Rule 3: Chained aggregations are disallowed
                             */

                            foreach (Column detailColumn in detailTable.TomTable.Columns)
                            {
                                if (detailColumn.AlternateOf != null)
                                {
                                    foundViolation = true;
                                    warningMessage = $"Removed aggregations on table {table.Name} because summarization {column.AlternateOf.Summarization.ToString()} on column {column.Name} (considering changes) the detail table {detailTableName} also contains aggregations, which is not allowed.\n";
                                    break;
                                }
                            }
                        }
                    }
                }

                //Clear all aggs on the agg table
                if (foundViolation)
                {
                    _parentComparison.OnValidationMessage(new ValidationMessageEventArgs(warningMessage, ValidationMessageType.AggregationDependency, ValidationMessageStatus.Warning));

                    foreach (Column column in table.TomTable.Columns)
                    {
                        if (column.AlternateOf != null)
                        {
                            column.AlternateOf = null;
                        }
                    }
                }
            }
        }

        private void ValidateLinkForAggsRls(RelationshipLink link, RelationshipChainsFromRoot chainsFromRoot, List<string> modelTablesWithRls, List<string> rlsTablesFiltering)
        {
            if (link.FilteringRelationship.TomRelationship.IsActive)
            {
                if (modelTablesWithRls.Contains(link.EndTable.Name) && !rlsTablesFiltering.Contains(link.EndTable.Name))
                {
                    rlsTablesFiltering.Add(link.EndTable.Name);
                }

                //Add the link to the chain and re-iterate ...
                chainsFromRoot.Add(link);

                Table beginTable = link.EndTable; //EndTable is now the begin table as iterating next level ...
                foreach (Relationship filteringRelationship in beginTable.FindFilteredRelationships(checkSecurityBehavior: true))
                {
                    // EndTable can be either the From or the To table of a Relationship object depending on direction of CrossFilteringBehavior
                    string endTableName = GetEndTableName(beginTable, filteringRelationship, out bool biDi);

                    //Need to check if endTableName has already been covered by TablePath to avoid CrossFilteringBehavior leading both ways and never ending loop
                    if (!link.TablePath.Contains("'" + endTableName + "'"))
                    {
                        RelationshipLink newLink = new RelationshipLink(beginTable, _tables.FindByName(endTableName), false, link.TablePath, link.PrecedingPathBiDiInvoked, filteringRelationship, biDi);
                        ValidateLinkForAggsRls(newLink, chainsFromRoot, modelTablesWithRls, rlsTablesFiltering);
                    }
                }
            }
        }

        #endregion

        #region Backup / Restore

        /// <summary>
        /// Perspectives, cultures and roles will be affected by changes to tables, measures, etc. and can end up invalid. To avoid this, take a backup.
        /// </summary>
        public void BackupAffectedObjects()
        {
            _tomPerspectivesBackup = new List<Tom.Perspective>();
            foreach (Tom.Perspective perspective in _database.Model.Perspectives)
            {
                Tom.Perspective perspectiveToBackup = new Tom.Perspective();
                perspective.CopyTo(perspectiveToBackup);
                _tomPerspectivesBackup.Add(perspectiveToBackup);
            }

            _tomRolesBackup = new List<ModelRole>();
            foreach (ModelRole role in _database.Model.Roles)
            {
                ModelRole roleToBackup = new ModelRole();
                role.CopyTo(roleToBackup);
                _tomRolesBackup.Add(roleToBackup);
            }

            _tomCulturesBackup = new List<Tom.Culture>();
            foreach (Tom.Culture culture in _database.Model.Cultures)
            {
                Tom.Culture cultureToBackup = new Tom.Culture();
                culture.CopyTo(cultureToBackup);
                _tomCulturesBackup.Add(cultureToBackup);
            }
        }

        /// <summary>
        /// Restore perspectives after changes to tables, measures, etc.
        /// </summary>
        public void RestorePerspectives()
        {
            if (_tomPerspectivesBackup != null)
            {
                _database.Model.Perspectives.Clear();
                _perspectives.Clear();

                foreach (Tom.Perspective tomPerspective in _tomPerspectivesBackup)
                {
                    CreatePerspective(tomPerspective);
                }
            }
        }

        /// <summary>
        /// Restore roles after changes to tables, measures, etc.
        /// </summary>
        public void RestoreRoles()
        {
            if (_tomRolesBackup != null)
            {
                _database.Model.Roles.Clear();
                _roles.Clear();

                foreach (ModelRole tomRole in _tomRolesBackup)
                {
                    CreateRole(tomRole, true);
                }
            }
        }

        /// <summary>
        /// Restore perspectives after changes to tables, measures, etc. Note that perspectives need to be done and dusted before restore cultures or dependency error.
        /// </summary>
        public void RestoreCultues()
        {
            if (_tomCulturesBackup != null)
            {
                _database.Model.Cultures.Clear();
                _cultures.Clear();

                foreach (Tom.Culture tomCulture in _tomCulturesBackup)
                {
                    CreateCulture(tomCulture);
                }
            }
        }

        #endregion

        #region Perspectives

        /// <summary>
        /// Delete perspective associated with the TabularModel object.
        /// </summary>
        /// <param name="name">Name of the perspective to be deleted.</param>
        public void DeletePerspective(string name)
        {
            if (_database.Model.Perspectives.ContainsName(name))
            {
                _database.Model.Perspectives.Remove(name);
            }

            // shell model
            if (_perspectives.ContainsName(name))
            {
                _perspectives.Remove(name);
            }
        }

        /// <summary>
        /// Create perspective associated with the TabularModel object.
        /// </summary>
        /// <param name="tomPerspective">Tabular Object Model Perspective object from the source tabular model to be abstracted in the target.</param>
        /// <returns>Newly abstracted Tabular Object Model Perspective object.</returns>
        public Tom.Perspective CreatePerspective(Tom.Perspective tomPerspective)
        {
            Tom.Perspective tomPerspectiveTarget = new Tom.Perspective();
            tomPerspectiveTarget.Name = tomPerspective.Name;
            _database.Model.Perspectives.Add(tomPerspectiveTarget);
            _perspectives.Add(new Perspective(this, tomPerspectiveTarget));

            SyncPerspectives(tomPerspective, tomPerspectiveTarget);
            return tomPerspectiveTarget;
        }

        /// <summary>
        /// Update perspective associated with the TabularModel object.
        /// </summary>
        /// <param name="tomPerspectiveSource">Tabular Object Model Perspective object from the source tabular model to be abstracted in the target.</param>
        /// <param name="tomPerspectiveTarget">Tabular Object Model Perspective object in the target tabular model to be abstracted.</param>
        public void UpdatePerspective(Tom.Perspective tomPerspectiveSource, Tom.Perspective tomPerspectiveTarget)
        {
            if (_comparisonInfo.OptionsInfo.OptionMergePerspectives)
            {
                Tom.Perspective perspectiveBackup = null;
                foreach (Tom.Perspective perspective in _tomPerspectivesBackup)
                {
                    if (perspective.Name == tomPerspectiveTarget.Name)
                    {
                        perspectiveBackup = perspective;
                        break;
                    }
                }

                if (perspectiveBackup != null)
                {
                    DeletePerspective(tomPerspectiveTarget.Name);
                    Tom.Perspective perspectiveTargetNew = CreatePerspective(perspectiveBackup);

                    SyncPerspectives(tomPerspectiveSource, perspectiveTargetNew);
                }
            }
            else
            {
                DeletePerspective(tomPerspectiveTarget.Name);
                CreatePerspective(tomPerspectiveSource);
            }
        }

        private void SyncPerspectives(Tom.Perspective perspectiveSource, Tom.Perspective perspectiveTarget)
        {
            //Tables
            foreach (PerspectiveTable perspectiveTableSource in perspectiveSource.PerspectiveTables)
            {
                Table tableTarget = _tables.FindByName(perspectiveTableSource.Name);
                if (tableTarget != null)
                {
                    //Following line is returning null in CTP3.3, when it shouldn't, so having to iterate to find
                    //PerspectiveTable perspectiveTableTarget = perspectiveTarget.PerspectiveTables.Find(perspectiveTableSource.Name); //When merging perspectives, might already be there
                    PerspectiveTable perspectiveTableTarget = null;
                    foreach (PerspectiveTable table in perspectiveTarget.PerspectiveTables)
                    {
                        if (table.Name == perspectiveTableSource.Name)
                        {
                            perspectiveTableTarget = table;
                            break;
                        }
                    }

                    if (perspectiveTableTarget == null)
                    {
                        perspectiveTableTarget = new PerspectiveTable();
                        perspectiveTarget.PerspectiveTables.Add(perspectiveTableTarget);
                        perspectiveTableTarget.Name = perspectiveTableSource.Name;
                        perspectiveTableTarget.Table = tableTarget.TomTable;
                    }

                    //Columns
                    foreach (PerspectiveColumn perspectiveColumnSource in perspectiveTableSource.PerspectiveColumns)
                    {
                        Column column = tableTarget.TomTable.Columns.Find(perspectiveColumnSource.Name);
                        if (column != null)
                        {
                            //Following line is returning null in CTP3.3, when it shouldn't, so having to iterate to find
                            //PerspectiveColumn perspectiveColumnTarget = perspectiveTarget.PerspectiveColumns.Find(perspectiveColumnSource.Name); //When merging perspectives, might already be there
                            PerspectiveColumn perspectiveColumnTarget = null;
                            foreach (PerspectiveColumn perspectiveColumn in perspectiveTableTarget.PerspectiveColumns)
                            {
                                if (perspectiveColumn.Name == perspectiveColumnSource.Name)
                                {
                                    perspectiveColumnTarget = perspectiveColumn;
                                    break;
                                }
                            }

                            if (perspectiveColumnTarget == null)
                            {
                                perspectiveColumnTarget = new PerspectiveColumn();
                                perspectiveTableTarget.PerspectiveColumns.Add(perspectiveColumnTarget);
                                perspectiveColumnTarget.Name = perspectiveColumnSource.Name;
                                perspectiveColumnTarget.Column = column;
                            }
                        }
                    }

                    //Hierarchies
                    foreach (PerspectiveHierarchy perspectiveHierarchySource in perspectiveTableSource.PerspectiveHierarchies)
                    {
                        Hierarchy hierarchy = tableTarget.TomTable.Hierarchies.Find(perspectiveHierarchySource.Name);
                        if (hierarchy != null)
                        {
                            //Following line is returning null in CTP3.3, when it shouldn't, so having to iterate to find
                            //PerspectiveHierarchy perspectiveHierarchyTarget = perspectiveTarget.PerspectiveHierarchies.Find(perspectiveHierarchySource.Name); //When merging perspectives, might already be there
                            PerspectiveHierarchy perspectiveHierarchyTarget = null;
                            foreach (PerspectiveHierarchy perspectiveHierarchy in perspectiveTableTarget.PerspectiveHierarchies)
                            {
                                if (perspectiveHierarchy.Name == perspectiveHierarchySource.Name)
                                {
                                    perspectiveHierarchyTarget = perspectiveHierarchy;
                                    break;
                                }
                            }

                            if (perspectiveHierarchyTarget == null)
                            {
                                perspectiveHierarchyTarget = new PerspectiveHierarchy();
                                perspectiveTableTarget.PerspectiveHierarchies.Add(perspectiveHierarchyTarget);
                                perspectiveHierarchyTarget.Name = perspectiveHierarchySource.Name;
                                perspectiveHierarchyTarget.Hierarchy = hierarchy;
                            }
                        }
                    }

                    //Measures
                    foreach (PerspectiveMeasure perspectiveMeasureSource in perspectiveTableSource.PerspectiveMeasures)
                    {
                        Tom.Measure measure = tableTarget.TomTable.Measures.Find(perspectiveMeasureSource.Name);
                        if (measure != null)
                        {
                            //Following line is returning null in CTP3.3, when it shouldn't, so having to iterate to find
                            //PerspectiveMeasure perspectiveMeasureTarget = perspectiveTarget.PerspectiveMeasures.Find(perspectiveMeasureSource.Name); //When merging perspectives, might already be there
                            PerspectiveMeasure perspectiveMeasureTarget = null;
                            foreach (PerspectiveMeasure perspectiveMeasure in perspectiveTableTarget.PerspectiveMeasures)
                            {
                                if (perspectiveMeasure.Name == perspectiveMeasureSource.Name)
                                {
                                    perspectiveMeasureTarget = perspectiveMeasure;
                                    break;
                                }
                            }

                            if (perspectiveMeasureTarget == null)
                            {
                                perspectiveMeasureTarget = new PerspectiveMeasure();
                                perspectiveTableTarget.PerspectiveMeasures.Add(perspectiveMeasureTarget);
                                perspectiveMeasureTarget.Name = perspectiveMeasureSource.Name;
                                perspectiveMeasureTarget.Measure = measure;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Cultures

        /// <summary>
        /// Delete culture associated with the TabularModel object.
        /// </summary>
        /// <param name="name">Name of the culture to be deleted.</param>
        public void DeleteCulture(string name)
        {
            if (_database.Model.Cultures.ContainsName(name))
            {
                _database.Model.Cultures.Remove(name);
            }

            // shell model
            if (_cultures.ContainsName(name))
            {
                _cultures.Remove(name);
            }
        }

        /// <summary>
        /// Create culture associated with the TabularModel object.
        /// </summary>
        /// <param name="tomCulture">Tabular Object Model Culture object from the source tabular model to be abstracted in the target.</param>
        /// <returns>Newly abstracted Tabular Object Model Culture object.</returns>
        public Tom.Culture CreateCulture(Tom.Culture tomCulture)
        {
            Tom.Culture tomCultureTarget = new Tom.Culture();
            tomCultureTarget.Name = tomCulture.Name;
            _database.Model.Cultures.Add(tomCultureTarget);
            _cultures.Add(new Culture(this, tomCultureTarget));

            SyncCultures(tomCulture, tomCultureTarget);
            return tomCultureTarget;
        }

        /// <summary>
        /// Update culture associated with the TabularModel object.
        /// </summary>
        /// <param name="tomCultureSource">Tabular Object Model Culture object from the source tabular model to be abstracted in the target.</param>
        /// <param name="tomCultureTarget">Tabular Object Model Culture object in the target tabular model to be abstracted.</param>
        public void UpdateCulture(Tom.Culture tomCultureSource, Tom.Culture tomCultureTarget)
        {
            if (_comparisonInfo.OptionsInfo.OptionMergeCultures)
            {
                Tom.Culture tomCultureBackup = null;
                foreach (Tom.Culture tomCulture in _tomCulturesBackup)
                {
                    if (tomCulture.Name == tomCultureTarget.Name)
                    {
                        tomCultureBackup = tomCulture;
                        break;
                    }
                }

                if (tomCultureBackup != null)
                {
                    DeleteCulture(tomCultureTarget.Name);
                    Tom.Culture tomCultureTargetNew = CreateCulture(tomCultureBackup);

                    SyncCultures(tomCultureSource, tomCultureTargetNew);
                }
            }
            else
            {
                DeleteCulture(tomCultureTarget.Name);
                CreateCulture(tomCultureSource);
            }
        }

        private void SyncCultures(Tom.Culture tomCultureSource, Tom.Culture tomCultureTarget)
        {
            foreach (ObjectTranslation translationSource in tomCultureSource.ObjectTranslations)
            {
                if (translationSource.Object is NamedMetadataObject)
                {
                    NamedMetadataObject namedObjectSource = (NamedMetadataObject)translationSource.Object;
                    NamedMetadataObject namedObjectTarget = null;

                    //Find the object in the target model that this translation applies to
                    switch (namedObjectSource.ObjectType)
                    {
                        case ObjectType.Model:
                            //if (namedObjectSource.Name == tomCultureTarget.Model.Name)
                            //{
                            //Model name can legitimately have different names - and there can only be 1 model, so we are OK not doing check in if clause above.
                            namedObjectTarget = tomCultureTarget.Model;
                            //}
                            break;
                        case ObjectType.Table:
                            foreach (Tom.Table tomTableTarget in tomCultureTarget.Model.Tables)
                            {
                                if (namedObjectSource.Name == tomTableTarget.Name)
                                {
                                    namedObjectTarget = tomTableTarget;
                                    break;
                                }
                            }
                            break;
                        case ObjectType.Column:
                            Column columnSource = (Column)namedObjectSource;
                            foreach (Tom.Table tableTarget in tomCultureTarget.Model.Tables)
                            {
                                bool foundColumn = false;
                                if (columnSource.Table?.Name == tableTarget.Name)
                                {
                                    foreach (Column columnTarget in tableTarget.Columns)
                                    {
                                        if (columnSource.Name == columnTarget.Name)
                                        {
                                            namedObjectTarget = columnTarget;
                                            foundColumn = true;
                                            break;
                                        }
                                    }
                                }
                                if (foundColumn) break;
                            }
                            break;
                        case ObjectType.Measure:
                            Tom.Measure tomMeasureSource = (Tom.Measure)namedObjectSource;
                            foreach (Tom.Table tomTableTarget in tomCultureTarget.Model.Tables)
                            {
                                bool foundMeasure = false;
                                if (tomMeasureSource.Table?.Name == tomTableTarget.Name)
                                {
                                    foreach (Tom.Measure measureTarget in tomTableTarget.Measures)
                                    {
                                        if (tomMeasureSource.Name == measureTarget.Name)
                                        {
                                            namedObjectTarget = measureTarget;
                                            foundMeasure = true;
                                            break;
                                        }
                                    }
                                }
                                if (foundMeasure) break;
                            }
                            break;
                        case ObjectType.Hierarchy:
                            Hierarchy hierarchySource = (Hierarchy)namedObjectSource;
                            foreach (Tom.Table tomTableTarget in tomCultureTarget.Model.Tables)
                            {
                                bool foundHierarchy = false;
                                if (hierarchySource.Table?.Name == tomTableTarget.Name)
                                {
                                    foreach (Hierarchy hierarchyTarget in tomTableTarget.Hierarchies)
                                    {
                                        if (hierarchySource.Name == hierarchyTarget.Name)
                                        {
                                            namedObjectTarget = hierarchyTarget;
                                            foundHierarchy = true;
                                            break;
                                        }
                                    }
                                }
                                if (foundHierarchy) break;
                            }
                            break;
                        case ObjectType.Level:
                            Level levelSource = (Level)namedObjectSource;
                            foreach (Tom.Table tomTableTarget in tomCultureTarget.Model.Tables)
                            {
                                bool foundLevel = false;
                                if (levelSource.Hierarchy?.Table?.Name == tomTableTarget.Name)
                                {
                                    foreach (Hierarchy hierarchyTarget in tomTableTarget.Hierarchies)
                                    {
                                        if (levelSource.Hierarchy?.Name == hierarchyTarget.Name)
                                        {
                                            foreach (Level levelTarget in hierarchyTarget.Levels)
                                            {
                                                if (levelSource.Name == levelTarget.Name)
                                                {
                                                    namedObjectTarget = levelTarget;
                                                    foundLevel = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (foundLevel) break;
                                    }
                                    if (foundLevel) break;
                                }
                            }
                            break;
                        case ObjectType.Perspective:
                            foreach (Tom.Perspective tomPerspectiveTarget in tomCultureTarget.Model.Perspectives)
                            {
                                if (namedObjectSource.Name == tomPerspectiveTarget.Name)
                                {
                                    namedObjectTarget = tomPerspectiveTarget;
                                    break;
                                }
                            }
                            break;
                        case ObjectType.Role:
                            foreach (ModelRole tomRoleTarget in tomCultureTarget.Model.Roles)
                            {
                                if (namedObjectSource.Name == tomRoleTarget.Name)
                                {
                                    namedObjectTarget = tomRoleTarget;
                                    break;
                                }
                            }
                            break;
                        case ObjectType.Expression:
                            foreach (NamedExpression tomExpressionTarget in tomCultureTarget.Model.Expressions)
                            {
                                if (namedObjectSource.Name == tomExpressionTarget.Name)
                                {
                                    namedObjectTarget = tomExpressionTarget;
                                    break;
                                }
                            }
                            break;
                        //case ObjectType.KPI:  //KPIs dealt with by measures above
                        //    break;
                        default:
                            break;
                    }

                    //If namedObjectTarget is null, the model object does not exist in target, so can ignore
                    if (namedObjectTarget != null)
                    {
                        //Does the translation already exist in cultureTarget?
                        ObjectTranslation translationTarget = null;
                        foreach (ObjectTranslation translation in tomCultureTarget.ObjectTranslations)
                        {
                            if (translation.Object is NamedMetadataObject &&
                                ((NamedMetadataObject)translation.Object).Name == namedObjectSource.Name &&
                                translation.Object.ObjectType == namedObjectSource.ObjectType &&
                                (
                                    //check columns are both in same table (could have columns with same name in different tables)
                                    !(translation.Object.Parent.ObjectType == ObjectType.Table && namedObjectSource.Parent.ObjectType == ObjectType.Table) ||
                                    (((NamedMetadataObject)translation.Parent).Name == ((NamedMetadataObject)namedObjectSource.Parent).Name) ||  //Is this line necessary? I don't think so.
                                    (((NamedMetadataObject)translation.Object.Parent).Name == ((NamedMetadataObject)namedObjectSource.Parent).Name)
                                ) &&
                                translation.Property == translationSource.Property
                               )
                            {
                                translationTarget = translation;
                                break;
                            }
                        }

                        if (translationTarget != null)
                        {   //Translation already exists in cultureTarget for this object, so just ensure values match
                            //Also decouple from object in model and reset coupling if removed
                            if (translationTarget.Object.IsRemoved)
                            {
                                ObjectTranslation translationTargetReplacement = new ObjectTranslation();
                                translationTargetReplacement.Object = namedObjectTarget;
                                translationTargetReplacement.Property = translationSource.Property;
                                translationTargetReplacement.Value = translationSource.Value;
                                tomCultureTarget.ObjectTranslations.Remove(translationTarget);
                                tomCultureTarget.ObjectTranslations.Add(translationTargetReplacement);
                                translationTarget = translationTargetReplacement;
                            }
                            //translationTarget.Object = namedObjectTarget;
                            translationTarget.Value = translationSource.Value;
                        }
                        else
                        {   //Translation does not exist in cultureTarget, so create it and add it to culture
                            translationTarget = new ObjectTranslation();
                            translationTarget.Object = namedObjectTarget;
                            translationTarget.Property = translationSource.Property;
                            translationTarget.Value = translationSource.Value;
                            tomCultureTarget.ObjectTranslations.Add(translationTarget);
                        }
                    }
                }
            }

            if (tomCultureSource.LinguisticMetadata != null)
            { 
                LinguisticMetadata linguisticMetadata = tomCultureSource.LinguisticMetadata.Clone();
                tomCultureTarget.LinguisticMetadata = linguisticMetadata;
            }
            else
            {
                tomCultureTarget.LinguisticMetadata = null;
            }
        }

        #endregion

        #region Roles

        /// <summary>
        /// Delete role associated with the TabularModel object.
        /// </summary>
        /// <param name="name">Name of the role to be deleted.</param>
        public void DeleteRole(string name)
        {
            if (_database.Model.Roles.Contains(name))
            {
                _database.Model.Roles.Remove(name);
            }

            // shell model
            if (_roles.ContainsName(name))
            {
                _roles.Remove(name);
            }
        }

        /// <summary>
        /// Create role associated with the TabularModel object.
        /// </summary>
        /// <param name="tomRoleSource">Tabular Object Model ModelRole object from the source tabular model to be abstracted in the target.</param>
        public ModelRole CreateRole(ModelRole tomRoleSource, bool restore)
        {
            ModelRole tomRoleTarget = new ModelRole();
            tomRoleSource.CopyTo(tomRoleTarget);

            List<string> permissionNamesToDelete = new List<string>();
            foreach (TablePermission permission in tomRoleTarget.TablePermissions)
            {
                if (_tables.ContainsName(permission.Name))
                {
                    //decouple table permissions from from original table to the one in target model (if exists)
                    permission.Table = _tables.FindByName(permission.Name).TomTable;
                }
                else
                {
                    permissionNamesToDelete.Add(permission.Name);
                }
            }
            foreach (string name in permissionNamesToDelete)
            {
                tomRoleTarget.TablePermissions.Remove(name);
            }

            //Workaround for bug where can't specify role member ID, only UPN
            tomRoleTarget.Members.Clear();
            foreach (ModelRoleMember roleMemberOrig in tomRoleSource.Members)
            {
                ModelRoleMember roleMemberTarget = roleMemberOrig.Clone();
                roleMemberTarget.MemberID = null;
                tomRoleTarget.Members.Add(roleMemberTarget);
            }

            if (!restore) tomRoleTarget.Annotations.Clear();
            _database.Model.Roles.Add(tomRoleTarget);

            // shell model
            _roles.Add(new Role(this, tomRoleTarget));

            return tomRoleTarget;
        }

        /// <summary>
        /// Update role associated with the TabularModel object.
        /// </summary>
        /// <param name="roleSource">Role object from the source tabular model to be updated in the target.</param>
        /// <param name="roleTarget">Role object in the target tabular model to be updated.</param>
        public void UpdateRole(Role roleSource, Role roleTarget)
        {
            if (_comparisonInfo.OptionsInfo.OptionRetainRoleMembers)
            {
                Tom.ModelRole tomModelRoleBackup = roleTarget.TomRole.Clone();
                DeleteRole(roleTarget.Name);
                Tom.ModelRole tomModelRoleNew = CreateRole(roleSource.TomRole, false);
                tomModelRoleNew.Members.Clear();
                foreach (ModelRoleMember roleMemberOrig in tomModelRoleBackup.Members)
                {
                    ModelRoleMember roleMemberTarget = roleMemberOrig.Clone();
                    tomModelRoleNew.Members.Add(roleMemberTarget);
                }
            }
            else
            {
                DeleteRole(roleTarget.Name);
                CreateRole(roleSource.TomRole, false);
            }
        }

        /// <summary>
        /// Remove any role references to non-existing tables.
        /// </summary>
        public void RolesCleanup()
        {
            //Check for roles' table permissions referring to non-existing tables
            foreach (ModelRole tomRole in _database.Model.Roles)
            {
                List<string> permissionNamesToDelete = new List<string>();

                foreach (TablePermission permission in tomRole.TablePermissions)
                {
                    if (permission.Table == null || !_tables.ContainsName(permission.Table.Name))
                    {
                        permissionNamesToDelete.Add(permission.Name);
                    }
                }

                foreach (string name in permissionNamesToDelete)
                {
                    tomRole.TablePermissions.Remove(name);
                }
            }
        }

        #endregion

        /// <summary>
        /// Update target tabular model with changes resulting from the comparison. For database deployment, this will fire DeployDatabaseCallBack.
        /// </summary>
        /// <returns>Boolean indicating whether update was successful.</returns>
        public bool Update()
        {
            SetBNormAnnotation();

            if (_connectionInfo.UseTmdlFolder)
            {
                SaveToTmdlFolder();
                return true;
            }

            if (_connectionInfo.UseBimFile)
            {
                SaveBimFile();
                return true;
            }

            if (_connectionInfo.UseProject)
            {
                UpdateProject();
            }
            else
            {   //Database deployement

                if (_comparisonInfo.PromptForDatabaseProcessing)
                {
                    //Call back to show deployment form
                    DatabaseDeploymentEventArgs args = new DatabaseDeploymentEventArgs();
                    _parentComparison.OnDatabaseDeployment(args);
                    return args.DeploymentSuccessful;
                }
                else
                {
                    //Simple update target without setting passwords or processing (mainly for command-line execution)
                    UpdateWithScript();
                }
            }

            return true;
        }

        private void SetBNormAnnotation()
        {
            //Set model annotation for telemetry tagging later
            const string AnnotationName = "__BNorm";
            Tom.Annotation annotationBNorm = new Tom.Annotation();
            annotationBNorm.Name = AnnotationName;
            annotationBNorm.Value = "1";
            if (!_model.TomModel.Annotations.Contains(AnnotationName))
                _model.TomModel.Annotations.Add(annotationBNorm);
        }

        private void UpdateProject()
        {
            UpdateWithScript();

            //if (_connectionInfo.Project != null)
            //{
            //    //Running in VS
            //    EnvDTE._DTE dte = _connectionInfo.Project.DTE;

            //    //check out bim file if necessary
            //    if (dte.SourceControl.IsItemUnderSCC(_connectionInfo.SsdtBimFile) && !dte.SourceControl.IsItemCheckedOut(_connectionInfo.SsdtBimFile))
            //    {
            //        dte.SourceControl.CheckOutItem(_connectionInfo.SsdtBimFile);
            //    }
            //}

            SaveBimFile();
        }

        private void SaveToTmdlFolder()
        {
            TmdlSerializer.SerializeDatabaseToFolder(_database, _connectionInfo.TmdlFolder);
        }

        private void SaveBimFile()
        {
            //Script out db and write to project file

            //serialize db to json
            SerializeOptions options = new SerializeOptions();
            bool isPbit = (_connectionInfo.UseBimFile && _connectionInfo.IsPbit);

            if (isPbit)
            {
                options.IgnoreInferredProperties = false;
                options.IgnoreInferredObjects = false;
                options.IgnoreTimestamps = false;
            }
            else
            { 
                options.IgnoreInferredProperties = true;
                options.IgnoreInferredObjects = true;
                options.IgnoreTimestamps = true;
            }
            options.SplitMultilineStrings = true;
            _database.CompatibilityMode = _connectionInfo.CompatibilityMode;
            string json = JsonSerializer.SerializeDatabase(_database, options);

            //replace db name with "SemanticModel"
            if (!isPbit)
            { 
                JObject jDb = JObject.Parse(json);
                jDb["name"] = "SemanticModel";
                jDb["id"] = "SemanticModel";
                json = jDb.ToString();
            }

            if (_connectionInfo.UseBimFile)
            {
                if (isPbit)
                {
                    PowerBiTemplate pbit = new PowerBiTemplate(_connectionInfo.BimFile);
                    pbit.ModelJson = json;
                    pbit.SaveAs(_connectionInfo.BimFile);
                }
                else
                { 
                    File.WriteAllText(_connectionInfo.BimFile, json);
                }
            }
            else
            {
                File.WriteAllText(_connectionInfo.SsdtBimFile, json);
            }
        }

        #region Database deployment and processing methods

        private const string _deployRowWorkItem = "Deploy metadata";
        private ProcessingTableCollection _tablesToProcess;

        /// <summary>
        /// Perform database deployment and processing of required tables.
        /// </summary>
        /// <param name="tablesToProcess">Tables to process.</param>
        public void DatabaseDeployAndProcess(ProcessingTableCollection tablesToProcess)
        {
            try
            {
                _tablesToProcess = tablesToProcess;

                //For each impersonated account ...
                //   1. Prompt for password and validate. If invalid back out of deployment. If valid, apply passwords.
                //   2. Deploy with script (using UpdateWithScript), which will lose the passwords.
                //   3. Re-apply the passwords.
                //Above steps allow backing out of deployment.

                //Set passwords
                foreach (Tom.DataSource dataSource in _database.Model.DataSources)
                {
                    PasswordPromptEventArgs args = new PasswordPromptEventArgs();
                    switch (dataSource.Type)
                    {
                        case DataSourceType.Structured:

                            StructuredDataSource structuredDataSource = (StructuredDataSource)dataSource;
                            args.AuthenticationKind = structuredDataSource.Credential.AuthenticationKind;

                            switch (structuredDataSource.Credential.AuthenticationKind)
                            {
                                case "Windows":
                                    //Same as impersonate account
                                    args.DataSourceName = dataSource.Name;
                                    args.Username = structuredDataSource.Credential.Username;
                                    _parentComparison.OnPasswordPrompt(args);
                                    if (args.UserCancelled)
                                    {
                                        // Show cancelled for all rows
                                        _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Deployment has been cancelled.", DeploymentStatus.Cancel));
                                        foreach (ProcessingTable table in _tablesToProcess)
                                        {
                                            _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Cancelled", DeploymentStatus.Cancel));
                                        }
                                        _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Cancel, null));
                                        return;
                                    }
                                    structuredDataSource.Credential.Username = args.Username;
                                    structuredDataSource.Credential.Password = args.Password;
                                    structuredDataSource.Credential.PrivacySetting = args.PrivacyLevel;
                                    break;

                                case "UsernamePassword":
                                    //Same as impersonate account
                                    args.DataSourceName = dataSource.Name;
                                    args.Username = structuredDataSource.Credential.Username;
                                    _parentComparison.OnPasswordPrompt(args);
                                    if (args.UserCancelled)
                                    {
                                        // Show cancelled for all rows
                                        _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Deployment has been cancelled.", DeploymentStatus.Cancel));
                                        foreach (ProcessingTable table in _tablesToProcess)
                                        {
                                            _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Cancelled", DeploymentStatus.Cancel));
                                        }
                                        _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Cancel, null));
                                        return;
                                    }
                                    structuredDataSource.Credential.Username = args.Username;
                                    structuredDataSource.Credential.Password = args.Password;
                                    structuredDataSource.Credential.PrivacySetting = args.PrivacyLevel;
                                    break;

                                case "Key":
                                    BlobKeyEventArgs keyArgs = new BlobKeyEventArgs();

                                    //Same as impersonate account
                                    keyArgs.DataSourceName = dataSource.Name;
                                    _parentComparison.OnBlobKeyPrompt(keyArgs);
                                    if (keyArgs.UserCancelled)
                                    {
                                        // Show cancelled for all rows
                                        _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Deployment has been cancelled.", DeploymentStatus.Cancel));
                                        foreach (ProcessingTable table in _tablesToProcess)
                                        {
                                            _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Cancelled", DeploymentStatus.Cancel));
                                        }
                                        _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Cancel, null));
                                        return;
                                    }
                                    structuredDataSource.Credential[CredentialProperty.Key] = keyArgs.AccountKey;
                                    structuredDataSource.Credential.PrivacySetting = keyArgs.PrivacyLevel;
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DataSourceType.Provider:
                            ProviderDataSource providerDataSource = (ProviderDataSource)dataSource;

                            if (providerDataSource.ImpersonationMode == ImpersonationMode.ImpersonateAccount)
                            {
                                args.AuthenticationKind = "Windows";
                                args.DataSourceName = dataSource.Name;
                                args.Username = providerDataSource.Account;
                                args.PrivacyLevel = "NA";
                                _parentComparison.OnPasswordPrompt(args);
                                if (args.UserCancelled)
                                {
                                    // Show cancelled for all rows
                                    _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Deployment has been cancelled.", DeploymentStatus.Cancel));
                                    foreach (ProcessingTable table in _tablesToProcess)
                                    {
                                        _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Cancelled", DeploymentStatus.Cancel));
                                    }
                                    _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Cancel, null));
                                    return;
                                }
                                providerDataSource.Account = args.Username;
                                providerDataSource.Password = args.Password;
                            }
                            break;
                        default:
                            break;
                    }
                }

                UpdateWithScript();
                _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Success. Metadata deployed.", DeploymentStatus.Success));

                //Kick off processing
                ProcessAsyncDelegate processAsyncCaller = new ProcessAsyncDelegate(Process);
                processAsyncCaller.BeginInvoke(null, null);

                _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Success, null));
            }
            catch (Exception exc)
            {
                _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Error deploying metadata.", DeploymentStatus.Error));
                foreach (ProcessingTable table in _tablesToProcess)
                {
                    _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Error", DeploymentStatus.Error));
                }
                _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Error, exc.Message));
            }
        }

        private void UpdateWithScript()
        {
            if (_connectionInfo.UseDesktop)
            {
                _database.Update(Amo.UpdateOptions.ExpandFull); //Not bothered about losing partition data, and till Desktop hardened, won't accept the full TMSL command
            }
            else
            {
                //_database.Update(Amo.UpdateOptions.ExpandFull); //If make minor changes to table (e.g. display folder) without changes to the partition or column structure, this command will still lose the data due to TOM applying a full log of operations. So instead reconnect and run TMSL script.

                //includeRestrictedInformation only includes passwords in connections if they were added during this session (does not allow derivation of passwords from the server)
                string tmslCommand = JsonScripter.ScriptCreateOrReplace(_database, includeRestrictedInformation: true);

                _server.Disconnect();
                _server = new Server();
                _server.Connect(_connectionInfo.BuildConnectionString());
                Amo.XmlaResultCollection results = _server.Execute(tmslCommand);
                if (results.ContainsErrors)
                    throw new Amo.OperationException(results);

                _database = _server.Databases.FindByName(_connectionInfo.DatabaseName);
            }

            //FROM THIS POINT ONWARDS use only TOM as have not bothered re-hydrating the BismNorm object model
        }

        private bool _stopProcessing;
        string _sessionId;
        internal delegate void ProcessAsyncDelegate();
        private void Process()
        {
            //string x13 = TraceColumn.ObjectName.ToString();
            //string x15 = TraceColumn.ObjectReference.ToString();
            //string x10 = TraceColumn.IntegerData.ToString();

            Trace trace = null;
            try
            {
                _stopProcessing = false;
                RefreshType refreshType = RefreshType.Calculate;
                switch (_comparisonInfo.OptionsInfo.OptionProcessingOption)
                {
                    //case ProcessingOption.Recalc:
                    //    refreshType = RefreshType.Calculate;
                    //    break;
                    case ProcessingOption.Default:
                        refreshType = RefreshType.Automatic;
                        break;
                    //case ProcessingOption.DoNotProcess:
                    //    break;
                    case ProcessingOption.Full:
                        refreshType = RefreshType.Full;
                        break;
                    default:
                        break;
                }

                TraceEvent traceEvent = null;
                if (refreshType != RefreshType.Calculate)
                { 
                    //Set up server trace to capture how many rows processed
                    _sessionId = _server.SessionID;
                    trace = _server.Traces.Add();
                    traceEvent = trace.Events.Add(Amo.TraceEventClass.ProgressReportCurrent);
                    traceEvent.Columns.Add(Amo.TraceColumn.ObjectID);
                    traceEvent.Columns.Add(Amo.TraceColumn.ObjectName);
                    traceEvent.Columns.Add(Amo.TraceColumn.ObjectReference);
                    traceEvent.Columns.Add(Amo.TraceColumn.IntegerData);
                    traceEvent.Columns.Add(Amo.TraceColumn.SessionID);
                    traceEvent.Columns.Add(Amo.TraceColumn.Spid);
                    trace.Update(Amo.UpdateOptions.Default, Amo.UpdateMode.CreateOrReplace);
                    trace.OnEvent += new TraceEventHandler(Trace_OnEvent);
                    trace.Start();

                    if (_tablesToProcess.Count > 0)
                    {
                        foreach (ProcessingTable tableToProcess in _tablesToProcess)
                        {
                            Tom.Table table = _database.Model.Tables.Find(tableToProcess.Name);
                            if (table != null)
                            {
                                table.RequestRefresh(refreshType);
                            }
                        }
                    }
                }

                if (_comparisonInfo.OptionsInfo.OptionProcessingOption != ProcessingOption.DoNotProcess)
                {
                    //Need recalc even if created no tables in case of new relationships without tables
                    _database.Model.RequestRefresh(RefreshType.Calculate);
                    _database.Model.SaveChanges();
                }

                if (refreshType != RefreshType.Calculate)
                {
                    // Show row count for each table
                    foreach (ProcessingTable table in _tablesToProcess)
                    {
                        string message = "";
                        if (
                                this._tables.FindByName(table.Name)?.TableModeType == ModeType.DirectQuery ||
                               (this._tables.FindByName(table.Name)?.TableModeType == ModeType.Default && _database.Model.DefaultMode == ModeType.DirectQuery)
                           )
                        {
                            message = "Success. 0 rows transferred (DirectQuery).";
                        }
                        else
                        {
                            Int64 rowCount = Core.Comparison.FindRowCount(_server, table.Name, _database.Name);
                            message = $"Success. {String.Format("{0:#,###0}", rowCount)} rows.";
                        }
                        _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, message, DeploymentStatus.Success));
                    }
                }
                else
                {
                    _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_database.Name, "Success. Process recalc done.", DeploymentStatus.Success));
                }

                _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Success, null));
            }
            //catch (InvalidOperationException exc) when (exc.Message == "Operation is not valid due to the current state of the object.")
            //{ //Azure AS sometimes loses connection - need extra filter for after save changes.
            //} 
            catch (Exception exc)
            {
                ShowErrorsForAllRows();
                _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Error, exc.Message));
            }
            finally
            {
                try
                {
                    if (trace != null)
                    {
                        trace.Stop();
                        trace.Drop();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Stop processing if possible.
        /// </summary>
        public void StopProcessing()
        {
            _stopProcessing = true;

            if (_comparisonInfo.OptionsInfo.OptionTransaction)
            {
                try
                {
                    _server.RollbackTransaction();
                    ShowErrorsForAllRows();
                    _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Error, "Rolled back transaction."));
                }
                catch (Exception exc)
                {
                    if (exc is NullReferenceException || exc is InvalidOperationException)
                    {
                        return;
                    }
                    else
                    {
                        ShowErrorsForAllRows();
                        _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Error, exc.Message));
                    }
                }
            }
        }

        private void Trace_OnEvent(object sender, TraceEventArgs e)
        {
            if (e.ObjectName != null && e.ObjectReference != null && e.SessionID == _sessionId)
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(e.ObjectReference);

                XmlNodeList partitionNodeList = document.GetElementsByTagName("Partition");
                XmlNodeList tableNodeList = document.GetElementsByTagName("Table");

                if (partitionNodeList != null && partitionNodeList.Count > 0 && tableNodeList != null && tableNodeList.Count > 0)
                {
                    if (_tablesToProcess.ContainsId(tableNodeList[0].InnerText))
                    {
                        ProcessingTable processingTable = _tablesToProcess.FindById(tableNodeList[0].InnerText);

                        if (!processingTable.ContainsPartition(partitionNodeList[0].InnerText))
                        {
                            processingTable.Partitions.Add(new PartitionRowCounter(partitionNodeList[0].InnerText));
                        }

                        PartitionRowCounter partition = processingTable.FindPartition(partitionNodeList[0].InnerText);
                        partition.RowCount = e.IntegerData;

                        _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(processingTable.Name, $"Retrieved {String.Format("{0:#,###0}", processingTable.GetRowCount())} rows ...", DeploymentStatus.Deploying));
                    }
                }

                if (_stopProcessing && !_comparisonInfo.OptionsInfo.OptionTransaction) //transactions get cancelled in StopProcessing, not here
                {
                    try
                    {
                        _server.CancelCommand(_sessionId);
                    }
                    catch { }
                }

                ////Doesn't work with Spid, so doing sessionid above
                //int spid;
                //if (_stopProcessing && int.TryParse(e.Spid, out spid))
                //{
                //    try
                //    {
                //        //_amoServer.CancelCommand(e.Spid);
                //        string commandStatement = String.Format("<Cancel xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\"><SPID>{0}</SPID><CancelAssociated>1</CancelAssociated></ Cancel>", e.Spid);
                //        System.Diagnostics.Debug.WriteLine(commandStatement);
                //        _amoServer.Execute(commandStatement);
                //        //_connectionInfo.ExecuteXmlaCommand(_amoServer, commandStatement);
                //    }
                //    catch { }
                //}
            }
        }

        private void ShowErrorsForAllRows()
        {
            // Show error for each item
            if (_comparisonInfo.OptionsInfo.OptionTransaction)
            {
                _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Error", DeploymentStatus.Error));
            }
            foreach (ProcessingTable table in _tablesToProcess)
            {
                _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Error", DeploymentStatus.Error));
            }
        }

        #endregion

        /// <summary>
        /// Generate script containing full tabular model definition.
        /// </summary>
        /// <returns>JSON script of tabular model defintion.</returns>
        public string ScriptDatabase()
        {
            SetBNormAnnotation();

            //script db to json
            string json = JsonScripter.ScriptCreateOrReplace(_database);

            if (_connectionInfo.UseProject || _connectionInfo.UseBimFile)
            {
                //replace db/cube name/id with name of deploymnet db from the project file
                JObject jScript = JObject.Parse(json);
                JObject createOrReplace = (JObject)jScript["createOrReplace"];
                if (!String.IsNullOrEmpty(_connectionInfo.DeploymentServerDatabase))
                {
                    ((JObject)createOrReplace["object"])["database"] = _connectionInfo.DeploymentServerDatabase;
                    ((JObject)createOrReplace["database"])["name"] = _connectionInfo.DeploymentServerDatabase;
                    ((JObject)createOrReplace["database"])["id"] = _connectionInfo.DeploymentServerDatabase;
                }
                else if (_connectionInfo.UseTmdlFolder)
                {
                    try
                    {
                        string folderName = Path.GetDirectoryName(_connectionInfo.TmdlFolder);
                        ((JObject)createOrReplace["object"])["database"] = folderName;
                        ((JObject)createOrReplace["database"])["name"] = folderName;
                    }
                    catch { }
                }
                else if (_connectionInfo.UseBimFile)
                {
                    try
                    {
                        string fileName = Path.GetFileNameWithoutExtension(_connectionInfo.BimFile);
                        ((JObject)createOrReplace["object"])["database"] = fileName;
                        ((JObject)createOrReplace["database"])["name"] = fileName;
                    }
                    catch { }
                }
                json = jScript.ToString();
            }

            return json;
        }

        public override string ToString() => this.GetType().FullName;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_server != null)
                    {
                        _server.Dispose();
                    }
                }

                _disposed = true;
            }
        }

    }

    public struct RelationshipToRetain
    {
        public SingleColumnRelationship TomRelationship;
        public string FromTableName;
        public string FromColumnName;
        public string ToTableName;
        public string ToColumnName;

        public RelationshipToRetain(SingleColumnRelationship tomRelationship, string fromTableName, string fromColumnName, string toTableName, string toColumnName)
        {
            TomRelationship = tomRelationship;
            FromTableName = fromTableName;
            FromColumnName = fromColumnName;
            ToTableName = toTableName;
            ToColumnName = toColumnName;
        }
    }

}
