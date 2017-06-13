using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using daigou.infrastructure.ExtensionMethods;

namespace daigou.modules.Recipient
{
    public class RecipientItemViewModel : NotificationObject
    {
        private daigou.domain.Recipient model = null;

        public daigou.domain.Recipient Model
        {
            get { return model; }
            set { model = value; }
        }

        public string PinyinName
        {
            get 
            {
                return this.Name.NameToPinyin();
            }
        }

        public string FormatDisplay
        {
            get
            {
                return string.Format("{0} {1} {2},{3}",
                        this.CnAddress,
                        this.PostCode,
                        this.MainTel,
                        this.OtherTels
                );
            }
        }

        public override string ToString()
        {

            return string.Format("{0} {1} {2}",
                this.Name.Trim().PadRight(10, '　'),
                this.ProviceCity.Trim().PadRight(10, '　'),
                this.MainTel.Trim());
        }

        public RecipientItemViewModel(daigou.domain.Recipient model)
        {
            this.model = model;
        }

        public string Remark
        {
            get { return model.Remark; }
            set
            {
                model.Remark = value;
                RaisePropertyChanged("Remark");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string AgentName
        {
            get { return model.AgentName; }
            set
            {
                model.AgentName = value;
                RaisePropertyChanged("AgentName");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string QQNumber
        {
            get { return model.QQNumber; }
            set
            {
                model.QQNumber = value;
                RaisePropertyChanged("QQNumber");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public int ID
        {
            get { return model.ID; }
            set
            {
                model.ID = value;
                RaisePropertyChanged("ID");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string Name
        {
            get { return model.Name; }
            set
            {
                model.Name = value;
                RaisePropertyChanged("Name");
                RaisePropertyChanged("PinyinName");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string Country
        {
            get { return model.Country; }
            set
            {
                model.Country = value;
                RaisePropertyChanged("Country");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string ProviceCity
        {
            get { return model.ProviceCity; }
            set
            {
                model.ProviceCity = value;
                RaisePropertyChanged("ProviceCity");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string District
        {
            get { return model.District; }
            set
            {
                model.District = value;
                RaisePropertyChanged("District");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string CnAddress
        {
            get { return model.CnAddress; }
            set
            {
                model.CnAddress = value;
                RaisePropertyChanged("CnAddress");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string MainTel
        {
            get { return model.MainTel; }
            set
            {
                model.MainTel = value;
                RaisePropertyChanged("MainTel");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string OtherTels
        {
            get { return model.OtherTels; }
            set
            {
                model.OtherTels = value;
                RaisePropertyChanged("OtherTels");
                RaisePropertyChanged("FormatDisplay");
            }
        }
        public string PostCode
        {
            get { return model.PostCode; }
            set
            {
                model.PostCode = value;
                RaisePropertyChanged("PostCode");
                RaisePropertyChanged("FormatDisplay");
            }
        }

        public string CardId
        {
            get { return model.CardId; }
            set
            {
                model.CardId = value;
                RaisePropertyChanged("CardId");                
            }
        }

    }
}
