using System.Data;

namespace Domain.ResponseModel
{
    public class SimStatusModel
    {
        public bool isAvailable { get; set; }
        public string productName { get; set; }
        public string issuedOn { get; set; }
        public string activatedOn { get; set; }
        public SimStatusModel(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                this.isAvailable = DBNull.Value != dr["isAvailable"] ? Convert.ToInt32(dr["isAvailable"]) == 0 ? false : true : false;
                this.productName = dr["productName"] as string;
                this.issuedOn = dr["issuedOn"] as string;
                this.activatedOn = dr["activatedOn"] as string;
            }
        }
    }
}
