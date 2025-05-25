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
    public class SelfCampaignKPIListModel
    {
        public int targetId { get; set; }
        public string targetName { get; set; }
        public string targetNameBN { get; set; }
        public string iconBase64 { get; set; }

        public SelfCampaignKPIListModel(DataRow dr, string lan)
        {
            targetId = Convert.ToInt32(dr["CAMP_KPI_ID"]);
            targetName = dr["CAMP_KPI"] as string;
            targetNameBN = dr["CAMP_KPI_BN"] as string;
            iconBase64 = dr["ICON_BASE64"] as string;
        }
    }
}
