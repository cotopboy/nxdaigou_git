using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.domain.Base;
using System.Text.RegularExpressions;

namespace daigou.services
{
    public class CnRecipientParser
    {
        private string input = string.Empty;

        private CnRecipientInfo recipient;

        private List<CnRecipientItem> itemList;

        public CnRecipientParser()
        {
            
        }

        public CnRecipientInfo Parse(string CN_Input)
        {
            recipient = new CnRecipientInfo();

            this.input = CN_Input;

            this.PreProcess();

            this.itemList = BreakInputIntoItemList(input);

            this.recipient.Name = this.FindName();

            this.recipient.PostCode = this.FindPostCode();

            this.recipient.TelList = this.FindTelList();

            this.recipient.Address = this.FindAddress();

            this.recipient.ProviceCity = this.FindProviceCity();


            return recipient;

        }

        private string FindProviceCity()
        {
            string valueRaw =  this.input.Substring(this.input.IndexOf(" "), this.input.IndexOf("市", 0) + 1 - this.input.IndexOf(" "))
                             .Replace("省", ",")
                             .Replace("市", "")
                             .Replace(" ","")
                             .Replace("　","");

            Regex rgx = new Regex("\\d+");
            string replacement = "";
            valueRaw = rgx.Replace(valueRaw, replacement);

            return valueRaw;

        }

        private void PreProcess()
        {
            this.input = this.input.Replace("邮编", "")
                                   .Replace("电话", "")
                                   .Replace("手机", "")
                                   .Replace("姓名", "")
                                   .Replace(":","")
                                   .Replace("：","")
                                   .Replace("广西壮族自治区", "广西省") ;
        }

        private string FindName()
        {

            var item = this.itemList[0] ;
           
            if (item.Value.Length > 1 && item.Value.Length <4)
            {
                item.IsProcessed = true;
                return item.Value;
            }
            
            return ""; 
        }

        private string FindAddress()
        {
            string[] addr_key = new string[] { "市", "区", "省", "路", "镇", "号", "路", "幢", "村",""};
            List<string> addrItemList = new List<string>();

            foreach (var item in this.itemList)
            {
                if (item.IsProcessed) continue;

                addrItemList.Add(item.Value);
                item.IsProcessed = true;

                
            }
            return string.Join("", addrItemList);
        }

        private List<string> FindTelList()
        {
            List<string> tel_list = new List<string>();

            foreach (var item in this.itemList)
            {
                if (item.IsProcessed) continue;

                if (
                        RexHelper.IsTelephone(item.Value) ||
                        RexHelper.IsHandset(item.Value)
                   )
                {
                    item.IsProcessed = true;
                    tel_list.Add(item.Value);
                }
            }

            return tel_list;
        }

       

        private string FindPostCode()
        {
            foreach (var item in this.itemList)
            {
                if (item.IsProcessed) continue;

                if(RexHelper.IsPostalcode(item.Value))
                {
                    item.IsProcessed = true;
                    return item.Value;
                }
            }
            return "";
        }

        private List<CnRecipientItem> BreakInputIntoItemList(string input)
        {
            return input.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries).
                        Select(x => new CnRecipientItem(x)).ToList(); 
        }

    }

   
}
