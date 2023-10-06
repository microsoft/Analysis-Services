using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AnalysisServices.Tabular;
using Tom=Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents a tabular object for comparison. This class handles JSON serialization.
    /// </summary>
    public class TabularObject
    {
        private string _objectDefinition;
        private string _name;
        private TabularModel _parentTabularModel;

        /// <summary>
        /// Initializes a new instance of the TabularObject class.
        /// </summary>
        /// <param name="namedMetaDataObject">The Tabular Object Model supertype of the class being abstracted.</param>
        public TabularObject(NamedMetadataObject namedMetaDataObject, TabularModel parentTabularModel)
        {
            _name = namedMetaDataObject.Name;
            _parentTabularModel = parentTabularModel;

            if (namedMetaDataObject is Tom.Model) //Model has custom JSON string
            {
                Tom.Model model = (Tom.Model)namedMetaDataObject;

                if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
                {
                    string customObjectDefinition = "{ ";
                    if (!string.IsNullOrEmpty(model.Description))
                    {
                        customObjectDefinition += $"\"description\": \"{model.Description}\", ";
                    }
                    customObjectDefinition += $"\"defaultMode\": \"{model.DefaultMode.ToString().ToLower()}\", ";
                    customObjectDefinition += $"\"discourageImplicitMeasures\": {model.DiscourageImplicitMeasures.ToString().ToLower()} }}";
                    _objectDefinition = JToken.Parse(customObjectDefinition).ToString();
                }
                else if(model.DiscourageImplicitMeasures)
                {
                    _objectDefinition = "\tdiscourageImplicitMeasures";
                }

                return;
            }

            //Serialize json
            SerializeOptions options = new SerializeOptions();
            options.IgnoreInferredProperties = true;
            options.IgnoreInferredObjects = true;
            options.IgnoreTimestamps = true;
            options.SplitMultilineStrings = true;

            //Use TMSL or TMDL
            if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
            {
                _objectDefinition = Tom.JsonSerializer.SerializeObject(namedMetaDataObject, options, parentTabularModel.ConnectionInfo.CompatibilityLevel, parentTabularModel.ConnectionInfo.CompatibilityMode);
            }
            else
            {
                _objectDefinition = Tom.TmdlSerializer.SerializeObject(namedMetaDataObject, qualifyObject:false);
            }

            //Remove annotations if required
            if (!_parentTabularModel.ComparisonInfo.OptionsInfo.OptionAnnotations)
            {
                if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
                { 
                    JToken token = JToken.Parse(_objectDefinition);
                    RemovePropertyFromObjectDefinition(token, "annotations");
                    _objectDefinition = token.ToString(Formatting.Indented);
                }
                else
                {
                    RemoveTmdlLines("annotation ");
                }
            }

            //Remove lineageTag if required
            if (!_parentTabularModel.ComparisonInfo.OptionsInfo.OptionLineageTag)
            {
                if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
                {
                    JToken token = JToken.Parse(_objectDefinition);
                    RemovePropertyFromObjectDefinition(token, "lineageTag");
                    _objectDefinition = token.ToString(Formatting.Indented);
                }
                else
                {
                    RemoveTmdlLines("lineageTag: ");
                }
            }

            //Ordering of table columns/role members and hide privacy settings on structured data sources
            if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
            {
                //Order table columns
                if (namedMetaDataObject is Tom.Table)
                {
                    if (((Tom.Table)namedMetaDataObject).CalculationGroup != null)
                    {
                        JToken token = JToken.Parse(_objectDefinition);
                        RemovePropertyFromObjectDefinition(token, "calculationItems");
                        _objectDefinition = token.ToString(Formatting.Indented);
                    }

                    _objectDefinition = SortArray(_objectDefinition, "columns");
                    _objectDefinition = SortArray(_objectDefinition, "partitions");
                }

                //Order role members
                if (namedMetaDataObject is Tom.ModelRole)
                {
                    _objectDefinition = SortArray(_objectDefinition, "members");
                }

                //Hide privacy setting on structured data sources
                if (namedMetaDataObject is Tom.StructuredDataSource)
                {
                    JToken token = JToken.Parse(_objectDefinition);
                    RemovePropertyFromObjectDefinition(token, "PrivacySetting");
                    _objectDefinition = token.ToString(Formatting.Indented);
                }
            }
            else
            {
                //TODOTMDL: TMDL version of ordering of table columns/role members
            }
        }

        #region Private methods

        private void RemoveTmdlLines(string lineStartStringToRemove)
        {
            StringBuilder newObjectDefinitionSB = new StringBuilder();
            using (StringReader lines = new StringReader(_objectDefinition))
            {
                string line = string.Empty;
                bool previousLineBlank = false; //let's avoid more than 1 blank line in a sequence
                bool removalInProgress = false;
                int matchLineWhitespaceCharCount = 0; //Whitespace character count for the first line that matched the start string to remove

                while ((line = lines.ReadLine()) != null)
                {
                    if (!(String.IsNullOrWhiteSpace(line) && previousLineBlank))
                    {
                        if (!removalInProgress)
                        {
                            ReadTmdlLine(lineStartStringToRemove, newObjectDefinitionSB, line, ref removalInProgress, ref matchLineWhitespaceCharCount, ref previousLineBlank);
                        }
                        else
                        {
                            //Object removal is in progress so need to check if it's a multi-line object declaration
                            if (!String.IsNullOrWhiteSpace(line) && GetWhitespaceCharacterCount(line) <= matchLineWhitespaceCharCount) //Is the current line for a new object based on indentation?
                            {
                                //Current line is not part of a multi-line declaration based on indentation, so exit from removing state and continue reading as normal
                                removalInProgress = false;
                                ReadTmdlLine(lineStartStringToRemove, newObjectDefinitionSB, line, ref removalInProgress, ref matchLineWhitespaceCharCount, ref previousLineBlank);
                            }
                            else if (String.IsNullOrWhiteSpace(line))
                            {
                                //Just insert the blank line as the last line wasn't blank so won't be introducing more than 1 on a sequence
                                ReadTmdlLine(lineStartStringToRemove, newObjectDefinitionSB, line, ref removalInProgress, ref matchLineWhitespaceCharCount, ref previousLineBlank);
                            }
                            //else: do nothing and thereby "remove" the current line
                        }
                    }
                }
            }
            _objectDefinition = newObjectDefinitionSB.ToString();
        }

        private void ReadTmdlLine(string lineStartToRemove, StringBuilder newObjectDefinition, string line, ref bool removalInProgress, ref int matchLineWhitespaceCharCount, ref bool previousLineBlank)
        {
            if (!line.Trim().StartsWith(lineStartToRemove))
            {
                newObjectDefinition.AppendLine(line);
                previousLineBlank = String.IsNullOrWhiteSpace(line);
            }
            else
            {
                //Found first line that matched the start string to remove
                matchLineWhitespaceCharCount = GetWhitespaceCharacterCount(line);
                removalInProgress = true;
            }
        }

        private int GetWhitespaceCharacterCount(string line)
        {
            int whitespaceCharacterCount;
            const int TabLength = 4;
            int positionOfFirstChar = line.IndexOf(line.Trim());
            string leadingWhitespace = line.Substring(0, positionOfFirstChar);
            whitespaceCharacterCount = leadingWhitespace.Length;
            WhitespaceType whitespaceType;

            if (leadingWhitespace == null) whitespaceType = WhitespaceType.Invalid;
            else if (leadingWhitespace.Length == 0) whitespaceType = WhitespaceType.None;
            else if (leadingWhitespace.All(c => c == ' ')) whitespaceType = WhitespaceType.Spaces;
            else if (leadingWhitespace.All(c => c == '\t')) whitespaceType = WhitespaceType.Tabs;
            else if (leadingWhitespace.All(c => c == '\t' || c == ' ')) whitespaceType = WhitespaceType.Mixed;
            else whitespaceType = WhitespaceType.Invalid;

            switch (whitespaceType)
            {
                case WhitespaceType.Spaces:
                case WhitespaceType.Tabs:
                case WhitespaceType.Mixed:
                    whitespaceCharacterCount =
                        leadingWhitespace.Count(c => c == ' ')
                        + leadingWhitespace.Count(c => c == '\t') * TabLength;
                    break;
            }

            return whitespaceCharacterCount;
        }
        private enum WhitespaceType { Invalid, None, Spaces, Tabs, Mixed };

        private string SortArray(string json, string arrayName)
        {
            JObject jObj = (JObject)JsonConvert.DeserializeObject(json);

            foreach (var prop in jObj.Properties())
            {
                if (prop.Value.Type == JTokenType.Array && prop.Name == arrayName)
                {
                    var vals = prop.Values()
                        .OfType<JObject>()
                        .OrderBy(x => x.Property((arrayName == "members" ? "memberName" : "name")).Value.ToString())
                        .ToList();
                    prop.Value = JContainer.FromObject(vals);
                }
            }

            return jObj.ToString(Formatting.Indented);
        }

        private void RemovePropertyFromObjectDefinition(JToken token, string propertyName)
        {
            //child object annotations
            List<JToken> removeList = new List<JToken>();
            foreach (JToken childToken in token.Children())
            {
                JProperty property = childToken as JProperty;
                if (property != null && property.Name == propertyName)
                {
                    removeList.Add(childToken);
                }
                RemovePropertyFromObjectDefinition(childToken, propertyName);
            }
            foreach (JToken tokenToRemove in removeList)
            {
                tokenToRemove.Remove();
            }
        }

        #endregion

        /// <summary>
        /// Explicitly remove partitions from JSON/TMDL definition.
        /// </summary>
        public void RemovePartitionsFromObjectDefinition()
        {
            if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
            {
                JObject jObj = JObject.Parse(_objectDefinition);
                jObj.Remove("partitions");
                _objectDefinition = jObj.ToString(Formatting.Indented);
            }
            else
            {
                RemoveTmdlLines("partition ");
            }
        }

        /// <summary>
        /// Explicitly remove measures from JSON/TMDL definition.
        /// </summary>
        public void RemoveMeasuresFromObjectDefinition()
        {
            if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
            {
                JObject jObj = JObject.Parse(_objectDefinition);
                jObj.Remove("measures");
                _objectDefinition = jObj.ToString(Formatting.Indented);
            }
            else
            {
                RemoveTmdlLines("measure ");
            }
        }

        /// <summary>
        /// Explicitly remove partitions from JSON/TMDL definition.
        /// </summary>
        public void RemoveCalcItemsFromTmdlObjectDefinition()
        {
            if (!_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
            {
                RemoveTmdlLines("calculationItem ");
            }
        }

        /// <summary>
        /// Set a custom JSON/TMDL string. An example is for the model class which contains properties that cannot be set.
        /// </summary>
        public void SetCustomObjectDefinition(string customObjectDefinition)
        {
            if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
            {
                _objectDefinition = JToken.Parse(customObjectDefinition).ToString();
            }
            else
            {
                _objectDefinition = customObjectDefinition;
            }
        }

        /// <summary>
        /// Retrieve a JSON/TMDL definition from the partitions.
        /// </summary>
        /// <param name="propertyToRetrieve"></param>
        /// <returns>Property definition retrieved.</returns>
        public string RetrievePartitionsFromObjectDefinition()
        {
            if (_parentTabularModel.ComparisonInfo.OptionsInfo.OptionTmsl)
            {
                JObject jObj = JObject.Parse(_objectDefinition);
                JProperty property = jObj.Property("partitions");
                return property.ToString();
            }
            else
            {
                StringBuilder returnObjectDefinitionSB = new StringBuilder();
                using (StringReader lines = new StringReader(_objectDefinition))
                {
                    string line = string.Empty;
                    bool readingInProgress = false;
                    int matchLineWhitespaceCharCount = 0; //Whitespace character count for the first line that matched the start string to remove

                    while ((line = lines.ReadLine()) != null)
                    {
                        if (!(String.IsNullOrWhiteSpace(line)))
                        {
                            if (line.Trim().StartsWith("partition "))
                            {
                                //Found first line that matched the start string to retain
                                returnObjectDefinitionSB.AppendLine(line);
                                matchLineWhitespaceCharCount = GetWhitespaceCharacterCount(line);
                                readingInProgress = true;
                            }
                            else if (readingInProgress && (matchLineWhitespaceCharCount < GetWhitespaceCharacterCount(line)))
                            {
                                //Continuation of partition definition to retrieve becase indentation is greater
                                returnObjectDefinitionSB.AppendLine(line);
                            }
                            else if (readingInProgress)
                            {
                                readingInProgress = false;
                            }
                        }
                    }
                }
                return returnObjectDefinitionSB.ToString();
            }
        }

        /// <summary>
        /// The serialized JSON definition of the tabular object.
        /// </summary>
        public string ObjectDefinition => _objectDefinition;

        /// <summary>
        /// The name of the tabular object. Gets overriden by Relationship to show friendly name.
        /// </summary>
        public virtual string Name => _name;

        /// <summary>
        /// The internal name of the tabular object. Gets overriden by Relationship to store the true name from TOM (GUID form).
        /// </summary>
        public virtual string InternalName => _name; 

    }
}

