///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	API Details Log View Model
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
    public class ApiDetailsLog
    {
        public string retailerCode { get; set; }
        public string methodName { get; set; }
        public string iTopUpNumber { get; set; }
        public long logId { get; set; }
        public string requestBody { get; set; }
        public string responseBody { get; set; }

        public ApiDetailsLog(LogModel log)
        {
            logId = log.logId;
            retailerCode = log.retailerCode;
            iTopUpNumber = log.iTopUpNumber;
            methodName = log.methodName;
            requestBody = log.requestBody;
            responseBody = log.responseBody;
        }
    }
}
