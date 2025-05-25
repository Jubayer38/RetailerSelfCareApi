using System.Data;

namespace Domain.RequestModel
{
    public class StockDetialsModel
    {
        public string categoryTitle { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        public string message { get; set; }
        public string dateTime { get; set; }

        public StockDetialsModel(DataRow dr)
        {
            categoryTitle = dr.Table.Columns.Contains("TITLE") ? dr["TITLE"] as string : null;
            quantity = dr.Table.Columns.Contains("QUANTITY") ? dr["QUANTITY"] as string : null;
            amount = dr.Table.Columns.Contains("AMOUNT") ? dr["AMOUNT"] as string : null;
            message = dr.Table.Columns.Contains("Message") ? dr["Message"] as string : null;
        }
    }
}