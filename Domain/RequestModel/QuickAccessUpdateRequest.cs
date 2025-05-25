using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class QuickAccessUpdateRequest
    {
        [Required]
        public string sessionToken { get; set; }

        [Required]
        public string retailerCode { get; set; }

        [Required]
        public string deviceId { get; set; }

        public string activeWidgetList { get; set; }

        public string activeWeidgets { get; set; }

        public string inactiveWidgetList { get; set; }

        public string inactiveWeidgets { get; set; }

        public int userId { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
