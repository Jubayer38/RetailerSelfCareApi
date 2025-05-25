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

namespace Domain.RequestModel
{
    public class DigitalServiceSubmitRequest : RetailerRequestV2
    {
        public int productId { get; set; }
        public string subscriberNumber { get; set; }
        public string amount { get; set; }
        public int paymentType { get; set; }
        public string userPin { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
}