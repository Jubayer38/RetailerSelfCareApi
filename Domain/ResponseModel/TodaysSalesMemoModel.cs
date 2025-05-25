using System.Data;

namespace Domain.ResponseModel
{
    public class TodaysSalesMemoModel
    {
        public string productName { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        public TodaysSalesMemoModel(DataRow dr)
        {
            this.productName = dr["productName"] as string;
            this.quantity = dr["quantity"] as string;
            this.amount = dr["amount"] as string;
        }
    }
}
