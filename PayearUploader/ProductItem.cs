using System.Configuration;
using System.IO;

namespace PayearUploader
{
    public class ProductItemDetailInfo
    {
        public ProductItemDetailInfo(string boobs, string waist, string hip, string length, string shoulder, string earlyArm, string finalArm, string crotch, string earlyLeg, string finalLeg)
        {
            FinalLeg = finalLeg;
            EarlyLeg = earlyLeg;
            Crotch = crotch;
            FinalArm = finalArm;
            EarlyArm = earlyArm;
            Shoulder = shoulder;
            Length = length;
            Hip = hip;
            Waist = waist;
            Boobs = boobs;
        }

        public string Boobs { get; private set; }
        public string Waist { get; private set; }
        public string Hip { get; private set; }
        public string Length { get; private set; }
        public string Shoulder { get; private set; }
        public string EarlyArm { get; private set; }
        public string FinalArm { get; private set; }
        public string Crotch { get; private set; }
        public string EarlyLeg { get; private set; }
        public string FinalLeg { get; private set; }
    }

    public class PayearProductItem
    {
        public PayearProductItem(string id, string productName, string quantity, string price, ProductItemDetailInfo detailInfo)
        {
            Price = price;
            Quantity = quantity;
            ProductName = productName;
            Id = id;
            _detailInfo = detailInfo;
            GenerateCategory();
            GenerateHtmlDescription();
            GenerateTag();
            GenerateConditionText();
        }

        private string Id { get; set; }
        private string ProductName { get; set; }
        public string Quantity { get; private set; }
        private string Price { get; set; }
        private string CategoryId { get; set; }
        private string HtmlDescription { get; set; }
        private string Tag { get; set; }
        private string ConditionText { get; set; }
        private readonly ProductItemDetailInfo _detailInfo;

        public override string ToString()
        {
            var sReader = new StreamReader("PostDataTemplate.txt");
            var postData = sReader.ReadToEnd();
            postData = postData.Replace("[PAYEAR_PRODUCTNAME]", ProductName);
            postData = postData.Replace("[PAYEAR_ID]", Id);
            postData = postData.Replace("[PAYEAR_CATEGORYID]", CategoryId);
            postData = postData.Replace("[PAYEAR_HTMLDESCRIPTION]", HtmlDescription);
            postData = postData.Replace("[PAYEAR_CONDITION]", ConditionText);
            postData = postData.Replace("[PAYEAR_PRICE]", Price);
            postData = postData.Replace("[PAYEAR_TAG]", Tag);
            return postData;
        }

        private void GenerateCategory()
        {
            var categoryIdString = Id.Substring(0, 3);
            var appSettingName = "CategoryId_" + categoryIdString;
            CategoryId = ConfigurationManager.AppSettings[appSettingName];
        }

        private void GenerateHtmlDescription()
        {
            // Get Id
            var categoryIdString = Id.Substring(0, 3);
            var appSettingName = "ProductNameTemplate_" + categoryIdString;
            ProductName += ConfigurationManager.AppSettings[appSettingName];

            switch (categoryIdString)
            {
                case "PYD":
                case "PYS":
                case "PYW":
                    ProductName = ProductName.Replace("[PAYEAR_VALUE]", _detailInfo.Boobs);
                    break;
                case "PYK":
                case "PYP":
                    ProductName = ProductName.Replace("[PAYEAR_VALUE]", _detailInfo.Waist);
                    break;
                case "PYB":
                    break;;
                default:
                    break;
            }

            var fileName = "Html_" + categoryIdString + ".html";
            var sReader = new StreamReader(fileName);
            HtmlDescription = sReader.ReadToEnd();
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_PRODUCTNAME]", ProductName);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_BOOBS]", _detailInfo.Boobs);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_WAIST]", _detailInfo.Waist);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_HIP]", _detailInfo.Hip);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_LENGTH]", _detailInfo.Length);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_SHOULDER]", _detailInfo.Shoulder);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_EARLY_ARM]", _detailInfo.EarlyArm);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_FINAL_ARM]", _detailInfo.FinalArm);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_CROTCH]", _detailInfo.Crotch);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_EARLY_LEG]", _detailInfo.EarlyLeg);
            HtmlDescription = HtmlDescription.Replace("[PAYEAR_FINAL_LEG]", _detailInfo.FinalLeg);
        }

        private void GenerateTag()
        {
            var tagIdString = Id.Substring(0, 3);
            var appSettingName = "Tag_" + tagIdString;
            Tag = ConfigurationManager.AppSettings[appSettingName];
        }

        private void GenerateConditionText()
        {
            var sReader = new StreamReader(ConfigurationManager.AppSettings["ConditionText"]);
            ConditionText = sReader.ReadToEnd();
        }

        
    }
}
