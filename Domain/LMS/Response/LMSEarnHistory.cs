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
    public class LMSEarnHistory
    {
        public string transactionID { get; set; }
        public string transactionDate { get; set; }
        public string transactionType { get; set; }
        public string loyaltyPoints { get; set; }
        public string pointsExpiryDate { get; set; }
        public string offerID { get; set; }
    }
}
