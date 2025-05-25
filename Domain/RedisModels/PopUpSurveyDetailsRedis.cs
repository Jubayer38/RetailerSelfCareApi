///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Survey (PopUp) view model for Redis
///	Creation Date :	16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************

using System.Data;

namespace Domain.RedisModels
{
    public class PopUpSurveyDetailsRedis
    {
        public long surveyId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool isShortSurvey { get; set; }
        public string surveyType { get; set; }
        public string shortAnsType { get; set; }
        public string surveyWebLink { get; set; }
        public string schedulerTime { get; set; }


        public PopUpSurveyDetailsRedis()
        { }


        public PopUpSurveyDetailsRedis(DataRow row)
        {
            if (row.ItemArray.Length > 0)
            {
                long.TryParse(row["SURVEY_ID"].ToString(), out long _id);
                surveyId = _id;

                title = row["TITLE"] as string;
                description = row["DESCRIPTION"] as string;
                surveyType = row["SURVEY_TYPE"] as string;
                shortAnsType = row["ANSWER_TYPE"] as string;

                string _isShortSurveyStr = row["IS_SHORT_SURVEY"].ToString();
                int.TryParse(_isShortSurveyStr, out int _isShortSurvey);
                isShortSurvey = Convert.ToBoolean(_isShortSurvey);

                surveyWebLink = row["SURVEY_WEB_LINK"] as string;
                schedulerTime = row["SCHEDULER_TIME"] as string;
            }
        }
    }
}
