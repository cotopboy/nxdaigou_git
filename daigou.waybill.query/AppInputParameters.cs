using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.waybill.query
{
    public class AppInputParameters
    {
        public static AppInputParameters Current { get; set; }

        public string DHLCode { get; set; }

        public string BpostCode { get; set; }

        public string EMSCode { get; set; }

        public int RecipientID { get; set; }

        public AppInputParameters(string[] args)
        {
            if (args.Length >= 1) RecipientID = int.Parse(args[0]);

            if (args.Length >= 2) DHLCode = args[1];

            if (args.Length >= 3) BpostCode = args[2];

            if (args.Length >= 4) EMSCode = args[3];

            Current = this;
        }
    }
}
