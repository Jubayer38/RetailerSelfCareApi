using Domain.StaticClass;
using System.Data;

namespace Domain.ResponseModel
{
    public class BestPracticesV2
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string retailerCode { get; set; }
        public string[] images { get; set; }
        public string fileLocation { get; set; }

        public BestPracticesV2(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                id = Convert.ToInt32(dr["BESTPRACTICE_ID"]);
                title = dr["TITLE"] as string;
                description = dr["DESCRIPTION"] as string;
                retailerCode = dr["RETAILERCODE"] as string;

                string imagepaths = dr["FILE_DATA"] as string;
                string baseurl = ExternalKeys.ImageVirtualDirPath;
                if (!string.IsNullOrWhiteSpace(imagepaths))
                {
                    images = imagepaths.Split('|');
                    images = images.Select(x => baseurl + x.Replace("\\", "/")).ToArray();
                }
                else
                    images = [];
            }
        }

    }
}