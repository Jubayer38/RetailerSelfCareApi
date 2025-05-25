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

using System.Data;


namespace Domain.ResponseModel
{
    public class FeedbackCategoryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public FeedbackCategoryModel(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                Id = dr["CAT_ID"] as string;
                Name = dr["CAT_NAME"] as string;
            }
        }
    }
}
