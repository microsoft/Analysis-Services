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
    public static class ListExtensions
    {
        public static List<CsvRow> GetValues(this List<ExpandoObject> collection, string containerColumnName, string referenceColumnName, string columnName)
        {
            if (collection == null) return new List<CsvRow>();

            var values = new List<CsvRow>();
            foreach (ExpandoObject row in collection)
            {
                var metaContainer = (MetadataObjectContainer)row.GetObject(containerColumnName);
                string refValue = row.GetValue(referenceColumnName);

                if (!string.IsNullOrEmpty(refValue))
                    values.Add(new CsvRow { Type = metaContainer.TranslatedProperty.ToString(), Original = refValue, Translation = row.GetValue(columnName) });
            }
            return values;
        }

        public static Dictionary<Guid, string> GetValues(this List<ExpandoObject> collection, string containerColumnName, string columnName)
        {
            if (collection == null) return new Dictionary<Guid, string>();

            var values = new Dictionary<Guid, string>();
            foreach (ExpandoObject row in collection)
            {
                var metaContainer = (MetadataObjectContainer)row.GetObject(containerColumnName);
                var columnValue = row.GetValue(columnName);

                if (metaContainer != null && !string.IsNullOrEmpty(columnValue))
                {
                    try
                    {
                        values.Add(metaContainer.TemporaryObjectId, columnValue);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message + " --- " + metaContainer.TemporaryObjectId.ToString() + metaContainer.ToString());
                    }
                }
            }
            return values;
        }
    }
}
