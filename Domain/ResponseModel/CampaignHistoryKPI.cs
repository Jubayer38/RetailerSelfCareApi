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
    public class CampaignHistoryKPI
    {
        public string campaignId { get; set; }
        public string kpiName { get; set; }
        public string kpiNameBN { get; set; }
        public string target { get; set; }
        public string achievement { get; set; }

        public CampaignHistoryKPI(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                campaignId = dr["CAMPAIGN_ID"] as string;
                kpiName = dr["KPI_NAME"] as string;
                kpiNameBN = dr["KPI_NAME_BN"] as string;
                target = dr["TARGET"] as string;
                achievement = dr["ACHIEVEMENT"] as string;
            }
        }
    }
}
