using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using daigou.domain;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using daigou.infrastructure.Events;
using daigou.infrastructure.ExtensionMethods;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using System.ComponentModel;
using System.Windows.Data;
using daigou.services;
using daigou.domain.Base;
using System.Xml.Serialization;
using System.IO;

namespace daigou.modules.Recipient
{
    public class RecipientViewModel : NotificationObject
    {
        private readonly DelegateCommand addToNewOrderCommand;
        private readonly DelegateCommand cleanFilterTxtCommand;
        private readonly DelegateCommand loadRecipientListCommand;
        private readonly DelegateCommand printCNAddressLabelCommand;
        private readonly DelegateCommand copyRecipientInfoCommand;
        private readonly DelegateCommand backupRecipientCommand;
        private readonly DelegateCommand addPhotoCommand;

        
        public DelegateCommand AddPhotoCommand
        {
            get { return addPhotoCommand; }
        }

        private readonly DelegateCommand addStampCommand;

        public DelegateCommand AddStampCommand
        {
            get { return addStampCommand; }
        }

        public DelegateCommand CleanCommand
        {
            get 
            {
                return new DelegateCommand(() => {
                    this.DeclarationInfo = string.Empty;
                });
            }
        }
     

        public DelegateCommand<string> AddOrderWeightCommand
        {
            get { return new DelegateCommand<string>((x) => 
                    {
                        var lines = this.DeclarationInfo.ToLines();
                        lines[0] = x;
                        this.DeclarationInfo = string.Join(Environment.NewLine, lines);
                    }); 
            }

        }

        public DelegateCommand<string> AddOrderDetailsCommand
        {
            get { return new DelegateCommand<string>((x) => 
            {
                var lines = this.DeclarationInfo.Trim().ToLines().ToList();

                if (x.StartWithAnyOf(new List<string>() { "milk.powder" }) && lines.Count >1)
                {
                    lines[1]=x;
                }
                else
                {
                    if (lines.Count >= 4) return;
                    lines.Add(x);
                }

                this.DeclarationInfo = string.Join(Environment.NewLine, lines);
            }); }
        }


        private readonly DelegateCommand<int?> removeRecipientCommand;
        private readonly DelegateCommand<RecipientItemViewModel> saveRecipientChangeCommand;
        private string cnLabelKgValue = "7";
        private CnRecipientLabelBuilder cnLabelPrinter;
        private IEventAggregator eventAggregator;
        private string filterTxt = string.Empty;
        private DirectoryService directoryService;
        private ICollectionView recipientCollectionView = null;
        private ObservableCollection<RecipientItemViewModel> recipientList = new ObservableCollection<RecipientItemViewModel>();

        private RecipientService recipientService;
        private string remarkTxt = string.Empty;
        private RecipientItemViewModel selectedItem;
        private AgentListService agentListSvc;

        private string declarationInfo = string.Empty;
        private string orderInfo = string.Empty;

        private ObservableCollection<OrderExampleItem> orderExampleList = new ObservableCollection<OrderExampleItem>() 
        {
            new OrderExampleItem( "其它*","blue"),
            new OrderExampleItem( "Ap 爱他美 Pre 段 X 6","#7700BFFF"),
            new OrderExampleItem( "Ap 爱他美 1   段 X 6","#7700BFFF"),
            new OrderExampleItem( "Ap 爱他美 2   段 X 6","#7700BFFF"),
            new OrderExampleItem( "Ap 爱他美 3   段 X 6","#7700BFFF"),
            new OrderExampleItem( "Ap 爱他美 1+  段 X 8","#7700BFFF"),
            new OrderExampleItem( "Ap 爱他美 2+  段 X 8","#7700BFFF"),
            new OrderExampleItem( "Combi益生菌 Pre 段 X 8","#77ADFF2F"),
            new OrderExampleItem( "Combi益生菌 1   段 X 8","#77ADFF2F"),
            new OrderExampleItem( "Combi益生菌 2   段 X 8","#77ADFF2F"),
            new OrderExampleItem( "Combi益生菌 3   段 X 8","#77ADFF2F"),
            new OrderExampleItem( "Combi益生菌 1+  段 X 8","#77ADFF2F"),
            new OrderExampleItem( "Combi益生菌 2+  段 X 8","#77ADFF2F"),
            new OrderExampleItem( "Bio有机 Pre 段 X 8","#77008000"),
            new OrderExampleItem( "Bio有机 1   段 X 8","#77008000"),
            new OrderExampleItem( "Bio有机 2   段 X 6","#77008000"),
            new OrderExampleItem( "Bio有机 3   段 X 6","#77008000"),
            new OrderExampleItem( "Bio有机 1+  段 X 6","#77008000")                     
        };

        public ObservableCollection<OrderExampleItem> OrderExampleList
        {
            get { return orderExampleList; }
            set { orderExampleList = value; RaisePropertyChanged("OrderExampleList"); }
        }

        private string selectedOrderInfo = string.Empty;

        public string SelectedOrderInfo
        {
            get { return selectedOrderInfo; }
            set { selectedOrderInfo = value; RaisePropertyChanged("SelectedOrderInfo"); }
        }

        public RecipientViewModel(
            RecipientService recipientService,
            ConfigurationService configurationService,
            IEventAggregator eventAggregator,
            DirectoryService directoryService,
            CnRecipientLabelBuilder cnLabelPrinter,
            AgentListService agentListSvc)
        {
            this.recipientService = recipientService;
            this.agentListSvc = agentListSvc;
            this.loadRecipientListCommand = new DelegateCommand(LoadRecipientList);

            this.removeRecipientCommand = new DelegateCommand<int?>(RemoveRecipient, CanExecuteRemoveRecipient);
            this.saveRecipientChangeCommand = new DelegateCommand<RecipientItemViewModel>(SaveRecipient, CanExeciteSaveRecipient);
            this.copyRecipientInfoCommand = new DelegateCommand(CopyRecipientInfo);

            this.cleanFilterTxtCommand = new DelegateCommand(() => this.FilterTxt = string.Empty);
            this.printCNAddressLabelCommand = new DelegateCommand(PrintCNAddressLabel, CanExecutePrintCNAddressLabel);
            this.addToNewOrderCommand = new DelegateCommand(AddToNewOrder, CanAddToNewOrder);
            this.addPhotoCommand = new DelegateCommand(AddPhotoFlag);
            this.addStampCommand = new DelegateCommand(AddStampFlat);

            this.eventAggregator = eventAggregator;
            this.cnLabelPrinter = cnLabelPrinter;
            this.directoryService = directoryService;

            this.PropertyChanged += new PropertyChangedEventHandler(RecipientViewModel_PropertyChanged);

            this.eventAggregator.GetEvent<ParseRawTextRecipientFinishedEvent>().Subscribe
                (OnParseRawTextRecipientFinished, ThreadOption.UIThread);
        }

        private void RecipientViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedAgent")
            {
                if (this.SelectedItem != null)
                {
                    this.SelectedItem.QQNumber = this.selectedAgent.QQNumberOrEmail;
                    this.SelectedItem.AgentName = this.selectedAgent.Name;
                }
            }

            if (e.PropertyName == "SelectedItem")
            {
                if (this.selectedItem != null)
                {
                    var ret =this.AgentList.FirstOrDefault(x => x.QQNumberOrEmail == this.selectedItem.QQNumber);
                    if (ret != null)
                    {
                        this.SelectedAgent = ret;
                    }
                }
            }
        }

        public ObservableCollection<Agent> AgentList
        {
            get { return this.agentListSvc.AgentList; }
        }

        private Agent selectedAgent;
        public Agent SelectedAgent 
        {
            get { return selectedAgent; }
            set { this.selectedAgent = value; RaisePropertyChanged("SelectedAgent"); }
        }
        
        public DelegateCommand CopyRecipientInfoCommand
        {
            get { return copyRecipientInfoCommand; }
        } 

        public DelegateCommand AddToNewOrderCommand
        {
            get { return addToNewOrderCommand; }
        }

        public DelegateCommand CleanFilterTxtCommand
        {
            get { return cleanFilterTxtCommand; }
        }

        //public string PinyingName
        //{
        //    get 
        //    {
        //        if(this.sel
        //    }
        //}

        public string CnLabelKgValue
        {
            get { return cnLabelKgValue; }
            set { cnLabelKgValue = value; RaisePropertyChanged("CnLabelKgValue"); }
        }

        public string FilterTxt
        {
            get { return filterTxt; }
            set 
            {
                filterTxt = value;
                RaisePropertyChanged("Filtertxt");
                this.recipientCollectionView.Refresh();
            }
        }

        private string errorMsg;
        public string ErrorMsg
        {
            get { return errorMsg; }

            set 
            {
                this.errorMsg = value;
                RaisePropertyChanged("ErrorMsg");
            }
        }


        public ICommand LoadRecipientListCommand
        {
            get { return this.loadRecipientListCommand; }
        }

        public string DeclarationInfo
        {
            get { return declarationInfo; }
            set { declarationInfo = value; RaisePropertyChanged("DeclarationInfo"); }
        }

        public string OrderInfo
        {
            get { return orderInfo; }
            set { orderInfo = value; RaisePropertyChanged("OrderInfo"); }
        }

        public DelegateCommand PrintCNAddressLabelCommand
        {
            get { return printCNAddressLabelCommand; }
        }

        public ICollectionView RecipientCollectionView
        {
            get { return recipientCollectionView; }
            set
            {
                this.recipientCollectionView = value;
                RaisePropertyChanged("RecipientCollectionView");
            }
        }

        public ObservableCollection<RecipientItemViewModel> RecipientList
        {
            get { return this.recipientList; }
            set
            {
                this.recipientList = value;
                RaisePropertyChanged("RecipientList");
            }
        }

        public string RemarkTxt
        {
            get { return remarkTxt; }
            set 
            {
                remarkTxt = value;
                RaisePropertyChanged("RemarkTxt");
            }
        }
        public DelegateCommand<int?> RemoveRecipientCommand
        {
            get { return removeRecipientCommand; }
        }

        public DelegateCommand<RecipientItemViewModel> SaveRecipientChangeCommand
        {
            get { return saveRecipientChangeCommand; }
        }

        public RecipientItemViewModel SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                if (this.selectedItem == value) return;

                this.selectedItem = value;
                RaisePropertyChanged("SelectedItem");
                this.removeRecipientCommand.RaiseCanExecuteChanged();
                this.saveRecipientChangeCommand.RaiseCanExecuteChanged();
                this.printCNAddressLabelCommand.RaiseCanExecuteChanged();
                this.addToNewOrderCommand.RaiseCanExecuteChanged();
            }
        }

        private void SetAsSuCustomer()
        {
            this.SelectedItem.AgentName = "susu";
            this.selectedItem.QQNumber = "16550587";

        }

        private void AddToNewOrder()
        {
            string fullOrderInfo = this.OrderInfo.Insert(0,this.SelectedOrderInfo + Environment.NewLine);
            var payload = new RecipientNewOrderAddedEventPayload(this.SelectedItem.ID, this.DeclarationInfo, fullOrderInfo);
            eventAggregator.GetEvent<RecipientNewOrderAddedEvent>().Publish(payload);
        }

        private void AddStampFlat()
        {
            this.OrderInfo += "[印章]";
        
        }

        private void AddPhotoFlag()
        {
            this.OrderInfo += "[照片]";
        
        }



        private bool CanAddToNewOrder()
        {
            return (this.SelectedItem != null);
        }

        private bool CanExeciteSaveRecipient(RecipientItemViewModel target)
        {
            return this.selectedItem != null;
        }

        private bool CanExecutePrintCNAddressLabel()
        {
            return (this.SelectedItem != null);
        }

        private bool CanExecuteRemoveRecipient(int? targetId)
        {

            return this.selectedItem != null;
        }

       

        private void LoadRecipientList()
        {
            this.recipientList.Clear();
            var ret = recipientService.GetAllRecipient();
            foreach (var item in ret)
            {
                this.recipientList.Add(new RecipientItemViewModel(item));
            }

            this.RecipientCollectionView = CollectionViewSource.GetDefaultView(this.recipientList);
            this.RecipientCollectionView.Filter = RecipientFilter;
            this.removeRecipientCommand.RaiseCanExecuteChanged();

        }

        private void OnParseRawTextRecipientFinished(ParseRawTextRecipientFinishedPayload payload)
        {
            foreach (var item in payload.CnRecipientInfoList)
            {
                bool isDuplicateName = this.recipientList.Any(x => x.Name == item.Name);


                domain.Recipient newObject = new domain.Recipient();
                newObject.ID = -1;
                newObject.Name = item.Name +( isDuplicateName ? "[有重复]" :string.Empty);
                newObject.CnAddress = item.Address;
                newObject.MainTel = item.TelList.FirstOrDefault();
                newObject.OtherTels = string.Join(",", item.TelList.Skip(1));
                newObject.PostCode = item.PostCode;
                newObject.ProviceCity = item.ProviceCity;
                var newVm = new RecipientItemViewModel(newObject);
                this.recipientList.Add(newVm);
                this.SelectedItem = newVm;
            }
        }
        private void PrintCNAddressLabel()
        {
            if (this.SelectedItem == null) return;

            var info = new CnRecipientInfo()
            {
                Name = this.SelectedItem.Name,
                Address = this.SelectedItem.CnAddress,
                PostCode = this.SelectedItem.PostCode,
                Remark = this.RemarkTxt,
                TelList = new List<string>() { this.SelectedItem.MainTel, this.SelectedItem.OtherTels },
            };
            this.cnLabelPrinter.BuildeDocument(info,
                this.CnLabelKgValue.StrToInt(),
                this.directoryService.GetOrCreateCnLabelDir(),true);


        }

        private bool RecipientFilter(object targetObj)
        {
            RecipientItemViewModel recipient = targetObj as RecipientItemViewModel;

            var filterItems = this.FilterTxt.Split(new string[] { ";", ",", "，" }, StringSplitOptions.RemoveEmptyEntries);

            if (filterItems.Length == 0) return true;

            foreach (var item in filterItems)
            {
                bool ret =
                    recipient.Name.GetPinyinInitials().Contains(item.ToUpper()) ||
                    recipient.Name.Contains(item) ||
                    recipient.AgentName.Contains(item) ||
                    recipient.QQNumber.Contains(item) ||
                    recipient.CnAddress.Contains(item);

                if (ret) return true;
            }

            return false;
             
        }

        private void RemoveRecipient(int? targetId)
        {
            if (!targetId.HasValue) return;
            this.recipientService.DeleteRecipient(targetId.Value);
            this.LoadRecipientList();
        }

        private void SaveRecipient(RecipientItemViewModel targetVM)
        {
            var target = targetVM.Model;

            if (target.MainTel.Trim().Length < 11)
            {
                this.ErrorMsg = "电话号码错误";
                return;
            }

            if (target.PostCode.Trim().Length < 6)
            {
                this.ErrorMsg = "邮编错误";
                return;
            }

            if (target.ProviceCity.Trim().Replace(",","").Length < 2)
            {
                this.ErrorMsg = "省市错误";
                return;
            }

            List<string> missCopyWordList = new List<string>() 
            {
                "地图","查看","在","拨号","电话","号码","邮编",":","是否"
            };

            string text = target.ToString() + " " + target.FormatDisplay;

            foreach (var item in missCopyWordList)
            {
                if (text.Contains(item))
                {
                    this.ErrorMsg = "收件复制有误,请检查";
                    return;
                }
            }

            this.ErrorMsg = "";         

            if (target.ID != -1)
                this.recipientService.SaveRecipient(target);
            else
                this.recipientService.AddRecipient(target);

            targetVM.ID = target.ID;
        }

        public void CopyRecipientInfo()
        {
            if (this.SelectedItem == null) return;

            string info = string.Format("{0} {1} {2} {3} {4}",
                                  this.SelectedItem.Name,
                                  this.selectedItem.CnAddress,
                                  this.SelectedItem.PostCode,
                                  this.selectedItem.MainTel,
                                  this.selectedItem.OtherTels
                );
            try
            {
                Clipboard.SetText(info);
            }
            catch { }
        }
    }

    public class OrderExampleItem
    {
        public string Content { get; set; }

        public string Color { get; set; }

        public OrderExampleItem(string content, string color)
        {
            this.Content = content;
            this.Color = color;
        }
    }
}
