using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    [Serializable]
    public class Product : DomainObject
    {
        private int _ID = 0;        
        private string applicableCrowd = "";
        private string brand = "";
        private string code = "";
        private decimal grossProfit = 0.0m;
        private int grossWeight = 0;
        private decimal importPrice = 0.0m;
        private string name = "";
        private decimal packingCost = 0.0m;
        private string photo = "";
        private decimal priceAdaption = 0.0m;
        private string remark = "";
        private decimal sellPrice = 0.0m;
        private decimal serviceRate = 0.0m;
        private decimal serviceStaticCost = 0.0m;
        private string spec = "";
        private string tagList = "";

        //标签
        public string TagList
        {
            get { return tagList; }
            set { tagList = value; RaisePropertyChanged("TagList"); }
        }

        // 适用人群
        public string ApplicableCrowd
        {
            get { return applicableCrowd; }
            set { applicableCrowd = value; RaisePropertyChanged("ApplicableCrowd"); }
        }

        //品牌
        public string Brand
        {
            get { return brand; }
            set { brand = value; RaisePropertyChanged("Brand"); }
        }

        private string catagory;
        //分类
        public string Catagory
        {
            get { return catagory; }
            set { catagory = value; RaisePropertyChanged("Catagory"); }
        }

        //代码
        public string Code
        {
            get { return code; }
            set { code = value; RaisePropertyChanged("Code"); }
        }

        //毛重
        public int GrossWeight
        {
            get { return grossWeight; }
            set { grossWeight = value; RaisePropertyChanged("GrossWeight"); }
        }

        //ID
        public int ID
        {
            get { return _ID; }
            set { _ID = value; RaisePropertyChanged("ID"); }
        }

        //成本价
        public decimal ImportPrice
        {
            get { return importPrice; }
            set { importPrice = value; RaisePropertyChanged("ImportPrice"); }
        }

        //名称
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged("Name"); }
        }

        //包装成本
        public decimal PackingCost
        {
            get { return packingCost; }
            set { packingCost = value; RaisePropertyChanged("PackingCost"); }
        }

        //图片 
        public string Photo
        {
            get { return photo; }
            set { photo = value; RaisePropertyChanged("Photo"); }
        }

        //价格微调
        public decimal PriceAdaption
        {
            get { return priceAdaption; }
            set { priceAdaption = value; RaisePropertyChanged("PriceAdaption"); }
        }

        //备注
        public string Remark
        {
            get { return remark; }
            set { remark = value; RaisePropertyChanged("Remark"); }
        }

        //价格
        public decimal SellPrice
        {
            get { return sellPrice; }
            set { sellPrice = value; RaisePropertyChanged("SellPrice"); }
        }

        //动态费率
        public decimal ServiceRate
        {
            get { return serviceRate; }
            set { serviceRate = value; RaisePropertyChanged("ServiceRate"); }
        }

        //静态费率
        public decimal ServiceStaticCost
        {
            get { return serviceStaticCost; }
            set { serviceStaticCost = value; RaisePropertyChanged("ServiceStaticCost"); }
        }

        //规格
        public string Spec
        {
            get { return spec; }
            set { spec = value; RaisePropertyChanged("Spec"); }
        }
    }
}
