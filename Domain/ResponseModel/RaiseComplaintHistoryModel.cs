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

using Domain.StaticClass;
using System.Data;


namespace Domain.ResponseModel
{
    public class RaiseComplaintHistoryModel
    {
        public string complaintType { get; set; }
        public string complaintTitle { get; set; }
        public string complaintDesciption { get; set; }
        public string superOfficeId { get; set; }
        public string complaintStatus { get; set; }
        public bool hasAttachment { get; set; }
        public string compaintDate { get; set; }
        public string[] images { get; set; }

        public RaiseComplaintHistoryModel(DataRow dr, string lan)
        {
            if (dr.ItemArray.Length > 0)
            {
                complaintType = lan == "bn" ? dr["COMP_TYPE_BN"] as string : dr["COMP_TYPE"] as string;
                complaintTitle = lan == "bn" ? dr["TITLE_BN"] as string : dr["TITLE"] as string;
                complaintDesciption = dr["DESCIPTIONS"] as string;
                superOfficeId = dr["SUPER_OFFICE_ID"] as string;
                complaintStatus = dr["COMPLAINT_STATUS"] as string;
                hasAttachment = Convert.ToInt32(dr["HASATTACHMENT"]) == 1;
                compaintDate = dr["COMPLAINT_DATE"] as string;

                string imagepaths = dr["FILE_LOCATION"] as string;
                string baseurl = ExternalKeys.ImageVirtualDirPath;
                if (!string.IsNullOrWhiteSpace(imagepaths))
                {
                    images = imagepaths.Split('|');
                    images = images.Select(x => baseurl + x.Replace("\\", "/")).ToArray();
                }
                else
                {
                    images = new string[] { };
                }
            }
        }
    }
}
