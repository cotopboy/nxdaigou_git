using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Environment.StartupHelper;
using System.Diagnostics;
using Utilities.IO;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using System.Threading;

namespace daigou.web
{
    class Program
    {
        static void Main(string[] args)
        {
            var otherRunningInstance = StartupHelper.RunningInstance();

            if (otherRunningInstance != null)
            {
                Console.WriteLine("Demo.exe has already started!");
                Thread.Sleep(1000);
                return;
            }

            string path = DirectoryHelper.CurrentProcessFullName;

            if (!StartupHelper.IsRunAsAdministrator() && !path.Contains(".vshost."))
            {
                StartupHelper.StartAppAsAdministrator(DirectoryHelper.GetAssemblyFileFullName(typeof(Program)), new string[] { });
                Environment.Exit(0);
            }

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            new DaigouWebStarter().Start();

            if (args.IsNullOrEmpty() || args[0] != "-RunAsService")
            {
                try
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "onStart.bat",
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    }.Start();
                }
                catch
                {
                    Console.WriteLine("An error happened during running onStart.bat!");
                }
            }

            Console.Read();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
