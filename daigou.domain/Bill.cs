using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    [Serializable]
    public class Bill : DomainObject
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; RaisePropertyChanged("Id"); }
        }

        private List<BillItem> billItemList = new List<BillItem> ();

        public List<BillItem> BillItemList
        {
            get { return billItemList; }
            set { billItemList = value; RaisePropertyChanged("BillItemList"); }
        }

        private DateTime createdTime;

        public DateTime CreatedTime
        {
            get { return createdTime; }
            set { createdTime = value; RaisePropertyChanged("CreatedTime"); }
        }

        private string remark = "";


        public string Remark
        {
            get { return remark; }
            set { remark = value; RaisePropertyChanged("Remark"); }
        }

        private string customerName = "";

        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; RaisePropertyChanged("CustomerName"); }
        }



        public Bill()
        {
            CreatedTime = DateTime.Now;
            
        }


    }

    [Serializable]
    public class BillItem : DomainObject
    {
        private int productId;

        public int ProductId
        {
            get { return productId; }
            set { productId = value; RaisePropertyChanged("ProductId"); }
        }

        private decimal sellPrice;

        public decimal SellPrice
        {
            get { return sellPrice; }
            set { sellPrice = value; RaisePropertyChanged("SellPrice"); }
        }

        private int count;

        public int Count
        {
            get { return count; }
            set { count = value; RaisePropertyChanged("Count"); }
        }

        private string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; RaisePropertyChanged("Remark"); }
        }



    }
}
