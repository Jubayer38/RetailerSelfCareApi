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


namespace Domain.ViewModel
{
    public class BannerDetailsRedis
    {
        public long bannerId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string date { get; set; }
        public long dateInMS { get; set; }
        public string bannerType { get; set; }
        public string imageUrl { get; set; }
        public string imageUrlLarge { get; set; }
    }
}
