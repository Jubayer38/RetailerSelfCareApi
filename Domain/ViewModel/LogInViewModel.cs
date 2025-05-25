///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	LoginSmartPos Controller
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.ViewModel
{
    public class LogInViewModel
    {
        public string SessionToken { get; set; }
        public bool ISAuthenticate { get; set; }
        public string AuthenticationMessage { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public bool HasUpdate { get; set; }
        public string MinimumScore { get; set; }
        public string OptionalMinimumScore { get; set; }
        public string MaximumRetry { get; set; }
        public string RoleAccess { get; set; }
        public int? ChannelId { get; set; }
        public string ChannelName { get; set; }
        public int InventoryId { get; set; }
        public string CenterCode { get; set; }
    }
}
