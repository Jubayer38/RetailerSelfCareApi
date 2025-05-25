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
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


using System.Text.Json.Serialization;

namespace Domain.LMS.Request
{
    public class LMSPointAdjustReq : CommonLMSRequest
    {
        [JsonIgnore]
        public string requestMethod { get; set; }

        [JsonIgnore]
        public string appPage { get; set; }
        public string points { get; set; }
        public string adjustmentType { get; set; }
    }
}