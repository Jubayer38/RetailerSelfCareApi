using Domain.Helpers;
using System.Data;

namespace Domain.ResponseModel
{
    public class RecommendedComplaintCategoryResponse
    {
        public bool hasValidExternalData { get; set; }
        public int complaintTypeId { get; set; }
        public string complaintType { get; }
        public int complaintTitleId { get; set; }
        public string complaintTitle { get; }
        public int subCategoryId { get; set; }
        public int categoryId { get; set; }
        public string category { get; set; }
        public int preferredLevelId { get; set; }
        public string preferredLevel { get; set; }

        public RecommendedComplaintCategoryResponse()
        {

        }
        public RecommendedComplaintCategoryResponse(DataRow dr, string lan)
        {
            complaintTypeId = dr["COMPLAINT_TYPE_ID"].DBNullToInteger();
            complaintType = lan == "bn" ? dr["COMPLAINT_TYPE_BN"].DBNullToString() : dr["COMPLAINT_TYPE"].DBNullToString();
            complaintTitleId = dr["COMPLAINT_TITLE_ID"].DBNullToInteger();
            complaintTitle = lan == "bn" ? dr["COMPLAINT_TITLE_BN"].DBNullToString() : dr["COMPLAINT_TITLE"].DBNullToString();
            subCategoryId = dr["SUBCATEGORY_ID"].DBNullToInteger();
            categoryId = dr["CATEGORY_ID"].DBNullToInteger();
            category = dr["CATEGORY"].DBNullToString();
            preferredLevelId = dr["PREFERRED_LEVEL_ID"].DBNullToInteger();
            preferredLevel = lan == "bn" ? dr["PREFERRED_LEVEL_BN"].DBNullToString() : dr["PREFERRED_LEVEL"].DBNullToString();
        }
    }
}
