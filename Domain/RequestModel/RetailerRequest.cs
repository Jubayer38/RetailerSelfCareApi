///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Retailer App common request model
///	Creation Date :	19-Dec-2023
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
    /// <summary>
    /// Common request model for Retailer App
    /// </summary>
    public class RetailerRequest
    {
        [Required]
        public string sessionToken { get; set; }

        [Required]
        public string deviceId { get; set; }

        [Required]
        public string retailerCode { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }


    /// <summary>
    /// Extend common request model for jwt session validation. In jwt validation request will restricted by device.
    /// </summary>
    public class RetailerRequestV2 : RetailerRequest
    {
        [Required]
        public string iTopUpNumber { get; set; }
    }

}