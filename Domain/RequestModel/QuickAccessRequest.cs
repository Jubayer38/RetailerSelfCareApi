using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class QuickAccessRequest : RetailerRequestV2
    {
        [Required]
        public bool isPrimary { get; set; }

        [Required]
        public bool isDark { get; set; }

        public int userId { get; set; }
    }
}
