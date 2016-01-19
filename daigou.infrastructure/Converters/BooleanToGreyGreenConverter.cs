using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace daigou.infrastructure.Converters
{
    public class BooleanToGreyGreenConverter : BooleanConverter<SolidColorBrush>
    {
        public BooleanToGreyGreenConverter() :
            base(Brushes.GreenYellow,Brushes.LightGray )
        { }
       
    }

    public class BooleanToBlueGreyConverter : BooleanConverter<SolidColorBrush>
    {
        public BooleanToBlueGreyConverter() :
            base(Brushes.LightBlue, Brushes.LightGray)
        { }

    }
}
