///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	15-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.ResponseModel
{
    public class IRISResponseMessage
    {
        public bool isError { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public object ErrorDetails { get; set; }

        private string _msgType = string.Empty;
        public string messageType { get { return _msgType; } set { _msgType = value ?? _msgType; } }
        public bool isUSIM { get; set; }
    }
}
