using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace daigou.infrastructure.Events
{
    public class SelectedItemsToCurrentBillEvent : CompositePresentationEvent<SelectedItemsToCurrentBillPayLoad>
    {

    }

    public class SelectedItemsToCurrentBillPayLoad
    {
        private List<string> selectedItemKeyList;

        public List<string> SelectedItemKeyList
        {
            get { return selectedItemKeyList; }
            set { selectedItemKeyList = value; }
        }
    }


    public class SelectedItemsToNewBillEvent : CompositePresentationEvent<SelectedItemsToNewBillPayLoad>
    {

    }

    public class SelectedItemsToNewBillPayLoad
    {
        private List<string> selectedItemKeyList;

        public List<string> SelectedItemKeyList
        {
            get { return selectedItemKeyList; }
            set { selectedItemKeyList = value; }
        }

    }




}
