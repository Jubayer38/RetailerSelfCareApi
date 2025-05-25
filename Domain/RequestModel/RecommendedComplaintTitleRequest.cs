

using Newtonsoft.Json;

namespace Domain.RequestModel
{
    public class RecommendedComplaintTitleRequest
    {
        public string requestId { get; set; } = Guid.NewGuid().ToString();
        public string description { get; set; } = string.Empty;
        [JsonIgnore]
        public string retailerCode { get; set; } = string.Empty;

    }
}
