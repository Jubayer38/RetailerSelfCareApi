using System.Data;

namespace Domain.ResponseModel
{
    public class BestPractices
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string retailerCode { get; set; }
        public List<string> images { get; set; }

        public BestPractices(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                id = Convert.ToInt32(dr["BESTPRACTICE_ID"]);
                title = dr["TITLE"] as string;
                description = dr["DESCRIPTION"] as string;
                retailerCode = dr["RETAILERCODE"] as string;
                images = [];

                if (dr["FILE_DATA"] as string != null)
                {
                    images.Add(dr["FILE_DATA"] as string);
                }
            }
        }

    }
}