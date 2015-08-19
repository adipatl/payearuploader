using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NakeAWishUploader
{
    class MainProduct
    {
        public MainProduct()
        {
            SubProducts = new List<SubProduct>();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public List<SubProduct> SubProducts { get; private set; }
        public string Description { private get; set; }
        public string CategoryId { get; private set; }
        public string HtmlDescription { get; private set; }
        public string Tag { get; private set; }
        public string ConditionText { get; private set; }

        public void Initialize()
        {
            Name = Name.Replace("%", "");
            generateCategoryId();
            generateHtmlDescription();
            generateTag();
            generateConditionText();
        }

        public override string ToString()
        {
            var sReader = new StreamReader("PostDataTemplate.txt");
            var postData = sReader.ReadToEnd();
            postData = postData.Replace("[MAINPRODUCT_NAME]", Name);
            postData = postData.Replace("[MAINPRODUCT_ID]", ID);
            postData = postData.Replace("[MAINPRODUCT_CATEGORYID]", CategoryId);
            postData = postData.Replace("[MAINPRODUCT_HTMLDESCRIPTION]", HtmlDescription);
            postData = postData.Replace("[CONDITION]", ConditionText);
            postData = postData.Replace("[PAYEAR_PRICE]", Price);
            postData = postData.Replace("[PRODUCT_TAG]", Tag);
            return postData;
        }

        private void generateCategoryId()
        {
            var categoryIdString = ID.Substring(0, 2);
            var appSettingName = "CategoryId_" + categoryIdString;
            CategoryId = ConfigurationManager.AppSettings[appSettingName];
        }

        private void generateHtmlDescription()
        {
            // Get Id
            var categoryIdString = ID.Substring(0, 2);
            
            var fileName = "Html_" + categoryIdString + ".html";
            var sReader = new StreamReader(fileName);
            HtmlDescription = sReader.ReadToEnd();
            HtmlDescription = HtmlDescription.Replace("[PRODUCT_DETAIL]", Description);
        }

        private void generateTag()
        {
            var tagIdString = ID.Substring(0, 2);
            var appSettingName = "Tag_" + tagIdString;
            Tag = ConfigurationManager.AppSettings[appSettingName];
        }

        private void generateConditionText()
        {
            var sReader = new StreamReader(ConfigurationManager.AppSettings["ConditionText"]);
            ConditionText = sReader.ReadToEnd();
        }

        
    }

    class SubProduct 
    {
        public SubProduct(string id, string name, string quantity, string price)
        {
            ID = id;
            Name = name;
            Quantity = quantity;
            Price = price;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }

        public override string ToString()
        {
            var sReader = new StreamReader("PostDataTemplate_SubProduct.txt");
            var postData = sReader.ReadToEnd();
            postData = postData.Replace("[SUBPRODUCT_NAME]", Name);
            postData = postData.Replace("[SUBPRODUCT_ID]", ID);
            postData = postData.Replace("[SUBPRODUCT_PRICE]", Price);
            return postData;
        }
    }
}
