///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Xml.Serialization;


namespace Domain.ResponseModel
{
    [XmlRoot("COMMAND")]
    public class EvPinChangeXMLResponse
    {
        [XmlElement("TYPE")]
        public string type { get; set; }

        [XmlElement("TXNSTATUS")]
        public string txnStatus { get; set; }

        [XmlElement("MESSAGE")]
        public string message { get; set; }
    }
}
