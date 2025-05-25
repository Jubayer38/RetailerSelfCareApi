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

using System.ComponentModel.DataAnnotations;


namespace Domain.RequestModel
{
    public class CampCompareDetailRequest
    {
        [Required]
        public string sessionToken { get; set; }
        [Required]
        public string retailerCode { get; set; }

        [Required]
        public List<CampIDAndCat> idList { get; set; }

        public int userId { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }

    }

    public class CampIDAndCat
    {
        public int campaignId { get; set; }
        public string campaignCategory { get; set; }
    }
}
