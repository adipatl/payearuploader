using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace NakeAWishUploader
{
    static class Uploader
    {
        public static void Upload(MainProduct mainProduct)
        {
            var httpWebReq = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["UploadURL"]);
            var encoding = new UTF8Encoding();
            var postDataString = mainProduct.ToString();
            var data = encoding.GetBytes(postDataString);

            httpWebReq.ContentLength = Encoding.UTF8.GetByteCount(postDataString);
            httpWebReq.Accept = ConfigurationManager.AppSettings["Http_Accept"];
            httpWebReq.UserAgent = ConfigurationManager.AppSettings["Http_UserAgent"];
            httpWebReq.ContentType = ConfigurationManager.AppSettings["Http_ContentType"];
            httpWebReq.Referer = ConfigurationManager.AppSettings["Http_Referer"];
            httpWebReq.Headers["Accept-Encoding"] = ConfigurationManager.AppSettings["Http_AcceptEncoding"];
            httpWebReq.Headers["Accept-Language"] = ConfigurationManager.AppSettings["Http_AcceptLanguage"];
            httpWebReq.Headers["Cookie"] = ConfigurationManager.AppSettings["Http_Cookie"];
            httpWebReq.Method = "POST";

            using (var stream = httpWebReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)httpWebReq.GetResponse();
            if (response == null)
                throw new Exception("Upload to PAYEAR.COM is failed");

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var jObject = JObject.Parse(responseString);
            var responseMessage = (bool)jObject["success"];
            if (responseMessage == false)
                throw new Exception("Upload to PAYEAR.COM is failed");

            var mainProductID = (string)jObject["data"];

            // Sub product

            foreach (var subProduct in mainProduct.SubProducts)
            {
                Upload(mainProductID, subProduct);
            }
        }

        private static void Upload(string mainProductId, SubProduct subProduct)
        {
            var uploadUrl = ConfigurationManager.AppSettings["SubUploadURL"];
            uploadUrl = uploadUrl.Replace("[MAINPRODUCTID]", mainProductId);

            var httpWebReq = (HttpWebRequest)WebRequest.Create(uploadUrl);
            var encoding = new UTF8Encoding();
            var postDataString = subProduct.ToString();
            var data = encoding.GetBytes(postDataString);

            httpWebReq.ContentLength = Encoding.UTF8.GetByteCount(postDataString);
            httpWebReq.Accept = ConfigurationManager.AppSettings["Http_Accept"];
            httpWebReq.UserAgent = ConfigurationManager.AppSettings["Http_UserAgent"];
            httpWebReq.ContentType = ConfigurationManager.AppSettings["Http_ContentType"];
            var httpReferer = ConfigurationManager.AppSettings["Http_Referer_SubProduct"];
            httpReferer = httpReferer.Replace("[MAINPRODUCTID]", mainProductId);
            httpWebReq.Referer = httpReferer;
            httpWebReq.Headers["Accept-Encoding"] = ConfigurationManager.AppSettings["Http_AcceptEncoding"];
            httpWebReq.Headers["Accept-Language"] = ConfigurationManager.AppSettings["Http_AcceptLanguage"];
            httpWebReq.Headers["Cookie"] = ConfigurationManager.AppSettings["Http_Cookie"];
            httpWebReq.Method = "POST";

            using (var stream = httpWebReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)httpWebReq.GetResponse();
            if (response == null)
                throw new Exception("Upload to PAYEAR.COM is failed");

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var jObject = JObject.Parse(responseString);
            var responseMessage = (bool)jObject["success"];
            if (responseMessage == false)
                throw new Exception("Upload to PAYEAR.COM is failed");

            var resultHtml = (string)jObject["data"]["table_tr"];

            var doc = new HtmlDocument();
            doc.LoadHtml(resultHtml);
            HtmlNode resultNode = doc.DocumentNode.SelectSingleNode("/tr");

            var subProductId = resultNode.Attributes["proid"].Value;

            // Set Quantity

            SetQuantity(mainProductId, subProductId, subProduct.Quantity);
        }

        private static void SetQuantity(string mainProductId, string subProductId, string quantity)
        {
            if (Int32.Parse(quantity) <= 0) return;

            var httpWebReq = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SetQuantityURL"].Replace("[PRODUCTID]", subProductId));
            var encoding = new UTF8Encoding();

            var postDataString = "action=deposit&amount=" + quantity + "&date=" + DateTime.Today.ToString("yyyy-MM-dd") + "&ajaxxxx=true";
            var data = encoding.GetBytes(postDataString);

            httpWebReq.ContentLength = Encoding.UTF8.GetByteCount(postDataString);
            httpWebReq.Accept = ConfigurationManager.AppSettings["Http_Accept"];
            httpWebReq.UserAgent = ConfigurationManager.AppSettings["Http_UserAgent"];
            httpWebReq.ContentType = ConfigurationManager.AppSettings["Http_ContentType"];
            var httpReferer = ConfigurationManager.AppSettings["Http_Referer_SubProduct"];
            httpReferer = httpReferer.Replace("[MAINPRODUCTID]", mainProductId);
            httpWebReq.Referer = httpReferer;
            httpWebReq.Headers["Accept-Encoding"] = ConfigurationManager.AppSettings["Http_AcceptEncoding"];
            httpWebReq.Headers["Accept-Language"] = ConfigurationManager.AppSettings["Http_AcceptLanguage"];
            httpWebReq.Headers["Cookie"] = ConfigurationManager.AppSettings["Http_Cookie"];
            httpWebReq.Method = "POST";

            using (var stream = httpWebReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)httpWebReq.GetResponse();
            if (response == null)
                throw new Exception("Upload to PAYEAR.COM is failed");
        }
    }
}
