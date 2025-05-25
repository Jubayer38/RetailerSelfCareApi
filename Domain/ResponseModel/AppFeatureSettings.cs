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

using Domain.ViewModel;
using System.ComponentModel;
using System.Data;


namespace Domain.ResponseModel
{
    public class AppFeatureSettings
    {
        [DefaultValue(false)]
        public bool isEnableInDeviceRegi { get; set; }

        [DefaultValue(false)]
        public bool isEnableInLogin { get; set; }

        [DefaultValue(false)]
        public bool isEnableInRecharge { get; set; }

        [DefaultValue(false)]
        public bool isEmailEnable { get; set; }

        [DefaultValue(null)]
        public string selfServiceDialCode { get; set; }

        [DefaultValue(false)]
        public bool isEnableEVPinReset { get; set; }

        [DefaultValue(false)]
        public bool isEnablePopUpSurvey { get; set; }

        [DefaultValue(false)]
        public bool isEnableFlashPopUp { get; set; }

        [DefaultValue(false)]
        public bool isEnablePOSM { get; set; }
        public string initiationTypeInAdvertisement { get; set; } = string.Empty;
        public string advertLastUpdateTime { get; set; } = string.Empty;


        [DefaultValue(false)]
        public bool hasAdvertisement { get; set; }

        [DefaultValue(false)]
        public bool IsLMSFeatureEnable { get; set; }

        public List<AppBannerIDDatesVM> bannerUpdatedInMSList { get; set; } = [];
        public List<AppBannerIDDatesVM> gamificationBannerUpdatedMSList { get; set; } = [];

        #region Constructor
        public AppFeatureSettings() { }

        public AppFeatureSettings(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];

                isEnableInDeviceRegi = Convert.ToBoolean(dr["IS_REQUIRED_LC_DR"]);
                isEnableInLogin = Convert.ToBoolean(dr["IS_REQUIRED_LC_LOGIN"]);
                isEnableInRecharge = Convert.ToBoolean(dr["IS_REQUIRED_LC_RECHARGE"]);
                isEmailEnable = Convert.ToBoolean(dr["IS_ENABLE_EMAIL_SENDING"]);
                selfServiceDialCode = dr["SELF_SERVICE_DIAL_CODE"] as string;
                isEnableEVPinReset = Convert.ToBoolean(dr["IS_ENABLE_EV_PIN_RESET"]);
                isEnablePopUpSurvey = Convert.ToBoolean(dr["IS_ENABLE_POPUP_SURVEY"]);
                isEnableFlashPopUp = Convert.ToBoolean(dr["IS_ENABLE_FLASH_POPUP"]);
                initiationTypeInAdvertisement = "";
                advertLastUpdateTime = "";
            }
        }

        #endregion

    }
}
