using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Metadata_Translator
{
    public class TranslationPropertyToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter is string toolTipString)
            {
                string[] toolTip = toolTipString.Split('|');
                if (value is ObservableCollection<ExpandoObject> collection && toolTip?.Length == 2)
                {
                    return collection.Count > 0 ? toolTip[0] : toolTip[1];
                }
                else
                    return toolTipString;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}