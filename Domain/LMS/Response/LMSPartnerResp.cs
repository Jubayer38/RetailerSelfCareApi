///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	
///	Purpose	      : 
///	Creation Date :	
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No. Date:		Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************


namespace Domain.LMS.Response
{
    public class LMSPartnerResp
    {
        public string msisdn { get; set; }
        public string transactionID { get; set; }
        public string statusCode { get; set; }
        public string statusMsg { get; set; }
        public string responseDateTime { get; set; }
        public string availablePoints { get; set; }
        public List<LMSPartner> partnerArray { get; set; }
    }
}