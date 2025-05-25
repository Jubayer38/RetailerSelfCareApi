///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Basher Sarkar
///	Purpose	            :	
///	Creation Date       :   07-Feb-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     07-Feb-2024		Basher Sarkar	7.0.0		upgrade project to .Net 8
///	 ----------------------------------------------------------------
///	*****************************************************************

namespace Domain.DummyAmarOffer
{
    internal class IRISOfferResponse
    {
        public string offersList { get; set; } = string.Empty;
        public string transactionId { get; set; } = string.Empty;
        public string statusMessage { get; set; } = string.Empty;
        public string statusCode { get; set; } = string.Empty;
    }
}