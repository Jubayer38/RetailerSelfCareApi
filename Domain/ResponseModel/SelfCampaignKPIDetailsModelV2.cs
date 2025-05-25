///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Description
///	Creation Date :	DD-MMM-YYYY
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
    public class SelfCampaignKPIDetailsModelV2
    {
        public string targetId { get; set; }
        public string targetName { get; set; }
        public string targetUnit { get; set; }
        public string minTarget { get; set; }
        public string maxTarget { get; set; }
        public string perUnitAmount { get; set; }
        public long kpiConfigId { get; set; }

        public SelfCampaignKPIDetailsModelV2(DataRow dr)
        {
            targetId = dr["KPI_ID"] as string;
            targetName = dr["KPI_NAME"] as string;
            targetUnit = dr["UNIT"] as string;
            minTarget = dr["MIN_TARGET"] as string;
            maxTarget = dr["MAX_TARGET"] as string;
            perUnitAmount = dr["PER_UNIT_AMOUNT"] as string;
            kpiConfigId = Convert.ToInt64((dr["CAMP_KPI_CONFIG_ID"]));
        }
    }
}
