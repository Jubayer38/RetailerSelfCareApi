namespace Domain.ResponseModel
{
    public class EVPinLessBalanceResponse
    {
        public string Type { get; set; }
        public string TxnStatus { get; set; }
        public string Message { get; set; }
        public string DateTime { get; set; }
        public string ExtRefNum { get; set; }

        public EVPinLessBalanceResponse() { }

        public EVPinLessBalanceResponse(string response)
        {
            Dictionary<string, string> keyValuePairs = response.Split('&').ToDictionary(item => item.Split('=')[0], item => item.Split('=')[1]);

            if (keyValuePairs.TryGetValue("TYPE", out string _type))
                Type = _type;

            if (keyValuePairs.TryGetValue("TXNSTATUS", out string _txnStatus))
                TxnStatus = _txnStatus;

            if (keyValuePairs.TryGetValue("MESSAGE", out string _message))
                Message = _message;

            if (keyValuePairs.TryGetValue("DATETIME", out string _datetime))
                DateTime = _datetime;

            if (keyValuePairs.TryGetValue("EXTREFNUM", out string _extRefNum))
                ExtRefNum = _extRefNum;
        }
    }
}
