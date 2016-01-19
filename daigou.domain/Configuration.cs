using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    [Serializable]
    public class Configuration : DomainObject
    {
        private int _ID = -1;

        private string _Key = "";

        private string _Value = "";

        public string Value
        {
            get { return _Value; }
            set
            { _Value = value;
            RaisePropertyChanged("Value");
            }
        }

        public string Key
        {
            get { return _Key; }
            set { _Key = value; RaisePropertyChanged("Key"); }
        }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; RaisePropertyChanged("ID"); }
        }



    }
}
