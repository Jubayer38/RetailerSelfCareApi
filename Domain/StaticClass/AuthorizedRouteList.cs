///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	08-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.StaticClass
{
    public static class AuthorizedRouteList
    {
        public static List<string> Routes
        {
            get
            {
                return
                [
                    "/",
                    "/favicon.ico",
                    "/wwwroot/bootstrap_5_0_2/css/bootstrap.min.css",
                    "/wwwroot/bootstrap_5_0_2/js/bootstrap.bundle.min.js",
                    "/home/index",
                    "/api/UpdateRetailer",
                    "/api/GetAppSettings",
                    "/api/GetAppSettingsV2",
                    "/api/v2/GetAppSettings",
                    "/api/v2/GetAppSettingsV2",
                    "/api/Security/LoginOTPLess",
                    "/api/Security/DeviceValidation",
                    "/api/Security/DeviceValidationV2",
                    "/api/Security/RegisterWithChangePWD",
                    "/api/Security/DevicePWDValidation",
                    "/api/Security/RegisterWithOTPValidation",
                    "/api/Security/GetRepeatOTP",
                    "/api/Security/RequestNewDevice",
                    "/api/Security/Login",
                    "/api/Security/InternalLogin",
                    "/api/Security/Logout",
                    "/api/UploadFile",
                    "/api/v2/UploadFile",
                    "/api/UpdateRaiseComplaintStatus",
                    "/api/UpdateDigitalServiceStatus",
                    "/api/SyncRetailerInfo",
                    "/api/UpdStockReqDeliveredOrder",
                    "/api/UpdateEVPinResetStatus",
                    "/api/InstallationProcess",
                    "/home/SurveyView",
                    "/home/GetSurveyQuestionsByID",
                    "/home/SubmitSurveyResponse",
                    "/api/Security/ForgetPwd"
                ];
            }
        }
    }
}