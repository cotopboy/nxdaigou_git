using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace daigou.domain
{
    [Serializable]
    public abstract class DomainObject : INotifyPropertyChanged
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainObject"/> class.
        /// </summary>
        protected DomainObject()
        {
        }
        
        [NonSerialized]
        private PropertyChangedEventHandler _changed;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _changed += value; }
            remove { _changed -= value; }
        }


        protected void RaisePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this._changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        
    }
}
