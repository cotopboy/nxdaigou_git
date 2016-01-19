using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;


namespace daigou.domain
{
    [Serializable]
    public class Recipient : DomainObject
    {
        private  int    _ID          = 0;
        private  string _Name        = string.Empty;
        private  string _Country     = string.Empty;
        private  string _ProviceCity = string.Empty;
        private  string _District    = string.Empty;
        private  string _CnAddress   = string.Empty;
        private  string _MainTel     = string.Empty;
        private  string _OtherTels   = string.Empty;
        private  string _PostCode    = string.Empty;
        private string _QQNumber     = string.Empty;
        private string _AgentName    = string.Empty;
        private string _Remark = string.Empty;

        public string Remark      { get { return _Remark; } set { _Remark = value; RaisePropertyChanged("Remark"); } }
        public string AgentName   { get { return _AgentName; } set { _AgentName = value; RaisePropertyChanged("AgentName"); } }
        public string QQNumber    { get { return _QQNumber; } set { _QQNumber = value; RaisePropertyChanged("QQNumber"); } }
        public int    ID          { get{return  _ID         ;} set{  _ID         = value; RaisePropertyChanged("ID"          ); RaisePropertyChanged("FormatDisplay"); }}
        public string Name        { get{return  _Name       ;} set{  _Name       = value; RaisePropertyChanged("Name"        ); RaisePropertyChanged("FormatDisplay"); }}
        public string Country     { get{return  _Country    ;} set{  _Country    = value; RaisePropertyChanged("Country"     ); RaisePropertyChanged("FormatDisplay"); }}
        public string ProviceCity { get{return  _ProviceCity;} set{  _ProviceCity= value; RaisePropertyChanged("ProviceCity" ); RaisePropertyChanged("FormatDisplay"); }}
        public string District    { get{return  _District   ;} set{  _District   = value; RaisePropertyChanged("District"    ); RaisePropertyChanged("FormatDisplay"); }}
        public string CnAddress   { get{return  _CnAddress  ;} set{  _CnAddress  = value; RaisePropertyChanged("CnAddress"   ); RaisePropertyChanged("FormatDisplay"); }}       
        public string MainTel     { get{return  _MainTel    ;} set{  _MainTel    = value; RaisePropertyChanged("MainTel"     ); RaisePropertyChanged("FormatDisplay"); }}
        public string OtherTels   { get{return  _OtherTels  ;} set{  _OtherTels  = value; RaisePropertyChanged("OtherTels"   ); RaisePropertyChanged("FormatDisplay"); }}
        public string PostCode    { get{return  _PostCode   ;} set{  _PostCode   = value; RaisePropertyChanged("PostCode"    ); RaisePropertyChanged("FormatDisplay"); }}

        public string FormatDisplay
        {
            get{
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
                this.Name.Trim().PadRight(10,'　'),
                this.ProviceCity.Trim().PadRight(10,'　'), 
                this.MainTel.Trim());
        }
    }
}
