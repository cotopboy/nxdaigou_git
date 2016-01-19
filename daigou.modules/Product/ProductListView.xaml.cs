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
using System.IO;

namespace daigou.modules.Product
{
    /// <summary>
    /// Interaction logic for ProductListView.xaml
    /// </summary>
    public partial class ProductListView : UserControl
    {
        public ProductListView(ProductListViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            double h = 79 * this.listView.Items.Count + 30;
            double w = this.listView.ActualWidth ;

            listView.Measure(new Size(w, h));
            listView.Arrange(new Rect(new Size(w, h)));

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)w -150, (int)h, 96.0, 96.0, PixelFormats.Pbgra32);

            bmp.Render(this.listView);

            try
            {
                Clipboard.SetImage(bmp);
            }
            catch { }
        }


    }
}
