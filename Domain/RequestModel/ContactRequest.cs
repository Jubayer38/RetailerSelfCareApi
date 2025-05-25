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

namespace Domain.RequestModel
{
    public class ContactSaveRequest : RetailerRequest
    {
        public string contactNo { get; set; }
        public string contactName { get; set; }
    }


    public class ContactDeleteRequest : RetailerRequest
    {
        public long contactId { get; set; }
    }
}