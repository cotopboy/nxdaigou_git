using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.services.Interfaces
{
    public interface IPacketDeliveredReportService
    {
        ReportResult ReportPacketSentByEmail(DhlWaybillParam param, string additionInfo = "");
    }
}
