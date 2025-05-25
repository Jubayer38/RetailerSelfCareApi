///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Basher Sarkar
///	Purpose	      :	External Log Write Model
///	Creation Date :	14-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.ViewModel.LogModels;

namespace Domain.ViewModel
{
    public class ExternalAPICallVM
    {
        public string retailerCode { get; set; }
        public int isSuccess { get; set; }
        public string errorMessage { get; set; }
        public string methodName { get; set; }
        public string reqBodyStr { get; set; }
        public string resBodyStr { get; set; }
        public DateTime reqStartTime { get; set; } = DateTime.Now;
        public DateTime reqEndTime { get; set; }
        public long logId { get; set; } = DateTime.Now.Ticks;
        public object errorDetails { get; set; }

        public static ExternalAPICallVM LogModelToExternalModel(LogModel log)
        {
            return new ExternalAPICallVM()
            {
                retailerCode = log.retailerCode,
                isSuccess = log.isSuccess,
                errorMessage = log.errorMessage,
                methodName = log.methodName,
                reqBodyStr = log.requestBody,
                resBodyStr = log.responseBody,
                reqStartTime = log.apiStartTime,
                reqEndTime = log.apiEndTime,
                logId = log.logId
            };
        }
    }
}
