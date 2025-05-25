///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	10-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.ViewModel
{
    public class UpdateLifting
    {
        public long RequestId { get; set; }
        public string RetailerCode { get; set; }
        public string ProductType { get; set; }
        public string ProductCode { get; set; }
        public int ProductCount { get; set; }
        public int Status { get; set; }
    }
}
