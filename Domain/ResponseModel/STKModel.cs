using System.Data;

namespace Domain.ResponseModel
{
    public class STKModel
    {
        public int sTKId { get; set; }
        public string ussd { get; set; }
        public string sTKName { get; set; }

        public STKModel(DataRow dr)
        {
            if (dr["STKID"] != DBNull.Value) { sTKId = Convert.ToInt32(dr["STKID"].ToString()); };
            ussd = dr["USSD"] as string;
            sTKName = dr["STKNAME"] as string;
        }
    }
}