using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.Practices.Prism.Events;
using daigou.modules.Product.Events;
using daigou.infrastructure.ExtensionMethods;
using Microsoft.Practices.Prism.Commands;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;
using daigou.domain.Services;
using Microsoft.Practices.Unity;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using daigou.infrastructure.Events;
using daigou.services;
using Utilities.Configuration;
using daigou.modules.Command;



namespace daigou.modules.Product
{
    public class ProductListViewModel : NotificationObject
    {
        private readonly DelegateCommand cleanFilterTxtCommand;

    

        private ObservableCollection<ProductItemViewModel> productList = new ObservableCollection<ProductItemViewModel>();
        private ICollectionView productListCollectionView = null;

        private ProductItemViewModel selectedItem = null;
        private IEventAggregator eventAggregator = null;

        private string exportFileType = ".pdf";

        public string ExportFileType
        {
            get { return exportFileType; }
            set
            {
                exportFileType = value;
                RaisePropertyChanged("ExportFileType");
            }
        }

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

        private string scriptText = "";

        public string ScriptText
        {
            get { return scriptText; }
            set 
            {
                scriptText = value; 
                RaisePropertyChanged("ScriptText"); 
            
            }
        }


        public DelegateCommand RunScriptCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        RunScript();
                    });
            }
        }

        private void RunScript()
        {
            this.status.AddOrUpdate("DomainProduct.ScriptText", this.ScriptText);
            this.status.SaveToFile();
            this.scriptExecuter.Execute(this.ProductList.Where(x => x.IsSelected).Select(x => x.DomainProduct), ScriptText);
        }

        public DelegateCommand DeleteCommand
        {
            get { return new DelegateCommand(() => { Delete(); }); }
        }

        public DelegateCommand GetScreenShotCommand
        {
            get { return new DelegateCommand(() => { GetScreenShot(); }); }
        }

        public DelegateCommand SaveAllCommand
        {
            get { return new DelegateCommand(() => { SaveAll(); }); }
        }

        public DelegateCommand SortByIDCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        this.ProductListCollectionView.SortDescriptions.Add(new SortDescription("DomainProduct.ID", ListSortDirection.Ascending));
                    });
            }
        }

        public DelegateCommand SortByGrossProfitCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        this.ProductListCollectionView.SortDescriptions.Add(new SortDescription("GrossProfit", ListSortDirection.Ascending));
                    });
            }
        }

        public DelegateCommand SortByNameCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.ProductListCollectionView.SortDescriptions.Add(new SortDescription("DomainProduct.Name", ListSortDirection.Ascending));
                });
            }
        }

        public DelegateCommand SelectAllCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        this.ProductList.ForEach(x => x.IsSelected = true);
                    });
            }
        }

        public DelegateCommand SelectDisplayedItemCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        var filteredItems = ProductListCollectionView.Cast<ProductItemViewModel>();
                        foreach (var item in filteredItems)
	                    {
                            item.IsSelected = true;
	                    }                                                
                    });
            }
        }

        public DelegateCommand DeSelectAllCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.ProductList.ForEach(x => x.IsSelected = false);
                });
            }
        }


        public DelegateCommand ExportCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        this.productExportToPdfService.Export(
                              this.ProductList.Where(x => x.IsSelected).Select(x => x.DomainProduct).ToList()
                            , this.Eruo2Cny
                            , this.SecondServiceRate
                            , this.ExportFileType);
                    });
            }
        }



        public DelegateCommand SortByBrandCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.ProductListCollectionView.SortDescriptions.Add(new SortDescription("DomainProduct.Brand", ListSortDirection.Ascending));
                });
            }
        }

        public DelegateCommand CleanSortCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        this.ProductListCollectionView.SortDescriptions.Clear();
                    });
            }
        }


        public DelegateCommand AddNewItemCommand
        {
            get { return new DelegateCommand(() => { AddNewItem(); }); }
        }

        public DelegateCommand CopyAddNewCommand
        {
            get { return new DelegateCommand(() => { CopyAddNew(); }); }
        }

        public DelegateCommand RecalculateCommand
        {
            get { return new DelegateCommand(() => { RecalculateItemPrice(); }); }
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

        public DelegateCommand SelectedItemsToNewBillCommand
        {
            get
            { return new DelegateCommand(() => { SelectedItemsToNewBill(); });
            }
        }


        private void SelectedItemsToNewBill()
        {
            var keyList = this.ProductList.Where(x => x.IsSelected).Select(x => x.DomainProduct.Code).ToList();

            this.eventAggregator.GetEvent<SelectedItemsToNewBillEvent>()
                .Publish(new SelectedItemsToNewBillPayLoad() 
                {
                     SelectedItemKeyList = keyList
                }
                );
        }

        private void RecalculateItemPrice()
        {
            foreach (var item in this.ProductList)
            {
                item.Euro2Cny = this.Eruo2Cny * this.SecondServiceRate;
            }

            this.eventAggregator.GetEvent<ProductSellPriceChangedEvent>()
                .Publish(new ProductSellPriceChangedEventPayLoad() 
                {
                     Euro2Cny = this.Eruo2Cny,
                     SecondServiceRate = this.SecondServiceRate
                });
        }

        private void Delete()
        {
            var selectedItems = this.ProductList.Where(x => x.IsSelected).ToList();

            foreach (var item in selectedItems)
            {
                this.productService.Delete(item.DomainProduct.ID);
                this.ProductList.Remove(item);                
            }
        }

        private void SaveAll()
        {
            this.productService.Save(null);
        }

        private void GetScreenShot()
        {
        
        }

        private void CopyAddNew()
        {
            domain.Product dmProduct = this.SelectedItem.DomainProduct.Clone();
            dmProduct.ID = 0;

            ProductItemViewModel vm = this.productVMContainer.CreateVM(this.container, dmProduct);
            vm.Euro2Cny = this.Eruo2Cny * this.SecondServiceRate;
            
            this.productService.Add(dmProduct);
            this.ProductList.Add(vm);
            this.SelectedItem = vm;

            this.eventAggregator.GetEvent<ProductNewItemCreatedEvent>().Publish(
                                new ProductNewItemCreatedEventPayLoad(vm));
            
        }

        private void AddNewItem()
        {
            domain.Product dmProduct = new domain.Product();

            ProductItemViewModel vm = this.productVMContainer.CreateVM(this.container, dmProduct);

            this.productService.Add(dmProduct);
            this.ProductList.Add(vm);
            this.SelectedItem = vm;

            this.eventAggregator.GetEvent<ProductNewItemCreatedEvent>().Publish(
                                new ProductNewItemCreatedEventPayLoad(vm));

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
        private ProductExportToPdfService productExportToPdfService;
        private IUnityContainer container;
        private ProductVMContainer productVMContainer;
        private DomainProductSrcriptExecuter scriptExecuter;
        private AppStatusManager status;

        public ProductListViewModel( IEventAggregator eventAggregator,
            ProductService productService,
            ProductVMContainer productVMContainer,
            IUnityContainer container,
            ProductExportToPdfService productExportToPdfService,
            DomainProductSrcriptExecuter scriptExecuter,
            AppStatusManager status
            )
        {

            this.container = container;
            this.scriptExecuter = scriptExecuter;
            this.productExportToPdfService = productExportToPdfService;
            this.eventAggregator = eventAggregator;
            this.productService = productService;
            this.productVMContainer = productVMContainer;
            this.status = status;

            this.cleanFilterTxtCommand = new DelegateCommand(() => this.FilterTxt = string.Empty);
            
            var dmProductList = productService.GetAllProduct();

            var list= dmProductList.Select(x =>
               CreateVM(container, x)
               ).ToList().OrderBy(x => x.DomainProduct.Brand).ThenBy(x => x.DomainProduct.Name);

            this.ProductList = new ObservableCollection<ProductItemViewModel>(list);
       
            this.ProductListCollectionView = CollectionViewSource.GetDefaultView(this.ProductList);
            this.ProductListCollectionView.Filter = FilterFunction;

            this.ScriptText = this.status.Get("DomainProduct.ScriptText", "");
            this.eventAggregator.GetEvent<ProductSellPriceChangedEvent>().Subscribe(ProductSellPriceChangedEventHandler);
             
        }

        private void ProductSellPriceChangedEventHandler(ProductSellPriceChangedEventPayLoad payload)
        {
            this.Eruo2Cny = payload.Euro2Cny;
            this.SecondServiceRate = payload.SecondServiceRate;
            foreach (var item in this.ProductList)
            {
                item.Euro2Cny = this.Eruo2Cny * this.SecondServiceRate;
            }

        }

        private static ProductItemViewModel CreateVM(IUnityContainer container, domain.Product x)
        {
            return container.Resolve<ProductItemViewModel>(new ParameterOverrides()
                    {
                        {"domainProduct",x}
                    }.OnType<ProductItemViewModel>());
        }
        
        private bool FilterFunction(object targetObj)
        {
            ProductItemViewModel productVm = targetObj as ProductItemViewModel;
            domain.Product product = productVm.DomainProduct;

          //  var filterItems = this.FilterTxt.Split(new string[] { ";", ",", "，" }, StringSplitOptions.RemoveEmptyEntries);

            string filterText = this.FilterTxt.ToLowerInvariant();
            if (this.FilterTxt.Length == 0) return true;



                bool ret =
                    productVm.IsSelected ||
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
