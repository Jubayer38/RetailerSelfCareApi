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
    public class SCSellsHistoryModel
    {
        public string scNumber { get; set; }
        public string amount { get; set; }
        public string customerMsisdn { get; set; }
        public string salesDate { get; set; }

        public SCSellsHistoryModel(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                scNumber = dr["SC_SERIAL_NO"] as string;
                amount = dr["AMOUNT"] as string;
                customerMsisdn = dr["CUSTOMER_NUMBER"] as string;
                salesDate = dr["SALES_DATE"] as string;
            }
        }
    }
}
