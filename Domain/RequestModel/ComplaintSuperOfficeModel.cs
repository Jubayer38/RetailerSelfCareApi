///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.RequestModel
{
    public class ComplaintSuperOfficeModel
    {
        public string retailerCode { get; set; }
        public string iTopUpNumber { get; set; }
        public string description { get; set; }
        public int soSubCategoryId { get; set; }
        public string fileName { get; set; }
        public string image { get; set; }
    }
}
