///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Error Log model for Text Logging
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.ViewModel.LogModels
{
    public class ErrorLogModel
    {
        public string methodName { get; set; }
        public DateTime logSaveTime { get; set; }
        public string requestModel { get; set; }
        public string procedureName { get; set; }
        public string errorMessage { get; set; }
        public string errorSource { get; set; }
        public int errorCode { get; set; }
        public string errorDetails { get; set; }
    }
}
