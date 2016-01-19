using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using daigou.modules.Product;
using Microsoft.Practices.Prism.Events;
using daigou.domain.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.ViewModel;

namespace daigou.modules.Command
{
    public class ProductVMContainer : NotificationObject
    {
        private ObservableCollection<ProductItemViewModel> productList = new ObservableCollection<ProductItemViewModel>();

        public ObservableCollection<ProductItemViewModel> ProductList
        {
            get { return productList; }
            set { productList = value; RaisePropertyChanged("ProductList"); }
        }

        public ProductVMContainer( )                                 
        {
          

        }

        public ProductItemViewModel CreateVM(IUnityContainer container, domain.Product x)
        {
            return container.Resolve<ProductItemViewModel>(new ParameterOverrides()
                    {
                        {"domainProduct",x}
                    }.OnType<ProductItemViewModel>());
        }
    }
}
