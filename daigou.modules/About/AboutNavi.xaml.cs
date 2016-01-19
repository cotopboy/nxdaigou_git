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
using Microsoft.Practices.Unity;

namespace daigou.modules.About
{
    /// <summary>
    /// Interaction logic for AboutNavi.xaml
    /// </summary>
    public partial class AboutNavi : UserControl
    {
        private IRegionManager regionManager;

        public AboutNavi(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(ViewNames.AboutView, UriKind.Relative);
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);
        }
    }
}
