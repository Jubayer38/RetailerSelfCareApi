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
    public class LoginRequests
    {
        /// <summary>
        /// User name for request
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The authentication cred
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// Reseller's device(TAB) IMEI number.
        /// </summary>
        //[Required]
        public string DeviceId { get; set; }
        /// <summary>
        /// Reseller App Language that will be seen on UI (i.e. Bangla, English).
        /// </summary>
        [Required]
        public string Lan { get; set; }
        /// <summary>
        /// Reseller app Apk version Code (i.e. 209). 
        /// </summary>
        [Required]
        public int VersionCode { get; set; }
        /// <summary>
        /// Reseller app Apk version name (i.e. "14.0.7").
        /// </summary>
        [Required]
        public string VersionName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int Type { get; set; }
        /// <summary>
        /// Reseller app andriod operating sysytem's version (i.e. "4.4.2").
        /// </summary>
        [Required]
        public string OSVersion { get; set; }
        /// <summary>
        /// Reseller app andriod operating sysytem's kernel version (i.e. "19").
        /// </summary>
        [Required]
        public string KernelVersion { get; set; }
        /// <summary>
        /// Reseller app andriod operating sysytem's ferware version (i.e. "215.90.172.87").
        /// </summary>
        public string FermwareVersion { get; set; }
    }
}
