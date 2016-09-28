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
using daigou.modules.Product;
using daigou.modules.Bill;

namespace daigou.main.XamlResources
{
    /// <summary>
    /// Interaction logic for TestingUserControl.xaml
    /// </summary>
    public partial class TestingUserControl : UserControl
    {
        public TestingUserControl()
        {
            InitializeComponent();
            
            //this.DataContext = vm;
        }
    }
}
