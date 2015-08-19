using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Spreadsheets;
using Google.Apis.Auth.OAuth2;

namespace GoogleSpreadsheetHelper
{
    public class SpreadsheetManager
    {
        private string _keyFilePath = @"D:\kobra-local\payearuploader\Payear-4dc29b9d3e4e.p12";
        private string _serviceAccountEmail = "279455445774-fv7i10ck453lo2jgtu46c7f4svc1vgbb@developer.gserviceaccount.com";   // found in developer console
        private SpreadsheetsService _service;
        private AtomLink _listFeedLink;
        public void Connect(string googleUserName, string googlePassword, string spreadSheetName, int sheetIndex)
        {
            var certificate = new X509Certificate2(_keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(_serviceAccountEmail) //create credential using certificate
            {
                Scopes = new[] { "https://spreadsheets.google.com/feeds/" } //this scopr is for spreadsheets, check google scope FAQ for others
            }.FromCertificate(certificate));

            credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Wait(); //request token

            var requestFactory = new GDataRequestFactory("Adipat");
            requestFactory.CustomHeaders.Add(string.Format("Authorization: Bearer {0}", credential.Token.AccessToken));

            _service = new SpreadsheetsService("You App Name"); //create your old service
            _service.RequestFactory = requestFactory; //add new request factory to your old service

            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            var query = new SpreadsheetQuery();

            // Make a request to the API and get all spreadsheets.
            var feed = _service.Query(query);

            var payearEntry = feed.Entries.Cast<SpreadsheetEntry>().FirstOrDefault(entry => entry.Title.Text == spreadSheetName);

            if (payearEntry == null)
            {
                return;
            }

            var wsFeed = payearEntry.Worksheets;
            var worksheet = (WorksheetEntry)wsFeed.Entries[sheetIndex];
            // Define the URL to request the list feed of the worksheet.
            _listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
        }

        public ListFeed GetAllRows()
        {
            if (_listFeedLink == null || _service == null) return null;

            // Fetch the list feed of the worksheet.
            var listQuery = new ListQuery(_listFeedLink.HRef.ToString());
            return _service.Query(listQuery);
        }

        public ListFeed Query(string query)
        {
            if (_listFeedLink == null || _service == null) return null;

            var listQuery = new ListQuery(_listFeedLink.HRef.ToString()) { SpreadsheetQuery = query };
            return _service.Query(listQuery);
        }

    }

}
