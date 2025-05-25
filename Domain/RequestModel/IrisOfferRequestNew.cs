using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.RequestModel
{
    public class IrisOfferRequestNew : RetailerRequestV2
    {
        private string _amount = string.Empty;
        public string amount { get { return _amount; } set { _amount = string.IsNullOrWhiteSpace(value) ? string.Empty : value; } }

        [Required]
        public string subscriberNo { get; set; }
        public bool isAmarOffer { get; set; }
        public int acquisition { get; set; }
        public int simReplacement { get; set; }

        [JsonIgnore]
        public string userAgent { get; set; }
    }
}