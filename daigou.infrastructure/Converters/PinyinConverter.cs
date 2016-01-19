using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.infrastructure.Converters
{
    public class PinyinConverter  : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string target = (string)value;
            return target.ToPinyin();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }


    }
}
