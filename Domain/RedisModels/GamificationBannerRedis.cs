///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Basher Sarkar
///	Purpose	            :	
///	Creation Date       :   20-Feb-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************

namespace Domain.RedisModels
{
    public class GamificationBannerRedis
    {
        public long bannerId { get; set; }
        public string name { get; set; } = string.Empty;
        public string rulesName { get; set; } = string.Empty;
        public int bannerType { get; set; }
        public int viewPosition { get; set; }
        public string date { get; set; } = string.Empty;
        public string imagePath { get; set; } = string.Empty;
        public string labelName { get; set; } = string.Empty;
        public string labelNameBn { get; set; } = string.Empty;
        public string hintsText { get; set; } = string.Empty;
        public string hintsTextBn { get; set; } = string.Empty;
        public string buttonText { get; set; } = string.Empty;
        public string buttonTextBn { get; set; } = string.Empty;
        public long dateInMS { get; set; }
    }
}