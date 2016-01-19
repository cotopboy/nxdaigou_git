using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using daigou.modules.Product;
using System.ComponentModel;
using Microsoft.Practices.Prism.Events;
using daigou.domain.Services;
using Microsoft.Practices.Unity;
using System.Windows.Data;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;
using daigou.modules.Product.Events;
using daigou.infrastructure.ExtensionMethods;
using daigou.infrastructure.Events;
using daigou.modules.Command;

namespace daigou.modules.Bill
{
    public class SearchProductViewModel : NotificationObject
    {
         private readonly DelegateCommand cleanFilterTxtCommand;

    

        private ObservableCollection<ProductItemViewModel> productList = new ObservableCollection<ProductItemViewModel>();
        private ICollectionView productListCollectionView = null;

        private ProductItemViewModel selectedItem = null;
        private IEventAggregator eventAggregator = null;
        private string filterTxt = "";

        public string FilterTxt
        {
            get { return filterTxt; }
            set 
            { 
                filterTxt = value; 
                RaisePropertyChanged("FilterTxt");
                this.productListCollectionView.Refresh();
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
 
        public DelegateCommand RecalculateCommand
        {
            get { return new DelegateCommand(() => { RecalculateItemPrice(); }); }
        }

        private void RecalculateItemPrice()
        {
            foreach (var item in this.ProductList)
            {
                item.Euro2Cny = this.Eruo2Cny * this.SecondServiceRate;
            }
        }

        public DelegateCommand ReloadCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        Reload();
                    });
            }
        }

        

        public ProductItemViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                RaisePropertyChanged("SelectedItem");
                this.eventAggregator.GetEvent<SelectedProductItemChangedEvent>()
                    .Publish(new SelectedProductItemChangedEventPayLoad(value));
            }
        }

        public ICollectionView ProductListCollectionView
        {
            get { return productListCollectionView; }
            set { productListCollectionView = value; RaisePropertyChanged("ProductListCollectionView"); }
        }

        public ObservableCollection<ProductItemViewModel> ProductList
        {
            get { return productList; }
            set { productList = value; RaisePropertyChanged("ProductList"); }
        }

        private ProductService productService;
        private IUnityContainer container;
        private ProductVMContainer productVMContainer;
        public SearchProductViewModel( 
            IEventAggregator eventAggregator,
            ProductService productService,
            IUnityContainer container,
            ProductVMContainer productVMContainer
            )
        {
            this.container = container;
            this.eventAggregator = eventAggregator;
            this.productService = productService;
            this.productVMContainer = productVMContainer;

            this.cleanFilterTxtCommand = new DelegateCommand(() => this.FilterTxt = string.Empty);

            Reload();

            this.eventAggregator.GetEvent<ProductSellPriceChangedEvent>()
                .Subscribe(ProductSellPriceChangedEventHandler);
            this.eventAggregator.GetEvent<SelectedItemsToCurrentBillEvent>()
                .Subscribe(SelectedItemsToCurrentBillEventHandler);

            this.eventAggregator.GetEvent<BillQuickActionEvent>()
                .Subscribe(BillQuickActionEventHandler);
           
        }

        private void BillQuickActionEventHandler(string key)
        {
            this.FilterTxt = key;
        }

        private void SelectedItemsToCurrentBillEventHandler(SelectedItemsToCurrentBillPayLoad payload)
        {
            foreach (var item in payload.SelectedItemKeyList)
            {
                var product = this.ProductList.FirstOrDefault(x => x.DomainProduct.Code == item);

                if (product != null)
                {
                    this.eventAggregator.GetEvent<ProductItemDoubleClickEvent>()
                        .Publish(new ProductItemDoubleClickEventPayLoad(product));
                }
            }
        }


        private void Reload()
        {
            var dmProductList = this.productService.GetAllProduct();

            var list = dmProductList.Select(x =>
               CreateVM(container, x)
               ).ToList();

            this.ProductList = new ObservableCollection<ProductItemViewModel>(list);

            this.productVMContainer.ProductList = this.ProductList;
            this.ProductListCollectionView = CollectionViewSource.GetDefaultView(this.productList);
            this.ProductListCollectionView.Filter = FilterFunction;
        }

        private void ProductSellPriceChangedEventHandler(ProductSellPriceChangedEventPayLoad payload)
        {
            this.Eruo2Cny = payload.Euro2Cny;
            this.SecondServiceRate = payload.SecondServiceRate;

            RecalculateCommand.Execute();
        }

        private ProductItemViewModel CreateVM(IUnityContainer container, domain.Product x)
        {
            var vm =  container.Resolve<ProductItemViewModel>(new ParameterOverrides()
                    {
                        {"domainProduct",x}
                    }.OnType<ProductItemViewModel>());

            vm.Euro2Cny = this.Eruo2Cny * this.SecondServiceRate;

            return vm;
        }
        
        private bool FilterFunction(object targetObj)
        {
            ProductItemViewModel productVm = targetObj as ProductItemViewModel;
            domain.Product product = productVm.DomainProduct;

          //  var filterItems = this.FilterTxt.Split(new string[] { ";", ",", "，" }, StringSplitOptions.RemoveEmptyEntries);

            string filterText = this.FilterTxt.ToLowerInvariant();
            if (this.FilterTxt.Length == 0) return true;

                bool ret =
                    product.ID == this.FilterTxt.StrToInt() ||
                    product.Code.ToLowerInvariant().Contains(filterText) ||
                    product.Name.ToLowerInvariant().Contains(filterText) ||
                    product.Brand.ToLowerInvariant().Contains(filterText) ||
                    product.Remark.ToLowerInvariant().Contains(filterText)  ||
                    product.TagList.ToLowerInvariant().Contains(filterText) ||
                    product.Name.GetPinyinInitials().Contains(filterText.ToUpper());

                return ret;
           

        }

        public DelegateCommand CleanFilterTxtCommand
        {
            get { return cleanFilterTxtCommand; }
        } 
    }
    
}
