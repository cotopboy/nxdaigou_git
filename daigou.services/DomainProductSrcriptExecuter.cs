using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.DataTypes.ExtensionMethods;
using Utilities.IO.ExtensionMethods;
using Utilities.Reflection.ExtensionMethods;
using Utilities.Math.ExtensionMethods;


namespace daigou.services
{
    public class DomainProductSrcriptExecuter
    {

        private IEnumerable<domain.Product> domainProductList;


        public DomainProductSrcriptExecuter()
        {

        }

        public void Execute(IEnumerable<domain.Product> domainProductList, string ScriptText)
        {
            this.domainProductList = domainProductList;

            var commandList = PreProcessScriptText(ScriptText);

            foreach (var item in commandList)
            {
                this.domainProductList.ForEach(x => item.Exceute(x));
            }

        }

        private List<DomainProductCommand> PreProcessScriptText(string input)
        {
            var rawArray = input.ToLines();
            var commandArray = rawArray.Where(x => !x.StartWithAnyOf(new List<string>(){"#","//",";"}));

            return commandArray.Select(x => DomainProductCommand.CreateCommand(x)).ToList();
            

        }
    }

    public class DomainProductCommand
    {
        public static DomainProductCommand CreateCommand(string input)
        {
            var array = input.Split('=');

            return new DomainProductCommand(array[0], "=", array[1]); 
        }


        private domain.Product domainProduct = null;
        private string propertyName = "";
        private string operatorName = "";
        private string Value = "";

        public DomainProductCommand(string propertyName,string operatorName,string value)
        {
            this.propertyName = propertyName.Trim();
            this.operatorName = operatorName.Trim();
            this.Value = value.Trim();
        }

        public  void Exceute(domain.Product domainProduct)
        {
            this.domainProduct = domainProduct;

            try
            {
                this.domainProduct.SetPropertyValue(this.propertyName, this.Value);
            }
            catch { }
        }


    }
}
