///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	
///	Purpose	      : 
///	Creation Date :	
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No. Date:		Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************

using System.Data;

namespace Domain.LMS
{
    public class LmsTermsFaqs
    {
        public string Title { get; set; }
        public string Description { get; set; }


        public LmsTermsFaqs() { }

        public LmsTermsFaqs(DataRow row)
        {
            if (row.ItemArray.Length > 0)
            {
                Title = row["TITLE"] as string;
                Description = row["DESCRIPTION"] as string;
            }
        }
    }
}