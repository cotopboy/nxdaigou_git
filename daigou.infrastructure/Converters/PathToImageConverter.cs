using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Utilities.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;


namespace daigou.infrastructure.Converters
{

    public class PathToImageConverter : IValueConverter
    {
        private static BitmapImage defaultImage = new BitmapImage();

        private static Uri baseUri = new Uri(DirectoryHelper.CurrentExeDirectory);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //若数据库中字段为空,返回默认值
            string  path = value as string;
            if (path.IsNullOrEmpty())
            {
                return PathToImageConverter.defaultImage;
            }
            try
            {
                return new BitmapImage(new Uri(baseUri, value.ToString()));
            }
            catch
            {
                return PathToImageConverter.defaultImage;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            //图片是不能编辑的，这里就没有必要支持反向转换
            throw new NotImplementedException();
        }
    }
}
