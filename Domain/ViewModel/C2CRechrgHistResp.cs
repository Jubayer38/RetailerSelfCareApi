using System.Xml.Serialization;

namespace Domain.ViewModel
{
    [Serializable()]
    [XmlRoot("TXNDETAILS")]
    public class TXNDetails
    {
        [XmlElement("TXNDETAIL")]
        public List<C2CRechrgHistResp> Transactions { get; set; }
    }


    [Serializable()]
    public class C2CRechrgHistResp
    {
        [XmlElement(ElementName = "TXNID")]
        public string transactionNumber { get; set; }

        [XmlElement(ElementName = "TXNDATETIME")]
        public string date { get; set; }

        [XmlElement(ElementName = "TRFTYPE")]
        public string transactionType { get; set; }

        [XmlElement(ElementName = "TXNSTATUS")]
        public string status { get; set; }

        [XmlElement(ElementName = "TXNAMOUNT")]
        public string amount { get; set; }

        [XmlElement(ElementName = "RECEIVERMSISDN")]
        public string msisdn { get; set; }

    }
}
