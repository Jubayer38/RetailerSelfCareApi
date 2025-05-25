///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Recharge Package Details data model for Redis Cache DB
///	Creation Date :	16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:		    Author:			Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************

namespace Domain.RedisModels
{
    public class RechargePackagesRedis
    {
        public long id { get; set; }
        public string title { get; set; }
        public string titleBn { get; set; }
        public string offerType { get; set; }
        public string offerTypeBn { get; set; }
        public string category { get; set; }
        public string categoryBn { get; set; }
        public string dataPack { get; set; }
        public string dataPackBn { get; set; }
        public string talkTime { get; set; }
        public string talkTimeBn { get; set; }
        public string sms { get; set; }
        public string smsBn { get; set; }
        public string toffee { get; set; }
        public string toffeeBn { get; set; }
        public string validity { get; set; }
        public string validityBn { get; set; }
        public int commission { get; set; }
        public int amount { get; set; }
        public string rechargeType { get; set; }
        public int isAcquisitionOffer { get; set; }
        public int isSimReplacement { get; set; }
    }
}