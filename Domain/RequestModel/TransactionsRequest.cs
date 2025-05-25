///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Al Mamun
///	Purpose	            :	
///	Creation Date       :   10-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     28-Jan-2024		Basher Sarkar	7.0.0		add summary
///	 ----------------------------------------------------------------
///	*****************************************************************

namespace Domain.RequestModel
{
    public class TransactionsRequest : RetailerRequestV2
    {
        private static readonly DateTime _dt;
        private static readonly int dayCount = 29;
        private static readonly DateTime today = DateTime.Now;
        private static readonly DateTime prvDay = DateTime.Now.AddDays(-dayCount);
        private static DateTime _startDate = new(prvDay.Year, prvDay.Month, prvDay.Day);
        private static DateTime _endDate = new(today.Year, today.Month, today.Day);

        public string subscriberNo { get; set; }

        public string sortByDate { get; set; }

        public string sortByAmt { get; set; }

        public string sortByInOut { get; set; }

        public DateTime startDate { get { return _startDate; } set { _startDate = (value.Date == _dt.Date) ? _startDate : value; } }
        public DateTime endDate { get { return _endDate; } set { _endDate = (value.Date == _dt.Date) ? _endDate : value; } }
    }
}