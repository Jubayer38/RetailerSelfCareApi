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

namespace Domain.RequestModel
{
    /// <summary>
    /// OTPGenerate new request
    /// </summary>
    public class OTPGenerateRequest
    {
        /// <summary>
        /// Reseller's iTopUpNumber number
        /// </summary>
        [Required]
        public string iTopUpNumber { get; set; }

        /// <summary>
        /// Reseller's device(TAB) device number provided by device system.
        /// </summary>
        [Required]
        public string deviceId { get; set; }

        public bool isNewOTP { get; set; }

        public string moduleName { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}