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

namespace daigou.modules.Bill
{
    /// <summary>
    /// Interaction logic for BillListView.xaml
    /// </summary>
    public partial class BillListView : UserControl
    {
        public BillListView(BillListViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer sv = FindVisualChild<ScrollViewer>(this.listView);
            sv.ScrollToHome();

            
            this.listView.InvalidateVisual();

            int totalHeight = 0;

            foreach (var item in this.listView.Items)
            {
                BillViewModel vm = (BillViewModel)item;
                totalHeight += vm.ItemHeight;
            }

            double w = this.listView.ActualWidth;

           
            listView.Measure(new Size(w, totalHeight));
            listView.Arrange(new Rect(new Size(w, totalHeight)));

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)w, totalHeight, 96.0, 96.0, PixelFormats.Pbgra32);

            bmp.Render(this.listView);

            try
            {
                Clipboard.SetImage(bmp);
            }
            catch { }

            this.listView.InvalidateVisual();

        }





        private childItem FindVisualChild<childItem>(DependencyObject obj) 
               where childItem : DependencyObject

            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is childItem)
                        return (childItem)child;
                    else
                    {
                        childItem childOfChild = FindVisualChild<childItem>(child);
                        if (childOfChild != null)
                            return childOfChild;
                    }
                }
                return null;
            } 
    }
}
