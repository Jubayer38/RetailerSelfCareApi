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


namespace Domain.ViewModel
{
    public class DeviceInfo
    {
        public string retailerCode { get; set; }

        public string deviceId { get; set; }

        public bool isPrimary { get; set; }

        public bool isEnable { get; set; }

        public string ipAddress { get; set; }

        public string machineName { get; set; }

        public string versionCode { get; set; }

        public string versionName { get; set; }

        public string osVersion { get; set; }

        public string kernelVersion { get; set; }

        public string fermwareVersion { get; set; }

        public long platformId { get; set; }

        public string imeiNumber { get; set; }

        public string deviceModel { get; set; }

        public double lat { get; set; } = 0.0;

        public double lng { get; set; } = 0.0;

        public int createdBy { get; set; }

        public int updateddBy { get; set; }

        public string iTopUpNumber { get; set; }
    }
}
