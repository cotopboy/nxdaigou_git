using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace daigou.infrastructure.Converters
{
    
    public class DateTimeToStringConverter : IValueConverter
    {
        private const string NoDate = "NO_TIME_DEFINE";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                var date = (DateTime)value;
                if (date == DateTime.MinValue)
                    return NoDate;
                return date.ToString("yyyy-MM-dd HH:mm");
            }
            return NoDate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
