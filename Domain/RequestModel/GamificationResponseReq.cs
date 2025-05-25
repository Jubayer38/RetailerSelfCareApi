
using Domain.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class GamificationResponseReq : RetailerRequest
    {
        [Required]
        public string campaignName { get; set; }

        [Required(ErrorMessage = "this field is required")]
        [StringDateTimeFormat(ErrorMessage = $"Please provide correct datetime value. Supported format: 2024-05-19T18:43")]
        public DateTime playTime { get; set; }
    }
}