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
using static Domain.Enums.EnumCollections;

namespace Domain.ResponseModel
{
    public class LiftingModel
    {
        public string date { get; set; }
        public string type { get; set; }
        public string category { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
        public string deliveredDate { get; set; }
        public string rsoCode { get; set; }
        public string receivedAmount { get; set; }

        public LiftingModel(DataRow dr)
        {
            date = dr["LIFTINGDATE"] as string;
            type = dr["TYPE"] as string;
            category = dr["CATEGORY"] as string;
            quantity = dr["QUANTITY"] as string;
            amount = dr["AMOUNT"] as string;
            deliveredDate = dr["DELIVERED_DATE"] as string;
            rsoCode = dr["RSO_CODE"] as string;
            receivedAmount = dr["RECEIVED_AMOUNT"] as string;

            int sts = Convert.ToInt32(dr["STATUS"]);
            status = Enum.GetName(typeof(OrderStatus), sts).Replace("_", " ");
        }
    }
}