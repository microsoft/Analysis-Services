using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metadata_Translator
{
    public static class ExpandoObjectExtensions
    {
        public static string SeparateCamelCase(this ExpandoObject expando, string columnName)
        {
            if (expando != null && ((IDictionary<String, Object>)expando)[columnName] is string text)
            {
                char separator = ' ';
                char lastChar = separator;

                var sb = new StringBuilder();
                foreach (var currentChar in text.Replace("_", ""))
                {
                    if (char.IsUpper(currentChar) && lastChar != separator)
                        sb.Append(separator);

                    sb.Append(currentChar);

                    lastChar = currentChar;
                }

                return sb.ToString();
            }
            return string.Empty;
        }

        public static string GetValue(this ExpandoObject expando, string columnName)
        {
            return expando.GetObject(columnName)?.ToString();
        }

        public static object GetObject(this ExpandoObject expando, string columnName)
        {
            if (expando != null && ((IDictionary<String, Object>)expando).ContainsKey(columnName))
            {
                return ((IDictionary<String, Object>)expando)[columnName];
            }
            return null;
        }

        public static void SetValue(this ExpandoObject expando, string columnName, string value, bool overwrite)
        {
            if (expando != null)
            {
                if (overwrite || string.IsNullOrEmpty(expando.GetValue(columnName)))
                {
                    ((IDictionary<String, Object>)expando)[columnName] = value;
                }
            }
        }
    }
}
