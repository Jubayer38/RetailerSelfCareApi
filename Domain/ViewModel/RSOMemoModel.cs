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


namespace Domain.ViewModel
{
    public class RSOMemoModel
    {
        public string memoDate { get; set; }
        public double subTotal { get; set; }
        public List<RSOMemoVM> memoList { get; set; }

        public RSOMemoModel(string distinctDate, List<RSOMemoVM> list)
        {
            memoDate = distinctDate;
            memoList = list.Where(i => i.mDate == distinctDate).ToList();

            foreach (var item in memoList)
            {
                subTotal += Convert.ToDouble(item.productAmount);
            }

        }

    }
}
