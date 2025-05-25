///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	DD-MMM-YYYY
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.RequestModel
{
    public class CampaignTargetRequestModel
    {
        public string retailerCode { get; set; }
        public int campaignId { get; set; }
        public int campEnrollId { get; set; }
        public int kpiId { get; set; }
        public string kpiTarget { get; set; }
        public string targetUnit { get; set; }
        public int userId { get; set; }
        public int kpiConfigId { get; set; }
    }
}
