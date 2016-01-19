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

namespace daigou.modules.Recipient
{
    /// <summary>
    /// Interaction logic for AddRecipientView.xaml
    /// </summary>
    public partial class AddRecipientView : UserControl
    {
        private AddRecipientViewModel vm = null;
        public AddRecipientView(AddRecipientViewModel vmzz)
        {
            InitializeComponent();
            this.vm = vmzz;
            this.DataContext = this.vm;
        }
    }
}
