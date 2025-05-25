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


namespace Domain.ViewModel
{
    public class RaiseComplaintExternalSubmissionModel
    {
        private string _userId;
        public string userName { get { return _userId; } set { _userId = value; } }
        public string user_name { get { return _userId; } set { _userId = value; } }
        public string password { get; set; }
        public long raiseComplaintID { get; set; }
        public string complaintType { get; set; }
        public string complaintTitle { get; set; }
        public string description { get; set; } = string.Empty;
        public List<string> images { get; set; } = new List<string>();
        public string preferredLevel { get; set; }
        public string preferredLevelName { get; set; }
        public string preferredLevelContact { get; set; }
        public string retailerCode { get; set; }

        public RaiseComplaintExternalSubmissionModel(DataRow dr)
        {
            complaintType = dr["COMPLAINT_TYPE"] as string;
            complaintTitle = dr["COMPLAINT_TITLE"] as string;
            preferredLevel = dr["PREFERRED_LEVEL"] as string;
            preferredLevelName = dr["REFERRED_LEVEL_NAME"] as string;
            preferredLevelContact = dr["CONTACT_NO"] as string;
        }
    }
}
