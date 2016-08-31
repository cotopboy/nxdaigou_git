using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using daigou.domain;
using Microsoft.Practices.Prism.Commands;

namespace daigou.modules.Bill
{
    public class BillItemVM : NotificationObject
    {

        public event Action<BillItemVM> OnItemDeleted;

        public DelegateCommand DeleteCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        if (OnItemDeleted != null) OnItemDeleted(this);
                    });
            }
        }

        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { productName = value; RaisePropertyChanged("ProductName"); }
        }

        private string productCode;

        public string ProductCode
        {
            get { return productCode; }
            set { productCode = value; RaisePropertyChanged("ProductCode"); }
        }

        private string productSpec;

        public string ProductSpec
        {
            get { return productSpec; }
            set { productSpec = value; RaisePropertyChanged("ProductSpec"); }
        }

        private string suitablePeople;
        public string SuitablePeople
        {
            get { return suitablePeople; }
            set { suitablePeople = value; RaisePropertyChanged("SuitablePeople"); }
        }

        private decimal weight;

        public decimal WeightSummary
        {
            get { return weight * this.DomainBillItem.Count / 1000.0m; }
           
        }

        private BillItem domainBillItem;

        public BillItem DomainBillItem
        {
            get { return domainBillItem; }
            set { domainBillItem = value; RaisePropertyChanged("DomainBillItem"); }
        }


        public decimal Summary
        {
            get { return domainBillItem.Count * DomainBillItem.SellPrice; }
        }


        public BillItemVM(string productName,string productSpec,string productCode,string suitablePeople,decimal weight,BillItem domainBillItem)
        {
            this.suitablePeople = suitablePeople; 
            this.productCode = productCode;
            this.productName = productName;
            this.productSpec = productSpec;
            this.weight = weight;
            this.domainBillItem = domainBillItem;
            this.domainBillItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(domainBillItem_PropertyChanged);
        }

        private void domainBillItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("Summary");
            RaisePropertyChanged("WeightSummary"); 
        }

    }
}
