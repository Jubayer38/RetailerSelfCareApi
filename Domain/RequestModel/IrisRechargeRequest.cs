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

using System.ComponentModel.DataAnnotations;


namespace Domain.RequestModel
{
    public class IrisRechargeRequest
    {
        [Required]
        public string sessionToken { get; set; }

        [Required]
        public string retailerCode { get; set; }

        [Required]
        public string amount { get; set; }

        [Required]
        public string subscriberNo { get; set; }

        [Required]
        public string tranId { get; set; }

        public string ussdCode { get; set; }

        public string email { get; set; }

        public double lat { get; set; }

        public double lng { get; set; }

        [Required]
        public string offerId { get; set; }

        [Required]
        public string pin { get; set; }

        public string denoValidity { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
