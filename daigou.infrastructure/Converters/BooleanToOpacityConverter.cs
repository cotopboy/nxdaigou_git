using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.infrastructure.Converters
{

    public class BooleanToOpacityConverter : BooleanConverter<float>
    {
        public BooleanToOpacityConverter() :
            base(1f, 0.5f)
        { }

    }
}
