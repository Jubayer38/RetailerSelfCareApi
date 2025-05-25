using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class GetBPImagesRequest
    {
        [Required]
        public string sessionToken { get; set; }

        [Required]
        public string retailerCode { get; set; }

        [Required]
        public int id { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
