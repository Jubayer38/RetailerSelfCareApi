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

using Domain.StaticClass;
using System.Text.Json.Serialization;

namespace Domain.LMS.Request
{
    public class CommonLMSRequest
    {
        public string msisdn { get; set; }
        public string transactionID { get; set; }
        public string language { get; set; } = "EN";
        public string channel { get; set; } = LMSKyes.LmsChannel;
        public string description { get; set; } = "RETAILERAPP";

        [JsonIgnore]
        public string retailerCode { get; set; }
    }
}