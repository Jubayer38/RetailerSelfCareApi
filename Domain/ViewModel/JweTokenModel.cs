///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	JWT Token model for Utils
///	Creation Date :	20-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.ViewModel
{
    public class JweTokenModel
    {
        public string RetailerCode { get; set; }
        public string ITopUpNumber { get; set; }
        public string DeviceId { get; set; }
        public string LoginProvider { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
