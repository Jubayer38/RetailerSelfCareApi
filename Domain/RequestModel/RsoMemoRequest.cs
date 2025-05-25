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
    public class RsoMemoRequest
    {
        private static readonly DateTime _dt;
        private static readonly DateTime today = DateTime.Now;
        private DateTime _startDate = new(today.Year, today.Month, 1);
        private DateTime _endDate = new(today.Year, today.Month, today.Day);

        [Required]
        public string sessionToken { get; set; }
        [Required]
        public string retailerCode { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }

        public DateTime startDate { get { return _startDate; } set { _startDate = (value.Date == _dt.Date) ? _startDate : value; } }
        public DateTime endDate { get { return _endDate; } set { _endDate = (value.Date == _dt.Date) ? _endDate : value; } }
    }
}