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
    public class CommunicationRequestV4 : RetailerRequestV2
    {
        [Required]
        public string type { get; set; }

        public string searchText { get; set; } = string.Empty;

        public string sortType { get; set; } = "ASC";
    }
}
