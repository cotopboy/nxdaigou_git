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

namespace daigou.modules.Order
{
    /// <summary>
    /// Interaction logic for OrderPrintView.xaml
    /// </summary>
    public partial class OrderPrintView : UserControl
    {
        public OrderPrintView(OrderPrintViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
