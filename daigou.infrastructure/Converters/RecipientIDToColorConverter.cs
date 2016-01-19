using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace daigou.infrastructure.Converters
{
    public class RecipientIDToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int target = (int)value;
            if (target == -1) return Brushes.OrangeRed;
            else return Brushes.DarkBlue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class AddressSeperatorCounterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string target = (string)value;
            int count =  target.Count((char c) => { return c == '`'; });
            return string.Format("中文地址   [xx省市 ` xx区 ` xx路 ` xx室]   [{0}/3]", count);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
