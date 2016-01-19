using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.modules.Order
{
    public class OrderViewModel : NotificationObject
    {
        private domain.Order order;
        private domain.Recipient recipient;
        private bool isSelected = false;
        private bool hasWaybillSn = false;
        

        public bool HasWaybillSn
        {
            get { return this.Order.DhlSn.IsNullOrEmpty(); }
            set 
            {
                hasWaybillSn = value; RaisePropertyChanged("HasWaybillSn"); 
            }
        }

        public bool IsOrderWaitToBeSent
        {
            get { return !this.Order.OrderStatusTags.Contains("sent");}
        }

        public bool IsOrderBillwayApplyEmailSent
        {
            get { return !this.Order.OrderStatusTags.Contains("applied"); }
        }

        public string OrderStatusTags
        {
            get { return this.Order.OrderStatusTags; }
            set
            {
                this.Order.OrderStatusTags = value;
                RaisePropertyChanged("OrderStatusTags"); 
            }
        }
 
        private OrderService orderService;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged("IsSelected"); }
        }

        private bool isShown = false;

        public bool IsShown
        {
            get { return isShown; }
            set
            {
                isShown = value; RaisePropertyChanged("IsShown");
            }
        }

        private uint nameIndex = 1;

        public uint NameIndex
        {
            get { return nameIndex; }
            set { nameIndex = value; RaisePropertyChanged("NameIndex"); }
        }



        public DelegateCommand SaveChangeCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        this.orderService.Save(this.Order);
                    });
            }
        }

        public DelegateCommand ItemDoubleClickCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        this.IsSelected = !this.IsSelected;
                    });
            }
        }

        public DelegateCommand Add8XCommand
        {
            get
            {
                return new DelegateCommand(() =>
                    {
                        this.order.Detail = "milk.powder 8 0.6 10";
                    });
            }
        }

        public DelegateCommand Add6XCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.order.Detail = "milk.powder 6 0.8 12";
                });
            }
        }

        public domain.Recipient Recipient
        {
            get { return recipient; }
            set 
            {
                recipient = value;
                RaisePropertyChanged("Recipient");
            }
        }

        public string RecipientPinyinName
        {
            get { return this.Recipient.Name.NameToPinyin(); }
        }

        public domain.Order Order
        {
            get { return order; }
            set { order = value; RaisePropertyChanged("Order"); }
        }


        public OrderViewModel(domain.Order order, domain.Recipient recipient, OrderService orderService)
        {
            this.order = order;
            this.order.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnOrder_PropertyChanged);
            this.orderService = orderService;

            this.recipient = recipient;
        }

        private void OnOrder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DhlSn")
            {
                this.HasWaybillSn = !this.Order.DhlSn.IsNullOrEmpty();
            }
            if (e.PropertyName == "OrderStatusTags")
            {
                RaisePropertyChanged("OrderStatusTags");
                RaisePropertyChanged("IsOrderWaitToBeSent");
                RaisePropertyChanged("IsOrderBillwayApplyEmailSent");
            }
        }

    }
}
