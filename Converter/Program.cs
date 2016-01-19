using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using daigou.dal.DaigouDataFile;
using daigou.domain;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            FileHelperEngine<CsvProduct> engine = new FileHelperEngine<CsvProduct>();
            engine.Encoding =  Encoding.UTF8;
            engine.ErrorManager.ErrorMode = ErrorMode.ThrowException;

            CsvProduct[] res = engine.ReadFile("csvProduct.txt");

            if (engine.ErrorManager.ErrorCount > 0)
                engine.ErrorManager.SaveErrors("Errors.txt");

            Console.WriteLine(res.Count());

            FileDBMgr file = new FileDBMgr();
            file.LoadFileDB();

            FileProductRepository prodoctRepo = new FileProductRepository(file);

            foreach (var item in res)
            {
                Product product = new Product() 
                {
                     ApplicableCrowd = item.howbig,
                      Brand = item.brand,
                       Code = item.code,
                         GrossWeight = (int) (item.weight ?? -1m),
                          ImportPrice = item.inPrice ?? 0,
                           Name = item.name,
                            PackingCost = item.pcost ?? 0,
                             PriceAdaption = item.adaption ??0,
                              ServiceRate = item.drate ??1,
                               Spec = item.spicification                               
                           
                           
                };

                prodoctRepo.Add(product);
                
            }


            file.Save();




        }
    }

    [DelimitedRecord(";")]
    public sealed class CsvProduct
    {

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public String code;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public String brand;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public String howbig;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public String name;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public String spicification;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public decimal ? sellPrice;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public decimal? inPrice;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public decimal? drate;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public decimal? pcost;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public decimal? euroPrice;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public decimal? adaption;

        [FieldTrim(TrimMode.Both)]
        [FieldOptional]
        public decimal? weight;


    }
}
