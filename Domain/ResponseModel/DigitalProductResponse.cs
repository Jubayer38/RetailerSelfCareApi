///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	16-Jan-2024
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
    public class DigitalProductResponse
    {
        public int productId { get; set; }

        public string productName { get; set; }

        public string productNameBN { get; set; }

        public string productUrl { get; set; }

        public int amount { get; set; }

        public string rechargeType { get; set; }

        public bool isRequiredAmount { get; set; }


        public DigitalProductResponse(DataRow dr, string lan)
        {
            productId = Convert.ToInt32(dr["PRODUCT_ID"].ToString());

            productName = dr["PRODUCT_NAME"].ToString();

            productNameBN = dr["PRODUCT_NAMEBN"].ToString();

            productUrl = dr["PRODUCT_URL"].ToString();

            amount = Convert.ToInt32(dr["AMOUNT"].ToString());

            rechargeType = dr["RECHARGE_TYPE"].ToString();

            isRequiredAmount = Convert.ToBoolean(dr["IS_REQUIRED_AMOUNT"]);


        }
    }
}
