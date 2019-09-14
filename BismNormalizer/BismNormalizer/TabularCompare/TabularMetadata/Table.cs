using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using Microsoft.AnalysisServices.Tabular;
using Tom=Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model table with properties and methods for comparison purposes.
    /// </summary>
    public class Table : TabularObject
    {
        private TabularModel _parentTabularModel;
        private Tom.Table _tomTable;
        private string _partitionsDefinition;
        private string _dataSourceName;
        private RelationshipCollection _relationships = new RelationshipCollection();
        private MeasureCollection _measures = new MeasureCollection();
        private bool _isCalculationGroup;
        private ModeType _tableModeType;
        private CalculationItemCollection _calculationItems = new CalculationItemCollection();

        /// <summary>
        /// Initializes a new instance of the Table class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the Table object belongs to.</param>
        /// <param name="tomTable">Tabular Object Model Table object abtstracted by the Table class.</param>
        public Table(TabularModel parentTabularModel, Tom.Table tomTable) : base(tomTable)
        {
            _parentTabularModel = parentTabularModel;
            _tomTable = tomTable;

            PopulateProperties();
        }

        /// <summary>
        /// TabularModel object that the Table object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// For tables with M/query partitions, return the partitions definition.
        /// </summary>
        public string PartitionsDefinition => _partitionsDefinition;

        /// <summary>
        /// Name of the DataSource object that the Table object belongs to.
        /// </summary>
        public string DataSourceName => _dataSourceName;

        /// <summary>
        /// Collection of relationships for the Table object.
        /// </summary>
        public RelationshipCollection Relationships => _relationships;

        /// <summary>
        /// Collection of measures for the Table object.
        /// </summary>
        public MeasureCollection Measures => _measures;

        /// <summary>
        /// True if the table is a calculation group.
        /// </summary>
        public bool IsCalculationGroup => _isCalculationGroup;

        public ModeType TableModeType => _tableModeType;

        /// <summary>
        /// Collection of calculation items for the Table object.
        /// </summary>
        public CalculationItemCollection CalculationItems => _calculationItems;

        /// <summary>
        /// Tabular Object Model Table object abtstracted by the Table class.
        /// </summary>
        public Tom.Table TomTable => _tomTable;

        private void PopulateProperties()
        {
            base.RemovePropertyFromObjectDefinition("measures");

            _isCalculationGroup = (_tomTable.CalculationGroup != null);
            _partitionsDefinition = "";
            _dataSourceName = "";
            bool hasMQueryOrPolicyPartition = false;

            //Associate table with a DataSource if possible. It's not possible if calc table or if M expression refers to a shared expression, or multiple data sources
            foreach (Partition partition in _tomTable.Partitions)
            {
                _tableModeType = partition.Mode;
                if (partition.SourceType == PartitionSourceType.M)
                {
                    hasMQueryOrPolicyPartition = true;

                    //Check M dependency tree to see if all partitions refer only to a single DataSource
                    CalcDependencyCollection calcDependencies = _parentTabularModel.MDependencies.DependenciesReferenceFrom(CalcDependencyObjectType.Partition, _tomTable.Name, partition.Name);
                    if (calcDependencies.Count == 1 && calcDependencies[0].ReferencedObjectType == CalcDependencyObjectType.DataSource)
                    {
                        if (_dataSourceName == "")
                        {
                            _dataSourceName = calcDependencies[0].ReferencedObjectName;
                        }
                        else if (_dataSourceName != calcDependencies[0].ReferencedObjectName)
                        {
                            //Partition depends on a different DataSource to another partition in same table, so ensure no DataSource association for the table and stop iterating partitions.
                            _dataSourceName = "";
                            break;
                        }
                    }
                    else
                    {
                        //Partition has mutiple dependencies, or depends on an expression instead of DataSource, so ensure no DataSource association for the table and stop iterating partitions.
                        _dataSourceName = "";
                        break;
                    }
                }

                //If old partition, find the primary partition (first one) to determine DataSource. Technically it is possible for different partitions in the same table to point to different DataSources, but the Tabular Designer in VS doesn't support it. If set manually in .bim file, the UI still associates with the first partition (e.g. when processing table by itself, or deletinig the DataSource gives a warning message listing associated tables).
                if (partition.SourceType == PartitionSourceType.Query)
                {
                    hasMQueryOrPolicyPartition = true;
                    _dataSourceName = ((QueryPartitionSource)partition.Source).DataSource.Name;
                    break;
                }

                //Might be a policy partition.
                if (partition.SourceType == PartitionSourceType.PolicyRange)
                {
                    hasMQueryOrPolicyPartition = true;
                    break;
                }
            }

            if (hasMQueryOrPolicyPartition || _isCalculationGroup)
            {
                _partitionsDefinition = base.RetrievePropertyFromObjectDefinition("partitions");

                //Option to hide partitions only applies to M, query and policy partitions (calculated tables hold dax defintitions in their partitions)
                if (!_parentTabularModel.ComparisonInfo.OptionsInfo.OptionPartitions)
                {
                    base.RemovePropertyFromObjectDefinition("partitions");
                }
            }

            //Find table relationships
            foreach (Tom.Relationship relationship in _tomTable.Model.Relationships)
            {
                if (relationship.FromTable.Name == _tomTable.Name && relationship.Type == RelationshipType.SingleColumn)  //currently only support single column
                {
                    _relationships.Add(new Relationship(this, (SingleColumnRelationship)relationship));
                }
            }

            //Find measures
            foreach (Tom.Measure measure in _tomTable.Measures)
            {
                _measures.Add(new Measure(this, measure, measure.KPI != null));
            }

            //Find calc items
            if (_isCalculationGroup)
            {
                foreach (Tom.CalculationItem calcItem in _tomTable.CalculationGroup.CalculationItems)
                {
                    _calculationItems.Add(new CalculationItem(this, calcItem));
                }
            }
        }

        #region Relationship collection methods

        /// <summary>
        /// Delete all associated relationships including those from other tables that refer to this table.
        /// </summary>
        /// <returns>Collection of all associated relationships that were deleted. Useful if updating tables as then need to add back.</returns>
        public List<SingleColumnRelationship> DeleteAllAssociatedRelationships()
        {
            List<SingleColumnRelationship> relationshipsToDelete = new List<SingleColumnRelationship>();

            foreach (Table table in _parentTabularModel.Tables)
            {
                List<string> relationshipsToDeleteInternalNames = new List<string>();
                foreach (Relationship relationship in table.Relationships)
                {
                    if (relationship.FromTableName == this.Name || relationship.ToTableName == this.Name)
                    {
                        SingleColumnRelationship relationshipTarget = new SingleColumnRelationship();
                        relationship.TomRelationship.CopyTo(relationshipTarget);

                        relationshipsToDelete.Add(relationshipTarget);
                        relationshipsToDeleteInternalNames.Add(relationship.InternalName);
                    }
                }
                foreach (string relationshipToDeleteInternalName in relationshipsToDeleteInternalNames)
                {
                    table.DeleteRelationship(relationshipToDeleteInternalName);
                }
            }
            return relationshipsToDelete;
        }

        /// <summary>
        /// Find all direct relationships that filter this table. This is all ACTIVE relationships where 1) this is FROM table, or 2) this is TO table with CrossFilteringBehavior=BothDirections
        /// </summary>
        /// <returns>All the associated Relationships.</returns>
        public List<Relationship> FindFilteredRelationships(bool checkSecurityBehavior = false)  
        {
            //T1[C1]->T2[C2]
            //FromTableName: T1   *** this.Name
            //ToTableName:   T2   

            //Considers DIRECT relationships for this table ONLY (1 level).
            List<Relationship> filteringRelationships = new List<Relationship>();
            foreach (Table table in _parentTabularModel.Tables)
            {
                foreach (Relationship relationship in table.Relationships)
                {
                    if (relationship.TomRelationship.IsActive &&
                           (relationship.FromTableName == this.Name ||
                               (relationship.ToTableName == this.Name && relationship.TomRelationship.CrossFilteringBehavior == CrossFilteringBehavior.BothDirections && (!checkSecurityBehavior || (checkSecurityBehavior && relationship.TomRelationship.SecurityFilteringBehavior == SecurityFilteringBehavior.BothDirections)))
                           )
                       )
                    {
                        filteringRelationships.Add(relationship);
                    }
                }
            }
            return filteringRelationships;
        }

        /// <summary>
        /// Find all direct relationships that filter this table. This is all ACTIVE relationships where 1) this is FROM table, or 2) this is TO table with CrossFilteringBehavior=BothDirections
        /// </summary>
        /// <returns>All the associated Relationships.</returns>
        public List<Relationship> FindFilteringRelationships(bool checkSecurityBehavior = false)
        {
            //T1[C1]->T2[C2]
            //FromTableName: T1
            //ToTableName:   T2   *** this.Name

            //Considers DIRECT relationships for this table ONLY (1 level).
            List<Relationship> filteringRelationships = new List<Relationship>();
            foreach (Table table in _parentTabularModel.Tables)
            {
                foreach (Relationship relationship in table.Relationships)
                {
                    if (relationship.TomRelationship.IsActive &&
                           (relationship.ToTableName == this.Name ||
                               (relationship.FromTableName == this.Name && relationship.TomRelationship.CrossFilteringBehavior == CrossFilteringBehavior.BothDirections && (!checkSecurityBehavior || (checkSecurityBehavior && relationship.TomRelationship.SecurityFilteringBehavior == SecurityFilteringBehavior.BothDirections)))
                           )
                       )
                    {
                        filteringRelationships.Add(relationship);
                    }
                }
            }
            return filteringRelationships;
        }

        #endregion

        #region Update Actions

        // Relationships

        /// <summary>
        /// Delete relationship associated with the Table object.
        /// </summary>
        /// <param name="internalName">Internal name of the relationship to be deleted.</param>
        public void DeleteRelationship(string internalName)
        {
            if (_tomTable.Model.Relationships.Contains(internalName))
            {
                _tomTable.Model.Relationships.Remove(internalName);
            }

            // shell model
            if (_relationships.ContainsInternalName(internalName))
            {
                _relationships.RemoveByInternalName(internalName);
            }
        }

        /// <summary>
        /// Update relationship associated with the Table object.
        /// </summary>
        /// <param name="relationshipSource">Relationship object from the source tabular model.</param>
        /// <param name="toTomTableSource">Tabular Object Model Table representing "to table" in the relationship.</param>
        /// <param name="relationshipName">Name of the relationship to be updated.</param>
        /// <param name="warningMessage">Warning message to return to caller.</param>
        /// <returns>Boolean indicating if update was successful.</returns>
        public bool UpdateRelationship(Relationship relationshipSource, Tom.Table toTomTableSource, string relationshipName, ref string warningMessage)
        {
            SingleColumnRelationship tabularRelationshipSource = relationshipSource.TomRelationship;

            // Check if "to" table exists (don't need to check "from" table as we are in the "from" table) ...
            if (!_parentTabularModel.Tables.ContainsName(toTomTableSource.Name))
            {
                warningMessage = $"Unable to update Relationship {relationshipName} because (considering changes) parent table not found in target model.";
                return false;
            }

            // does the required child column exist?  In this case need to check child column as user might have skipped Update of table meaning columns are out of sync.
            if (!_tomTable.Columns.ContainsName(tabularRelationshipSource.FromColumn.Name))
            {
                warningMessage = $"Unable to update Relationship {relationshipName} because (considering changes) child column not found in target model.";
                return false;
            }

            // does the required "to" column exist?
            Tom.Table toTableTarget = _parentTabularModel.Tables.FindByName(tabularRelationshipSource.ToTable.Name).TomTable;
            if (
                    (toTableTarget == null) ||
                    (!toTableTarget.Columns.ContainsName(tabularRelationshipSource.ToColumn.Name))
                )
            {
                warningMessage = $"Unable to update Relationship {relationshipName} because (considering changes) parent column not found in target model.";
                return false;
            }

            // at this point we know we will update the relationship
            SingleColumnRelationship relationshipTarget = new SingleColumnRelationship();
            tabularRelationshipSource.CopyTo(relationshipTarget);

            //decouple from original table to the current one
            relationshipTarget.FromColumn = this.TomTable.Columns.Find(relationshipTarget.FromColumn.Name);
            relationshipTarget.ToColumn = toTableTarget.Columns.Find(relationshipTarget.ToColumn.Name);

            // Delete the target relationship with same tables/columns if still there. Not using RemoveByInternalName in case internal name is actually different.
            if (this.Relationships.ContainsName(relationshipSource.Name))
            {
                this.DeleteRelationship(this.Relationships.FindByName(relationshipSource.Name).InternalName);
            }

            CreateRelationship(relationshipTarget);
            return true;
        }

        /// <summary>
        /// Create a relationship for the Table object, with validation to ensure referential integrity.
        /// </summary>
        /// <param name="relationshipSource">Relationship object from the source tabular model.</param>
        /// <param name="toTomTableSource">Tabular Object Model Table representing "to table" in the relationship.</param>
        /// <param name="relationshipName">Name of the relationship to be created.</param>
        /// <param name="warningMessage">Warning message to return to caller.</param>
        /// <returns>Boolean indicating if creation was successful.</returns>
        public bool CreateRelationshipWithValidation(Relationship relationshipSource, Tom.Table toTomTableSource, string relationshipName, ref string warningMessage)
        {
            SingleColumnRelationship tabularRelationshipSource = relationshipSource.TomRelationship;

            // Check if "to" table exists (don't need to check "from" table as we are in the "from" table) ...
            if (!_parentTabularModel.Tables.ContainsName(toTomTableSource.Name))
            {
                warningMessage = $"Unable to create Relationship {relationshipName} because (considering changes) parent table not found in target model.";
                return false;
            }

            // does the required child column exist?  In this case need to check child column as user might have skipped Update of table meaning columns are out of sync.
            if (!_tomTable.Columns.ContainsName(tabularRelationshipSource.FromColumn.Name))
            {
                warningMessage = $"Unable to create Relationship {relationshipName} because (considering changes) child column not found in target model.";
                return false;
            }

            // does the required "to" column exist?
            Tom.Table toTableTarget = _parentTabularModel.Tables.FindByName(tabularRelationshipSource.ToTable.Name).TomTable;
            if (
                    (toTableTarget == null) ||
                    (!toTableTarget.Columns.ContainsName(tabularRelationshipSource.ToColumn.Name))
                )
            {
                warningMessage = $"Unable to create Relationship {relationshipName} because (considering changes) parent column not found in target model.";
                return false;
            }

            if (this.IsCalculationGroup || _parentTabularModel.Tables.FindByName(tabularRelationshipSource.ToTable.Name).IsCalculationGroup)
            {
                warningMessage = $"Unable to create Relationship {relationshipName} because one or more tables is a calculation group.";
                return false;
            }

            // Delete the target relationship with same tables/columns if still there. Not using RemoveByInternalName in case internal name is actually different.
            if (this.Relationships.ContainsName(relationshipSource.Name))
            {
                warningMessage = $"Unable to create Relationship {relationshipName} because (considering changes) relationship already exists in target model.";
                return false;
            }

            // at this point we know we will update the relationship
            SingleColumnRelationship relationshipTarget = new SingleColumnRelationship();
            tabularRelationshipSource.CopyTo(relationshipTarget);

            //decouple from original table to the current one
            relationshipTarget.FromColumn = this.TomTable.Columns.Find(relationshipTarget.FromColumn.Name);
            relationshipTarget.ToColumn = toTableTarget.Columns.Find(relationshipTarget.ToColumn.Name);

            CreateRelationship(relationshipTarget);
            return true;
        }

        /// <summary>
        /// Create a relationship for the Table object.
        /// </summary>
        /// <param name="tomRelationshipTarget">Tabular Object Model SingleColumnRelationship object to be abstracted by the Relationship object being created.</param>
        public void CreateRelationship(SingleColumnRelationship tomRelationshipTarget)
        {
            bool modifiedInternalName = false;
            string oldInternalName = "";

            // check if there is an existing relationship with same internal name
            if (_parentTabularModel.ContainsRelationshipByInternalName(tomRelationshipTarget.Name))
            {
                modifiedInternalName = true;
                oldInternalName = tomRelationshipTarget.Name;
                tomRelationshipTarget.Name = Convert.ToString(Guid.NewGuid());
            }

            _parentTabularModel.TomDatabase.Model.Relationships.Add(tomRelationshipTarget);
            _relationships.Add(new Relationship(this, tomRelationshipTarget, copiedFromSource: true, modifiedInternalName: modifiedInternalName, oldInternalName: oldInternalName));
        }

        // Measures

        /// <summary>
        /// Delete measure associated with the Table object.
        /// </summary>
        /// <param name="name">Name of the measure to be deleted.</param>
        public void DeleteMeasure(string name)
        {
            if (_tomTable.Measures.ContainsName(name))
            {
                _tomTable.Measures.Remove(name);
            }

            // shell model
            if (_measures.ContainsName(name))
            {
                _measures.RemoveByName(name);
            }
        }

        /// <summary>
        /// Create measure associated with the Table object.
        /// </summary>
        /// <param name="tomMeasureSource">Tabular Object Model Measure object from the source tabular model to be abstracted in the target.</param>
        public void CreateMeasure(Tom.Measure tomMeasureSource)
        {
            if (_tomTable.Measures.ContainsName(tomMeasureSource.Name))
            {
                _tomTable.Measures.Remove(tomMeasureSource.Name);
            }

            Tom.Measure tomMeasureTarget = new Tom.Measure();
            tomMeasureSource.CopyTo(tomMeasureTarget);
            _tomTable.Measures.Add(tomMeasureTarget);

            // shell model
            _measures.Add(new Measure(this, tomMeasureTarget, tomMeasureTarget.KPI != null));
        }

        /// <summary>
        /// Update measure associated with the Table object.
        /// </summary>
        /// <param name="tomMeasureSource">Tabular Object Model Measure object from the source tabular model to be abstracted in the target.</param>
        public void UpdateMeasure(Tom.Measure tomMeasureSource)
        {
            if (_measures.ContainsName(tomMeasureSource.Name))
            {
                DeleteMeasure(tomMeasureSource.Name);
            }
            CreateMeasure(tomMeasureSource);
        }

        // CalculationItems

        /// <summary>
        /// Delete calculation item associated with the Table object.
        /// </summary>
        /// <param name="name">Name of the calculationItem to be deleted.</param>
        public void DeleteCalculationItem(string name)
        {
            if (_tomTable.CalculationGroup.CalculationItems.ContainsName(name))
            {
                _tomTable.CalculationGroup.CalculationItems.Remove(name);
            }

            // shell model
            if (_calculationItems.ContainsName(name))
            {
                _calculationItems.RemoveByName(name);
            }
        }

        /// <summary>
        /// Create calculationItem associated with the Table object.
        /// </summary>
        /// <param name="tomCalculationItemSource">Tabular Object Model CalculationItem object from the source tabular model to be abstracted in the target.</param>
        public void CreateCalculationItem(Tom.CalculationItem tomCalculationItemSource)
        {
            if (_tomTable.CalculationGroup.CalculationItems.ContainsName(tomCalculationItemSource.Name))
            {
                _tomTable.CalculationGroup.CalculationItems.Remove(tomCalculationItemSource.Name);
            }

            Tom.CalculationItem tomCalculationItemTarget = new Tom.CalculationItem();
            tomCalculationItemSource.CopyTo(tomCalculationItemTarget);
            _tomTable.CalculationGroup.CalculationItems.Add(tomCalculationItemTarget);

            // shell model
            _calculationItems.Add(new CalculationItem(this, tomCalculationItemTarget));
        }

        /// <summary>
        /// Update calculationItem associated with the Table object.
        /// </summary>
        /// <param name="tomCalculationItemSource">Tabular Object Model CalculationItem object from the source tabular model to be abstracted in the target.</param>
        public void UpdateCalculationItem(Tom.CalculationItem tomCalculationItemSource)
        {
            if (_calculationItems.ContainsName(tomCalculationItemSource.Name))
            {
                DeleteCalculationItem(tomCalculationItemSource.Name);
            }
            CreateCalculationItem(tomCalculationItemSource);
        }

        #endregion

        #region Other public methods

        /// <summary>
        /// For option when retain storage mode in composite models.
        /// </summary>
        /// <param name="modeType"></param>
        public void ResetStorageMode(ModeType modeType)
        {
            foreach (Partition partition in _tomTable.Partitions)
            {
                partition.Mode = modeType;
            }
            _tableModeType = modeType;
        }

        /// <summary>
        /// A Boolean specifying whether the table contains a column with the same name searching without case sensitivity.
        /// </summary>
        /// <param name="columnName">The name of the column being searched for.</param>
        /// <returns>True if the object is found, or False if it's not found.</returns>
        public bool ColumnsContainsNameCaseInsensitive(string columnName)
        {
            foreach (Column column in _tomTable.Columns)
            {
                if (column.Name.ToUpper() == columnName.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
        
        public override string ToString() => this.GetType().FullName;
    }
}
