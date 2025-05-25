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

using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class CampaignKPIRequest : RetailerRequest
    {
        [Required]
        public int duration { get; set; }

        [Required]
        public List<int> targetIdList { get; set; } = [];

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }
    }
}