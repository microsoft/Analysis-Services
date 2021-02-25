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
    public static class StringExtensions
    {
        public static string ToCsvString(this string value)
        {
            if (value == null) return string.Empty;
            return value.Contains("\"") ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
        }
    }
}
