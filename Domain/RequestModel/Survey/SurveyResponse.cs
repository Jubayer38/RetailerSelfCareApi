///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	11-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.ComponentModel.DataAnnotations;


namespace Domain.RequestModel.Survey
{
    public class SurveyResponse
    {
        [Required]
        public int surveyId { get; set; }

        public string questionType { get; set; }

        public int questionId { get; set; }

        public string quesOptions { get; set; }

        [Required]
        public string answer { get; set; }

        public string file { get; set; }

        public string fileMimeType { get; set; }

        public int userId { get; set; }

        public string filePath { get; set; }

        public string retailerCode { get; set; }

        public int isShortSurvey { get; set; }
        public string qnDescription { get; set; }
        public List<SurveyAnswerGrid> qnGridAnswers { get; set; }

        public SurveyResponse()
        {
            qnGridAnswers = new List<SurveyAnswerGrid>();
        }
    }

    public class SurveyAnswerGrid
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int QuestionGridId { get; set; }
        public string QuestionRow { get; set; }
        public string QnRowAnswer { get; set; }
        public string ChangeBy { get; set; }
    }
}
