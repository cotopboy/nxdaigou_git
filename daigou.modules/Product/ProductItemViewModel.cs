using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using daigou.modules.Product.Events;
using Microsoft.Practices.Prism.Commands;
using daigou.domain.Services;
using daigou.services;

namespace daigou.modules.Product
{
    public class ProductItemViewModel : NotificationObject
    {
        private domain.Product domainProduct;

        #region command


        public DelegateCommand SaveItemCommand
        {
            get
            {
                return new DelegateCommand(() => { SaveItem(); });
            }
        }



        public DelegateCommand ItemDoubleClickCommand
        {
            get 
            {
                return new DelegateCommand(() => { ItemDoubleClick(); });
            }        
        }

        public DelegateCommand ItemDoubleClickSelectCommand
        {
            get
            {
                return new DelegateCommand(() => { this.IsSelected = !this.IsSelected; });
            }
        }

        #endregion


        private void SaveItem()
        {
            this.productService.Save(this.domainProduct);            
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



        public domain.Product DomainProduct
        {
            get { return domainProduct; }
            set 
            {
                this.domainProduct = value; 
                RaisePropertyChanged("DomainProduct"); 
            }
        }

        private decimal euro2Cny = 8.5m;
        
        public decimal Euro2Cny
        {
            get
            {
                return euro2Cny;
            }
            set
            {
                this.euro2Cny = value;
                RaisePropertyChanged("Euro2Cny");
                domainProduct_PropertyChanged(null, null);
            }
        }
            

        public void ItemDoubleClick()
        {
            
            this.eventAggregator.GetEvent<ProductItemDoubleClickEvent>()
                .Publish(new ProductItemDoubleClickEventPayLoad(this));
        }

        public decimal SellPrice
        {
            get { return CalculateSellPrice(); }
        }

        public decimal GrossProfit
        {
            get { return CalculateGrossProfit(); }
        }

        private decimal CalculateGrossProfit()
        { 
            var o = domainProduct;
            return Math.Round(SellPrice - (o.ImportPrice - o.PackingCost) * Euro2Cny,2);
        }

        private decimal CalculateSellPrice()
        {
            var o = domainProduct;

            return this.productPriceCalcuateService.GetPrice(o, this.Euro2Cny);
        }


        private ProductService productService;
        private IEventAggregator eventAggregator;
        private ProductPriceCalcuateService productPriceCalcuateService;

        public ProductItemViewModel(ProductService productService, 
            domain.Product domainProduct, 
            IEventAggregator eventAggregator,
            ProductPriceCalcuateService productPriceCalcuateService)
        {            
            this.productService = productService;
            this.productPriceCalcuateService = productPriceCalcuateService;
            this.eventAggregator = eventAggregator;
            this.domainProduct = domainProduct;
            this.domainProduct.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(domainProduct_PropertyChanged);
        }

        public void domainProduct_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged("SellPrice");
            this.RaisePropertyChanged("GrossProfit");
        }

        public void SetDomainObject(domain.Product domainProduct)
        {
            this.domainProduct = domainProduct;
        }

       

    }
}
