using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using daigou.domain;

namespace daigou.services
{
    public class WaybillSetting
    {
        private string _CcEmail = "";
        private string _CxEmail = "";
        private string _SenderCity = "";
        private string _SenderCountry = "";
        private string _SenderHouseNumber = "";
        private string _SenderName = "";
        private string _SenderPostCode = "";
        private string _SenderStreet = "";
        private string _SenderTel = "";
        private string _WanwanName = "";
        private string _WaybillEmail = "";


        public WaybillSetting()
        {

        }


        public string CxEmail
        {
            get { return _CxEmail; }
            set { _CxEmail = value; }
        }

        public string SenderCity
        {
            get { return _SenderCity; }
            set { _SenderCity = value; }
        }

        public string SenderCountry
        {
            get { return _SenderCountry; }
            set { _SenderCountry = value; }
        }

        public string SenderHouseNumber
        {
            get { return _SenderHouseNumber; }
            set { _SenderHouseNumber = value; }
        }

        public string SenderName
        {
            get { return _SenderName; }
            set { _SenderName = value; }
        }

        public string SenderPostCode
        {
            get { return _SenderPostCode; }
            set { _SenderPostCode = value; }
        }

        public string SenderStreet
        {
            get { return _SenderStreet; }
            set { _SenderStreet = value; }
        }

        public string SenderTel
        {
            get { return _SenderTel; }
            set { _SenderTel = value; }
        }

        public string WanwanName
        {
            get { return _WanwanName; }
            set { _WanwanName = value; }
        }
        public string WaybillEmail
        {
            get { return _WaybillEmail; }
            set { _WaybillEmail = value; }
        }

        public string ZdEmail { get; set; }

        public string CcEmail { get; set; }

    }
}
