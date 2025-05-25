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

namespace Domain.LMS
{
    public class RewardDetails
    {
        public string earnPoints { get; set; }
        public string longDescription { get; set; }
        public string morePointsToRedeem { get; set; }
        public string pointsToRedeem { get; set; }
        public string rewardID { get; set; }
        public string rewardName { get; set; }
        public string rewardType { get; set; }
        public string smallDescription { get; set; }
        public string imageURL { get; set; }
        public string linkURL { get; set; }
        public string offerLongDescription1 { get; set; }
        public string rewardCategory { get; set; }
        public string shopsavailable { get; set; }
        public string voucherType { get; set; }
        public string discountType { get; set; }
        public string minOrderValue { get; set; }
        public string maxDiscountAmount { get; set; }
        public string discountPercentage { get; set; }
    }
}