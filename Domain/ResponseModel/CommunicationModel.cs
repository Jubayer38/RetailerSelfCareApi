///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	17-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using System.Data;

namespace Domain.ResponseModel
{
    public class CommunicationModel
    {
        public long communicaionId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string imageURL { get; set; }
        public string type { get; set; }
        public string communicationDate { get; set; }
        public string redirection { get; set; } = string.Empty;

        public CommunicationModel(DataRow row, string imageBasePath)
        {
            _ = int.TryParse(row["COMMUNICATION_ID"] as string, out int id);
            communicaionId = id;
            title = row["COMMUNICATION_TITLE"] == DBNull.Value ? "" : row["COMMUNICATION_TITLE"].ToString();
            description = row["COMMUNICATION_DESCRIPTION"] == DBNull.Value ? "" : row["COMMUNICATION_DESCRIPTION"].ToString();
            url = row["EXTERNAL_URL"] == DBNull.Value ? "" : row["EXTERNAL_URL"].ToString();
            type = row["COMMUNICATION_TYPE"] == DBNull.Value ? "" : row["COMMUNICATION_TYPE"].ToString();
            communicationDate = DBNull.Value != row["COMMUNICATION_DATE"] ? row["COMMUNICATION_DATE"].ToString() : DateTime.Now.ToEnUSDateString("hh:mm tt, dd MMM yyyy");
            if (!type.Equals("ARCHIVED"))
                redirection = row["REDIRECTION"] == DBNull.Value ? "" : row["REDIRECTION"].ToString();

            string fileLocation = row["FILE_LOCATION"] == DBNull.Value ? "" : row["FILE_LOCATION"].ToString();
            if (!string.IsNullOrWhiteSpace(fileLocation))
                imageURL = imageBasePath + fileLocation;
        }
    }
}