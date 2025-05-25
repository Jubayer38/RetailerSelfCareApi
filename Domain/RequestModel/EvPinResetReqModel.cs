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

using System.Text.Json.Serialization;


namespace Domain.RequestModel
{
    public class EvPinResetReqModel
    {
        public string userName { get; set; }
        public string password { get; set; }
        public long resetReqId { get; set; }
        public string retailerCode { get; set; }
        public string iTopUpNumber { get; set; }
        public string pinResetReason { get; set; }

        [JsonIgnore]
        public string submitUrl { get; set; }
    }
}
