using System.Data;

namespace Domain.ResponseModel
{
    public class SalesPerformModel
    {
        public string kpiName { get; set; }
        public string routePerformance { get; set; }
        public string routePerformancePer { get; set; }
        public string myPerformance { get; set; }
        public string myPerformancePer { get; set; }
        public SalesPerformModel(DataRow dr)
        {
            this.kpiName = dr["kpiName"] as string;
            this.routePerformance = dr["routePerformance"] as string;
            this.routePerformancePer = dr["routePerformancePer"] as string;
            this.myPerformance = dr["myPerformance"] as string;
            this.myPerformancePer = dr["myPerformancePer"] as string;
        }
    }
}
