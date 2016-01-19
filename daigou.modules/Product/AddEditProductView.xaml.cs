using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using daigou.modules.Product.Events;
using Microsoft.Practices.Prism.Events;
using daigou.infrastructure.ExtensionMethods;
using Utilities.IO;

namespace daigou.modules.Product
{
    /// <summary>
    /// Interaction logic for AddEditProductView.xaml
    /// </summary>
    public partial class AddEditProductView : UserControl
    {

        private ProductItemViewModel vm = null;
        private IUnityContainer container;
        private IEventAggregator eventAggregator;

        public AddEditProductView(IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.container = container;
            InitializeComponent();

            this.eventAggregator.GetEvent<SelectedProductItemChangedEvent>().Subscribe(SelectedProductItemChangedEventHandler);
        }

        private void SelectedProductItemChangedEventHandler(SelectedProductItemChangedEventPayLoad payload)
        {
            this.DataContext = payload.ProductItemViewModel;
            this.vm = payload.ProductItemViewModel;
        }

        private void btn_loadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (ofd.ShowDialog() == true)
            {
                string filename = ofd.FileName;
                this.vm.DomainProduct.Photo = filename.MakeRelative(DirectoryHelper.CurrentExeDirectory);
            }

            this.vm.SaveItemCommand.Execute();
        }



    }
}
