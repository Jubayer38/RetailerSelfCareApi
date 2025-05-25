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
    public class CreateCampaignByRetailerRequest : RetailerRequest
    {
        public string campaignTitle { get; set; }

        public string campaignDescription { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public int rewardId { get; set; }

        public int rewardTypeId { get; set; }

        public string reward { get; set; }

        public string rewardImageLocation { get; set; }

        public int userId { get; set; }

        public List<CampaignTargetListRequest> targets { get; set; } = [];
    }
}