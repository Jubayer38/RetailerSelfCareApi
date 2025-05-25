///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	10-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.StaticClass
{
    public sealed class LmsApiEndPoints
    {
        public static string getMemberProfile { get { return "/loyalty2/get-member-profile"; } }
        public static string getPartners { get { return "/loyalty2/get-partners"; } }
        public static string getPartnerShop { get { return "/loyalty2/get-partner-shops"; } }
        public static string getRewardList { get { return "/loyalty2/get-reward-list"; } }
        public static string getPointHistory { get { return "/loyalty2/get-point-history"; } }
        public static string getRedeemReward { get { return "/loyalty2/redeem-reward"; } }
        public static string getAdjustPoints { get { return "/loyalty2/adjust-points"; } }
        public static string getCombinedPointHistory { get { return "/loyalty2/combined-point-history"; } }
        public static string getMemberNextTier { get { return "/loyalty2/get-member-next-tier"; } }
    }
}
