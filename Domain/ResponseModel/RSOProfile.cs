using System.Data;

namespace Domain.ResponseModel
{
    public class RSOProfile
    {
        public string name { get; set; }
        public string code { get; set; }
        public string number { get; set; }
        public double rating { get; set; }
        public string lastSubmitDate { get; set; }
        public string image { get; set; }


        public RSOProfile(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                name = dr["NAME"] as string;
                number = dr["MOBILE"] as string;
                code = dr["CODE"] as string;
                lastSubmitDate = dr["LAST_SUBMIT_DATE"] as string;

                double.TryParse(dr["RATING"].ToString(), out double _rating);
                rating = _rating;
            }
        }
    }
}
