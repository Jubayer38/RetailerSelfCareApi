using System.Xml.Serialization;

namespace Domain.ViewModel
{
    [XmlRoot("COMMAND")]
    public class EvXmlResponse
    {
        [XmlElement("TYPE")]
        public string type { get; set; }
        [XmlElement("TXNSTATUS")]
        public string txnStatus { get; set; }
        [XmlElement("DATE")]
        public string date { get; set; }

        [XmlElement("EXTREFNUM")]
        public string extRefNum { get; set; }
        [XmlElement("TXNID")]
        public string txnId { get; set; }
        [XmlElement("MESSAGE")]
        public string message { get; set; }
        [XmlElement("RECORD")]
        public Records Record { get; set; }
    }

    public class Records
    {
        [XmlElement("PRODUCTCODE")]
        public string productCode { get; set; }
        [XmlElement("PRODUCTSHORTNAME")]
        public string productShortName { get; set; }
        [XmlElement("BALANCE")]
        public string balance { get; set; }
    }
}
