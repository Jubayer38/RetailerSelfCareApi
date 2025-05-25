///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	15-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Domain.RequestModel
{
    public class LiftingRequest : RetailerRequest
    {
        public long? requestId { get; set; }
        public string category { get; set; }
        [Required]
        public string type { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        [Required]
        public string iTopUpNumber { get; set; }
        public string paymentType { get; set; }

        [JsonIgnore]
        public string? appVisibleType { get; set; }
    }
}
