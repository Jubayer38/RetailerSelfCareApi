
using Domain.RequestModel;

///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Request Validation Model
///	Creation Date :	19-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************
namespace Domain.ViewModel
{
    public class RequestValidityModel
    {
        public ContextHelperModel retailerRequest { get; set; }
        public bool isValid { get; set; }
    }
}
