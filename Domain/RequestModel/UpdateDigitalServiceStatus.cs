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

using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class UpdateDigitalServiceStatus
    {
        [Required]
        public string userName { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string ServiceName { get; set; }

        [Required]
        public string iTopUpNumber { get; set; }

        [Required]
        public string SubscriberNumber { get; set; }

        [Required]
        public DateTime RegisteredDate { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

    }
}
