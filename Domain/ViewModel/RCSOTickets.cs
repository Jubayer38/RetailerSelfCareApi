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

using System.Data;


namespace Domain.ViewModel
{
    public class RCSOTickets
    {
        public long SO_ID { get; set; }
        public DateTime SO_Update_Time { get; set; }
        public bool IsTicketOpen { get; set; }

        public RCSOTickets(DataRow dr)
        {
            SO_ID = dr["SO_ID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["SO_ID"]);
            SO_Update_Time = Convert.ToDateTime(dr["UPDATE_TIME"]);
            IsTicketOpen = Convert.ToBoolean(dr["IS_TICKET_OPEN"]);
        }
    }
}
