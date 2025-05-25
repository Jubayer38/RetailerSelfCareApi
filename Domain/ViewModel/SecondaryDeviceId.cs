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
    public class SecondaryDeviceId
    {
        public string DeviecId { get; set; }
        public bool isPrimary { get; set; }

        public SecondaryDeviceId(DataRow dr)
        {
            DeviecId = dr["DEVICEID"] as string;
            isPrimary = Convert.ToBoolean(dr["IS_PRIMARY"]);
        }
    }
}