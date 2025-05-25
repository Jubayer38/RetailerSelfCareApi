///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Data;

namespace Domain.ResponseModel
{
    public class SecondaryDeviceListModel
    {
        public string deviceId { get; set; }
        public string deviceModel { get; set; }
        public string osVersion { get; set; }
        public bool isEnable { get; set; }
        public bool isLoggedIn { get; set; }
        public bool isPrimary { get; set; }


        public SecondaryDeviceListModel(DataRow dr)
        {
            deviceId = dr["DEVICEID"] as string;
            deviceModel = dr["DEVICEMODEL"] as string;
            osVersion = dr["OSVERSION"] as string;
            isEnable = Convert.ToBoolean(dr["IS_ENABLE"]);
            isLoggedIn = Convert.ToBoolean(dr["ISLOGGEDIN"]);
            isPrimary = Convert.ToBoolean(dr["IS_PRIMARY"]);
        }
    }
}