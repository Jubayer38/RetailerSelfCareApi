namespace Domain.ResponseModel
{
    public class SOResponse
    {
        public bool isSuccess { get; set; }
        public int errorCode { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }
}
