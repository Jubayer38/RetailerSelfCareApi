using System.ComponentModel.DataAnnotations;


///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Login Model For Smart POS
///	Creation Date :	19-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.RequestModel
{
    public class LoginRequest
    {
        private string uName;
        [Required]
        public string UserName { get { return uName; } set { uName = value.Substring(1); } }
        public string retailerCode { get { return uName; } set { uName = value.Substring(1); } }
        [Required]
        public string Lan { get; set; }
        [Required]
        public int VersionCode { get; set; }
        [Required]
        public string VersionName { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public string OSVersion { get; set; }
        [Required]
        public string KernelVersion { get; set; }
        public string FermwareVersion { get; set; }
    }
}
