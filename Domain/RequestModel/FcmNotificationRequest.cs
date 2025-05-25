
using System.Text.Json.Serialization;

///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Basher Sarkar
///	Purpose	            :	
///	Creation Date       :   1-Apr-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///	 ----------------------------------------------------------------
///	*****************************************************************
namespace Domain.RequestModel
{
    public class FcmNotificationRequest
    {
        public string sessionToken { get; set; }
        public string iTopUpNumber { get; set; }
    }


    public class FcmNotificationTemp
    {
        public string apiToken { get; set; }
        public string deviceIds { get; set; }
        public string notificationHeader { get; set; }
        public string notificationBody { get; set; }

        [JsonIgnore]
        public string iTopUpNumber { get; set; }
    }
}