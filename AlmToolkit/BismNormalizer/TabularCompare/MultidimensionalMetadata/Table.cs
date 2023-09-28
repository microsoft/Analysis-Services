using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.AnalysisServices;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Abstraction of a tabular model table with properties and methods for comparison purposes.
    /// </summary>
    public class Table : ITabularObject
    {
        #region Private Members

        private TabularModel _parentTabularModel;
        private Dimension _amoDimension;
        private DataTable _amoTable;
        private CubeDimension _amoCubeDimension;
        private MeasureGroup _amoMeasureGroup;
        private RelationshipCollection _relationships = new RelationshipCollection();
        private string _datasourceId;
        private string _objectDefinition;
        private string _substituteId;
        private Dimension _amoDimensionBackup = null;
        private const int _spacing = 31;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Table class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the Table object belongs to.</param>
        /// <param name="dimension">Analysis Management Objects Dimension object abtstracted by the Table class.</param>
        public Table(TabularModel parentTabularModel, Dimension dimension)
        {
            _parentTabularModel = parentTabularModel;
            _amoDimension = dimension;

            PopulateProperties();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// TabularModel object that the Table object belongs to.
        /// </summary>
        public TabularModel TabularModel => _parentTabularModel;

        /// <summary>
        /// Name of the Table object.
        /// </summary>
        public string Name => _amoDimension.Name;

        /// <summary>
        /// Long name of the Table object.
        /// </summary>
        public string LongName => _amoDimension.Name;

        /// <summary>
        /// Id of the Table object.
        /// </summary>
        public string Id => _amoDimension.ID;

        /// <summary>
        /// Data source id of the Table object.
        /// </summary>
        public string DataSourceID => _datasourceId;

        /// <summary>
        /// Object definition of the Table object. This is a simplified list of relevant attribute values for comparison; not the XMLA definition of the abstracted AMO object.
        /// </summary>
        public string ObjectDefinition => _objectDefinition;

        /// <summary>
        /// Collection of relationships for the Table object.
        /// </summary>
        public RelationshipCollection Relationships => _relationships;

        /// <summary>
        /// DataTable object abtstracted by the Table class.
        /// </summary>
        public DataTable AmoTable => _amoTable;

        /// <summary>
        /// Analysis Management Objects CubeDimension object abtstracted by the Table class.
        /// </summary>
        public CubeDimension AmoCubeDimension => _amoCubeDimension;

        /// <summary>
        /// Analysis Management Objects MeasureGroup object abtstracted by the Table class.
        /// </summary>
        public MeasureGroup AmoMeasureGroup => _amoMeasureGroup;

        /// <summary>
        /// Analysis Management Objects Dimension object abtstracted by the Table class.
        /// </summary>
        public Dimension AmoDimension => _amoDimension;

        /// <summary>
        /// Backed up version of the AMO Dimension object. Used for when updating a table.
        /// </summary>
        public Dimension AmoOldDimensionBackup
        {
            get
            {
                return _amoDimensionBackup;
            }
            set
            {
                _amoDimensionBackup = value;
            }
        }

        /// <summary>
        /// Substitute Id of the Table object.
        /// </summary>
        public string SubstituteId
        {
            get
            {
                if (string.IsNullOrEmpty(_substituteId))
                {
                    return _amoDimension.ID;
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

        #endregion

        private void PopulateProperties()
        {
            // find the datasourceid for the Table - and also the datasourceid
            foreach (DataSourceView dsv in _parentTabularModel.AmoDatabase.DataSourceViews)
            {
                _datasourceId = dsv.DataSourceID;
                foreach (DataTable tbl in dsv.Schema.Tables)
                {
                    if ((tbl.TableName == _amoDimension.ID) ||
                         (_amoDimension.Source is DsvTableBinding && tbl.TableName == ((DsvTableBinding)_amoDimension.Source).TableID)
                       )
                    {
                        _amoTable = tbl;

                        //_tableInDsv = tbl;
                        if (tbl.ExtendedProperties["DataSourceID"] != null)
                        {
                            _datasourceId = tbl.ExtendedProperties["DataSourceID"].ToString();
                            break;
                        }
                    }
                }
            }

            foreach (CubeDimension cd in _parentTabularModel.AmoDatabase.Cubes[0].Dimensions)
            {
                if (cd.ID == _amoDimension.ID)
                {
                    _amoCubeDimension = cd;
                    break;
                }
            }

            foreach (MeasureGroup mg in _parentTabularModel.AmoDatabase.Cubes[0].MeasureGroups)
            {
                if (mg.ID == _amoDimension.ID)
                {
                    _amoMeasureGroup = mg;
                    break;
                }
            }

            string baseColumns = "";
            string calculatedColumns = "";
            foreach (DimensionAttribute attribute in _amoDimension.Attributes)
            {
                // ignore key attribute - which is internal built in "RowNumber"
                //if (attribute.ID != _amoDimension.KeyAttribute.ID)
                if (attribute.ID != "__XL_RowNumber")  //if (attribute.ID != "RowNumber") //Before SQL 2016, was just "RowNumber"
                {
                    // if calculated column, show expression
                    if (attribute.NameColumn.Source is ExpressionBinding)
                    {
                        string expression = ((ExpressionBinding)attribute.NameColumn.Source).Expression;
                        calculatedColumns += "[" + attribute.Name + "]:=" + expression + "; " + SetColumnFormatAndVisibility(attribute) + "\n";
                    }
                    else
                    {
                        string baseColumn = "[" + attribute.Name + "]";

                        /*  DATA TYPE MAPPING
                            Text: WChar
                            Whole Number: BigInt
                            Decimal Number: Double
                            True/False: Boolean
                            Currency: Currency
                            Date: Date
                            Binary: Binary
                        */
                        // insert spaces to line up data types nicely
                        if (baseColumn.Length < _spacing)
                        {
                            baseColumn += new String(' ', _spacing - baseColumn.Length);
                        }

                        switch (attribute.KeyColumns[0].DataType)
                        {
                            case System.Data.OleDb.OleDbType.WChar:
                                baseColumn += " Data Type: Text, ";
                                break;
                            case System.Data.OleDb.OleDbType.BigInt:
                            case System.Data.OleDb.OleDbType.Integer:
                            case System.Data.OleDb.OleDbType.SmallInt:
                                baseColumn += " Data Type: Whole Number, ";
                                break;
                            case System.Data.OleDb.OleDbType.Double:
                                baseColumn += " Data Type: Decimal Number, ";
                                break;
                            case System.Data.OleDb.OleDbType.Boolean:
                                baseColumn += " Data Type: True/False, ";
                                break;
                            case System.Data.OleDb.OleDbType.Currency:
                                baseColumn += " Data Type: Currency, ";
                                break;
                            case System.Data.OleDb.OleDbType.Date:
                                baseColumn += " Data Type: Date, ";
                                break;
                            case System.Data.OleDb.OleDbType.Binary:
                                baseColumn += " Data Type: Binary, ";
                                break;
                            default:
                                break;
                        }

                        // Format & visibility
                        baseColumn += SetColumnFormatAndVisibility(attribute);

                        baseColumns += baseColumn + "\n";
                    }
                }
            }

            _objectDefinition += "Base Columns:\n" + baseColumns + "\n";
            _objectDefinition += "Calculated Columns:\n" + calculatedColumns + "\n";

            _objectDefinition += "Hierarchies:\n";
            if (_amoDimension.Hierarchies.Count == 0)
            {
                _objectDefinition += "\n";
            }
            else
            {
                foreach (Hierarchy hierarchy in _amoDimension.Hierarchies)
                {
                    _objectDefinition += "[" + hierarchy.Name + "] ";
                    //if ((_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders || _parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations) && hierarchy.Name.Length + 2 < _spacing)
                    //{
                    //    _objectDefinition += new String(' ', _spacing - hierarchy.Name.Length - 2);
                    //}

                    //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
                    //{
                    //    _objectDefinition += "Display Folder: ";
                    //    if (hierarchy.DisplayFolder != null)
                    //    {
                    //        _objectDefinition += hierarchy.DisplayFolder;
                    //    }
                    //    if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations) _objectDefinition += ", ";
                    //}
                    //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations)
                    //{
                    //    _objectDefinition += "Hierarchy Translations: ";
                    //    if (hierarchy.Translations.Count > 0)
                    //    {
                    //        _objectDefinition += "[";
                    //        foreach (Translation hierarchyTranslation in hierarchy.Translations)
                    //        {
                    //            _objectDefinition += CultureInfo.GetCultureInfo(hierarchyTranslation.Language).DisplayName + ": " + hierarchyTranslation.Caption + ", ";
                    //        }
                    //        _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
                    //    }

                    //    if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
                    //    {
                    //        _objectDefinition += ", Display Folder Translations: ";
                    //        if (hierarchy.Translations.Count > 0)
                    //        {
                    //            _objectDefinition += "[";
                    //            foreach (Translation hierarchyTranslation in hierarchy.Translations)
                    //            {
                    //                _objectDefinition += CultureInfo.GetCultureInfo(hierarchyTranslation.Language).DisplayName + ": " + hierarchyTranslation.DisplayFolder + ", ";
                    //            }
                    //            _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
                    //        }
                    //    }
                    //}
                    
                    _objectDefinition += "\nLevels:\n";
                    foreach (Level level in hierarchy.Levels)
                    {
                        _objectDefinition += "   [" + level.Name + "]";
                        //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations && level.Name.Length + 4 < _spacing)
                        //{
                        //    _objectDefinition += new String(' ', _spacing - level.Name.Length - 4);
                        //}

                        //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations)
                        //{
                        //    _objectDefinition += "Level Translations: ";
                        //    if (level.Translations.Count > 0)
                        //    {
                        //        _objectDefinition += "[";
                        //        foreach (Translation levelTranslation in level.Translations)
                        //        {
                        //            _objectDefinition += CultureInfo.GetCultureInfo(levelTranslation.Language).DisplayName + ": " + levelTranslation.Caption + ", ";
                        //        }
                        //        _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
                        //    }

                        //    if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
                        //    {
                        //        _objectDefinition += ", Display Folder Translations: ";
                        //        if (level.Translations.Count > 0)
                        //        {
                        //            _objectDefinition += "[";
                        //            foreach (Translation levelTranslation in level.Translations)
                        //            {
                        //                _objectDefinition += CultureInfo.GetCultureInfo(levelTranslation.Language).DisplayName + ": " + levelTranslation.DisplayFolder + ", ";
                        //            }
                        //            _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
                        //        }
                        //    }
                        //}
                        _objectDefinition += "\n";
                    }
                    _objectDefinition += "\n";
                }
            }

            if (_amoCubeDimension != null)
            {
                _objectDefinition += "Format & Visibility:\nHidden:" + (!_amoCubeDimension.Visible).ToString();

                //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations)
                //{
                //    _objectDefinition += ", Table Translations: ";
                //    if (_amoCubeDimension.Translations.Count > 0)
                //    {
                //        _objectDefinition += "[";
                //        foreach (Translation tableTranslation in _amoCubeDimension.Translations)
                //        {
                //            _objectDefinition += CultureInfo.GetCultureInfo(tableTranslation.Language).DisplayName + ": " + tableTranslation.Caption + ", ";
                //        }
                //        _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
                //    }

                //    if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
                //    {
                //        _objectDefinition += ", Display Folder Translations: ";
                //        if (_amoCubeDimension.Translations.Count > 0)
                //        {
                //            _objectDefinition += "[";
                //            foreach (Translation tableDisplayFolderTranslation in _amoCubeDimension.Translations)
                //            {
                //                _objectDefinition += CultureInfo.GetCultureInfo(tableDisplayFolderTranslation.Language).DisplayName + ": " + tableDisplayFolderTranslation.DisplayFolder + ", ";
                //            }
                //            _objectDefinition = _objectDefinition.Substring(0, _objectDefinition.Length - 2) + "]";
                //        }
                //    }
                //}
                
                _objectDefinition += "\n";
            }

            if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionPartitions && _amoMeasureGroup != null)
            {
                _objectDefinition += "\nPartitions:\n";

                List<string> partitionNames = new List<string>();  // put in here to sort
                foreach (Partition partition in _amoMeasureGroup.Partitions)
                {
                    partitionNames.Add(partition.Name);
                }
                partitionNames.Sort();

                foreach (string partitionName in partitionNames)
                {
                    foreach (Partition partition in _amoMeasureGroup.Partitions)
                    {
                        if (partition.Name == partitionName)
                        {
                            _objectDefinition += "Name: [" + partition.Name + "]\nSQL:\n" + ((QueryBinding)partition.Source).QueryDefinition + "\n";
                            break;
                        }
                    }
                }
            }

            foreach (Microsoft.AnalysisServices.Relationship relationship in _amoDimension.Relationships)
            {
                _relationships.Add(new Relationship(this, relationship));
            }
        }

        private string SetColumnFormatAndVisibility(DimensionAttribute attribute)
        {
            string columnDef = "";

            if (attribute != null && attribute.Annotations != null && attribute.Annotations.Contains("Format") && attribute.Annotations["Format"].Value.Attributes["Format"] != null)
            {
                switch (attribute.Annotations["Format"].Value.Attributes["Format"].Value)
                {
                    case "General":
                        columnDef += "Data Format: General";
                        break;
                    case "NumberDecimal":
                        columnDef += "Data Format: Decimal Number" +
                                             (attribute.Annotations["Format"].Value.Attributes["Accuracy"] != null ? ", Decimal Places: " + attribute.Annotations["Format"].Value.Attributes["Accuracy"].Value : "") +
                                             (attribute.Annotations["Format"].Value.Attributes["ThousandSeparator"] != null ? ", Show Thousand Separator: " + attribute.Annotations["Format"].Value.Attributes["ThousandSeparator"].Value : "");
                        break;
                    case "NumberWhole":
                        columnDef += "Data Format: Whole Number" +
                                             (attribute.Annotations["Format"].Value.Attributes["ThousandSeparator"] != null ? ", Show Thousand Separator: " + attribute.Annotations["Format"].Value.Attributes["ThousandSeparator"].Value : "");
                        break;
                    case "Percentage":
                        columnDef += "Data Format: Percentage" +
                                             (attribute.Annotations["Format"].Value.Attributes["Accuracy"] != null ? ", Decimal Places: " + attribute.Annotations["Format"].Value.Attributes["Accuracy"].Value : "") +
                                             (attribute.Annotations["Format"].Value.Attributes["ThousandSeparator"] != null ? ", Show Thousand Separator: " + attribute.Annotations["Format"].Value.Attributes["ThousandSeparator"].Value : "");
                        break;
                    case "Scientific":
                        columnDef += "Data Format: Scientific" +
                                             (attribute.Annotations["Format"].Value.Attributes["Accuracy"] != null ? ", Decimal Places: " + attribute.Annotations["Format"].Value.Attributes["Accuracy"].Value : "");
                        break;
                    case "Currency":
                        columnDef += "Data Format: Currency" +
                                             (attribute.Annotations["Format"].Value.Attributes["Accuracy"] != null ? ", Decimal Places: " + attribute.Annotations["Format"].Value.Attributes["Accuracy"].Value : "") +
                                             (attribute.Annotations["Format"].Value.HasChildNodes &&
                                               attribute.Annotations["Format"].Value.ChildNodes[0].Attributes["DisplayName"] != null
                                               ? ", Currency Symbol: " + attribute.Annotations["Format"].Value.ChildNodes[0].Attributes["DisplayName"].Value : "");
                        break;
                    case "DateTimeCustom":
                        columnDef += "Data Format: Date" +
                                             (attribute.Annotations["Format"].Value.HasChildNodes &&
                                               attribute.Annotations["Format"].Value.ChildNodes[0].HasChildNodes &&
                                               attribute.Annotations["Format"].Value.ChildNodes[0].ChildNodes[0].Attributes["FormatString"] != null
                                               ? ", Date Format: " + attribute.Annotations["Format"].Value.ChildNodes[0].ChildNodes[0].Attributes["FormatString"].Value : "");
                        break;
                    case "DateTimeGeneral":
                        columnDef += "Data Format: General";
                        break;
                    case "Text":
                        columnDef += "Data Format: Text";
                        break;
                    case "Boolean":
                        columnDef += "Data Format: TRUE/FALSE";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Sometimes annotations are not populated, so just show the default formats (which are text/general)
                switch (attribute.KeyColumns[0].DataType)
                {
                    case System.Data.OleDb.OleDbType.WChar:
                        columnDef += "Data Format: Text";
                        break;
                    case System.Data.OleDb.OleDbType.BigInt:
                    case System.Data.OleDb.OleDbType.Integer:
                    case System.Data.OleDb.OleDbType.SmallInt:
                    case System.Data.OleDb.OleDbType.Double:
                    case System.Data.OleDb.OleDbType.Currency:
                    case System.Data.OleDb.OleDbType.Date:
                    case System.Data.OleDb.OleDbType.Binary:
                        columnDef += "Data Format: General";
                        break;
                    case System.Data.OleDb.OleDbType.Boolean:
                        columnDef += "Data Format: TRUE/FALSE";
                        break;
                    default:
                        break;
                }
            }

            columnDef += ", Hidden: " + (!attribute.AttributeHierarchyVisible).ToString();

            //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
            //{
            //    columnDef += ", Display Folder: ";
            //    if (attribute.AttributeHierarchyDisplayFolder != null)
            //    {
            //        columnDef += attribute.AttributeHierarchyDisplayFolder;
            //    }
            //}

            //if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTranslations)
            //{
            //    columnDef += ", Column Translations: ";
            //    if (attribute.Translations.Count > 0)
            //    {
            //        columnDef += "[";
            //        foreach (AttributeTranslation attributeTranslation in attribute.Translations)
            //        {
            //            columnDef += CultureInfo.GetCultureInfo(attributeTranslation.Language).DisplayName + ": " + attributeTranslation.Caption + ", ";
            //        }
            //        columnDef = columnDef.Substring(0, columnDef.Length - 2) + "]";
            //    }

            //    if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionDisplayFolders)
            //    {
            //        columnDef += ", Display Folder Translations: ";
            //        if (attribute.Translations.Count > 0)
            //        {
            //            columnDef += "[";
            //            foreach (AttributeTranslation attributeTranslation in attribute.Translations)
            //            {
            //                columnDef += CultureInfo.GetCultureInfo(attributeTranslation.Language).DisplayName + ": " + attributeTranslation.DisplayFolder + ", ";
            //            }
            //            columnDef = columnDef.Substring(0, columnDef.Length - 2) + "]";
            //        }
            //    }
            //}

            return columnDef;
        }

        #region Actions: Relationships

        /// <summary>
        /// Delete relationship associated with the Table object.
        /// </summary>
        /// <param name="relationshipId">The Id of the relationship to be deleted.</param>
        public void DeleteRelationship(string relationshipId)
        {
            if (_amoDimension.Relationships.Contains(relationshipId))
            {
                _amoDimension.Relationships.Remove(relationshipId);
            }

            // shell model
            if (_relationships.ContainsId(relationshipId))
            {
                _relationships.RemoveById(relationshipId);
            }
        }

        /// <summary>
        /// Clear all relationships for the Table object.
        /// </summary>
        public void ClearRelationships()
        {
            _amoDimension.Relationships.Clear();

            // shell model
            _relationships.Clear();
        }

        /// <summary>
        /// Create a relationship for the Table object.
        /// </summary>
        /// <param name="relationshipSource"></param>
        /// <param name="parentDimSource"></param>
        /// <param name="relationshipName"></param>
        /// <param name="warningMessage"></param>
        /// <returns></returns>
        public bool CreateRelationship(Relationship relationshipSource, Dimension parentDimSource, string relationshipName, ref string warningMessage) 
            => this.CreateRelationship(relationshipSource.AmoRelationship, parentDimSource, relationshipName, ref warningMessage, relationshipSource.IsActive);

        /// <summary>
        /// Create a relationship for the Table object.
        /// </summary>
        /// <param name="amoRelationshipSource"></param>
        /// <param name="parentDimSource"></param>
        /// <param name="relationshipName"></param>
        /// <param name="warningMessage"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool CreateRelationship(Microsoft.AnalysisServices.Relationship amoRelationshipSource, Dimension parentDimSource, string relationshipName, ref string warningMessage, bool active)
        {
            // Do the required tables exist?

            // Child table is guaranteed to exist - we are in an instance of it. Just need to ensure the FromRelationshipEnd.DimensionID is correct
            amoRelationshipSource.FromRelationshipEnd.DimensionID = _amoDimension.ID;

            // Parent table need to check ...
            if (_parentTabularModel.Tables.ContainsName(parentDimSource.Name))
            {
                //plug in the substitute id for the parent and move on with my life (I have a life you know)
                amoRelationshipSource.ToRelationshipEnd.DimensionID = _parentTabularModel.Tables.FindByName(parentDimSource.Name).Id;
            }
            else
            {
                warningMessage = "Unable to create Relationship " + relationshipName + " because (considering changes) parent table not found in target model.";
                return false;
            }

            Dimension childDimSource = (Dimension)amoRelationshipSource.Parent;
            Dimension childDimTarget = _amoDimension;
            //Dimension parentDimSource = ... //had to pass in as parameter
            Dimension parentDimTarget = _parentTabularModel.Tables.FindById(amoRelationshipSource.ToRelationshipEnd.DimensionID).AmoDimension;

            // do the required columns exist?
            Microsoft.AnalysisServices.Relationship amoRelationshipSourceTemp = null; // can't modify attribute values while in dim collection, so (might) need a temporary holder

            foreach (RelationshipEndAttribute childDimAttributeSource in amoRelationshipSource.FromRelationshipEnd.Attributes)
            {
                DimensionAttribute childDimAttributeTarget = childDimTarget.Attributes.FindByName(childDimSource.Attributes[childDimAttributeSource.AttributeID].Name);
                if (childDimAttributeTarget == null)
                {
                    warningMessage = "Unable to create Relationship " + relationshipName + " because (considering changes) child column not found in target model.";
                    return false;
                }

                // just in case the attribute ids are not the same between source/target (though obviously the names are the same), then set the correct id (below) in the source relationship and everything will just work
                if (childDimAttributeSource.AttributeID != childDimAttributeTarget.ID)
                {
                    amoRelationshipSourceTemp = amoRelationshipSource.Clone();
                    RelationshipEndAttribute childDimAttributeSourceTemp = childDimAttributeSource.Clone();
                    childDimAttributeSourceTemp.AttributeID = childDimAttributeTarget.ID;
                    amoRelationshipSourceTemp.FromRelationshipEnd.Attributes.Remove(childDimAttributeSource.AttributeID);
                    amoRelationshipSourceTemp.FromRelationshipEnd.Attributes.Add(childDimAttributeSourceTemp);
                }
            }

            // now check parent columns
            foreach (RelationshipEndAttribute parentDimAttributeSource in amoRelationshipSource.ToRelationshipEnd.Attributes)
            {
                if (!parentDimSource.Attributes.Contains(parentDimAttributeSource.AttributeID))
                {
                    warningMessage = "Unable to create Relationship " + relationshipName + " because (considering changes) parent column not found in target model.";
                    return false;
                }

                DimensionAttribute parentDimAttributeTarget = parentDimTarget.Attributes.FindByName(parentDimSource.Attributes[parentDimAttributeSource.AttributeID].Name);
                if (parentDimAttributeTarget == null)
                {
                    warningMessage = "Unable to create Relationship " + relationshipName + " because (considering changes) parent column not found in target model.";
                    return false;
                }

                ////does the parent column allow non-unique values? (if so, won't work as a parent in the relationship)
                //if (!( parentDimTarget.Attributes.Contains("RowNumber") &&
                //       parentDimTarget.Attributes["RowNumber"].AttributeRelationships.Contains(parentDimAttributeTarget.ID) &&
                //       parentDimTarget.Attributes["RowNumber"].AttributeRelationships[parentDimAttributeTarget.ID].Cardinality == Cardinality.One ))
                //{
                //    warningMessage = "Unable to create Relationship " + relationshipName + " because (considering changes) parent column allows non-unique values.";
                //    return false;
                //}

                // just in case the attribute ids are not the same between source/target (though obviously the names are the same), then set the correct id (below) in the source relationship and everything will just work
                if (parentDimAttributeSource.AttributeID != parentDimAttributeTarget.ID)
                {
                    if (amoRelationshipSourceTemp == null)
                    {
                        amoRelationshipSourceTemp = amoRelationshipSource.Clone();
                    }
                    RelationshipEndAttribute parentDimAttributeSourceTemp = parentDimAttributeSource.Clone();
                    parentDimAttributeSourceTemp.AttributeID = parentDimAttributeTarget.ID;
                    amoRelationshipSourceTemp.ToRelationshipEnd.Attributes.Remove(parentDimAttributeSource.AttributeID);
                    amoRelationshipSourceTemp.ToRelationshipEnd.Attributes.Add(parentDimAttributeSourceTemp);
                }
            }

            if (amoRelationshipSourceTemp != null) //i.e. we had to replace at least one attribute id
            {
                childDimSource.Relationships.Remove(amoRelationshipSource.ID);
                childDimSource.Relationships.Add(amoRelationshipSourceTemp);
                amoRelationshipSource = amoRelationshipSourceTemp;
            }

            // is there already a relationship with the same tables/columns?
            bool foundMatch = false;
            foreach (Relationship relationshipTarget in _relationships)
            {
                // has same parent table?
                if (relationshipTarget.AmoRelationship.ToRelationshipEnd.DimensionID == amoRelationshipSource.ToRelationshipEnd.DimensionID)
                {
                    // check columns
                    bool columnsMatch = true;

                    foreach (RelationshipEndAttribute attribute in amoRelationshipSource.FromRelationshipEnd.Attributes)
                    {
                        if (!relationshipTarget.AmoRelationship.FromRelationshipEnd.Attributes.Contains(attribute.AttributeID))
                        {
                            columnsMatch = false;
                        }
                    }
                    foreach (RelationshipEndAttribute attribute in amoRelationshipSource.ToRelationshipEnd.Attributes)
                    {
                        if (!relationshipTarget.AmoRelationship.ToRelationshipEnd.Attributes.Contains(attribute.AttributeID))
                        {
                            columnsMatch = false;
                        }
                    }

                    if (columnsMatch) foundMatch = true;
                }
            }
            if (foundMatch)
            {
                warningMessage = "Unable to create Relationship " + relationshipName + " because (considering changes) relationship already exists in target model."; 
                return false;
            }

            // at this point we know we will add the relationship, but need to check that parent column only allows unique values.  If not, change it.
            foreach (RelationshipEndAttribute parentDimAttributeSource in amoRelationshipSource.ToRelationshipEnd.Attributes)
            {
                DimensionAttribute parentDimAttributeTarget = parentDimTarget.Attributes.FindByName(parentDimSource.Attributes[parentDimAttributeSource.AttributeID].Name);
                //(already checked for existence of parentDimAttributeTarget above)
                if (parentDimTarget.Attributes.Contains("RowNumber") &&
                    parentDimTarget.Attributes["RowNumber"].AttributeRelationships.Contains(parentDimAttributeTarget.ID) &&
                    parentDimTarget.Attributes["RowNumber"].AttributeRelationships[parentDimAttributeTarget.ID].Cardinality != Cardinality.One)
                {
                    parentDimTarget.Attributes["RowNumber"].AttributeRelationships[parentDimAttributeTarget.ID].Cardinality = Cardinality.One;
                    foreach (DataItem di in parentDimAttributeTarget.KeyColumns)
                    {
                        di.NullProcessing = NullProcessing.Error;
                    }
                    if (_parentTabularModel.AmoDatabase.Cubes.Count > 0)
                    {
                        foreach (MeasureGroup mg in _parentTabularModel.AmoDatabase.Cubes[0].MeasureGroups)
                        {
                            if (mg.ID == parentDimTarget.ID)
                            {
                                foreach (MeasureGroupDimension mgd in mg.Dimensions)
                                {
                                    if (mgd.CubeDimensionID == parentDimTarget.ID && mgd is DegenerateMeasureGroupDimension)
                                    {
                                        foreach (MeasureGroupAttribute mga in ((DegenerateMeasureGroupDimension)mgd).Attributes)
                                        {
                                            if (mga.AttributeID == parentDimAttributeTarget.ID)
                                            {
                                                mga.KeyColumns[0].NullProcessing = NullProcessing.Error;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // at this point we know we will add the relationship
            Microsoft.AnalysisServices.Relationship relationshipClone = amoRelationshipSource.Clone();

            // but first check if there is an existing relationship with same id
            if (_parentTabularModel.ContainsRelationship(relationshipClone.ID))
            {
                //Id already exists, but still need to add because different definition - this is due to clever clog users changing table names that were originially in both source and target
                string oldRelationshipId = relationshipClone.ID;
                relationshipClone.ID = Convert.ToString(Guid.NewGuid());
            }

            if (active && !_parentTabularModel.ActiveRelationshipIds.Contains(relationshipClone.ID))
            {
                _parentTabularModel.ActiveRelationshipIds.Add(relationshipClone.ID);
            }
            _amoDimension.Relationships.Add(relationshipClone);
            _relationships.Add(new Relationship(this, relationshipClone, copiedFromSource: true));

            return true;
        }

        #endregion

        public override string ToString() => this.GetType().FullName;
    }
}
