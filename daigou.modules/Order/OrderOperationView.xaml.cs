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
    /// Interaction logic for OrderOperationView.xaml
    /// </summary>
    public partial class OrderOperationView : UserControl
    {
        private OrderOperationViewModel vm;

        public OrderOperationView(OrderOperationViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            this.vm = vm;
            this.vm.LoadOrderListCommand.Execute();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.vm.OnRecipientNewOrderAdded(new infrastructure.Events.RecipientNewOrderAddedEventPayload (2,"23213","23"));
                
        }
    }
}
