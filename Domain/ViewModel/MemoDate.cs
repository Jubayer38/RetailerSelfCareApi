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
    public class MemoDate
    {
        public string memoDate { get; set; }

        public MemoDate(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                memoDate = dr["TDATE"] as string;
            }
        }
    }
}
