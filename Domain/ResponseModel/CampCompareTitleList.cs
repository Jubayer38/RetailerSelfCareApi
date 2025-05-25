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
    public class CampCompareTitleList
    {
        public string campaignId { get; set; }

        public string campaignTitle { get; set; }

        public string campaignCategory { get; set; }

        public string campaignType { get; set; }

        public CampCompareTitleList(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                campaignId = dr["CAMPAIGN_ID"] as string;

                campaignTitle = dr["TITLE"] as string;

                campaignCategory = dr["CAMP_CATEGORY"] as string;

                campaignType = dr["CAMPAIGN_TYPE"] as string;
            }

        }
    }
}
