///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Validatoin Methods for API Methods
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************
///	


using Domain.Helpers;
using Domain.StaticClass;

namespace Application.Utils
{
    public class ApiManager
    {
        public static bool IsApkVersionBlockAsync(int versionCode, int versionName, string appToken)
        {
            bool isTokenSuccess = HelperMethod.IsTokenValid(versionName, appToken);

            //int.TryParse(TextLogging.isLowerVersionBlocked, out int isLowerVersionBlocked);
            //int.TryParse(AppAllowedVersion.block_lower_version_code_from, out int blockedLowerVersionCodeFrom);
            //int.TryParse(WebConfiguration.blockLowerVersioNameFrom, out int blockedLowerVersionNameFrom);

            bool isVCLock = versionCode <= AppAllowedVersion.block_lower_version_code_from;
            bool isVNLock = versionName <= AppAllowedVersion.block_lower_version_name_from;
            bool blockStatus = !isTokenSuccess || AppAllowedVersion.block_lower_version == 1 && (isVCLock || isVNLock);

            return blockStatus;
        }
    }
}
