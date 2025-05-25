///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Write External API Call Text Log
///	Creation Date :	18-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.ViewModel.LogModels
{
    public class ExternalAPICallLogModel
    {
        public string retailerCode { get; set; }
        public int isSuccess { get; set; }
        public string errorMessage { get; set; }
        public string methodName { get; set; }
        public string reqBodyStr { get; set; }
        public string resBodyStr { get; set; }
        public DateTime reqStartTime { get; set; }
        public DateTime reqEndTime { get; set; }
        public long logId { get; set; } = DateTime.Now.Ticks;
        public object errorDetails { get; set; }
    }
}
