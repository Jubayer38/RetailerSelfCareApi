using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class RecommendedComplaintCategoryRequest : RetailerRequestV2
    {
        [Required]
        public string complaintDetails { get; set; }
    }
}
