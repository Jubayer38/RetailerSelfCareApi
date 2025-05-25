namespace Domain.RequestModel
{
    public class ComplainRequest : RetailerRequest
    {
        public string title { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public List<string> images { get; set; }
        public string FileLocation { get; set; }
    }
}
