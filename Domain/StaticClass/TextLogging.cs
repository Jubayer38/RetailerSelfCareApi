///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Text Log View Model
///	Creation Date :	18-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.StaticClass
{
    public class TextLogging
    {
        public static string TextLogPath { get; set; }
        public static string ApplicationTitle { get; set; }
        public static bool IsEnableApiTextLog { get; set; }
        public static bool IsEnableEVTextLog { get; set; }
        public static bool IsEnableIrisTextLog { get; set; }
        public static bool IsEnableExternalTextLog { get; set; }
        public static bool IsEnableErrorTextLog { get; set; }
        public static bool IsEnableApiDetailsLog { get; set; }
        public static bool IsEnableEVDetailsLog { get; set; }
        public static bool IsEnableIRISDetailsLog { get; set; }
        public static bool IsEnableExternalDetailsLog { get; set; }
        public static bool IsEnableTextLogToDms { get; set; }
        public static bool IsEnableDetailsTextLogToDms { get; set; }
        public static bool IsEnableRechargeApiExternalLog { get; set; }
        public static bool IsEnableRechargeApiExternalDetailLog { get; set; }
        public static bool IsEnableOfferSMSSending { get; set; }
    }
}
