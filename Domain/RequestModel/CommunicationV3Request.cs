using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class CommunicationV3Request : RetailerRequestV2
    {
        [Required]
        public string type { get; set; }

        public string searchText { get; set; } = string.Empty;

        public string sortType { get; set; } = "ASC";
    }
}