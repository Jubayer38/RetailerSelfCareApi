///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	04-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.ViewModel
{
    public class UserLogInAttempt
    {
        public int login_attempt_id { get; set; }
        public string userid { get; set; } = string.Empty;
        public DateTime attempt_time { get; set; }
        public int is_success { get; set; }
        public string ip_address { get; set; } = string.Empty;
        public string machine_name { get; set; } = string.Empty;
        public string loginprovider { get; set; } = string.Empty;
        public string deviceid { get; set; } = string.Empty;
        public string lan { get; set; } = string.Empty;
        public int versioncode { get; set; }
        public string versionname { get; set; } = string.Empty;
        public string osversion { get; set; } = string.Empty;
        public string kernelversion { get; set; } = string.Empty;
        public string fermwarevirsion { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
        public string version { get; set; } = string.Empty;
        public string imei { get; set; } = string.Empty;
        public string devicemodel { get; set; } = string.Empty;
        public double lat { get; set; }
        public double lng { get; set; }
    }
}