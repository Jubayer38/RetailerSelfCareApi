namespace Domain.ViewModel
{
    public class AdvertisementDetailsRedis
    {
        public long advertisementId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string externalUrl { get; set; }
        public string actionType { get; set; }
        public string date { get; set; }
        public string imageUrl { get; set; }
    }
}
