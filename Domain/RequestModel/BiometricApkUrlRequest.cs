using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class BiometricApkUrlRequest
    {
        [Required]
        public string iTopUpNumber { get; set; }

        [Required]
        public string deviceId { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }


    public class BimetricExtranalRequest
    {
        public string username { get; set; }
        public string appVersion { get; set; }
    }
}
