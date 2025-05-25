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
    public class RSOEligibility
    {
        public string RsoNumber { get; set; }
        public int IsEligible { get; set; }
        public double CurrentBalance { get; set; }
        public string UpdateTime { get; set; }
        public string iTopUpNumber { get; set; }
        public string retailerCode { get; set; }


        public RSOEligibility() { }

        public RSOEligibility(DataRow row)
        {
            if (row.ItemArray.Length > 0)
            {
                _ = int.TryParse(row["IS_ELIGIBLE"].ToString(), out int _isEligible);

                RsoNumber = row["SRNUMBER"] as string;
                IsEligible = _isEligible;
                CurrentBalance = Convert.ToDouble(row["CURR_BALANCE"]);
                UpdateTime = row["UPDATETIME"] as string;
            }
        }
    }
}