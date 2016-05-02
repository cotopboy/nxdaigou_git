using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;

namespace daigou.domain
{
    [Serializable]
    public class Order : DomainObject
    {
        private DateTime _CreatedTime = DateTime.MinValue;

        private int _ID            = 0;
        private uint _PacketWeight = 7;
        private int _RecipientId   = -1;
        private string _Detail     = string.Empty;
        private string _DhlSn      = string.Empty;
        private string _Content    = "atm";
        private string _Remark = "";
        private string _EmailRemark = "";
        private string _OrderStatusTags = "";
        private string _LogisticsType = "";

        private double _ActualWeight = 0.0;

        public double ActualWeight
        {
            get { return _ActualWeight; }
            set { _ActualWeight = value; RaisePropertyChanged("ActualWeight"); }
        }

        public string LogisticsType
        {
            get { return _LogisticsType; }
            set { _LogisticsType = value; RaisePropertyChanged("LogisticsType"); }
        }

        public string OrderStatusTags
        {
            get { return _OrderStatusTags; }
            set { _OrderStatusTags = value; RaisePropertyChanged("OrderStatusTags"); }
        }

        public string EmailRemark
        {
            get { return _EmailRemark; }
            set { _EmailRemark = value; RaisePropertyChanged("EmailRemark"); }
        }

        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; RaisePropertyChanged("Remark"); }
        }



        public DateTime CreatedTime { get { return _CreatedTime; } set  { _CreatedTime = value; RaisePropertyChanged("CreatedTime"); } }
        public string Detail        { get { return _Detail; } set       { _Detail = value; RaisePropertyChanged("Detail"); } }
        public string DhlSn         { get { return _DhlSn; } set        { _DhlSn = value; RaisePropertyChanged("DhlSn"); } }
        public int ID               { get { return _ID; } set           { _ID = value; RaisePropertyChanged("ID"); } }
        public uint PacketWeight    { get { return _PacketWeight; } set { _PacketWeight = value; RaisePropertyChanged("PacketWeight"); } }
        public int RecipientId      { get { return _RecipientId; } set  { _RecipientId = value; RaisePropertyChanged("RecipientId"); } }

        public string Content
        {
            get { return _Content.Replace("\t", "*"); }
            set 
            {
                _Content = value.Replace("\t", "*") ;
                RaisePropertyChanged("Content"); 
            }
        }

        public Order()
        {
        }

    }
}
