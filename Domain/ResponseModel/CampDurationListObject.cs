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

using System.Data;


namespace Domain.ResponseModel
{
    public class CampDurationListObject
    {
        public int duration { get; set; }
        public bool isSelected { get; set; }

        public CampDurationListObject(DataRow dt)
        {
            if (dt.ItemArray.Count() > 0)
            {
                duration = Convert.ToInt32(dt["KPI_DAY"]);
                isSelected = false;
            }
        }
    }
}
