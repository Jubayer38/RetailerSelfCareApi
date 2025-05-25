

///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Database connections static model
///	Creation Date :	13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.StaticClass
{
    public class Connections
    {
        public static string DefaultCS { get; set; }
        public static string DMSCS { get; set; }
        public static string RetAppDbCS { get; set; }
        public static string REDISCS { get; set; }
        public static string RetAppMySqlCS { get; set; }
    }
}
