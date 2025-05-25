using System.Data;

namespace Domain.ResponseModel
{
    public class RetailerV2Model
    {
        public string retailerName { get; set; }

        public string retailerType { get; set; }

        public string contactPerson { get; set; }

        public string contactNumber { get; set; }

        public string email { get; set; }

        public string dob { get; set; }

        public string address { get; set; }

        public string iTopUpNumber { get; set; }

        public string outletName { get; set; }

        public string imageUrl { get; set; }

        public string servicePoint { get; set; }

        public string starRating { get; set; }

        public RetailerV2Model(DataRow dr, string baseUrl)
        {
            if (DBNull.Value != dr["NAME"])
            {
                retailerName = dr["NAME"] as string;
                retailerType = dr["TypeName"] as string;
                contactPerson = dr["CONTACTPERSON"] as string;
                contactNumber = dr["CONTACTNO"] as string;
                email = dr["EMAIL"] as string;
                dob = dr["DOB"] as string;
                address = dr["ADDRESS"] as string;
                iTopUpNumber = dr["ITOPUPSRNUMBER"] as string;
                outletName = dr["OUTLET"] as string;
                servicePoint = dr["SERVICE_POINT"] as string;

                //if (dr["IMAGEURL"] != DBNull.Value)
                //{
                //    KeyValuePair<string, string> image = ImageFormate.UrlToBase64(dr["IMAGEURL"] as string, baseUrl);
                //    imageUrl = image.Value;
                //}
            }
        }
    }
}
