namespace Domain.RequestModel
{
    public class ItopUpXmlRequestV2
    {
        public string Url { get; set; }
        public string Type { get; set; }
        /// <summary>
        /// RSO Number
        /// </summary>
        public string Msisdn { get; set; }
        public string Pin { get; set; }
        public string Loginid { get; set; }
        public string Password { get; set; }
        public string DateTime { get; set; }
        public string Imei { get; set; }
        /// <summary>
        /// Retailer Number
        /// </summary>
        public string Msisdn2 { get; set; }
        public string Language1 { get; set; }
        public string Extrefnum { get; set; }
    }
}
