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


namespace Domain.ViewModel
{
    public class IRISJSONRequestModel
    {
        public IRISOfferRequest request { get; set; }

    }

    /// <summary>
    /// Request Model for IRIS External API
    /// </summary>
    public class IRISOfferRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string retailerMsisdn { get; set; }
        public string channel { get; set; }
        public string subscriberMsisdn { get; set; }
        public string rechargeAmount { get; set; }
        public string gatewayCode { get; set; }
        public string transactionID { get; set; }
    }


}
