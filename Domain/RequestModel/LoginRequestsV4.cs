///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Login Request Model
///	Creation Date :	03-Jan-2024
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
    public class LoginRequestsV4
    {
        /// <summary>
        /// Reseller's iTopUpNumber number
        /// </summary>
        [Required]
        public string iTopUpNumber { get; set; }

        /// <summary>
        /// The authentication cred
        /// </summary>
        [Required]
        public string password { get; set; }

        /// <summary>
        /// Reseller's device(TAB) device number provided by device system.
        /// </summary>
        [Required]
        public string deviceId { get; set; }

        /// <summary>
        /// Reseller App Language that will be seen on UI (i.e. Bangla, English).
        /// </summary>
        [Required]
        public string lan { get; set; }

        /// <summary>
        /// Reseller app Apk version Code (i.e. 209). 
        /// </summary>
        [Required]
        public int versionCode { get; set; }

        /// <summary>
        /// Reseller app Apk version name (i.e. "14.0.7").
        /// </summary>
        [Required]
        public string versionName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int type { get; set; }

        /// <summary>
        /// Reseller app andriod operating system's version (i.e. "4.4.2").
        /// </summary>
        [Required]
        public string osVersion { get; set; }

        /// <summary>
        /// Reseller app andriod operating system's kernel version (i.e. "19").
        /// </summary>
        //[Required]
        public string kernelVersion { get; set; }

        /// <summary>
        /// Reseller app andriod operating system's fermware version (i.e. "215.90.172.87").
        /// </summary>
        public string fermwareVersion { get; set; }

        /// <summary>
        /// Reseller's device name or model provided by device company
        /// </summary>
        //[Required]
        public string deviceModel { get; set; }

        /// <summary>
        /// The authentication OTP
        /// </summary>
        //[Required]
        public string otp { get; set; }

        /// <summary>
        /// Device location Latitude
        /// </summary>
        //[Required]
        public double lat { get; set; }

        /// <summary>
        /// Device location Longitude
        /// </summary>
        //[Required]
        public double lng { get; set; }

        public string userName { get; set; }

        public string appToken { get; set; } = string.Empty;
    }
}
