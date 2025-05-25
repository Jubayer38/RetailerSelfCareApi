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
    public class POSMQRRequest : RetailerRequest
    {
        private readonly DateTime _issueDate = DateTime.Now;

        [Required]
        public string productCode { get; set; }

        public string issueDateStr { get; set; } = string.Empty;

        public DateTime? issueDate
        {
            get { return _issueDate; }
            set { _ = !string.IsNullOrWhiteSpace(issueDateStr) ? Convert.ToDateTime(issueDateStr) : _issueDate; }
        }
    }
}