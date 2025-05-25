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
    public class RetailerInfoRequest
    {
        [Required]
        public string userName { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string retailerCode { get; set; }

        [Required]
        public string iTopUpNumber { get; set; }

        [Required]
        public int isActive { get; set; }

        [Required]
        public string typeName { get; set; }

        //public string address { get; set; }
        //public string contactPersonName { get; set; }
        //public string contactNo { get; set; }
        //public int channelId { get; set; }
        //public string scSeller { get; set; }
        //public string simSeller { get; set; }
        //public string iTopUpSeller { get; set; }
    }
}
