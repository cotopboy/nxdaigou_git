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
using Microsoft.Practices.Prism.Events;
using daigou.infrastructure.Events;

namespace daigou.modules.Bill
{
    /// <summary>
    /// Interaction logic for BillQuickActionView.xaml
    /// </summary>
    public partial class BillQuickActionView : UserControl
    {
        private IEventAggregator eventAggregator;

        public BillQuickActionView(
            IEventAggregator eventAggregator
            )
        {
            InitializeComponent();
            this.eventAggregator = eventAggregator;

            
        }

        private void btnOnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            this.eventAggregator.GetEvent<BillQuickActionEvent>().Publish(btn.Content.ToString());
        }
    }
}
