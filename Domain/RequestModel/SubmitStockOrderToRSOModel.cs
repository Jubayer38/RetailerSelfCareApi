

using System.Text.Json.Serialization;

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
    public class SubmitStockOrderToRSOModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public long RequestId { get; set; }
        public string RetailerCode { get; set; }
        public string ProductType { get; set; }
        public string ProductCode { get; set; }
        public int RequestProductCount { get; set; }
        public string RetailerMsisdn { get; set; }
        public string PaymentType { get; set; }

        [JsonIgnore]
        public string SubmitUrl { get; set; }
        [JsonIgnore]
        public string OriginMethodName { get; set; }
    }
}