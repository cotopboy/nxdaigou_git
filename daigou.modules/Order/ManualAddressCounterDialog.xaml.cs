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
using System.Windows.Shapes;
using daigou.services;

namespace daigou.modules.Order
{
    /// <summary>
    /// Interaction logic for ManualAddressCounterDialog.xaml
    /// </summary>
    public partial class ManualAddressCounterDialog : Window, IManualAddressLineCounter
    {


        public ManualAddressCounterDialog()
        {
            InitializeComponent();
        }

        public int GetAddressLine(string recipientName, string DocType)
        {
            this.lbName.Content = recipientName;
            this.lbType.Content = DocType;

            this.ShowDialog();

            return int.Parse(this.tbCount.Text);

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }


    }

  
}
