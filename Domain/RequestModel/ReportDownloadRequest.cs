///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class ReportDownloadRequest : RetailerRequestV2
    {
        private static readonly DateTime _dt;
        private static readonly DateTime today = DateTime.Now;
        private static DateTime _startDate = new(today.Year, today.Month, 1);
        private static DateTime _endDate = new(today.Year, today.Month, today.Day);

        [Required]
        public string reportType { get; set; }

        public DateTime startDate { get { return _startDate; } set { _startDate = (value.Date == _dt.Date) ? _startDate : value; } }

        public DateTime endDate { get { return _endDate; } set { _endDate = (value.Date == _dt.Date) ? _endDate : value; } }

        public string month { get; set; }
    }
}