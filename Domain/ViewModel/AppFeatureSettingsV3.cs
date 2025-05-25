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

using System.ComponentModel;
using System.Data;


namespace Domain.ViewModel
{
    public class AppFeatureSettingsV3
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
        public bool isEnableRaiseComplaint { get; set; }

        [DefaultValue(false)]
        public bool isEnableRetailerChoice { get; set; }

        [DefaultValue(false)]
        public bool isEnableEVPinReset { get; set; }

        [DefaultValue(false)]
        public bool isEnablePopUpSurvey { get; set; }

        [DefaultValue(false)]
        public bool isEnableFlashPopUp { get; set; }

        [DefaultValue(false)]
        public bool isEnablePOSM { get; set; }


        public List<AppBannerIDDatesVM> bannerUpdatedInMSList { get; set; } = new List<AppBannerIDDatesVM>();

        public AppFeatureSettingsV3() { }

        public AppFeatureSettingsV3(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];

                isEnableInDeviceRegi = Convert.ToBoolean(dr["IS_REQUIRED_LC_DR"]);
                isEnableInLogin = Convert.ToBoolean(dr["IS_REQUIRED_LC_LOGIN"]);
                isEnableInRecharge = Convert.ToBoolean(dr["IS_REQUIRED_LC_RECHARGE"]);
                isEmailEnable = Convert.ToBoolean(dr["IS_ENABLE_EMAIL_SENDING"]);
                selfServiceDialCode = dr["SELF_SERVICE_DIAL_CODE"] as string;
                isEnableRaiseComplaint = Convert.ToBoolean(dr["IS_ENABLE_RAISE_COMPLAINT"]);
                isEnableRetailerChoice = Convert.ToBoolean(dr["IS_ENABLE_RETAILER_CHOICE"]);
                isEnableEVPinReset = Convert.ToBoolean(dr["IS_ENABLE_EV_PIN_RESET"]);
                isEnablePopUpSurvey = Convert.ToBoolean(dr["IS_ENABLE_POPUP_SURVEY"]);
                isEnableFlashPopUp = Convert.ToBoolean(dr["IS_ENABLE_FLASH_POPUP"]);
            }
        }

    }
}
