///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	08-Jan-2024
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
    public class SendRechargeOfferRequest : RetailerRequestV2
    {
        [Required]
        public string subscriberNo { get; set; }

        private string _amount = string.Empty;
        public string amount { get { return _amount; } set { _amount = string.IsNullOrWhiteSpace(value) ? string.Empty : value; } }

        [JsonIgnore]
        public string userAgent { get; set; }
    }
}