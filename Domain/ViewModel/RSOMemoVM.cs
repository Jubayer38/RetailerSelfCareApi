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


namespace Domain.ViewModel
{
    public class RSOMemoVM
    {
        public string mDate { get; set; }
        public string productCategory { get; set; }
        public string productQuantity { get; set; }
        public string productAmount { get; set; }

        public RSOMemoVM(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                mDate = dr["TDATE"] as string;
                productCategory = dr["CODE"] as string;
                productQuantity = dr["QUANTITY"] as string;
                productAmount = dr["AMOUNT"] as string;
            }

        }

    }
}
