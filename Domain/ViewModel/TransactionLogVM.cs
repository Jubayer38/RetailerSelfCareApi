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
    public class TransactionLogVM
    {
        public string rCode { get; set; } = string.Empty;
        public string tranNo { get; set; } = string.Empty;
        public string tranType { get; set; } = string.Empty;
        public long amount { get; set; }
        public DateTime tranDate { get; set; }
        public string msisdn { get; set; } = string.Empty;//subscriber msisdn
        public int rechargeType { get; set; }
        public string email { get; set; } = string.Empty;
        public int isTranSuccess { get; set; }
        public string tranMsg { get; set; } = string.Empty;
        public string retMsisdn { get; set; } = string.Empty;
        public double commissionAmount { get; set; }
        public string loginProvider { get; set; } = string.Empty;
        public string respTranId { get; set; } = string.Empty;
        public double lat { get; set; }
        public double lng { get; set; }
        public string ipAddress { get; set; } = string.Empty;
        //public int denoValidity { get; set; }
    }
}