using System.Data;

namespace Domain.ResponseModel
{
    public class RSORatingHistoryModel
    {
        public long rating_Id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string number { get; set; }
        public double rating { get; set; }
        public string comment { get; set; }
        public string status { get; set; }
        public string date { get; set; }


        public RSORatingHistoryModel(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                rating_Id = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ID"]);
                name = dr["RSO_NAME"] as string;
                code = dr["RSO_CODE"] as string;
                number = dr["RSO_NUMBER"] as string;
                rating = Convert.ToDouble(dr["RSO_RATING"]);
                comment = dr["COMMENT"] as string;
                status = dr["STATUS"] as string;
                date = dr["RATING_DATE"] as string;
            }
        }
    }
}
