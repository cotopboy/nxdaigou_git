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
using UtilitiesGui;
using Utilities.WinAPI;
using System.Windows.Interop;
using System.Threading;

namespace daigou.main
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();
        }

        private void LogContentRegion_Loaded(object sender, RoutedEventArgs e)
        {
            CbbSplash.CloseSplash();
            WindowInteropHelper helper = new WindowInteropHelper(this);

            Thread.Sleep(1); 
            WinAPI.SetForegroundWindow(helper.Handle);



            Action action = new Action(delegate{

                Thread.Sleep(100);
                this.Dispatcher.Invoke(new Action(delegate
                {
                    WindowInteropHelper helper2 = new WindowInteropHelper(this);
                    WinAPI.SetForegroundWindow(helper2.Handle);
                }));

            });
            action.BeginInvoke(null, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
