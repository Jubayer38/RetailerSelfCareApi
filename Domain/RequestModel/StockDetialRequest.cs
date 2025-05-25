namespace Domain.RequestModel
{
    public class StockDetialRequest
    {
        public string sessionToken { get; set; }
        public string retailerCode { get; set; }
        public int itemCode { get; set; }
        public string userPin { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
