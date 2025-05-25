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

namespace Domain.RequestModel
{
    public class CampaignEnrollOrCancelRequest : RetailerRequest
    {
        public int campaignId { get; set; }
        public int rewardId { get; set; }
        public int userId { get; set; }
        public string operationType { get; set; }
        public int status { get; set; }
        public string campaignCategory { get; set; }
    }
}