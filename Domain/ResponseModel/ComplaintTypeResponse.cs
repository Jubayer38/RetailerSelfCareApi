using System.Data;

namespace Domain.ResponseModel
{
    public class ComplaintTypeResponse
    {
        public int complaintTypeId { get; set; }
        public string complaintType { get; set; }
        public int preferredLevelId { get; set; }
        public string preferredLevel { get; set; }

        /// <summary>
        /// data category. App or SuperOffice
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// SuperOffice Category id
        /// </summary>
        public int categoryId { get; set; }


        public ComplaintTypeResponse(DataRow dr, string lan)
        {
            complaintTypeId = Convert.ToInt32(dr["COMPLAINT_TYPE_ID"]);
            complaintType = lan == "bn" ? dr["COMPLAINT_TYPE_BN"] as string : dr["COMPLAINT_TYPE"] as string;
            preferredLevelId = Convert.ToInt32(dr["PREFERRED_LEVEL_ID"]);
            preferredLevel = lan == "bn" ? dr["PREFERRED_LEVEL_BN"] as string : dr["PREFERRED_LEVEL"] as string;
            category = dr["DATA_CATEGORY"] as string;

            int.TryParse(dr["CATEGORY_ID"].ToString(), out int _catId);
            categoryId = _catId;
        }
    }
}
