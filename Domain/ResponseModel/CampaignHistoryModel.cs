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

using System.Data;

namespace Domain.ResponseModel
{
    public class CampaignHistoryModel
    {
        public string campaignId { get; set; }
        public string campaignName { get; set; }
        public string campaignType { get; set; }
        public string reward { get; set; }
        public string dayLeft { get; set; }
        public List<CampaignHistoryKPI> kpiList { get; set; }

        public CampaignHistoryModel(DataRow dr, List<CampaignHistoryKPI> campKpiList)
        {
            kpiList = new List<CampaignHistoryKPI>();

            campaignId = dr["CAMPAIGN_ID"] as string;
            campaignName = dr["TITLE"] as string;
            campaignType = dr["CAMPAIGN_TYPE"] as string;
            reward = dr["REWARD"] as string;
            dayLeft = dr["DAYLEFT"] as string;
            kpiList = campKpiList.Where(cId => cId.campaignId == dr["CAMPAIGN_ID"].ToString()).ToList();
        }

    }
}
