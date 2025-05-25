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
    public class ChangePasswordRequestsV2
    {
        [Required]
        public string sessionToken { get; set; }

        /// <summary>
        /// The cred which currently used by user
        /// </summary>
        /// 
        [Required]
        public string oldPassword { get; set; }

        /// <summary>
        /// The new cred which will be applied for next login
        /// </summary>
        [Required]
        public string newPassword { get; set; }
        [Required]
        public string retailerCode { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
