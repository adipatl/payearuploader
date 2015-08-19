using System.Configuration;

namespace PayearUploader
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Connect to Google
            var spreadsheetManager = new SpreadsheetManager();
            spreadsheetManager.Connect(ConfigurationManager.AppSettings["Google_UserName"], ConfigurationManager.AppSettings["Google_Password"], ConfigurationManager.AppSettings["Payear_DatabaseFile"], 0);

            // Retrieve Items + Preparing Data
            // Upload each item
            Uploader.Upload(spreadsheetManager.GetPayearItemsItems());
           
        }
    }
}
