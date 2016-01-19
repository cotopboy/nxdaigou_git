using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using daigou.domain.Base;


namespace daigou.infrastructure.Events
{
    public class ParseRawTextRecipientFinishedEvent 
        : CompositePresentationEvent<ParseRawTextRecipientFinishedPayload>
    {

    }

    public class ParseRawTextRecipientFinishedPayload
    {
        private List<CnRecipientInfo> cnRecipientInfoList = new List<CnRecipientInfo>();

        public List<CnRecipientInfo> CnRecipientInfoList
        {
            get { return cnRecipientInfoList; }
        }

        public ParseRawTextRecipientFinishedPayload(List<CnRecipientInfo> cnRecipientInfoList)
        {
            this.cnRecipientInfoList = cnRecipientInfoList;


        }
    }
}
