
///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
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
    public class TarVsAchvDetailsModel
    {
        public string shortDate { get; set; }
        public string date { get; set; }
        public string target { get; set; }
        public string MTD { get; set; }
        public string LMTD { get; set; }
        public string LM { get; set; }


        public TarVsAchvDetailsModel(DataRow dr)
        {
            shortDate = dr["ACHVSHORTDATE"] as string;
            date = dr["ACHVDATE"] as string;
            target = dr["TARGET"] as string;
            MTD = dr["MTD"] as string;
            LMTD = dr["LMTD"] as string;
            LM = dr["LM"] as string;
        }
    }
}
