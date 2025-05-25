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
    public class SelfCampaignRewardList
    {
        public string rewardId { get; set; }
        public string rewardTypeId { get; set; }
        public string rewardType { get; set; }
        public string reward { get; set; }
        public string rewardImageLocation { get; set; }
        public string imageBase64 { get; set; }

        public SelfCampaignRewardList(DataRow dr, string baseURL)
        {
            if (dr.ItemArray.Length > 0)
            {
                rewardId = dr["REWARD_ID"] as string;
                rewardTypeId = dr["REWARD_TYPE_ID"] as string;
                rewardType = dr["REWARD_TYPE"] as string;
                reward = dr["REWARD"] as string;
                string _imageLocation = dr["IMAGE_LOCATION"] as string;

                if (!string.IsNullOrWhiteSpace(_imageLocation))
                {
                    rewardImageLocation = baseURL + _imageLocation;
                }
            }

        }

    }
}