using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace daigou.infrastructure.Events
{
    public class RecipientNewOrderAddedEvent:
         CompositePresentationEvent<RecipientNewOrderAddedEventPayload>
    {
        
    }

    public class RecipientNewOrderAddedEventPayload
    {
        private int recipientID = -1;
        private string declarationInfo = string.Empty;
        private string orderInfo = string.Empty;

        

        public RecipientNewOrderAddedEventPayload(
            int recipientID,
            string declarationInfo,
            string orderInfo)
        {
            this.recipientID = recipientID;
            this.declarationInfo = declarationInfo;
            this.orderInfo = orderInfo;
        }

        public string DeclarationInfo
        {
            get { return declarationInfo; }
            set { declarationInfo = value; }
        }

        public string OrderInfo
        {
            get { return orderInfo; }
            set { orderInfo = value; }
        }

        public int RecipientID
        {
            get { return recipientID; }
            set { recipientID = value; }
        }

    }
}
