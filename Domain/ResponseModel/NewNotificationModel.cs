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
using static Domain.Enums.EnumCollections;


namespace Domain.ResponseModel
{
    public class NewNotificationModel
    {
        public int id { get; set; }

        public string dateTime { get; set; }

        public string logoURL { get; set; }

        public string imageURL { get; set; }

        public string imagePath { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string detailsURL { get; set; }

        public string PDF { get; set; }

        public string PDFUrl { get; set; }

        public bool isLiveTicker { get; set; }

        public string isRead { get; set; }

        public bool isReadOnly { get; set; }

        public string redirectToAction { get; set; }


        // new property add for fit flash/popup and survey
        public ModelType modelType { get; set; }

        public bool isShortSurvey { get; set; }

        public string surveyType { get; set; }

        public string shortAnsType { get; set; }

        public string surveyWebLink { get; set; }

        public NewNotificationModel(DataRow dr, string imageUrl)
        {
            if (dr.ItemArray.Length > 0)
            {
                if (dr["ID"] != DBNull.Value)
                {
                    id = Convert.ToInt32(dr["ID"].ToString());
                }

                dateTime = dr["NOTIFICATIONDATE"] as string;
                title = dr["TITLE"] as string;
                description = dr["DESCRIPTION"] as string;
                detailsURL = dr["DETAILSURL"] as string;

                if (DBNull.Value != dr["IMAGEURL"])
                {
                    imageURL = imageUrl + dr["IMAGEURL"].ToString();
                    imagePath = string.Empty;
                }

                logoURL = dr["LOGOURL"] as string;
                isRead = Convert.ToString(dr["ISREAD"]);
                isReadOnly = DBNull.Value != dr["IS_READ_ONLY"] && Convert.ToBoolean(dr["IS_READ_ONLY"]);

                isLiveTicker = DBNull.Value != dr["ISLIVETICKER"] ? Convert.ToBoolean(dr["ISLIVETICKER"]) : false;

                if (dr.Table.Columns.Contains("REDIRECT_TO_ACTION"))
                {
                    redirectToAction = dr["REDIRECT_TO_ACTION"] as string;
                }

                modelType = (ModelType)Convert.ToInt32(dr["MODEL_TYPE"]);

                if (modelType == ModelType.Survey)
                {
                    isShortSurvey = Convert.ToBoolean(dr["IS_SHORT_SURVEY"]);
                    surveyType = dr["SURVEY_TYPE"] as string;
                    shortAnsType = dr["ANSWER_TYPE"] as string;
                    surveyWebLink = dr["SURVEYWEBLINK"] as string;
                }

            }
        }
    }
}
