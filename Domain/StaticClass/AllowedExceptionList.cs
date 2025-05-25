///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Description
///	Creation Date :	DD-MMM-YYYY
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.StaticClass
{
    public static class AllowedExceptionList
    {
        public static List<string> Routes
        {
            get
            {
                return
                [
                    "/api/Security/Login"
                ];
            }
        }
    }
}