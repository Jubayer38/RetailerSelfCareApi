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

namespace Domain.RequestModel
{
    public class RechargeInfo
    {
        public string name { get; set; }
        public string msisdn { get; set; } // SubscriberNo

        private int actual_amount;
        public string amount { get { return actual_amount.ToString(); } set { actual_amount = Convert.ToInt32(value) * 100; } }
        public bool isSuccess { get; set; }
        public int paymentType { get; set; }
        public string denoValidity { get; set; }
    }
}