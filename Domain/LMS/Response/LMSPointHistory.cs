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


namespace Domain.LMS.Response
{
    public class LMSPointHistory
    {
        public string statusCode { get; set; }
        public string statusMsg { get; set; }
        public string transactionID { get; set; }
        public string pointsToBeExpired { get; set; }
        public string totalPointsEarned { get; set; }
        public string totalPointsRedeemed { get; set; }
        public List<LMSEarnHistory> earnHistory { get; set; } = new List<LMSEarnHistory>();
        public List<LMSRedeemHistory> redeemHistory { get; set; } = new List<LMSRedeemHistory>();
    }
}
