///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      : 
///	Creation Date :	16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No. Date:		Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************

namespace Domain.RedisModels
{
    public class QuickAccessRedisModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string bengaliTitle { get; set; }
        public string iconBase64 { get; set; }
        public string iconBase64Dark { get; set; }
        public string redirectUrl { get; set; }
        public string packageName { get; set; }
        public string appAction { get; set; }
        public string appType { get; set; }
        public bool isWidgetActive { get; set; }
        public bool hasRight { get; set; }
    }
}