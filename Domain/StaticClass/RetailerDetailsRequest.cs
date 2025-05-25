namespace Domain.StaticClass
{
    public class RetailerDetailsRequest
    {
        public string sessionToken { get; set; }
        public string retailerCode { get; set; }
        public string retailerName { get; set; }
        public string retailerAddress { get; set; }
        public string contactPersonName { get; set; }
        public string contactPersonNumber { get; set; }
        public string itopupNumber { get; set; }
        public string email { get; set; }
        public string DOB { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
