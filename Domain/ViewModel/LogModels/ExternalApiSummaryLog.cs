///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	View Model For External API Summary Log Write
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
    public class ExternalApiSummaryLog
    {
        public string retailerCode { get; set; }
        public int isSuccess { get; set; }
        public string methodName { get; set; }
        public double totalApiTimeInS { get; set; }
        public string errorMessage { get; set; }
        public DateTime reqStartTime { get; set; }
        public DateTime reqEndTime { get; set; }
        public long logId { get; set; }

        public ExternalApiSummaryLog(ExternalAPICallVM log)
        {
            logId = log.logId;
            retailerCode = log.retailerCode;
            isSuccess = log.isSuccess;
            errorMessage = log.errorMessage;
            methodName = log.methodName;
            reqStartTime = log.reqStartTime;
            reqEndTime = log.reqEndTime;
            totalApiTimeInS = (log.reqEndTime - log.reqStartTime).TotalSeconds;
        }

    }
}