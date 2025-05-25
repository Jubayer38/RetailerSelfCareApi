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
    public class DigitalServiceHistoryResponse
    {
        public string productName { get; set; }

        public string subscriberNumber { get; set; }

        public string status { get; set; }

        public string date { get; set; }

        public DigitalServiceHistoryResponse(DataRow dr, string lan)
        {
            productName = dr["PRODUCT_NAME"].ToString();

            subscriberNumber = dr["SUBSCRIBER_NUMBER"].ToString();

            status = dr["STATUS"].ToString();

            date = dr["CREATEDDATE"].ToString();
        }
    }
}
