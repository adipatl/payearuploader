using System.Collections.Generic;
using System.Linq;
using Google.GData.Client;
using Google.GData.Spreadsheets;

namespace GoogleSpreadsheetHelper
{
    public class SpreadsheetManager
    {
        private SpreadsheetsService _service;
        private AtomLink _listFeedLink;
        public void Connect(string googleUserName, string googlePassword, string spreadSheetName, int sheetIndex)
        {
            _service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
            _service.setUserCredentials(googleUserName, googlePassword);

            // Make the request to Google
            // See other portions of this guide for code to put here...

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

            var listQuery = new ListQuery(_listFeedLink.HRef.ToString()) {SpreadsheetQuery = query};
            return _service.Query(listQuery);
        }

    }

}
