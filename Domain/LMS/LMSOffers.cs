///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	10-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.LMS
{
    public class LMSOffers
    {
        public string msisdn { get; set; }
        public string transactionID { get; set; }
        public string statusCode { get; set; }
        public string statusMsg { get; set; }
        public string responseDateTime { get; set; }
        public List<RewardDetails> rewardArray { get; set; }
    }
}
