///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Basher Sarkar
///	Purpose	            :	
///	Creation Date       :   29-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     29-Jan-2024		Basher Sarkar	7.0.0		upgrade project to .Net 8
///	 ----------------------------------------------------------------
///	*****************************************************************

namespace Domain.ViewModel
{
    public class IrisDataOfferParseVM
    {
        public string offerType { get; set; } = string.Empty;
        public string dataPack { get; set; } = string.Empty;
        public string perDayData { get; set; } = string.Empty;
        public string toffee { get; set; } = string.Empty;
        public string streamingPack { get; set; } = string.Empty;
        public bool hasStreamingPack { get; set; }
    }
}