using System.Data;

namespace Domain.ResponseModel
{
    public class SalesDetailModel
    {
        public string date { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        public SalesDetailModel(DataRow dr)
        {
            this.date = dr["SALES_DATE"] as string;
            this.quantity = dr["QTY"] as string;
            this.amount = dr["AMOUNT"] as string;
        }
    }
}
