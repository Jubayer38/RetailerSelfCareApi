///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Response Message static keys
///	Creation Date :	30-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.StaticClass
{
    public class ResponseMessages
    {
        public static string InvalidSessionMsg { get; set; }
        public static string ValidSessionMsg { get; set; }
        public static string InvalidUserName { get; set; }
        public static string InvalidUserCred { get; set; }
        public static string UserSuccessfullyValidated { get; set; }
        public static string UserNotFound { get; set; }
        public static string LoginSuccess { get; set; }
        public static string LoginFailed { get; set; }
        public static string EvPinRestNotifiTitle { get; set; }
        public static string EvPinRestNotifiDetails { get; set; }
        public static string CredPSentToMobile { get; set; }
        public static string InvalidCredP { get; set; }
    }
}