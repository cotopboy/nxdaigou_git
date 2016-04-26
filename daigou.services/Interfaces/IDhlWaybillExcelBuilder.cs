using System;
namespace daigou.services
{
    public interface IDhlWaybillExcelBuilder
    {
        string Generate(string targetDir, System.Collections.Generic.List<DhlWaybillParam> dhlWaybillParamList,string taobaoOrderSn = "");
        event Action<float> OnReportStep;
    }
}
