using System.Xml.Serialization;

namespace Domain.ViewModel
{
    [Serializable, XmlRoot("COMMAND")]
    public class C2CHistMainResp
    {
        [XmlElement(ElementName = "TYPE")]
        public string TYPE { get; set; }

        [XmlElement(ElementName = "REQSTATUS")]
        public string REQSTATUS { get; set; }

        [XmlElement(ElementName = "DATE")]
        public string DATE { get; set; }

        [XmlElement(ElementName = "EXTREFNUM")]
        public string EXTREFNUM { get; set; }

        [XmlElement(ElementName = "MESSAGE")]
        public string MESSAGE { get; set; }

        [XmlElement(ElementName = "TXNDETAILS")]
        public TXNDetails TXNDETAILS { get; set; }
    }
}
