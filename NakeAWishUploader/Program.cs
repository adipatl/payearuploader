
using System.Collections.Generic;
using System.Configuration;
using Google.GData.Spreadsheets;
using GoogleSpreadsheetHelper;

namespace NakeAWishUploader
{
    static class Program
    {
        static void Main()
        {
            var spreadsheetManager = new SpreadsheetManager();
            spreadsheetManager.Connect(ConfigurationManager.AppSettings["Google_UserName"], ConfigurationManager.AppSettings["Google_Password"], ConfigurationManager.AppSettings["DatabaseName"], 0);

            var mainProducts = new List<MainProduct>();

            foreach (ListEntry mainProductEntry in spreadsheetManager.GetAllRows().Entries)
            {
                if (mainProductEntry.Elements[0].Value != "Main") continue;

                if (mainProductEntry.Elements[1].Value == null || mainProductEntry.Elements[2].Value == null) continue;
                    
                var mainProduct = new MainProduct
                {
                    ID = mainProductEntry.Elements[1].Value,
                    Name = mainProductEntry.Elements[2].Value,
                    Description = mainProductEntry.Elements[3].Value
                };

                var query = "producttype = \"" + mainProduct.ID + "\"";
                var subProductList = spreadsheetManager.Query(query);

                foreach (ListEntry subproductEntry in subProductList.Entries)
                {
                    mainProduct.SubProducts.Add(new SubProduct(subproductEntry.Elements[1].Value,
                        subproductEntry.Elements[2].Value, subproductEntry.Elements[4].Value,
                        subproductEntry.Elements[5].Value));
                }

                mainProduct.Initialize();

                mainProducts.Add(mainProduct);
            }

            // Upload

            foreach (var mainproduct in mainProducts)
            {
                Uploader.Upload(mainproduct);
            }
        }
    }
}
