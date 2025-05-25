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


namespace Domain.ViewModel
{
    public class UserLogInOTPLessAttempt
    {
        public int login_attempt_id { get; set; }
        public string userid { get; set; }
        public DateTime attempt_time { get; set; }
        public int is_success { get; set; }
        public string ip_address { get; set; }
        public string machine_name { get; set; }
        public string loginprovider { get; set; }
        public string deviceid { get; set; }
        public string lan { get; set; }
        public int versioncode { get; set; }
        public string versionname { get; set; }
        public string osversion { get; set; }
        public string kernelversion { get; set; }
        public string fermwarevirsion { get; set; }
        public string version { get; set; }
    }
}
