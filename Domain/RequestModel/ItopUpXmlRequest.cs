namespace Domain.RequestModel
{
    public class ItopUpXmlRequest
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string Extnwcode { get; set; }
        /// <summary>
        /// Retailer No
        /// </summary>
        public string Msisdn { get; set; }
        public string Pin { get; set; }
        public string Loginid { get; set; }
        public string Pass { get; set; }
        public string Extcode { get; set; }
        public string Extrefnum { get; set; }
        /// <summary>
        /// Subscriber No
        /// </summary>
        public string Msisdn2 { get; set; }
        public string Amount { get; set; }
        public string Language1 { get; set; }
        public string Language2 { get; set; }
        public string Selector { get; set; }
    }
}
