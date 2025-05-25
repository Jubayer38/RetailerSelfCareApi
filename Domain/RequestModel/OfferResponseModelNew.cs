namespace Domain.RequestModel
{
    public class OfferResponseModelNew
    {
        public string statusCode { get; set; }
        public string statusMessage { get; set; }
        public string transactionId { get; set; }
        public bool isUSIM { get; set; }
        public List<OfferModelNew> OfferList { get; set; } = [];
    }
}