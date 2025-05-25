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
    internal class OfferModel
    {
        public string offerCommission { get; set; } = string.Empty;
        public string offerName { get; set; } = string.Empty;
        public string rechargeAmount { get; set; } = string.Empty;
        public string sno { get; set; } = string.Empty;
        public string offerID { get; set; } = string.Empty;
        public string offerDisplayName { get; set; } = string.Empty;
    }
}