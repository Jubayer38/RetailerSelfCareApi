///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	16-Jan-2024
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
    public class DeviceStatusRequest
    {
        [Required]
        public string sessionToken { get; set; }

        [Required]
        public string retailerCode { get; set; }

        /// <summary>
        /// Primary Device Id.
        /// </summary>
        [Required]
        public string deviceId { get; set; }

        [Required]
        public string operationType { get; set; }

        /// <summary>
        /// Target device id where activities (Deregister, SecondaryToPrimary, Enable, Disable) will done.
        /// </summary>
        [Required]
        public string operationalDeviceId { get; set; }

        public int deviceStatus { get; set; }

        public int userId { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
