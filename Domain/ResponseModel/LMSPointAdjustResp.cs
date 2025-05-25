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


namespace Domain.ResponseModel
{
    public class LMSPointAdjustResp
    {
        public string statusCode { get; set; }
        public string statusMsg { get; set; }
        public string transactionID { get; set; }
        public string msisdn { get; set; }
        public string responseDateTime { get; set; }
        public string membershipID { get; set; }
        public string points { get; set; }
        public string srcTransactionID { get; set; }
        public string totalPoints { get; set; }

        // Below property not include in lms response. created here for save into database so that no extra model initialize needed.
        public string retailerCode { get; set; }
        public string appPage { get; set; }
        public string adjustmentType { get; set; }
        public string description { get; set; }
    }
}
