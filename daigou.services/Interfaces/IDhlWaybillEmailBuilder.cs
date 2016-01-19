using System;
namespace daigou.services
{
    public interface IDhlWaybillEmailBuilder
    {
        string GetEmailBody(System.Collections.Generic.List<DhlWaybillParam> dhlWaybillParamList,string taoBaoOrderSn = "");
        string GetEmailHeader(System.Collections.Generic.List<DhlWaybillParam> dhlWaybillParamList, string taoBaoOrderSn = "");
        string ReceiverEmail { get; }
    }
}
