using System.Data;

namespace Domain.ResponseModel
{
    public class ScExpireModel
    {
        public string serialNumber { get; set; }
        public string expireDate { get; set; }
        public string syncDate { get; set; }
        public bool willExpirSoon { get; set; }
        public string productCode { get; set; }
        public ScExpireModel(DataRow dr)
        {
            this.serialNumber = dr["SERIALNO"] as string;
            this.syncDate = dr["syncdate"] as string;
            this.productCode = dr["PRODUCTCODE"] as string;
            this.expireDate = (dr["EXPIRYDATE"] as string);
            this.willExpirSoon = (dr["WILLEXPIRE"] != DBNull.Value) ? Convert.ToBoolean(dr["WILLEXPIRE"]) : false;
        }
    }
}
