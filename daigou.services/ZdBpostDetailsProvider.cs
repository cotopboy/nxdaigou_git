using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.IO;
using System.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;

namespace daigou.services
{
    public class ZdBpostDetailsItem
    {
        public string Content { get; set; }
        public string Quantity { get; set; }
        public string NetWeight { get; set; }
        public string Value { get; set; }

        public float FloatNetWeight
        {
            get { return GetFloatValue(NetWeight); }
        }

        public float IntQuantity
        {
            get { return GetFloatValue(Quantity); }
        }

        public float IntValue
        {
            get { return GetFloatValue(Value); }
        }

        public float IntTotalValue
        {
            get { return (float)Math.Round(IntQuantity * IntValue,2); }
        }


        public float GetFloatValue(string input)
        {
            return float.Parse(input);
        }

    }

    public class ZdBpostDetails
    {
        private string recipientName;

        public string RecipientName
        {
            get { return recipientName; }
            set { recipientName = value; }
        }

        private List<ZdBpostDetailsItem> items = new List<ZdBpostDetailsItem> ();

        public List<ZdBpostDetailsItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        public float TotalValue
        {
            get { return items.Sum(x => x.IntTotalValue); }
            
        }

        internal void AcceptNewItem(string input)
        {
            input = input.Trim();
            if (input.Length == 0) return;

            var array = input.Split(new string[] {" "}, StringSplitOptions.None);

            ZdBpostDetailsItem item = new ZdBpostDetailsItem();
            item.Content = array[0] ;
            item.Quantity = array[1] ;
            item.NetWeight = array[2] ;
            item.Value = array[3];

            this.items.Add(item);
        }
    }

    public class ZdBpostDetailsProvider
    {
        private List<ZdBpostDetails> detailsList;

        public List<ZdBpostDetails> DetailsList
        {
            get
            {
                if (detailsList == null) InitializeDetailsList();
                return detailsList; 
            }
        }

        private DirectoryService directoryService;

        public ZdBpostDetailsProvider(DirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }

        public ZdBpostDetails GetZdBpostDetails(string recipientName)
        {
            var ret = this.DetailsList.FirstOrDefault(x => x.RecipientName == recipientName);

            return ret;
        }

        private void InitializeDetailsList()
        {
            this.detailsList = new List<ZdBpostDetails>();
            string detailsFileContent = new FileInfo(this.directoryService.DetailsLogFilePath).Read();

            var lines = detailsFileContent.ToLines();

            bool isNew = true;
            ZdBpostDetails details = null;
            foreach (var item in lines)
            {
                if (isNew)
                {
                    if (item.Trim().IsNullOrEmpty()) continue;

                    details = new ZdBpostDetails();
                    details.RecipientName = item;
                    this.detailsList.Add(details);
                    isNew = false;
                }
                else
                {
                    details.AcceptNewItem(item);
                }

                if (item.Trim().Length == 0) isNew = true;
            }

        }


    }
}
