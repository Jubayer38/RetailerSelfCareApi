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

using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class RaiseComplSubmitRequest : RetailerRequest
    {
        [Required]
        public int complaintTypeID { get; set; }

        [Required]
        public int preferredLevelId { get; set; }

        [Required]
        public int complaintTitleId { get; set; }

        public string description { get; set; }

        [Required]
        public string category { get; set; }

        [Required]
        public string iTopUpNumber { get; set; }

        public string address { get; set; }

        public string fileName { get; set; }
        public List<string> images { get; set; } = new List<string>();

        public int soCategoryId { get; set; }
        public int soSubCategoryId { get; set; }
        public string FileLocation { get; set; }
    }
}