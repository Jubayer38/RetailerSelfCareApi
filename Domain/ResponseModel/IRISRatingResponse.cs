///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	15-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.ResponseModel
{
    public class IRISRatingResponse
    {
        public string title { get; set; }

        public string offerType { get; set; } = "Amar Offer";

        public string category { get; set; } = string.Empty;

        public string dataPack { get; set; }

        public string talkTime { get; set; }

        public string sms { get; set; }

        public string toffee { get; set; }

        public string validity { get; set; }

        public int commission { get; set; }

        public int amount { get; set; }

        public string rechargeType { get; set; } = "IRIS";

        public bool isRated { get; set; } = false;

        public int rating { get; set; } = 0;

        public string ratingDate { get; set; } = string.Empty;
    }
}
