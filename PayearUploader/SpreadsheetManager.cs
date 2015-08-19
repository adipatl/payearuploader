using System.Collections.Generic;
using Google.GData.Spreadsheets;

namespace PayearUploader
{
    public class SpreadsheetManager
    {
        private ListFeed _listFeed;
        private readonly GoogleSpreadsheetHelper.SpreadsheetManager _spreadsheetManager = new GoogleSpreadsheetHelper.SpreadsheetManager();
        public void Connect(string googleUserName, string googlePassword, string spreadSheetName, int sheetIndex)
        {
            _spreadsheetManager.Connect(googleUserName, googlePassword, spreadSheetName, sheetIndex);
            _listFeed = _spreadsheetManager.GetAllRows();
        }

        public IEnumerable<PayearProductItem> GetPayearItemsItems()
        {
            if (_listFeed == null)
                return null;

            var productItems = new List<PayearProductItem>();
// ReSharper disable once LoopCanBeConvertedToQuery
            foreach (ListEntry row in _listFeed.Entries)
            {
                var detailInfo = new ProductItemDetailInfo(
                    row.Elements[4].Value, 
                    row.Elements[5].Value, 
                    row.Elements[6].Value, 
                    row.Elements[7].Value, 
                    row.Elements[8].Value, 
                    row.Elements[9].Value, 
                    row.Elements[10].Value, 
                    row.Elements[11].Value, 
                    row.Elements[12].Value, 
                    row.Elements[13].Value);

                productItems.Add(new PayearProductItem(row.Elements[0].Value, row.Elements[1].Value, row.Elements[2].Value,
                    row.Elements[3].Value, detailInfo));
            }

            return productItems;
        }
    }
}
