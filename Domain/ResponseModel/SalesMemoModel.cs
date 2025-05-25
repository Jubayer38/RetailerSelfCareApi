using System.Data;

namespace Domain.ResponseModel
{
    public class SalesMemoModel
    {
        public string date { get; set; }
        public string productName { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        public SalesMemoModel(DataRow dr)
        {
            this.date = dr["SalesMemoDate"] as string;
            this.productName = dr["productName"] as string;
            this.quantity = dr["quantity"] as string;
            this.amount = dr["amount"] as string;
        }
    }
}
