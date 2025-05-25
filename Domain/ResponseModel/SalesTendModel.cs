using System.Data;

namespace Domain.ResponseModel
{
    public class SalesTendModel
    {
        public string kpiName { get; set; }
        public string week1 { get; set; }
        public string week1Per { get; set; }
        public string week2 { get; set; }
        public string week2Per { get; set; }
        public string week3 { get; set; }
        public string week3Per { get; set; }
        public SalesTendModel(DataRow dr)
        {
            this.kpiName = dr["kpiName"] as string;
            this.week1 = dr["week1"] as string;
            this.week1Per = dr["week1Per"] as string;
            this.week2 = dr["week2"] as string;
            this.week2Per = dr["week2Per"] as string;
            this.week3 = dr["week3"] as string;
            this.week3Per = dr["week3Per"] as string;
        }
    }
}
