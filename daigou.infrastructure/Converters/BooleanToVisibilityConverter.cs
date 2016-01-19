using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace daigou.infrastructure.Converters
{
    public class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
       
    }
}
