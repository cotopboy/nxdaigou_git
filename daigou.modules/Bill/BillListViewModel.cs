using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using daigou.domain.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Commands;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using daigou.modules.Product.Events;
using System.Windows;
using daigou.infrastructure.Events;
using daigou.modules.Command;
using System.IO;
using Utilities.IO;


namespace daigou.modules.Bill
{
    public class BillListViewModel : NotificationObject
    {
        private BillService billService;
        private IUnityContainer container;

        private ObservableCollection<BillViewModel> billList = new ObservableCollection<BillViewModel>();

        public ObservableCollection<BillViewModel> BillList
        {
            get { return billList; }
            set { billList = value; RaisePropertyChanged("BillList"); }
        }
        private ICollectionView billListCollectionView = null;

        public ICollectionView BillListCollectionView
        {
            get { return billListCollectionView; }
            set { billListCollectionView = value; RaisePropertyChanged("BillListCollectionView"); }
        }

        private BillViewModel selectedItem = null;

        public BillViewModel SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; RaisePropertyChanged("SelectedItem"); }
        }

        public DelegateCommand CleanFilterTxtCommand
        {
            get
            {
                return new DelegateCommand(() => { this.FilterTxt = string.Empty; });
            }
        }

        private IEventAggregator eventAggregator = null;
        private string filterTxt = "";

        public string FilterTxt
        {
            get { return filterTxt; }
            set
            {
                filterTxt = value;
                RaisePropertyChanged("FilterTxt");
                this.billListCollectionView.Refresh();
            }
        }
        private bool isTimeFilterEnabled = true;

        public bool IsTimeFilterEnabled
        {
            get { return isTimeFilterEnabled; }
            set
            {
                isTimeFilterEnabled = value;
                RaisePropertyChanged("IsTimeFilterEnabled");
                this.billListCollectionView.Refresh();
            }
        }

        private bool isShowOnlySelected;

        public bool IsShowOnlySelected
        {
            get { return isShowOnlySelected; }
            set
            {
                isShowOnlySelected = value; RaisePropertyChanged("IsShowOnlySelected");
                this.billListCollectionView.Refresh();
            }
        }

        public DelegateCommand Selected2TodayCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        BillList.Where(x => x.IsSelected).ForEach(x => x.DomainBill.CreatedTime = DateTime.Now);
                        billService.Save();
                    });
            }
        }


        public DelegateCommand CopyPayInformationCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    try
                    {
                        string path = DirectoryHelper.CombineWithCurrentExeDir("paid_info.txt");

                        FileInfo file = new FileInfo(path);

                        var content = file.Read();

                        Clipboard.SetText(content);
                    }
                    catch { }
                });
            }
        }

        public DelegateCommand DeSelectAllCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        foreach (var item in this.billListCollectionView)
                        {
                            var vm = item as BillViewModel;
                            vm.IsSelected = false;
                        }
                    });
            }
        }

        private decimal eruo2Cny = 8.5m;

        public decimal Eruo2Cny
        {
            get { return eruo2Cny; }
            set { eruo2Cny = value; RaisePropertyChanged("Eruo2Cny"); }
        }

        private decimal secondServiceRate = 1.0m;

        public decimal SecondServiceRate
        {
            get { return secondServiceRate; }
            set { secondServiceRate = value; RaisePropertyChanged("SecondServiceRate"); }
        }

        private int selectBillCount = 0;

        public int SelectBillCount
        {
            get { return selectBillCount; }
            set { selectBillCount = value; RaisePropertyChanged("SelectBillCount"); }
        }

        private decimal billSummayTotal = 0;

        public decimal BillSummayTotal
        {
            get { return billSummayTotal; }
            set { billSummayTotal = value; RaisePropertyChanged("BillSummayTotal"); }
        }

        public DelegateCommand Recalculate_8_5_Command
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.Eruo2Cny = 8.5m;
                    this.SecondServiceRate = 1.0m;
                    this.RecalculateCommand.Execute();
                });
            }
        }

        public DelegateCommand Recalculate_7_5_Command
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.Eruo2Cny = 7.5m;
                    this.SecondServiceRate = 1.0m;
                    this.RecalculateCommand.Execute();
                });
            }
        }

        public DelegateCommand UpdateSelectedPriceCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        UpdateSelectedPrice();
                    });
            }
        }

     

        public DelegateCommand SelectAllCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        foreach (var item in this.billListCollectionView)
                        {
                            var vm = item as BillViewModel;
                            vm.IsSelected = true;
                        }
                    });
            }
        }

        public DelegateCommand FilterDayDecreaseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (Filterlastdays > 0)
                        Filterlastdays--;
                });
            }
        }

        private int filterlastdays = 0;
        public int Filterlastdays
        {
            get { return filterlastdays; }
            set
            {
                filterlastdays = value;
                RaisePropertyChanged("Filterlastdays");
                this.billListCollectionView.Refresh();
            }
        }

        public DelegateCommand FilterDayIncreaseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (Filterlastdays < 50)
                        Filterlastdays++;
                });
            }
        }


        public DelegateCommand Selected2EmailTitleCommand
        {
            get { return new DelegateCommand(() => { Selected2EmailTitle();  }); }
        }

        private void Selected2EmailTitle()
        { 
            var selectedList = BillList.Where(x => x.IsSelected);

            string txt = string.Join(",", selectedList.Select(x => x.DomainBill.CustomerName));

            string template = "[NxDaigou奶粉账单][总计:{2}元][{1}单][{3}][{0}]";

            string content = template.FormatAs(DateTime.Now.ToString("s"), selectedList.Count(), selectedList.Sum(x => x.Summary), txt);

            try
            {
                Clipboard.SetText(content);
            }
            catch { }

        
        }


        public DelegateCommand RecalculateCommand
        {
            get { return new DelegateCommand(() => { RecalculateItemPrice(); }); }
        }

        private void RecalculateItemPrice()
        {


            this.eventAggregator.GetEvent<ProductSellPriceChangedEvent>()
                .Publish(new ProductSellPriceChangedEventPayLoad()
                {
                    Euro2Cny = this.Eruo2Cny,
                    SecondServiceRate = this.SecondServiceRate
                });
        }

        private ProductVMContainer productVMContainer;

        public BillListViewModel(
                IEventAggregator eventAggregator,
                BillService billService,
                ProductVMContainer productVMContainer,
                IUnityContainer container
            )
        {
            this.productVMContainer = productVMContainer;
            this.billService = billService;

            this.eventAggregator = eventAggregator;
            this.container = container;

            this.eventAggregator.GetEvent<ProductItemDoubleClickEvent>().Subscribe(OnProductItemDoubleClickEventHandler);
            this.eventAggregator.GetEvent<ProductSellPriceChangedEvent>().Subscribe(ProductSellPriceChangedEventHandler);
            this.eventAggregator.GetEvent<SelectedItemsToNewBillEvent>().Subscribe(OnSelectedItemsToNewBillEventHandler);

            var dmBillList = billService.GetAllBills();

            var list = dmBillList.Select(x =>
               CreateVM(container, x)
               ).ToList();

            this.BillList = new ObservableCollection<BillViewModel>(list);

            this.BillListCollectionView = CollectionViewSource.GetDefaultView(this.billList);
            this.BillListCollectionView.Filter = FilterFunction;

        }

        private void UpdateSelectedPrice()
        {
            var selectedList = BillList.Where(x => x.IsSelected);

            var productList = this.productVMContainer.ProductList;

            foreach (var itemBill in selectedList)
            {
                foreach (var item in itemBill.BillItemVMList)
                {
                    var product  = productList.FirstOrDefault(x => x.DomainProduct.ID == item.DomainBillItem.ProductId);
                    if(product != null)
                    {
                        item.DomainBillItem.SellPrice = product.SellPrice;
                    }
                }
            }
        }

        private void OnSelectedItemsToNewBillEventHandler(SelectedItemsToNewBillPayLoad payload)
        {
            AddNewItem();
            this.SelectedItem.IsSelected = true;
            this.eventAggregator.GetEvent<SelectedItemsToCurrentBillEvent>()
                .Publish(new SelectedItemsToCurrentBillPayLoad() 
                {
                     SelectedItemKeyList = payload.SelectedItemKeyList
                });

        }

        private void ProductSellPriceChangedEventHandler(ProductSellPriceChangedEventPayLoad payload)
        {
            this.Eruo2Cny = payload.Euro2Cny;
            this.SecondServiceRate = payload.SecondServiceRate;

        }

        private void OnProductItemDoubleClickEventHandler(ProductItemDoubleClickEventPayLoad payload)
        {
            var productVm = payload.ProductItemViewModel;
            if (this.SelectedItem != null)
            {
                this.SelectedItem.Add(productVm);
            }

        }

        private bool FilterFunction(object targetObj)
        {
            BillViewModel billVM = targetObj as BillViewModel;
            domain.Bill bill = billVM.DomainBill;

            string filterText = this.FilterTxt.ToLowerInvariant();
            
            bool textFilter =   bill.Id == this.FilterTxt.StrToInt()
                             ||  bill.Remark.ToLower().Contains(this.FilterTxt.ToLower())
                             || bill.CustomerName.Contains(this.FilterTxt)
                             || bill.CustomerName.GetPinyinInitials().Contains(filterText.ToUpper());                        

            bool timeFilter = IsTimeFilterEnabled ? (bill.CreatedTime.AddDays(this.filterlastdays).Date == DateTime.Today) : true;

            bool isShowSelected = !IsShowOnlySelected || billVM.IsSelected;

            return textFilter && timeFilter && isShowSelected;

        }


        private void Delete()
        {
            this.billService.Delete(this.SelectedItem.DomainBill.Id);

            this.billList.Remove(SelectedItem);
        }

        private void SaveAll()
        {
            this.billService.Save();
        }

        private void CopyAddNew()
        {
            domain.Bill dmBill = this.SelectedItem.DomainBill.Clone();
            dmBill.Id = 0;

            BillViewModel vm = CreateVM(this.container, dmBill);

            this.billService.Add(dmBill);
            this.billList.Add(vm);
            this.SelectedItem = vm;


        }

        private void AddNewItem()
        {
            domain.Bill dmBill = new domain.Bill();

            BillViewModel vm = CreateVM(this.container, dmBill);

            this.billService.Add(dmBill);
            this.billList.Add(vm);
            this.SelectedItem = vm;


        }

        private  BillViewModel CreateVM(IUnityContainer container, domain.Bill x)
        {
            var vm  = container.Resolve<BillViewModel>(new ParameterOverrides()
                    {
                        {"domainBill",x}
                    }.OnType<BillViewModel>());

            vm.PropertyChanged += new PropertyChangedEventHandler(BillViewModel_PropertyChanged);


            return vm;
        }

        private void BillViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.selectBillCount = BillList.Where(x => x.IsSelected).Count();

            this.BillSummayTotal = BillList.Where(x => x.IsSelected).Sum(x => x.Summary);           
           


            RaisePropertyChanged("SelectBillCount");
            RaisePropertyChanged("BillSummayTotal");
        }


        #region
        public DelegateCommand DeleteCommand
        {
            get { return new DelegateCommand(() => { Delete(); }); }
        }

        public DelegateCommand SaveAllCommand
        {
            get { return new DelegateCommand(() => { SaveAll(); }); }
        }

        public DelegateCommand AddNewItemCommand
        {
            get { return new DelegateCommand(() => { AddNewItem(); }); }
        }

        public DelegateCommand CopyAddNewCommand
        {
            get { return new DelegateCommand(() => { CopyAddNew(); }); }
        }
        #endregion

    }
}
