///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Static Models to hold JWT token generation keys
///	Creation Date :	20-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.StaticClass
{
    public class JweKeysModel
    {
        public static string TokenSignKey { get; set; }
        public static string TokenKey { get; set; }
        public static string Issuer { get; set; }
        public static string Audience { get; set; }
    }
}
