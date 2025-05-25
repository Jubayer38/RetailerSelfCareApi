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
    public class PRHistoryVM
    {
        public string productName { get; set; }
        public string rating { get; set; }
        public string ratingDate { get; set; }

        public PRHistoryVM(DataRow dr, string lan)
        {
            productName = dr["PRODUCT_NAME"] as string;
            rating = dr["RATING"] as string;
            ratingDate = dr["RATING_DATE"] as string;
        }

    }
}
