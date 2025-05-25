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


namespace Domain.ViewModel
{
    public class Ticket
    {
        public int ticketId { get; set; }
        public int categoryId { get; set; }
        public string title { get; set; } = string.Empty;
        public string categoryName { get; set; } = string.Empty;
        public string categoryFullName { get; set; } = string.Empty;
        public string createdAt { get; set; } = string.Empty;
        public string serviceDate { get; set; } = string.Empty;
        public string lastModified { get; set; } = string.Empty;
        public string channelName { get; set; } = string.Empty;
        public string currentStatus { get; set; } = string.Empty;
        public string subscription { get; set; } = string.Empty;
        public string callerNumber { get; set; } = string.Empty;
        public List<TicketMessages> messages { get; set; } = new List<TicketMessages>();
    }
}
