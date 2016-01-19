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
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;

namespace daigou.modules.Order
{
    /// <summary>
    /// Interaction logic for OrderPrintNavi.xaml
    /// </summary>
    public partial class OrderPrintNavi : UserControl
    {
        private IRegionManager manager;

        public OrderPrintNavi(IRegionManager manager)
        {
            this.manager = manager;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(ViewNames.OrderPrintView, UriKind.Relative);
            this.manager.RequestNavigate(RegionNames.MainContentRegion, uri);
        }
    }
}
