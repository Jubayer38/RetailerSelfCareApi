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
    public class CampCompareDetails
    {
        public string campaignType { get; set; }
        public string campaignTitle { get; set; }
        public string campaignCategory { get; set; }
        public string campaignDuration { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string campaignEndDate { get; set; }
        public List<CampaignKPIListModel> kpiTargetList { get; set; }
        public List<CampaignRewardListModel> rewardList { get; set; }

    }
}
