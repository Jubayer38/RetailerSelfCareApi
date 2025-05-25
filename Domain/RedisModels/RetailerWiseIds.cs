///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Retailer Wise Distribution Ids Data Model for Banner, and Notification.
///	Creation Date :	16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:		Author:			Ver:	Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************

using System.Collections.Concurrent;
using System.Data;

namespace Domain.RedisModels
{
    public class RetailerWiseIds
    {
        public string RetailerCode { get; set; }
        public string ItemIds { get; set; }

        public RetailerWiseIds()
        { }

        public RetailerWiseIds(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                RetailerCode = dr["RETAILER_CODE"] as string;
                ItemIds = dr["ITEM_IDS"] as string;
            }
        }


        public static ConcurrentDictionary<string, string> ToDataDictionary(DataTable dt)
        {
            ConcurrentDictionary<string, string> pairs = new();

            foreach (DataRow row in dt.Rows)
            {
                string retailerCode = row["RETAILER_CODE"] as string;
                string bannerIds = row["ITEM_IDS"] as string;

                pairs.AddOrUpdate(retailerCode, bannerIds, (key, value) => bannerIds);
            }

            return pairs;
        }

    }
}
