﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace daigou.infrastructure.Events
{
    public class OnOrderCombinePrintClicked
             : CompositePresentationEvent<OnOrderCombinePrintClickedPayLoad>
    {

    }

    public class OnOrderCombinePrintClickedPayLoad
    { 
    
    }
}
