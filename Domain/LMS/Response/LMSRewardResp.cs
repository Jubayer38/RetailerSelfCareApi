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
    public class LMSRewardResp
    {
        public string msisdn { get; set; }
        public string transactionID { get; set; }
        public string statusCode { get; set; }
        public string statusMsg { get; set; }
        public string responseDateTime { get; set; }
        public string rewardID { get; set; }
        public string pointsBefore { get; set; }
        public string pointsRedeemed { get; set; }
        public string pointsAvailable { get; set; }
        public string channel { get; set; }
        public string promoCode { get; set; }
        public string discountAmount { get; set; }
        public string qrCodeUrl { get; set; }
    }
}