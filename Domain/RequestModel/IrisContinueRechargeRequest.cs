///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.RequestModel
{
    public class IrisContinueRechargeRequest
    {
        public IrisContinueRequest request { get; set; }
    }


    public class IrisContinueRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string retailerMsisdn { get; set; }
        public string subscriberMsisdn { get; set; }
        public string rechargeAmount { get; set; }
        public string transactionID { get; set; }
        public string offerID { get; set; }
        public string pin { get; set; }
        public string channel { get; set; }
        public string gatewayCode { get; set; }
    }
}
