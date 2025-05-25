///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	08-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.ViewModel.LogModels
{
    public class EvLogViewModel
    {
        public string retailerCode { get; set; }
        public string sessionToken { get; set; }
        public int isSuccess { get; set; }
        public string errorMessage { get; set; }
        public string methodName { get; set; }
        public string reqBodyStr { get; set; }
        public string resBodyStr { get; set; }
        public string subMSISDN { get; set; }
        public string retMSISDN { get; set; }
        public string amount { get; set; }
        public int isTranSuccess { get; set; }
        public string tranMsg { get; set; }
        public string ipAddress { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public long logId { get; set; } = DateTime.Now.Ticks;
        public string userAgentNdIp { get; set; }
    }
}
