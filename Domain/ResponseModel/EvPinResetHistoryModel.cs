///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Description
///	Creation Date :	DD-MMM-YYYY
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
    public class EvPinResetHistoryModel
    {
        public string requestDate { get; set; }
        public string resetReason { get; set; }
        public string status { get; set; }
        public string successOn { get; set; }


        public EvPinResetHistoryModel(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                requestDate = dr["REQUEST_DATETIME"] as string;
                resetReason = dr["RESET_REASON"] as string;
                status = dr["STATUS"] as string;
                successOn = dr["RESET_SUCCESS_ON"] as string;
            }
        }
    }
}
