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

using System.Data;
using System.Security.Claims;

namespace Domain.StaticClass
{
    public static class UserSession
    {
        public static int userId { get; set; }

        public static string retailerCode { get; set; }

        public static string iTopUpNumber { get; set; }

        public static string loginProvider { get; set; }

        public static bool isLoggedIn { get; set; }

        public static bool isDeviceEnable { get; set; }

        public static void InitSession(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];

                userId = Convert.ToInt32(row["USERID"].ToString());
                retailerCode = row["USERNAME"].ToString();
                iTopUpNumber = row["MOBILE_NUMBER"].ToString();
                loginProvider = row["LOGINPROVIDER"].ToString();
                isLoggedIn = Convert.ToBoolean(row["ISLOGGEDIN"]);
                isDeviceEnable = Convert.ToBoolean(row["IS_DEVICE_ENABLE"]);
            }
            else
            {
                userId = 0;
                retailerCode = null;
                iTopUpNumber = null;
                loginProvider = null;
                isLoggedIn = false;
                isDeviceEnable = false;
            }
        }

        public static void InitSessionNew(ClaimsIdentity claims)
        {
            Claim userIdClaim = claims.FindFirst("userId");
            Claim lpClaim = claims.FindFirst("jti");
            Claim rcClaim = claims.FindFirst("retailerCode");

            int.TryParse(userIdClaim?.Value, out int _userId);
            userId = _userId;

            retailerCode = rcClaim?.Value;
            loginProvider = lpClaim?.Value;
        }
    }
}