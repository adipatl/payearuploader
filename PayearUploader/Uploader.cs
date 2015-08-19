using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net.Cache;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;

namespace PayearUploader
{
    static class Uploader
    {
        public static void Upload(IEnumerable<PayearProductItem> productItems)
        {
            var isFirst = true;
            
            foreach (var item in productItems)
            {
                var sessionCookie = new CookieContainer();
                //GenerateFirstCookie(ref sessionCookie);

                var productId = "";
                if (UploadItem(item, ref productId, ref sessionCookie))
                    SetQuantity(productId, item, ref sessionCookie);
            }
        }

        //private static bool GenerateFirstCookie(ref CookieContainer sessionCookie)
        //{
        //    var httpWebReq = (HttpWebRequest)WebRequest.Create("http://www.payear.com/lnwbar/action/session?_=1398350010217");
        //    //var encoding = new UTF8Encoding();
        //    //var postDataString = product.ToString();
        //    //var data = encoding.GetBytes(postDataString);

        //    //httpWebReq.ContentLength = Encoding.UTF8.GetByteCount(postDataString);
        //    httpWebReq.Accept = ConfigurationManager.AppSettings["Http_Accept"];
        //    httpWebReq.UserAgent = ConfigurationManager.AppSettings["Http_UserAgent"];
        //    httpWebReq.ContentType = ConfigurationManager.AppSettings["Http_ContentType"];
            
        //    httpWebReq.KeepAlive = true;
        //    //httpWebReq.Referer = ConfigurationManager.AppSettings["Http_Referer"];
        //    httpWebReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        //    httpWebReq.Headers["Accept-Encoding"] = ConfigurationManager.AppSettings["Http_AcceptEncoding"];
        //    httpWebReq.Headers["Accept-Language"] = ConfigurationManager.AppSettings["Http_AcceptLanguage"];
        //    httpWebReq.Headers["Connection"] = "keep-alive";
        //    //httpWebReq.Headers["Cookie"] = ConfigurationManager.AppSettings["Http_Cookie"];
        //    var cookieCollection = GetAllCookiesFromHeader(ConfigurationManager.AppSettings["Http_Cookie"], "www.payear.com");
        //    sessionCookie.Add(cookieCollection);
        //    httpWebReq.CookieContainer = sessionCookie;

        //    httpWebReq.Method = "GET";
        //    httpWebReq.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        //    //using (var stream = httpWebReq.GetRequestStream())
        //    //{
        //    //    stream.Write(data, 0, data.Length);
        //    //}

        //    var response = (HttpWebResponse)httpWebReq.GetResponse();
            
        //    sessionCookie.Add(response.Cookies);
            
        //    //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //    //var jObject = JObject.Parse(responseString);
        //    //var responseMessage = (string)jObject["success"];
        //    //if (responseMessage != "True")
        //    //    return false;

        //    //productId = (string)jObject["data"];

        //    return true;
        //}

        private static bool UploadItem(PayearProductItem product, ref string productId, ref CookieContainer sessionCookie)
        {
            var httpWebReq = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["UploadURL"]);
            var encoding = new UTF8Encoding();
            var postDataString = product.ToString();
            var data = encoding.GetBytes(postDataString);

            httpWebReq.ContentLength = Encoding.UTF8.GetByteCount(postDataString);
            httpWebReq.Accept = ConfigurationManager.AppSettings["Http_Accept"];
            httpWebReq.UserAgent = ConfigurationManager.AppSettings["Http_UserAgent"];
            httpWebReq.ContentType = ConfigurationManager.AppSettings["Http_ContentType"];
            httpWebReq.Referer = ConfigurationManager.AppSettings["Http_Referer"];
            
            httpWebReq.Headers["Accept-Encoding"] = ConfigurationManager.AppSettings["Http_AcceptEncoding"];
            httpWebReq.Headers["Accept-Language"] = ConfigurationManager.AppSettings["Http_AcceptLanguage"];
            httpWebReq.Headers["X-Requested-With"] = "XMLHttpRequest";
            httpWebReq.Headers["Origin"] = "www.payear.com";
            httpWebReq.KeepAlive = true;


            httpWebReq.Headers["Cookie"] = ConfigurationManager.AppSettings["Http_Cookie"];
            
            httpWebReq.Method = "POST";
            httpWebReq.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            using (var stream = httpWebReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)httpWebReq.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var jObject = JObject.Parse(responseString);
            var responseMessage = (string)jObject["success"];
            if (responseMessage != "True")
                return false;

            productId = (string)jObject["data"];
            //sessionCookie.Add(response.Cookies);

            return true;
        }

        private static void SetQuantity(string productId, PayearProductItem item, ref CookieContainer sessionCookie)
        {
            if (Int32.Parse(item.Quantity) <= 0) return;

            var httpWebReq = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SetQuantityURL"].Replace("[PAYEAR_PRODUCT_ID]", productId));
            var encoding = new UTF8Encoding();

            var postDataString = "action=deposit&amount=" + item.Quantity + "&date=" + DateTime.Today.ToString("yyyy-MM-dd") + "&ajaxxxx=true";
            var data = encoding.GetBytes(postDataString);

            httpWebReq.ContentLength = Encoding.UTF8.GetByteCount(postDataString);
            httpWebReq.Accept = ConfigurationManager.AppSettings["Http_Accept"];
            httpWebReq.UserAgent = ConfigurationManager.AppSettings["Http_UserAgent"];
            httpWebReq.ContentType = ConfigurationManager.AppSettings["Http_ContentType"];
            httpWebReq.Referer = ConfigurationManager.AppSettings["Http_Referer"];
            httpWebReq.Headers["Accept-Encoding"] = ConfigurationManager.AppSettings["Http_AcceptEncoding"];
            httpWebReq.Headers["Accept-Language"] = ConfigurationManager.AppSettings["Http_AcceptLanguage"];
            httpWebReq.Headers["X-Requested-With"] = "XMLHttpRequest";
            httpWebReq.Headers["Origin"] = "www.payear.com";
            httpWebReq.KeepAlive = true;
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
