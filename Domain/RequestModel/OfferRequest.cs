namespace Domain.RequestModel
{
    public class OfferRequest : RetailerRequest
    {
        public int acquisition { get; set; }
        public int simReplacement { get; set; }
        public int rechargeType { get; set; }
    }
}