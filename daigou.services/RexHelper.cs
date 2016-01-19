using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using System.Windows.Forms;
using System.Drawing;

namespace daigou.services
{
    public class RexHelper
    {
        public static bool IsTelephone(string str_telephone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_telephone, @"^(\d{3,4}-)?\d{7,8}$");
        }

        public static bool IsHandset(string str_handset)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^[1]+[3,4,5,8]+\d{9}");
        }

        public static bool IsIDcard(string str_idcard)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_idcard, @"(^\d{18}$)|(^\d{15}$)");
        }

        public static bool IsNumber(string str_number)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_number, @"^[0-9]*$");
        }

        public static bool IsPostalcode(string str_postalcode)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_postalcode, @"^\d{6}$");
        }

        public static string ExtractZdBpostWaybillSn(string input)
        {
            Match match = Regex.Match(input, @"\s\d{26}\s");

            if (match.Success)
            {
               return match.Value.Trim();
            }

            return string.Empty;
        }

        public static string ExtractDHLWaybillSn(string input)
        {
            Match match = Regex.Match(input, @"\s\d{12}\s");

            if (match.Success)
            {
                return match.Value.Trim();
            }

            match = Regex.Match(input, @"\s\d{20}\s");

            if (match.Success)
            {
                return match.Value.Trim() ;
            }

            return string.Empty;
        }

       
    }
}
