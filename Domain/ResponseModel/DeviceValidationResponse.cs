///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Device validation response type.
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.ResponseModel
{
    public class DeviceValidationResponse
    {
        public DeviceValidationResponse()
        {
            isSuccess = false;
            isRegistered = false;
            isPrimary = false;
            isDeviceLimitExceed = false;
            primaryDeviceModel = string.Empty;
            isSimSeller = false;
        }

        public bool isSuccess { get; set; }
        public bool isRegistered { get; set; }
        public bool isPrimary { get; set; }
        public bool isDeviceLimitExceed { get; set; }
        public bool isSimSeller { get; set; }
        public string primaryDeviceModel { get; set; }
        public string responseMessage { get; set; }
        public string ErrorDetails { get; set; }

    }
}
