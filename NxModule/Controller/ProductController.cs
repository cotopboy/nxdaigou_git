using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NxModule.CommonCS;
using Microsoft.Practices.Unity;
using WebServer.Infrasturcture;
using WebServer.Infrasturcture.Status;
using WebServer.Infrasturcture.MVC;
using daigou.domain.Services;
using daigou.domain;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;
using Utilities.IO;
using System.Web;
using daigou.services;


namespace NxModule.Controller
{
    public class ProductController : NxModuleControllerBase<ProductControllerModel, object>
    {
        private IUnityContainer container;
        private ProductService productService;
        private ProductPriceCalcuateService priceCalSvc;

        public ProductController(IUnityContainer container, ProductService productService,ProductPriceCalcuateService priceCalSvc)
        {
            this.priceCalSvc = priceCalSvc;
            this.container = container;
            this.productService = productService;
        }


        [AllowAnonymous]
        public ActionRet Welcome()
        {
            Model.IsLoginned = (this.Session.LoginStatus == LoginStatus.Loginned);
            Model.Username = this.Session.GetStringValue("AuthenModel.Username");

            var productList = productService.GetAllProduct();

            productList.ForEach(x => x.SellPrice = priceCalSvc.GetPrice(x, 7.5m));
            Model.ProductList = ConvertToVMList(productList);

            return View("Welcome", Model);
        }

        [AllowAnonymous]
        public ActionRet Image()
        {                 
       
            
            byte[] content = null;
            try
            {
                string fileName64 = RequestDict.GetValue_safe("path", UrlEncode("images/default.png"));

                string fileName = UrlDecode(fileName64);

                content = LoadExternalResource(DirectoryHelper.CombineWithCurrentExeDir(fileName));
            }
            catch
            {
                content = LoadExternalResource(DirectoryHelper.CombineWithCurrentExeDir("images/default.png"));
            }

            return ViewImg(content, "jpg");
        }

        private string UrlEncode(string input)
        {
            return HttpUtility.UrlEncode(HttpUtility.UrlEncode(input, Encoding.UTF8), Encoding.UTF8);
        }

        private string UrlDecode(string input)
        { 
            return HttpUtility.UrlDecode(HttpUtility.UrlDecode(input,Encoding.UTF8),Encoding.UTF8);
        }

        private List<ProductVM> ConvertToVMList(IEnumerable<Product> productList)
        {


            return productList.Where(x => x.SellPrice > 0).Select(x => new ProductVM() 
            {                 
                 TagList         = x.TagList,
                 ApplicableCrowd = x.ApplicableCrowd,
                 Brand           = x.Brand,//
                 Catagory        = x.Catagory,
                 Code            = x.Code,
                 GrossWeight     = x.GrossWeight,
                 ID              = x.ID,//
                 Name            = x.Name,//
                 Photo           = UrlEncode(x.Photo),
                 Remark          = x.Remark,
                 SellPrice       = x.SellPrice,
                 Spec            = x.Spec
            }).OrderBy(x=>x.Brand).ToList();
        }

        
    }

    public class ProductControllerModel : NxModuleBaseModel
    {
        public string Index { get { return DateTime.Now.Second.ToString(); } }
        public bool IsLoginned { get; set; }
        public string Username { get; set; }

        public List<ProductVM> ProductList { get; set; }
    }








    public class ProductVM
    {
        public string  TagList         { get; set; }
        public string  ApplicableCrowd { get; set; }
        public string  Brand           { get; set; }
        public string  Catagory        { get; set; }
        public string  Code            { get; set; }
        public int     GrossWeight     { get; set; }
        public int     ID              { get; set; }
        public string  Name            { get; set; }
        public string  Photo           { get; set; }
        public string  Remark          { get; set; }
        public decimal SellPrice       { get; set; }
        public string  Spec            { get; set; }
    }
}
