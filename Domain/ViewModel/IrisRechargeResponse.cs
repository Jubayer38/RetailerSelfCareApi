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
    public class IrisRechargeResponse
    {
        public IrisInnerResponse response { get; set; }
    }

    public class IrisInnerResponse
    {
        public string transactionId { get; set; }
        public string statusCode { get; set; }
        public string statusMessage { get; set; }
    }
}
