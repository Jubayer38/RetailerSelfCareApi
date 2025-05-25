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

using Domain.Helpers;
using Domain.LMS.Response;
using System.Globalization;


namespace Domain.LMS
{
    public class LMSPointHist
    {
        public string title { get; set; }
        public string date { get; set; }
        public string points { get; set; }
        public string action { get; set; }
        public DateTime queryDate { get; set; }

        public LMSPointHist(LMSEarnHistory instance)
        {
            title = instance.transactionID;
            queryDate = DateTime.ParseExact(instance.transactionDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            date = queryDate.ToEnUSDateString("hh:mm tt, dd MMM yyyy");
            points = instance.loyaltyPoints;
            action = instance.transactionType;
        }

        public LMSPointHist(LMSRedeemHistory instance)
        {
            title = instance.transactionID;
            queryDate = DateTime.ParseExact(instance.transactionDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            date = queryDate.ToEnUSDateString("hh:mm tt, dd MMM yyyy");
            points = instance.loyaltyPoints;
            action = instance.transactionType;
        }
    }
}