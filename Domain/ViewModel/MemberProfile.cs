namespace Domain.ViewModel
{
    public class MemberProfile
    {
        public string msisdn { get; set; }
        public string transactionID { get; set; }
        public string statusCode { get; set; }
        public string statusMsg { get; set; }
        public string responseDateTime { get; set; }
        public LoyaltyProfileInfo loyaltyProfileInfo { get; set; }
    }
}
