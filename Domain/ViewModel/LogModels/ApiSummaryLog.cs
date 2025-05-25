///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	API Summary Log write model
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
    public class ApiSummaryLog
    {
        public string retailerCode { get; set; }
        public int isSuccess { get; set; }
        public string methodName { get; set; }
        public double totalApiTimeInS { get; set; }
        public string errorMessage { get; set; }
        public DateTime apiStartTime { get; set; }
        public DateTime apiEndTime { get; set; }
        public string iTopUpNumber { get; set; }
        public string userAgentNdIP { get; set; }
        public string hostAndOtherInfo { get; set; }
        public long logId { get; set; }
        public string sessionToken { get; set; }

        public ApiSummaryLog(LogModel log)
        {
            retailerCode = log.retailerCode;
            isSuccess = log.isSuccess;
            methodName = log.methodName;
            errorMessage = log.errorMessage;
            iTopUpNumber = log.iTopUpNumber;
            apiStartTime = log.apiStartTime;
            apiEndTime = log.apiEndTime;
            totalApiTimeInS = (log.apiEndTime - log.apiStartTime).TotalSeconds;
            userAgentNdIP = log.userAgentNdIP;
            hostAndOtherInfo = log.hostAndOtherInfo;
            logId = log.logId;
            sessionToken = log.sessionToken;
        }
    }
}
