namespace Domain.RequestModel
{
    public class ProductCategoryRequest
    {
        public string sessionToken { get; set; }
        public string retailerCode { get; set; }
        public string type { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
