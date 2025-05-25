///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
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
    public class SendMailRequest : RetailerRequest
    {
        [Required]
        public string mailAddress { get; set; }

        public string mailSubject { get; set; }

        public string mailBody { get; set; }

        public string regards { get; set; }

        [Required]
        public string reportType { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

        public string month { get; set; }
    }
}