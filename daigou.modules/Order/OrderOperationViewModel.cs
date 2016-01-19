using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel;
using System.Collections.ObjectModel;
using daigou.domain;
using System.Windows.Data;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using daigou.infrastructure.Events;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.services;
using daigou.infrastructure.ExtensionMethods;
using System.IO;
using daigou.infrastructure;
using System.Diagnostics;
using Utilities.WinAPI;
using System.Windows.Threading;

namespace daigou.modules.Order
{
    public class OrderOperationViewModel : NotificationObject
    {
        private readonly DelegateCommand bringTodayCommand;
        private readonly DelegateCommand bringYesterdayCommand;
        private readonly DelegateCommand cancelProcessCommand;
        private readonly DelegateCommand cleanFilterTxtCommand;
        private readonly DelegateCommand combinePrintCommand;
        private readonly DelegateCommand createDlogCommand;
        private readonly DelegateCommand deleteOrderCommand;
        private readonly DelegateCommand deSelectAllCommand;
        private readonly DelegateCommand loadOrderListCommand;
        private readonly DelegateCommand printSelectedCnLabelCommand;
        private readonly DelegateCommand printWaybillsCommand;
        private readonly DelegateCommand<string> processWaybillCommand;
        private readonly DelegateCommand saveOrderCommand;
        private readonly DelegateCommand saveSelectedOrderCommand;
        private readonly DelegateCommand selectAllCommand;
        private readonly DelegateCommand showConfirmDialogCommand;
        private ProcessModeInfo _SelectedDHLAgent;
        private CnRecipientLabelBuilder cnRecipientLabelBuilder;
        private DhlWaybillService dhlWaybillService;
        private DirectoryService directoryService;
        private IEventAggregator eventAggregator;
        private int filterlastdays = 0;
        private string filterTxt = string.Empty;
        private bool isShowConfirmDialog;
        private bool isShowOnlySelected = false;
        private bool isTimeFilterEnabled = true;
        private bool isUIEnable = true;        


        private ICollectionView orderCollectionView = null;
        private ObservableCollection<OrderViewModel> orderList = new ObservableCollection<OrderViewModel>();
        private OrderService orderService;
        private RecipientService recipientService;
        private IRegionManager manager;
        private PacketDeliveredReportService PacketDeliveredReportService;
        private WaybillQueryManager SearchManager;
        private ProcessModeInfoProvider ProcessModeInfoProvider;
        private List<ProcessModeInfo> ModeList;

        public OrderOperationViewModel(
            OrderService orderService,
            RecipientService recipientService,
            IEventAggregator eventAggregator,
            DhlWaybillService dhlWaybillService,
            CnRecipientLabelBuilder cnRecipientLabelBuilder,
            IRegionManager manager,
            PacketDeliveredReportService PacketDeliveredReportService,
            WaybillQueryManager SearchManager,
            DirectoryService directoryService,
            ProcessModeInfoProvider ProcessModeInfoProvider
            )
        {
            this.ProcessModeInfoProvider = ProcessModeInfoProvider;
            this.SearchManager = SearchManager;
            this.manager = manager;
            this.orderService = orderService;
            this.recipientService = recipientService;
            this.eventAggregator = eventAggregator;
            this.dhlWaybillService = dhlWaybillService;
            this.cnRecipientLabelBuilder = cnRecipientLabelBuilder;
            this.directoryService = directoryService;
            this.PacketDeliveredReportService = PacketDeliveredReportService;
            this.loadOrderListCommand = new DelegateCommand(LoadOrderList);
            this.saveOrderCommand = new DelegateCommand(SaveOrder);
            this.cleanFilterTxtCommand = new DelegateCommand(CleanFilterTxt);
            this.selectAllCommand = new DelegateCommand(SelectAll);
            this.deSelectAllCommand = new DelegateCommand(DeSelectAll);
            this.deleteOrderCommand = new DelegateCommand(DeleteOrder);
            this.processWaybillCommand = new DelegateCommand<string>(ProcessWaybill);
            this.saveSelectedOrderCommand = new DelegateCommand(SaveSelectedOrder);
            this.showConfirmDialogCommand = new DelegateCommand(() => { IsShowConfirmDialog = true; });
            this.cancelProcessCommand = new DelegateCommand(() => { IsShowConfirmDialog = false; });
            this.printSelectedCnLabelCommand = new DelegateCommand(PrintSelectedCnLabel);
            this.printWaybillsCommand = new DelegateCommand(PrintWaybill);
            this.combinePrintCommand = new DelegateCommand(CombinationPrint);
            this.createDlogCommand = new DelegateCommand(CreateDLog);
            this.bringTodayCommand = new DelegateCommand(BringToday);
            this.bringYesterdayCommand = new DelegateCommand(BringToYesterday);

            this.eventAggregator.GetEvent<RecipientNewOrderAddedEvent>().Subscribe
             (OnRecipientNewOrderAdded, ThreadOption.UIThread);

            this.ModeList = this.ProcessModeInfoProvider.ModeList;
            this.SelectedDHLAgent = this.ModeList.Single(x => x.Name.StartsWith("中德快递"));

            
        }


        public bool IsUIEnable
        {
            get { return isUIEnable; }
            set { isUIEnable = value; RaisePropertyChanged("IsUIEnable"); }
        }


        private string emailAdditionalInfo;

        public string EmailAdditionalInfo
        {
            get { return emailAdditionalInfo; }
            set { emailAdditionalInfo = value; RaisePropertyChanged("EmailAdditionalInfo"); }
        }


        public DelegateCommand BringTodayCommand
        {
            get { return bringTodayCommand; }
        }

        public DelegateCommand BringYesterdayCommand
        {
            get { return bringYesterdayCommand; }
        }

        public DelegateCommand CancelProcessCommand
        {
            get { return cancelProcessCommand; }
        }

        public DelegateCommand CleanFilterTxtCommand
        {
            get { return cleanFilterTxtCommand; }
        }

        public DelegateCommand CombinePrintCommand
        {
            get { return combinePrintCommand; }
        }

        public DelegateCommand CreateDlogCommand
        {
            get { return createDlogCommand; }
        }
        public DelegateCommand DeleteOrderCommand
        {
            get { return deleteOrderCommand; }
        }

        public DelegateCommand DeSelectAllCommand
        {
            get { return deSelectAllCommand; }
        }

        public List<ProcessModeInfo> DHLAgentList
        {
            get { return this.ModeList; }
        }

        public DelegateCommand StartSearchCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        StartSearch();
                    });
            }
        }

     

        public DelegateCommand FilterDayIncreaseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        if (Filterlastdays < 50 )
                        Filterlastdays++;
                    });
            }
        }

        public DelegateCommand CopyQueryMsgCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        CopyQueryMsg();
                    });
            }
        }

        private void CopyQueryMsg()
        {
            var list = this.orderList.Where(x => x.IsSelected)
                                 .Select(x => new PrintWayBillParam()
                                 {
                                     Order = x.Order,
                                     Recipient = x.Recipient
                                 }).ToList();

            StringBuilder content = new StringBuilder();

            foreach (var item in list)
	        {
                var array = item.Order.DhlSn.Split(';');

                string dhl = (array.Length >=1) ? array[0]:"";
                string bpost = (array.Length >=2) ? array[1]:"";
                string ems = (array.Length >=3) ? array[2]:"";
                content.AppendLine(this.SearchManager.GetQueryMsgComplete(item, dhl, bpost, ems));
	        }

            try
            {
                Clipboard.SetText(content.ToString());
            }
            catch { }
            
        }

        public DelegateCommand OpenMainDirCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        Process.Start(this.directoryService.BaseDir);
                    });
            }
        }

        public DelegateCommand FilterDayDecreaseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if(Filterlastdays > 0)
                        Filterlastdays--;
                });
            }
        }

        public int Filterlastdays
        {
            get { return filterlastdays; }
            set
            {
                filterlastdays = value;
                RaisePropertyChanged("Filterlastdays");
                this.orderCollectionView.Refresh();
            }
        }

        public string FilterTxt
        {
            get { return filterTxt; }
            set
            {
                filterTxt = value;
                RaisePropertyChanged("Filtertxt");
                this.orderCollectionView.Refresh();
            }
        }

        public string SelectedItemHints
        {
            get { return "选中{0}单".FormatAs(this.orderList.Count(x => x.IsSelected)); }
        }

        public string CurrentViewSelectedItemHints
        {
            get { return "当前{0}单".FormatAs(this.orderList.Count(x => x.IsShown)); }
        }

        public string CurrentViewSelectedStatistic
        {
            get { return GetSelectedStatistic(); }
        }

        public string GetSelectedStatistic()
        {
            Dictionary<uint, int> dict = new Dictionary<uint, int>();

            var list = this.orderList.Where(x => x.IsSelected).ToList();

            foreach (var item in list)
            {
                if (dict.ContainsKey(item.Order.PacketWeight))
                {
                    dict[item.Order.PacketWeight]++;
                }
                else
                {
                    dict.Add(item.Order.PacketWeight, 1);
                }
            }

            List<string> templist = new List<string>();

            foreach (var dictItem in dict)
            {
                templist.Add(string.Format("{0}kg×{1}", dictItem.Key, dictItem.Value));
            }

            string dynanicName = string.Join(",", templist);

            return "["+dynanicName+"]";
        }


        public bool IsShowConfirmDialog
        {
            get { return isShowConfirmDialog; }
            set { isShowConfirmDialog = value; RaisePropertyChanged("IsShowConfirmDialog"); }
        }

        public bool IsShowOnlySelected
        {
            get { return isShowOnlySelected; }
            set
            {
                isShowOnlySelected = value; RaisePropertyChanged("IsShowOnlySelected");
                this.orderCollectionView.Refresh();
            }
        }

        private void StartSearch()
        {
            var item = this.orderList.Where(x => x.IsSelected).FirstOrDefault();

            if (item == null || item.Order.DhlSn.IsNullOrEmpty()) return;
            
            string dhlCode; 
            string bpostCode;
            string emsCode;

            this.SearchManager.SeperateCode(item.Order.DhlSn,out dhlCode,out bpostCode,out emsCode);
         
            if(emsCode.IsNullOrEmpty())
            {
                emsCode = this.SearchManager.GetEMSCodeByBpostCode(bpostCode);

                if (!emsCode.IsNullOrEmpty()) item.Order.DhlSn += ";" + emsCode;

                this.orderService.Save(item.Order);
            }
            

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "daigou.waybill.query.exe",
                    Arguments = string.Format("{0} {1} {2} {3}",item.Recipient.ID, dhlCode, bpostCode, emsCode),
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };
            process.Start();


        }

        public bool IsTimeFilterEnabled
        {
            get { return isTimeFilterEnabled; }
            set
            {
                isTimeFilterEnabled = value;
                RaisePropertyChanged("IsTimeFilterEnabled");
                this.orderCollectionView.Refresh();
            }
        }

        public DelegateCommand LoadOrderListCommand
        {
            get { return loadOrderListCommand; }
        }

        public DelegateCommand TestCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {

                    });
            }
        }

        public DelegateCommand SendPacketDeliveredEmailCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        SendPacketDeliveredEmail();
                    });
            }
        }

        private void SendPacketDeliveredEmail()
        {

            if (MessageBox.Show("确定要发?", "确定吗", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.No) return;


            this.IsUIEnable = false;
            var list = this.orderList.Where(x => x.IsSelected)
                                  .Select(x => new DhlWaybillParam()
                                  {
                                      Order = x.Order,
                                      Recipient = x.Recipient
                                  }).ToList();

            foreach (var item in list)
            {
                var array = item.Order.DhlSn.Split(';');

                string dhl = (array.Length >= 1) ? array[0] : "";
                string bpost = (array.Length >= 2) ? array[1] : "";
                string ems = (array.Length >= 3) ? array[2] : "";
                string emsQuery = "";                
                if (ems.IsNullOrEmpty())
                {
                    emsQuery = this.SearchManager.GetEMSCodeByBpostCode(bpost);
                }

                if (ems.IsNullOrEmpty() && !emsQuery.IsNullOrEmpty())
                {
                    item.Order.DhlSn += ";" + emsQuery;
                    this.orderService.Save(item.Order);
                }
                SetOrderSent(item.Order);
            }

            this.PacketDeliveredReportService.ReportPacketSentByEmail(list,this.EmailAdditionalInfo);
            this.IsUIEnable = true;
            WinAPI.MessageBeep(WinAPI.BeepType.SimpleBeep);
        }

        private void SetOrderSent(domain.Order order)
        {
            if (!order.OrderStatusTags.Contains("sent"))
            {
                order.OrderStatusTags += "sent;";
            }
        }


        public ICollectionView OrderCollectionView
        {
            get { return orderCollectionView; }
            set
            {
                orderCollectionView = value;
                RaisePropertyChanged("OrderCollectionView");
            }
        }


        public DelegateCommand PrintSelectedCnLabelCommand
        {
            get { return printSelectedCnLabelCommand; }
        }

        public DelegateCommand PrintWaybillsCommand
        {
            get { return printWaybillsCommand; }
        }
        public DelegateCommand<string> ProcessWaybillCommand
        {
            get { return processWaybillCommand; }
        }

        public DelegateCommand SaveOrderCommand
        {
            get { return saveOrderCommand; }
        }

        public DelegateCommand SaveSelectedOrderCommand
        {
            get { return saveSelectedOrderCommand; }
        }

        public DelegateCommand SelectAllCommand
        {
            get { return selectAllCommand; }
        }

        public ProcessModeInfo SelectedDHLAgent
        {
            get { return _SelectedDHLAgent; }
            set 
            { 
                _SelectedDHLAgent = value; 
                RaisePropertyChanged("SelectedDHLAgent");
                RaisePropertyChanged("PacketDevelivedBtnText");
            }
        }

        public DelegateCommand ShowConfirmDialogCommand
        {
            get { return showConfirmDialogCommand; }
        }

        public void CombinationPrint()
        {
            this.eventAggregator.GetEvent<OnOrderCombinePrintClicked>().Publish(new OnOrderCombinePrintClickedPayLoad ());
            Uri uri = new Uri(ViewNames.OrderPrintView, UriKind.Relative);
            this.manager.RequestNavigate(RegionNames.MainContentRegion, uri);
        }

        public void OnRecipientNewOrderAdded(RecipientNewOrderAddedEventPayload payload)
        {
            var recipient = this.recipientService.GetRecipient(payload.RecipientID);
            var orderItem = this.orderService.CreateOrder(recipient.ID);
            uint kg = 7;
            string detail = payload.DeclarationInfo;

            try
            {
                string detailRaw = payload.DeclarationInfo;
                var detailRawArray = detailRaw.ToLines();
                var firstLine = detailRawArray.First().ToLower();
                bool isFirstLineContainWeightInfo = firstLine.Contains("kg");
                if (isFirstLineContainWeightInfo)
                {
                    kg = (uint)firstLine.Replace("kg", "").StrToInt();
                    detail = string.Join(Environment.NewLine, detailRawArray.Skip(1));
                }
            }
            catch { }

            orderItem.Content = payload.OrderInfo;
            orderItem.Detail = detail;
            orderItem.PacketWeight = kg;


            var orderVm = new OrderViewModel(orderItem, recipient,this.orderService);
            orderVm.PropertyChanged += new PropertyChangedEventHandler(orderVm_PropertyChanged);
            this.orderList.Add(orderVm);
            this.orderService.Add(orderVm.Order);
            this.orderCollectionView.Refresh();

        }

        void orderVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                RaisePropertyChanged("SelectedItemHints");
                RaisePropertyChanged("CurrentViewSelectedItemHints");
                RaisePropertyChanged("CurrentViewSelectedStatistic");
                
            }
        }

        public void SelectAll()
        {
            foreach (var item in this.orderCollectionView)
            {
                OrderViewModel orderVm = item as OrderViewModel;
                orderVm.IsSelected = true;
            }
        }

        private void BringToday()
        {
            var list = this.orderList.Where(x => x.IsSelected);

            foreach (var item in list)
            {
                item.Order.CreatedTime = DateTime.Today;
                this.orderService.Save(item.Order);
            }

        }

        private void BringToYesterday()
        {

            var list = this.orderList.Where(x => x.IsSelected);

            foreach (var item in list)
            {
                item.Order.CreatedTime = DateTime.Today.AddDays(-1);
                this.orderService.Save(item.Order);
            }
        }

        private void CleanFilterTxt()
        {
            this.FilterTxt = string.Empty;
        }

        private void CreateDLog()
        { 
            string dlog = Path.Combine(this.directoryService.BaseDir,"d.log");

            if(File.Exists(dlog))File.Delete(dlog);
            FileInfo fileinfo = new FileInfo(dlog);

            var list = this.orderList.Where(x => x.IsSelected).ToList();

            StringBuilder builder = new StringBuilder ();

            foreach (var item in list)
	        {
                builder.AppendLine(item.Recipient.Name);
                if (item.Order.Detail.Trim().IsNullOrEmpty())
                    builder.AppendLine("milk 6 4.8 17");
                else
                    builder.AppendLine(item.Order.Detail);
                builder.AppendLine("");
	        }


            fileinfo.Append(builder.ToString(), Encoding.UTF8);
        }

        private void DeleteOrder()
        {
            var current = this.orderCollectionView.CurrentItem as OrderViewModel;
            if (current == null) return;

            this.orderList.Remove(current);
            if (current.Order.ID != -1)
                this.orderService.Delete(current.Order.ID);
        }

        private void DeSelectAll()
        {
            foreach (var item in this.orderCollectionView)
            {
                OrderViewModel orderVm = item as OrderViewModel;
                orderVm.IsSelected = false;
            }
        }

        private void LoadOrderList()
        {
            this.orderList.Clear();
            var ret = orderService.GetAllOrders().OrderByDescending(x => x.CreatedTime);
            foreach (var item in ret)
            {
                var recipient = this.recipientService.GetRecipient(item.RecipientId);
                var tempOrder = new OrderViewModel(item, recipient, this.orderService);
                tempOrder.PropertyChanged += new PropertyChangedEventHandler(orderVm_PropertyChanged);
                this.orderList.Add(tempOrder);
            }


            this.OrderCollectionView = CollectionViewSource.GetDefaultView(this.orderList);
            this.orderCollectionView.Filter = OrdersFilter;
        }



        private bool OrdersFilter(object target)
        {
            OrderViewModel orderVm = target as OrderViewModel;
            bool textFilter = orderVm.Recipient.Name.Contains(this.FilterTxt) ||
                   orderVm.Recipient.CnAddress.Contains(this.FilterTxt) ||
                   orderVm.Recipient.MainTel.Contains(this.FilterTxt) ||
                   orderVm.Recipient.PostCode.Contains(this.FilterTxt) ||
                   orderVm.Recipient.Name.GetPinyinInitials().Contains(this.FilterTxt.ToUpper())
                   ;

            bool timeFilter = IsTimeFilterEnabled ? (orderVm.Order.CreatedTime.AddDays(this.filterlastdays).Date == DateTime.Today) : true;

            bool isShowSelected = !IsShowOnlySelected || orderVm.IsSelected;

            orderVm.IsShown = textFilter && timeFilter && isShowSelected;
            RaisePropertyChanged("CurrentViewSelectedItemHints");
            return orderVm.IsShown;
        }

        private void PrintSelectedCnLabel()
        {
            IsUIEnable = false;
            var list = this.orderList.Where(x => x.IsSelected).ToList();
            string baseDir = this.directoryService.GetOrCreateCnLabelDir();

            foreach (var item in list)
            {
                domain.Base.CnRecipientInfo info = new domain.Base.CnRecipientInfo();
                info.Address = item.Recipient.CnAddress;

                info.Name = item.Recipient.Name;
                info.PostCode = item.Recipient.PostCode;
                info.ProviceCity = item.Recipient.ProviceCity;
                info.Remark = item.Order.Detail;
                info.TelList = new List<string>() { item.Recipient.MainTel, item.Recipient.OtherTels };

                this.cnRecipientLabelBuilder.BuildeDocument(info, (int)item.Order.PacketWeight, baseDir, true);
            }

            IsUIEnable = true;
        }

        private void PrintWaybill()
        {

            if (!File.Exists(this.directoryService.DetailsLogFilePath))
            {
                CreateDLog();
            }

            IsUIEnable = false;

            try
            {
                List<PrintWayBillParam> list = this.orderList.Where(x => x.IsSelected).Select(x =>
                    new PrintWayBillParam()
                    {
                        Recipient = x.Recipient,
                        Order = x.Order,
                        NameIndex = x.NameIndex
                    }).ToList();

                this.dhlWaybillService.PrintProcessWaybill(list, this.SelectedDHLAgent.Name);

                list.ForEach(x => this.orderService.Save(x.Order));

                try
                {
                    File.Delete(this.directoryService.DetailsLogFilePath);
                }
                catch { }
            }
            catch
            {
                MessageBox.Show("出现错误","出错了", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
            }

            IsUIEnable = true;

            WinAPI.MessageBeep(WinAPI.BeepType.SimpleBeep);
        }

        private void ProcessWaybill(string enableSendEmail)
        {
            IsUIEnable = false;

            var list = this.orderList.Where(x => x.IsSelected)
                                     .Select(x => new DhlWaybillParam()
                                     {
                                         Order = x.Order,
                                         Recipient = x.Recipient
                                     }).ToList();

            foreach (var item in list)
            {
                if (!item.Order.OrderStatusTags.Contains("applied"))
                {
                    item.Order.OrderStatusTags += "applied";
                }
            }

            do
            {
                try
                {
                    this.dhlWaybillService.Process(list, this.SelectedDHLAgent, enableSendEmail, EmailAdditionalInfo);
                    break;   
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.GetDetailErrorText());
                    if (MessageBox.Show("发送失败,是否重试?", "重试?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        continue;
                    }
                }

                break;
            } while (true);

            IsUIEnable = true;

            WinAPI.MessageBeep(WinAPI.BeepType.SimpleBeep);
            MessageBox.Show("包裹单已发送","完成", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        private void SaveOrder()
        {
            var current = this.orderCollectionView.CurrentItem as OrderViewModel;
            if (current == null) return;

            if (current.Order.ID == -1)
                this.orderService.Add(current.Order);
            else
                this.orderService.Save(current.Order);

        }

        private void SaveSelectedOrder()
        {
            var list = this.orderList.Where(x => x.IsSelected).ToList();

            foreach (var item in list)
            {
                if (item.Order.ID == -1)
                    this.orderService.Add(item.Order);
                else
                    this.orderService.Save(item.Order);
            }


        }
    }
}
