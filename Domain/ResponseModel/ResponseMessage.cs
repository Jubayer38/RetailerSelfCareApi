///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Response Model 
///	Creation Date :	20-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.ResponseModel
{
    public class ResponseMessage
    {
        public bool isError { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public string ErrorDetails { get; set; }

        private string _msgType = string.Empty;
        public string messageType { get { return _msgType; } set { _msgType = value ?? _msgType; } }

        public ResponseMessage() { }

        public ResponseMessage(bool _isError, string _message, object _data, string _errorDetails, string _messageType)
        {
            isError = _isError;
            message = _message;
            data = _data;
            ErrorDetails = _errorDetails;
            messageType = _messageType;
        }
    }


    public class ResponseMessageV2 : ResponseMessage
    {
        public bool isThisVersionBlocked { get; set; }
    }
}