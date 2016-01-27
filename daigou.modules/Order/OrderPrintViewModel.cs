using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Forms;
using daigou.services;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Events;
using daigou.infrastructure.Events;
using Utilities.WinAPI;
using Microsoft.Practices.Prism.Regions;
using daigou.infrastructure;

namespace daigou.modules.Order
{
    public class OrderPrintViewModel : NotificationObject
    {

        private DirectoryService directoryService;
        private string dlogDirectory = @"T:\temp";
        private List<string> printExtensionList = new List<string>() { ".pdf", ".xls" };

        private ObservableCollection<string> printFileList = new ObservableCollection<string>();

        private PrintService printService;
        private IEventAggregator eventAggregator;
        private EmailDownloadService emailDownloadService;
        private IRegionManager regionManager;

        public OrderPrintViewModel(
            DirectoryService directoryService, 
            PrintService printService,
            IEventAggregator eventAggregator,
            EmailDownloadService emailDownloadService,
            IRegionManager regionManager
            )
        {
            this.eventAggregator = eventAggregator;
            this.directoryService = directoryService;
            this.printService = printService;
            this.emailDownloadService = emailDownloadService;
            this.regionManager = regionManager;

            this.eventAggregator.GetEvent<OnOrderCombinePrintClicked>().Subscribe(OnOrderCombinePrintClicked);
        }


        public DelegateCommand DonwLoadEmailAttachmentCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        DonwLoadEmailAttachment();
                    });
            }
        }

        public DelegateCommand PDFRenameCommand
        {
            get             
            {
                return new DelegateCommand(() => 
                {
                    CorrectAndRenamePdfFile();
                });
            }
        }

        private void CorrectAndRenamePdfFile()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(this.directoryService.BaseDir);

            var files = dirInfo.GetFiles("*.pdf");


            foreach (var item in files)
            {
                string newname = item.FullName.Replace('·', '`');

                if (RenameChar.Length > 0)
                    newname = newname.Replace(renameChar, "`");

                item.MoveTo(newname);                                              
            }

        }

        private void DonwLoadEmailAttachment()
        {
            do
            {
                int count = 0;
                try
                {
                    this.emailDownloadService.StartDownload();
                    WinAPI.MessageBeep(WinAPI.BeepType.SimpleBeep);
                    Uri uri = new Uri(ViewNames.OrderOperationView, UriKind.Relative);
                    this.regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);

                    break;
                }
                catch
                {
                    count++;
                }

                if (count >= 5)
                {
                    MessageBox.Show("下载失败");
                    break;
                }

            } while (true);

        }

        public void OnOrderCombinePrintClicked(OnOrderCombinePrintClickedPayLoad payload)
        {
            this.DlogDirectory = this.directoryService.BaseDir;

            AnalyseDirectory();
        }


        public DelegateCommand AnalyseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        AnalyseDirectory();
                    });
            }
        }

        private string renameChar;
        public string RenameChar
        {
            get { return renameChar; }
            set { renameChar = value; RaisePropertyChanged("RenameChar"); }
        }

        public string DlogDirectory
        {
            get { return dlogDirectory; }
            set { dlogDirectory = value; RaisePropertyChanged("DlogDirectory"); }
        }

        private bool isContainXlsInvoice = false;

        public bool IsContainXlsInvoice
        {
            get { return this.isContainXlsInvoice; }
            set { this.isContainXlsInvoice = value; RaisePropertyChanged("IsContainXlsInvoice"); }
        }

        public DelegateCommand OpenDirectoryCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        OpenDirectory();
                        AnalyseDirectory();
                    });
            }
        }

        public ObservableCollection<string> PrintFileList
        {
            get { return printFileList; }
            set { printFileList = value; RaisePropertyChanged("PrintFileList"); }
        }

        public DelegateCommand StartPrintCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        StartPrint();
                    });
            }
        }
        private void AnalyseDirectory()
        {
            if (!Directory.Exists(this.DlogDirectory)) return;

            printFileList.Clear();

            DirectoryInfo dinfo = new DirectoryInfo(this.DlogDirectory);
            List<string> list = new List<string>();

            foreach (var item in dinfo.GetFiles())
	        {
                if(printExtensionList.Contains(item.Extension))
                    list.Add(item.FullName);
	        }

            list.Sort();

            this.PrintFileList = new ObservableCollection<string>(list);
            
        }

        private void OpenDirectory()
        {
            FolderBrowserDialog dia = new FolderBrowserDialog();
            dia.SelectedPath = GetDefaultSelectedDir();
            if (dia.ShowDialog() == DialogResult.OK)
            {
                this.DlogDirectory = dia.SelectedPath;
            }
        }

        private string GetDefaultSelectedDir()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(this.directoryService.WaybillRepositoryDir);

            DirectoryInfo dir = dirInfo.GetDirectories().OrderByDescending(x => x.Name).FirstOrDefault();

            return (dir != null) ? dir.FullName : this.directoryService.WaybillRepositoryDir;
            
        }

        private void StartPrint()
        {
            foreach (var item in this.PrintFileList)
            {
                if (!IsContainXlsInvoice && item.EndsWith(".xls")) continue;

                this.printService.PrintFile(item);
            }
            WinAPI.MessageBeep(WinAPI.BeepType.SimpleBeep);
        }
    }
}
