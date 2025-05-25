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


namespace Domain.ViewModel
{
    public class POSMQModel
    {
        public int retailerId { get; set; }
        public string retailerCode { get; set; }
        public string productCode { get; set; }
        public DateTime issue_date { get; set; }
        public string key { get; set; }
        public string deviceid { get; set; }
    }
}
