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


namespace Domain.ResponseModel
{
    public class LogInOTPLessResponse
    {
        /// <summary>
        /// Security token. A valid token can be used to request other API.  
        /// </summary>
        public string SessionToken { get; set; }
        /// <summary>
        /// Contains true or false. Is the user is valid/ authenticated then returns true, other wise false.
        /// </summary>
        public bool ISAuthenticate { get; set; }
        /// <summary>
        /// Contains user validation message.(i.e. "User Successfully Validated.")
        /// </summary>
        public string AuthenticationMessage { get; set; }
        /// <summary>
        /// Reseller user name. (i.e. "201949")
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// User cred (encripted) (i.e. "dsasbda6567ara")
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Device IMEI number.
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// Unknown usage
        /// </summary>
        public bool HasUpdate { get; set; }
        /// <summary>
        /// Fingure print minimum score. While captureing FP for submitting order 
        /// by FP device through reseller app, this value is used (i.e. "65"). 
        /// </summary>
        public string MinimumScore { get; set; }
        /// <summary>
        /// Unknown usage
        /// </summary>
        public string OptionalMinimumScore { get; set; }
        /// <summary>
        /// Unknown usage 
        /// </summary>
        public string MaximumRetry { get; set; }
        /// <summary>
        /// Contains role right access id.
        /// </summary>
        public string RoleAccess { get; set; }
        /// <summary>
        /// Reseller channel id.
        /// </summary>
        public int? ChannelId { get; set; }
        /// <summary>
        /// Reseller channel name (i.e. "RESELLER", "Corporate")
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// Reseller inventory id.
        /// </summary>
        public int InventoryId { get; set; }
        /// <summary>
        /// Reseller center code.
        /// </summary>
        public string CenterCode { get; set; }
        /// <summary>
        /// Reseller app current API veraion.
        /// </summary>
        public string version { get; set; }
    }
}
