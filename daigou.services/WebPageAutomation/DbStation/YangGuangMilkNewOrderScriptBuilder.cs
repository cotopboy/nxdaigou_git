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
using daigou.infrastructure.ExtensionMethods;


namespace daigou.services.WebPageAutomation.DbStation
{
    public class YangGuangMilkNewOrderScriptBuilder
    {
        private List<string> SpecialCity = new List<string>() 
        {
            "北京","上海","天津","重庆"
        };

        private readonly RecipientService recipentSvc;
        private readonly Product2CodeMapping production2CodeSvc;



        public YangGuangMilkNewOrderScriptBuilder(RecipientService recipentSvc, Product2CodeMapping production2CodeSvc)
        {
            this.recipentSvc = recipentSvc;
            this.production2CodeSvc = production2CodeSvc;
        }

        public string BuildScript(RecipientNewOrderAddedEventPayload payload)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            var recipient = recipentSvc.GetRecipient(payload.RecipientID);          

            dict.Add("#RecipentName#", recipient.Name);
            dict.Add("#Province#", GetProvice(recipient));
            dict.Add("#City#", GetCity(recipient));
            dict.Add("#District#", GetDistrict(recipient));
            dict.Add("#Address#", recipient.CnAddress);
            dict.Add("#PinyinXing#", GetPinyinXin(recipient));
            dict.Add("#PinyinMing#", GetPinyinMing(recipient));
            dict.Add("#Zip#", recipient.PostCode);
            dict.Add("#Tel#", recipient.MainTel);
            dict.Add("#CardId#", recipient.CardId);
            dict.Add("#BarCode#", this.production2CodeSvc.GetCode(payload.OrderInfo));
            dict.Add("#Count#", GetDefaultCount(payload.OrderInfo));
            dict.Add("#OrderNote#", GetOrderNote(recipient,payload));

            string template = Properties.Resources.YangGuangMilkNewOrderScriptTemplate;



            return BuildScript(template, dict) ;

        }

        private string GetOrderNote(Recipient recipient, RecipientNewOrderAddedEventPayload payload)
        {
            return payload.OrderInfo + Environment.NewLine
                 + recipient.AgentName + Environment.NewLine
                 + recipient.QQNumber;
        }

        private string GetDefaultCount(string orderInfo)
        {
            return orderInfo.SubStringAfter("X", 1).Trim();
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

        private string GetPinyinMing(Recipient recipient)
        {
            return recipient.Name.NameToPinyin().SubStringAfter(" ",1);
        }

        private string GetPinyinXin(Recipient recipient)
        {
            return recipient.Name.NameToPinyin().SubStringBefore(" ");
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
                    return cityProvice.EnsureEndsWith("市") + "市辖区";
                }
                else
                {
                    return recipient.ProviceCity.SubStringAfter(",", 1) + "市";
                }
            }
            catch 
            {
                return "";
            }
        }

        private string GetProvice(Recipient recipient)        
        {
            var cityProvice =recipient.ProviceCity;

            bool isSpecialCity = IsSpecialCity(cityProvice);

            if (isSpecialCity)
            {
                return cityProvice.EnsureEndsWith("市");
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

