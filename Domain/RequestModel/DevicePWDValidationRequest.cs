///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	09-Jan-2024
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
    public class DevicePWDValidationRequest
    {
        /// <summary>
        /// User mobile number
        /// </summary>
        [Required]
        public string iTopUpNumber { get; set; }

        /// <summary>
        /// User login cred
        /// </summary>
        [Required]
        public string password { get; set; }

        /// <summary>
        /// Device ID for OTP
        /// </summary>
        [Required]
        public string deviceId { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
