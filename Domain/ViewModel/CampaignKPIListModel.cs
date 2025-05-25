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


namespace Domain.ViewModel
{
    public class CampaignKPIListModel
    {
        public string kpiId { get; set; }
        public string kpiName { get; set; }
        public string kpiNameBN { get; set; }
        public string target { get; set; }
        public string targetUnit { get; set; }
        public string achievement { get; set; }
        public string currentRate { get; set; }
        public string requiredRate { get; set; }

        public CampaignKPIListModel(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                kpiId = dr["CAMP_KPI_ID"] as string;
                kpiName = dr["KPI_NAME"] as string;
                kpiNameBN = dr["KPI_NAME_BN"] as string;
                target = dr["TARGET"] as string;
                targetUnit = dr["TARGET_UNIT"] as string;
                achievement = dr["ACHIEVEMENT"] as string;
                currentRate = dr["CURRENTRATE"] as string;
                requiredRate = dr["REQUIREDRATE"] as string;
            }
        }
    }
}
