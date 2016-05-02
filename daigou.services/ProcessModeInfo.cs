using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.services
{
    public class ProcessModeInfoProvider
    {
        private List<ProcessModeInfo> modeList = new List<ProcessModeInfo>();
        private WaybillSettingFactory waybillSettingFactory = null;

        public List<ProcessModeInfo> ModeList
        {
            get { return modeList; }
            set { modeList = value; }
        }

        public ProcessModeInfoProvider(WaybillSettingFactory waybillSettingFactory)
        {
            this.waybillSettingFactory = waybillSettingFactory;

            var setting = this.waybillSettingFactory.Create();

            modeList.Add(new ProcessModeInfo(
                "中德快递Bpost",
                "包裹代理",
#if DEBUG
                "ruifeng.zhang@cbb.de",
#else
                setting.ZdEmail,
#endif
                "" ,
                setting.WanwanName, ""));

            modeList.Add(new ProcessModeInfo(
               "EMS",
               "包裹代理",
#if DEBUG
                "ruifeng.zhang@cbb.de",
#else
               "dt8ang@qq.com",
#endif
                "",
               setting.WanwanName, ""));        
/*
            modeList.Add(new ProcessModeInfo(
             "散客=>包裹已发",
             "下家客户",
             "",
             "2777888@qq.com",
             "散客",
             ""));*/

        }
    }

    public class ProcessModeInfo
    {
        private string packetExcelEmail;


        private string packetReportEmail;

        private string type;


        private string name;
        private string wanwanName;
        private string emailRemark;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public override string ToString()
        {
            return this.name;
        }

        public ProcessModeInfo(string name,string type ,string packetExcelEmail, string packetReportEmail, string wanwanName, string emailRemark)
        {
            this.type =  type;
            this.packetExcelEmail = packetExcelEmail;
            this.packetReportEmail = packetReportEmail;
            this.name = name;
            this.wanwanName = wanwanName;
            this.emailRemark = emailRemark;
        }



        public string PacketExcelEmail
        {
            get { return packetExcelEmail; }
            set { packetExcelEmail = value; }
        }

        public string PacketReportEmail
        {
            get { return packetReportEmail; }
            set { packetReportEmail = value; }
        }


        public string EmailRemark
        {
            get { return emailRemark; }
            set { emailRemark = value; }
        }

        public string WanwanName
        {
            get { return wanwanName; }
            set { wanwanName = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
