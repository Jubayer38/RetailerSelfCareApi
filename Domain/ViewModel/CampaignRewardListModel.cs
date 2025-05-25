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
    public class CampaignRewardListModel
    {
        public string rewardId { get; set; }

        public string rewardTypeId { get; set; }

        public string rewardTypeName { get; set; }

        public string reward { get; set; }

        public string rewardTitle { get; set; }

        public string imageBase64 { get; set; } = string.Empty;

        public string imagePath { get; set; } = string.Empty;

        public CampaignRewardListModel(DataRow dr, string baseURL)
        {
            if (dr.ItemArray.Length > 0)
            {
                rewardId = dr["REWARD_ID"] as string;
                rewardTypeId = dr["REWARD_TYPE_ID"] as string;
                rewardTypeName = dr["REWARD_NAME"] as string;
                reward = dr["REWARD"] as string;
                rewardTitle = dr["REWARD_TITLE"] as string;
                string _imageUrl = dr["IMAGEURL"] as string;

                if (!string.IsNullOrWhiteSpace(_imageUrl))
                {
                    imagePath = baseURL + _imageUrl;
                }
            }
        }


        public CampaignRewardListModel(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                rewardId = dr["REWARD_ID"] as string;
                rewardTypeId = dr["REWARD_TYPE_ID"] as string;
                rewardTypeName = dr["REWARD_NAME"] as string;
                reward = dr["REWARD"] as string;
                rewardTitle = dr["REWARD_TITLE"] as string;
                string _imageUrl = dr["IMAGEURL"] as string;

                if (!string.IsNullOrWhiteSpace(_imageUrl))
                {
                    imagePath = _imageUrl;
                }
            }
        }
    }
}
