using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.infrastructure.Events;
using daigou.domain;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;

namespace daigou.services.WebPageAutomation.Dt8ang
{
    public class EMSNewOrderScriptBuilder
    {
        private List<string> SpecialCity = new List<string>() 
        {
            "北京","上海","天津","重庆"
        };

        private readonly RecipientService recipentSvc;

        public EMSNewOrderScriptBuilder(RecipientService recipentSvc)
        {
            this.recipentSvc = recipentSvc;
        }

        public string BuildScript(RecipientNewOrderAddedEventPayload payload)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            var recipient = recipentSvc.GetRecipient(payload.RecipientID);

            dict.Add("#RecipentName#", recipient.Name);
            dict.Add("#Province#", GetProvice(recipient));
            dict.Add("#City#", GetCity(recipient));
            dict.Add("#Address#", GetAddress(recipient));
            dict.Add("#Zip#", recipient.PostCode);
            dict.Add("#Tel#", recipient.MainTel);
            dict.Add("#Weight#", "7");

            string template = Properties.Resources.EMSNewOrderScriptTemplate;

            return BuildScript(template, dict);
        }

        private  string GetAddress(Recipient recipient)
        {
            return recipient.CnAddress.SubStringAfter("市", 1);
        }

        private string BuildScript(string template, Dictionary<string, string> dict)
        {
            StringBuilder builder = new StringBuilder(template);

            foreach (var item in dict)
            {
                builder.Replace(item.Key, item.Value);
            }

            return builder.ToString();
        }

        private string GetDistrict(Recipient recipient)
        {
            try
            {
                string sub = recipient.CnAddress.SubStringBefore("区");

                return sub.SubStringAfter("市", 1) + "区";
            }
            catch
            {
                return "";
            }

        }

        private string GetCity(Recipient recipient)
        {
            try
            {
                var cityProvice = recipient.ProviceCity;

                bool isSpecialCity = IsSpecialCity(cityProvice);

                if (isSpecialCity)
                {
                    return cityProvice.Replace("市", "");
                }
                else
                {
                    return recipient.ProviceCity.SubStringAfter(",", 1);
                }
            }
            catch             
            {
                return "";
            }
        }

        private string GetProvice(Recipient recipient)
        {
            var cityProvice = recipient.ProviceCity;

            bool isSpecialCity = IsSpecialCity(cityProvice);

            if (isSpecialCity)
            {
                return cityProvice + "市";
            }
            else
            {
                return recipient.ProviceCity.SubStringBefore(",") + "省";
            }

        }

        private bool IsSpecialCity(string cityProvice)
        {
            foreach (var item in SpecialCity)
            {
                if (cityProvice.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
