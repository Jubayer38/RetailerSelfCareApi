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

namespace Domain.ResponseModel
{
    public class LogInResponse
    {
        /// <summary>
        /// Security token. A valid token can be used to request other API.  
        /// </summary>
        public string sessionToken { get; set; } = string.Empty;

        /// <summary>
        /// Contains true or false. Is the user is valid/ authenticated then returns true, other wise false.
        /// </summary>
        public bool isAuthenticate { get; set; }

        /// <summary>
        /// Contains user validation message.(i.e. "User Successfully Validated.")
        /// </summary>
        public string authenticationMessage { get; set; } = string.Empty;

        /// <summary>
        /// User cred (encripted) (i.e. "dsasbda6567ara")
        /// </summary>
        public string password { get; set; } = string.Empty;

        /// <summary>
        /// Reseller Device number.
        /// </summary>
        public string deviceId { get; set; } = string.Empty;

        /// <summary>
        /// Unknown usage
        /// </summary>
        public bool hasUpdate { get; set; }

        /// <summary>
        /// Fingure print minimum score. While captureing FP for submitting order 
        /// by FP device through reseller app, this value is used (i.e. "65"). 
        /// </summary>
        public string minimumScore { get; set; } = string.Empty;

        /// <summary>
        /// Unknown usage
        /// </summary>
        public string optionalMinimumScore { get; set; } = string.Empty;

        /// <summary>
        /// Unknown usage 
        /// </summary>
        public string maximumRetry { get; set; } = string.Empty;

        /// <summary>
        /// Contains role right access id.
        /// </summary>
        public string roleAccess { get; set; } = string.Empty;

        /// <summary>
        /// Reseller channel id.
        /// </summary>
        public int? channelId { get; set; }

        /// <summary>
        /// Reseller channel name (i.e. "RESELLER", "Corporate")
        /// </summary>
        public string channelName { get; set; } = string.Empty;

        /// <summary>
        /// Reseller inventory id.
        /// </summary>
        public int inventoryId { get; set; }

        /// <summary>
        /// Reseller center code.
        /// </summary>
        public string centerCode { get; set; } = string.Empty;

        /// <summary>
        /// Reseller app current API veraion.
        /// </summary>
        public string version { get; set; } = string.Empty;

        /// <summary>
        /// Reseller iTopUpNumber.
        /// </summary>
        public string iTopUpNumber { get; set; } = string.Empty;

        /// <summary>
        /// Reseller user name. (i.e. "R201949")
        /// </summary>
        public string retailerCode { get; set; } = string.Empty;

        public bool isPrimary { get; set; }

        public bool isRegistered { get; set; }

        public bool isDeviceEnable { get; set; }

        public string ErrorDetails { get; set; }
        public bool isThisVersionBlocked { get; set; }
        public string regionCode { get; set; } = string.Empty;
        public string regionName { get; set; } = string.Empty;
        public bool isEnableSCSales { get; set; }
    }

}