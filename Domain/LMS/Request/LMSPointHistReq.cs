///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	
///	Purpose	      : 
///	Creation Date :	
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No. Date:		Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************
namespace Domain.LMS.Request
{
    public class LMSPointHistReq : CommonLMSRequest
    {
        public string month { get; set; }

        public string year { get; set; }
    }
}