using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.services
{
    public class RandomNameService
    {
        private List<string> familyNameList = new List<string>() { "李", "王", "张", "刘", "陈", "杨", "黄", "赵", "吴", "周", "徐", "孙", "马", "朱", "胡", "郭", "何", "高", "林", "罗", "郑", "梁", "谢", "宋", "唐", "许", "韩", "冯", "邓", "曹", "彭", "曾", "肖", "田", "董", "袁", "潘", "于", "蔡", "余", "杜", "叶", "程", "苏", "魏", "吕", "丁", "任", "沈", "姚", "卢", "姜", "崔", "钟", "谭", "陆", "汪", "范", "金", "石", "廖", "贾", "夏", "韦", "傅", "方", "白", "邹", "孟", "熊", "秦", "邱", "江", "尹", "薛", "闫", "段", "雷", "侯", "龙", "史", "陶", "黎", "贺", "顾", "毛", "郝", "龚", "邵", "万", "钱", "严", "武", "戴", "莫", "孔", "向", "汤" };

        private List<string> givenNameList = new List<string>() { "伟", "刚", "勇", "毅", "俊", "峰", "强", "军", "平", "保", "东", "文", "辉", "力", "明", "永", "健", "世", "广", "志", "义", "兴", "良", "海", "山", "仁", "波", "宁", "贵", "福", "生", "龙", "元", "全", "国", "胜", "学", "祥", "才", "发", "武", "新", "利", "清", "飞", "彬", "富", "顺", "信", "子", "杰", "涛", "昌", "成", "康", "星", "光", "天", "达", "安", "岩", "中" };

        private Random RD = new Random(DateTime.Now.Millisecond);

        public RandomNameService()
        {

        }

        public string GetNextRandomName()
        {
            int familyNameIndex = RD.Next(0, this.familyNameList.Count-1);
            int givennameIndex = RD.Next(0, this.givenNameList.Count-1);
            int givennameIndex2 = RD.Next(0, this.givenNameList.Count-1);
            string familyName = familyNameList[familyNameIndex];
            string givenName = (familyNameIndex %2 ==0) ? givenNameList[givennameIndex] : givenNameList[givennameIndex]+ givenNameList[givennameIndex2];

            return string.Format("{0}{1}", familyName, givenName);
        }

    }
}
