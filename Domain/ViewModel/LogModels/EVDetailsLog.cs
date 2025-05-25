///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Application EV releated api log Details model.
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
    public class EVDetailsLog
    {
        public string retailerCode { get; set; }
        public string methodName { get; set; }
        public long logId { get; set; }
        public string requestBody { get; set; }
        public string responseBody { get; set; }

        public EVDetailsLog(EvLogViewModel log)
        {
            logId = log.logId;
            retailerCode = log.retailerCode;
            methodName = log.methodName;
            requestBody = log.reqBodyStr;
            responseBody = log.resBodyStr;
        }

    }
}
