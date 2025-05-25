///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	04-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.StaticClass
{
    public class FeatureStatus
    {
        public static bool IsFlashPopUpEnable { get; set; }
        public static bool IsSurveyPopUpEnable { get; set; }
        public static bool IsEnableSCSales { get; set; }
        public static bool IsEnablePOSM { get; set; }
        public static string PopUpMethodsRestrictTime { get; set; }
        public static string PopUpMethodsCallingSlot { get; set; }
        public static bool IsNewFlashPopUpEnable { get; set; }
        public static bool IsNewPopUpSurveyEnable { get; set; }
        public static bool IsEnableOfferSMSSending { get; set; }
        public static bool IsLMSFeatureEnable { get; set; }
    }
}
