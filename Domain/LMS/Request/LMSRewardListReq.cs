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

using Domain.RequestModel;
using System.ComponentModel.DataAnnotations;


namespace Domain.LMS.Request
{
    public class LMSRewardListReq : RetailerRequestV2
    {
        public string isPartnerQrMapped { get; set; } = string.Empty;

        [Required]
        public string partnerCategoryID { get; set; } = string.Empty;
    }
}
