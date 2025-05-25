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

using Domain.ViewModel;


namespace Domain.ResponseModel
{
    public class CampaignDetailsModel
    {
        public int campaignId { get; set; }
        public string updateTill { get; set; }
        public string enrollTypeId { get; set; }
        public string enrollType { get; set; }
        public string ussd { get; set; }
        public bool isEnrolled { get; set; }
        public string campaignCategory { get; set; }

        public List<CampaignKPIListModel> kpiTargetList { get; set; }
        public List<CampaignRewardListModel> rewardList { get; set; }
    }
}
