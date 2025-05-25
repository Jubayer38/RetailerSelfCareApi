
using Domain.LMS.Request;

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
namespace Domain.LMS
{
    public class LMSRedeemReward : CommonLMSRequest
    {
        public string rewardID { get; set; }

        public string redeemFor { get; set; }

        public string redeemForMsisdn { get; set; }

        public string redeemAmount { get; set; }

        public string billAmount { get; set; }
    }
}