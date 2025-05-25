///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
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
    public class RSOFeedbackModel
    {
        public string serialNo { get; set; }
        public string visitedDate { get; set; }
        public string checkInTime { get; set; }
        public string checkOutTime { get; set; }
        public string timeSpent { get; set; }
        public string feedback { get; set; }
        public string productName { get; set; }
        public string salesQTY { get; set; }
        public string amount { get; set; }

        public RSOFeedbackModel(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                serialNo = dr["SERIAL_NO"] as string;
                visitedDate = dr["VISITED_DATE"] as string;
                checkInTime = dr["IN_TIME"] as string;
                checkOutTime = dr["OUT_TIME"] as string;
                timeSpent = dr["TIME_SPENT"] as string;
                feedback = dr["FEEDBACK"] as string;
                productName = (dr["PRODUCT_NAME"] as string) == null ? string.Empty : dr["PRODUCT_NAME"] as string;
                salesQTY = (dr["SALES_QTY"] as string) == null ? "0" : dr["SALES_QTY"] as string;
                amount = (dr["AMOUNT"] as string) == null ? "0" : dr["AMOUNT"] as string;
            }
        }

    }
}
