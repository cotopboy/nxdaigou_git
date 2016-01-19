using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace daigou.services
{
    public class EMSLocalBarcodeService
    {
        public string GetEMSLocalBarcode(string bpostCode)
        {
            string bpostTempalte = "http://www.bpost2.be/bpi/track_trace/find.php?search=s&lng=en&trackcode={0}";

            WebClient client = new WebClient();

            string downloadString = client.DownloadString(string.Format(bpostTempalte,bpostCode));

            int index = downloadString.IndexOf("<relabelBarcode>");

            if (index != -1)
                return downloadString.Substring(index + 16, 13);
            else return string.Empty;
           
        }
    }
}
