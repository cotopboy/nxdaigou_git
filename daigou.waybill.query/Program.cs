using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace daigou.waybill.query
{
    static class Program
    {
        
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new AppInputParameters(args);

            Application.Run(new QueryMain());
        }
    }
}
