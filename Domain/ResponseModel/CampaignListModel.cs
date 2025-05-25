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
    public class CampaignListModel
    {
        public string campaignId { get; set; }

        public string campaignTitle { get; set; }

        public string campaignDescription { get; set; }

        public string campaignType { get; set; }

        public string fromDate { get; set; }

        public string endDate { get; set; }

        public bool isEnrolled { get; set; }

        public string campaignCategory { get; set; }

        public string imageBase64 { get; set; } = string.Empty;

        public string imagePath { get; set; } = string.Empty;

        public DateTime createdDate { get; set; }

        public string createdDateStr { get; set; }


        public CampaignListModel(DataRow dr, string baseURL)
        {
            if (dr.ItemArray.Length > 0)
            {
                campaignId = dr["CAMPAIGN_ID"] as string;
                campaignTitle = dr["TITLE"] as string;
                campaignDescription = dr["DESCRIPTION"] as string;
                campaignType = dr["CAMPAIGN_TYPE"] as string;
                fromDate = dr["FROM_DATE"] as string;
                endDate = dr["TILL_DATE"] as string;
                isEnrolled = Convert.ToBoolean(dr["IS_ENROLLED"]);
                campaignCategory = dr["CAMP_CATEGORY"] as string;
                createdDate = Convert.ToDateTime(dr["CREATEDDATE"]);
                createdDateStr = dr["CREATEDDATESTR"] as string;
                string _imageUrl = dr["IMAGEURL"] as string;

                if (!string.IsNullOrWhiteSpace(_imageUrl))
                {
                    imagePath = baseURL + _imageUrl;
                }
            }
        }
    }
}