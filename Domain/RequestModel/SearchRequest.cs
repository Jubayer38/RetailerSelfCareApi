///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Basher Sarkar
///	Purpose	            :	
///	Creation Date       :   28-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     28-Jan-2024		Basher Sarkar	7.0.0		upgrade project to .Net 8
///	 ----------------------------------------------------------------
///	*****************************************************************

namespace Domain.RequestModel
{
    public class SearchRequest : RetailerRequest
    {
        public string searchText { get; set; }
    }


    public class SearchRequestV2 : RetailerRequestV2
    {
        public string searchText { get; set; }
    }
}