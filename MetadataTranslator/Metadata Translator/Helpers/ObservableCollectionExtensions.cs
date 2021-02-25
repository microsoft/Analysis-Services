using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metadata_Translator
{
    public static class ObservableCollectionExtensions
    {
        public static string GetValueAt(this ObservableCollection<ExpandoObject> collection, int index, string columnName)
        {
            if (collection == null) return string.Empty;

            ExpandoObject row = collection[index];
            return ((IDictionary<String, Object>)row)[columnName]?.ToString();
        }

        public static void SetValueAt(this ObservableCollection<ExpandoObject> collection, int index, string columnName, string value)
        {
            if (collection == null) return;

            ExpandoObject row = collection[index];
            ((IDictionary<String, Object>)row)[columnName] = value;
        }

        public static void UpdateDataValues(this ObservableCollection<ExpandoObject> collection, List<string> sourcePhrases, string sourceLanguage, List<string> targetPhrases, string targetLanguage)
        {
            if (collection == null) return;
            int rowOffset = 0;
            for (int i = 0; i < collection.Count; i++)
            {
                string columnValue = collection.GetValueAt(i, sourceLanguage);
                if (!string.IsNullOrEmpty(columnValue) && columnValue.Equals(sourcePhrases[i - rowOffset]))
                {
                    collection.SetValueAt(i, targetLanguage, targetPhrases[i - rowOffset]);
                }
                else
                {
                    rowOffset++;
                }
            }
        }

        public static List<string> GetValues(this ObservableCollection<ExpandoObject> collection, string columnName)
        {
            if (collection == null) return new List<string>();

            List<string> values = new List<string>();
            foreach (ExpandoObject row in collection)
            {
                string value = ((IDictionary<String, Object>)row)[columnName]?.ToString();

                if (!string.IsNullOrEmpty(value))
                    values.Add(value);
            }
            return values;
        }

        public static void AddDisplayFolder(this ObservableCollection<ExpandoObject> collection, NamedMetadataObject metadataObject, string displayString, string defaultCulture, CultureCollection cultures)
        {
            if (collection == null) return;

            if (!string.IsNullOrEmpty(displayString))
            {
                foreach (ExpandoObject item in collection)
                {
                    if (((IDictionary<String, Object>)item)[defaultCulture] is string displayName && displayName.Equals(displayString))
                    {
                        var existingDisplayFolderContainer = ((IDictionary<String, Object>)item)["Object"] as DisplayFolderContainer;
                        existingDisplayFolderContainer.TabularObjects.Add(metadataObject);
                        return;
                    }
                }
            }

            dynamic row = new ExpandoObject();
            var displayFolderContainer = new DisplayFolderContainer(metadataObject, TranslatedProperty.DisplayFolder);

            ((IDictionary<String, Object>)row)["Object"] = displayFolderContainer;
            foreach (var culture in cultures)
            {
                ((IDictionary<String, Object>)row)[culture.Name] = culture.Name.Equals(defaultCulture) ? displayString :
                    culture.ObjectTranslations[displayFolderContainer.TabularObject, displayFolderContainer.TranslatedProperty]?.Value;
            }

            collection.Add(row);
        }

    }
}
