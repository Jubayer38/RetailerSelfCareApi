///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Arafat
///	Purpose	            :	
///	Creation Date       :   13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     28-Jan-2024		Basher Sarkar	7.0.0		add summary
///	 ----------------------------------------------------------------
///	*****************************************************************

using System.Data;

namespace Domain.ResponseModel
{
    public class ComplaintTitleResponse
    {
        public int complaintTitleId { get; set; }
        public string complaintTitle { get; set; }
        public string code { get; set; }
        public int subCategoryId { get; set; }

        public ComplaintTitleResponse(DataRow dr, string lan)
        {
            complaintTitleId = Convert.ToInt32(dr["COMPLAINT_TITLE_ID"]);
            complaintTitle = lan == "bn" ? dr["COMPLAINT_TITLE_BN"] as string : dr["COMPLAINT_TITLE"] as string;
            code = dr["CODE"] as string;

            int.TryParse(dr["SUBCATEGORY_ID"].ToString(), out int _subCatId);
            subCategoryId = _subCatId;
        }
    }
}