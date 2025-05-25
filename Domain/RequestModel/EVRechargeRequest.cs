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

using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class EVRechargeRequest : RetailerRequest
    {
        private int actual_amount;

        [Required]
        public string amount { get { return actual_amount.ToString(); } set { actual_amount = Convert.ToInt32(value) * 100; } }

        //[Required]
        public string subscriberNo { get; set; }

        [Required]
        public string userPin { get; set; }

        public string email { get; set; }

        public int paymentType { get; set; }

        public double lat { get; set; }

        public double lng { get; set; }

        public List<RechargeInfo> rechargeList { get; set; }

        public EVRechargeRequest()
        {
            rechargeList = [];
        }
    }
}