using System.Data;

namespace Domain.ResponseModel
{
    public class SalesUpdateModel
    {
        public string kpiName { get; set; }
        public string projectedSales { get; set; }
        public string projectedSalesPer { get; set; }
        public string actualSales { get; set; }
        public string actualSalesPer { get; set; }
        public SalesUpdateModel(DataRow dr)
        {
            this.kpiName = dr["kpiName"] as string;
            this.projectedSales = dr["projectedSales"] as string;
            this.projectedSalesPer = dr["projectedSalesPer"] as string;
            this.actualSales = dr["actualSales"] as string;
            this.actualSalesPer = dr["actualSalesPer"] as string;
        }
    }
}
