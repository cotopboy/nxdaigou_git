using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using daigou.domain.Services;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Events;
using daigou.modules.Product.Events;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;


namespace daigou.modules.Bill
{
    public class BillViewModel : NotificationObject
    {
        private domain.Bill domainBill;

        public domain.Bill DomainBill
        {
            get { return domainBill; }
            set { domainBill = value; RaisePropertyChanged("DomainBill"); }
        }

        public DelegateCommand ItemDoubleClickSelectCommand
        {
            get
            {
                return new DelegateCommand(() => { this.IsSelected = !this.IsSelected; });
            }
        }

        private BillService billService;
        private ProductService productService;
        private IEventAggregator eventAggregator;

        private ObservableCollection<BillItemVM> billItemVMList = new ObservableCollection<BillItemVM>();

        public ObservableCollection<BillItemVM> BillItemVMList
        {
            get { return billItemVMList; }
            set 
            {
                billItemVMList = value; 
                RaisePropertyChanged("BillItemVMList");
                RaisePropertyChanged("Summary");

            }
        }

        
        public int ItemHeight
        {
            get { return 50 + this.domainBill.BillItemList.Count * 24; }
        }


        public decimal Summary
        {
            get { return BillItemVMList.Sum(x => x.Summary); }
        }

        private bool isSelected = false;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value; RaisePropertyChanged("IsSelected");
            }
        }


        public decimal WeightSummary
        {
            get { return billItemVMList.Sum(x => x.WeightSummary) ; }
        }


        public BillViewModel(BillService billService,
            ProductService productService,
            domain.Bill domainBill,
            IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.billService = billService;
            this.productService = productService;
            this.domainBill = domainBill;
            this.domainBill.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(domainBill_PropertyChanged);

            this.BillItemVMList = BuildBillItemVmList(this.domainBill);
        
        }

        public DelegateCommand CopyDetailCommand
        {
            get
            {
                return new DelegateCommand(() => { CopyDetail();  });
            }
        }

        private void CopyDetail()
        {
            var list = BillItemVMList.Select(x => x.ProductCode + "x" + x.DomainBillItem.Count);

            string content = string.Join(Environment.NewLine, list);

            try
            {
                Clipboard.SetText(content.Replace(" ", "_"));
            }
            catch { }
        }

       

        private ObservableCollection<BillItemVM> BuildBillItemVmList(domain.Bill bill)
        {
            List<BillItemVM> retList = new List<BillItemVM>();


            foreach (var item in bill.BillItemList)
            {
                domain.Product product = productService.GetAllProduct().SingleOrDefault(x => x.ID == item.ProductId);

                if(product == null) continue;

                BillItemVM vm = new BillItemVM(product.Brand +"->"+product.Name, product.Spec, product.Code,product.ApplicableCrowd, product.GrossWeight, item);
                vm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(vm_PropertyChanged);
                vm.OnItemDeleted += new Action<BillItemVM>(BillItem_OnItemDeleted);
                item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(item_PropertyChanged);
                retList.Add(vm);
            }

            return new ObservableCollection<BillItemVM>(retList);
        }

        private void BillItem_OnItemDeleted(BillItemVM obj)
        {
            this.BillItemVMList.Remove(obj);
            var dmObj =this.DomainBill.BillItemList.FirstOrDefault(x => x.ProductId == obj.DomainBillItem.ProductId);

            if (dmObj != null)
            {
                this.DomainBill.BillItemList.Remove(dmObj);
            }
            vm_PropertyChanged(null, null);
        }

        private void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("Summary");
            RaisePropertyChanged("WeightSummary"); 
        }

        private void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("Summary");
            RaisePropertyChanged("WeightSummary"); 
        }

         private void domainBill_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
         {
             RaisePropertyChanged("Summary");
             RaisePropertyChanged("WeightSummary"); 
         }

         internal void Add(Product.ProductItemViewModel productVm)
         {
             var targetItem =this.BillItemVMList.SingleOrDefault(x => x.ProductCode == productVm.DomainProduct.Code);
             if (targetItem != null)
             {
                 targetItem.DomainBillItem.SellPrice = productVm.SellPrice;
                 targetItem.DomainBillItem.Count++;
             }
             else
             {

                 var dmObj = new domain.BillItem()
                 {
                     ProductId = productVm.DomainProduct.ID,
                     Count = 1,
                     SellPrice = productVm.SellPrice
                 };

                 var vmObj = new BillItemVM(productVm.DomainProduct.Brand + "->" + productVm.DomainProduct.Name,
                     productVm.DomainProduct.Spec,
                     productVm.DomainProduct.Code,
                     productVm.DomainProduct.ApplicableCrowd,
                     productVm.DomainProduct.GrossWeight,
                     dmObj);

                 vmObj.OnItemDeleted += new Action<BillItemVM>(BillItem_OnItemDeleted);
                 vmObj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(vm_PropertyChanged);
                 dmObj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(item_PropertyChanged);

                 this.DomainBill.BillItemList.Add(dmObj);

                 this.BillItemVMList.Add(vmObj);
             }
             RaisePropertyChanged("Summary");
             RaisePropertyChanged("WeightSummary"); 
         }
    }
}
