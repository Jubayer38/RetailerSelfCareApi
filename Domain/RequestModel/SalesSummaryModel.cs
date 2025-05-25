///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	Description
///	Creation Date :	15-JAN-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Data;
using System.Globalization;

namespace Domain.RequestModel
{
    public class SalesSummaryModel
    {
        public string itemCode { get; set; }
        public string itemTitle { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        public string updateTime { get; set; }

        public SalesSummaryModel() { }

        public SalesSummaryModel(DataRow dr)
        {
            itemCode = dr["CODE"] as string;
            itemTitle = dr["ITEM"] as string;
            quantity = dr["QUANTITY"] as string;
            amount = dr["AMOUNT"] as string;
            updateTime = dr["UPDATETIME"] as string;
        }

        public static List<SalesSummaryModel> InitSalesModel()
        {
            List<SalesSummaryModel> listData = [];
            string todauDateStr = DateTime.Now.ToString("dd MMM. yyyy", CultureInfo.InvariantCulture);

            SalesSummaryModel model = new()
            {
                itemCode = "1",
                itemTitle = "SC",
                quantity = "0",
                amount = "0",
                updateTime = todauDateStr
            };
            listData.Add(model);

            model = new()
            {
                itemCode = "2",
                itemTitle = "SIM",
                quantity = "0",
                amount = "0",
                updateTime = todauDateStr
            };
            listData.Add(model);

            model = new()
            {
                itemCode = "3",
                itemTitle = "iTopUp",
                quantity = "0",
                amount = "0",
                updateTime = todauDateStr
            };
            listData.Add(model);

            return listData;
        }
    }
}
