///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Notification Details data model for Redis Cache DB
///	Creation Date :	16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:		    Author:			Ver:	Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************

using System.Text.Json.Serialization;

namespace Domain.RedisModels
{
    public class NotificationDetailsRedis
    {
        public long id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string imageURL { get; set; }
        public string detailsURL { get; set; }
        public string pdfUrl { get; set; }
        public int isLiveTicker { get; set; }
        public int isReadOnly { get; set; }
        public string redirectToAction { get; set; }
        public string schedulerTimes { get; set; }
        public string dateTime { get; set; }
        public int isRead { get; set; }
        public string logoURL { get; set; } = string.Empty;
        public string imageURLLarge { get; set; }

        [JsonIgnore]
        public int notificationType { get; set; }
    }
}