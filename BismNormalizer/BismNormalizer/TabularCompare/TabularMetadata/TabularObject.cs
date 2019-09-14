using System;
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

        /// <summary>
        /// Initializes a new instance of the TabularObject class.
        /// </summary>
        /// <param name="namedMetaDataObject">The Tabular Object Model supertype of the class being abstracted.</param>
        public TabularObject(NamedMetadataObject namedMetaDataObject)
        {
            _name = namedMetaDataObject.Name;
            if (namedMetaDataObject is Tom.Model) return; //Model has custom JSON string
            
            //Serialize json
            SerializeOptions options = new SerializeOptions();
            options.IgnoreInferredProperties = true;
            options.IgnoreInferredObjects = true;
            options.IgnoreTimestamps = true;
            options.SplitMultilineStrings = true;
            _objectDefinition = Tom.JsonSerializer.SerializeObject(namedMetaDataObject, options);

            //Remove annotations
            {
                JToken token = JToken.Parse(_objectDefinition);
                RemovePropertyFromObjectDefinition(token, "annotations");
                _objectDefinition = token.ToString(Formatting.Indented);
            }

            //todo: remove with Giri's fix
            //Remove return characters
            if (namedMetaDataObject is Tom.NamedExpression || namedMetaDataObject is Tom.Table)
            {
                _objectDefinition = _objectDefinition.Replace("\\r", "");
            }

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

        /// <summary>
        /// Explicitly remove a JSON property from definition. An example of this is removing parititions from table definitions.
        /// </summary>
        /// <param name="propertyName">The property to remove</param>
        public void RemovePropertyFromObjectDefinition(string propertyName)
        {
            JObject jObj = JObject.Parse(_objectDefinition);
            jObj.Remove(propertyName);
            _objectDefinition = jObj.ToString(Formatting.Indented);
        }

        /// <summary>
        /// Set a custom JSON string. An example is for the model class which contains properties that cannot be set.
        /// </summary>
        public void SetCustomObjectDefinition(string customObjectDefinition)
        {
            _objectDefinition = JToken.Parse(customObjectDefinition).ToString();
        }

        /// <summary>
        /// Retrieve a JSON property definition from the full object definition. An example is partitions.
        /// </summary>
        /// <param name="propertyToRetrieve"></param>
        /// <returns>Property definition retrieved.</returns>
        public string RetrievePropertyFromObjectDefinition(string propertyToRetrieve)
        {
            JObject jObj = JObject.Parse(_objectDefinition);
            JProperty property = jObj.Property(propertyToRetrieve);
            return property.ToString();
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

