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


namespace Domain.RequestModel
{
    public class CampaignTargetListRequest
    {
        public int kpiId { get; set; }
        public string kpiTarget { get; set; }
        public string targetUnit { get; set; }
        public int kpiConfigId { get; set; }
    }
}
