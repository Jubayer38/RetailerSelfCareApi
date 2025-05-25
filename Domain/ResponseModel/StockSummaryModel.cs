using Domain.Helpers;
using System.Data;

namespace Domain.ResponseModel
{
    public class StockSummaryModel
    {
        public string itemCode { get; set; }
        public string itemTitle { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        public string updateTime { get; set; }

        public StockSummaryModel(DataRow dr, string item)
        {
            if (dr.ItemArray.Length > 0)
            {
                if (DBNull.Value != dr["CODE"])
                {
                    itemCode = dr["CODE"] as string;
                    itemTitle = dr["ITEM"] as string;
                    quantity = dr["QUANTITY"] as string;
                    amount = dr["AMOUNT"] as string;
                    updateTime = dr["UPDATETIME"] as string;
                }
            }
            else
            {
                string _itemCode = item switch
                {
                    "SC" => "1",
                    "SIM" => "2",
                    "iTopUp" => "3",
                    _ => "3"
                };

                itemTitle = item;
                itemCode = _itemCode;
                quantity = "0";
                amount = "0";
                updateTime = DateTime.Now.ToEnUSDateString("hh:mm:ss tt, dd MMM yyyy");
            }
        }
    }
}