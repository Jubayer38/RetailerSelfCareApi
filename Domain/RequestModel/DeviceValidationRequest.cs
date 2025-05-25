///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	The model for maintain device validation
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
    public class DeviceValidationRequest
    {
        /// <summary>
        /// Reseller's iTopUp number
        /// </summary>
        [Required]
        public string iTopUpNumber { get; set; }

        /// <summary>
        /// Reseller's Device unique id provide by android system.
        /// </summary>
        [Required]
        public string deviceId { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
