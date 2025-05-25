///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Application EV releated api log Summary model.
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
    public class EVSummaryLog
    {
        public string retailerCode { get; set; }
        public int isSuccess { get; set; }
        public int isTranSuccess { get; set; }
        public string methodName { get; set; }
        public double totalApiTimeInS { get; set; }
        public string amount { get; set; }
        public string subMSISDN { get; set; }
        public string errorMessage { get; set; }
        public string tranMsg { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string retMSISDN { get; set; }
        public string ipAddress { get; set; }
        public string userAgentNdIp { get; set; }
        public long logId { get; set; }
        public string sessionToken { get; set; }


        public EVSummaryLog(EvLogViewModel log)
        {
            logId = log.logId;
            retailerCode = log.retailerCode;
            sessionToken = log.sessionToken;
            isSuccess = log.isSuccess;
            errorMessage = log.errorMessage;
            methodName = log.methodName;
            subMSISDN = log.subMSISDN;
            retMSISDN = log.retMSISDN;
            amount = log.amount;
            isTranSuccess = log.isTranSuccess;
            tranMsg = log.tranMsg;
            ipAddress = log.ipAddress;
            userAgentNdIp = log.userAgentNdIp;
            startTime = log.startTime;
            endTime = log.endTime;
            totalApiTimeInS = (log.endTime - log.startTime).TotalSeconds;
        }

    }
}
