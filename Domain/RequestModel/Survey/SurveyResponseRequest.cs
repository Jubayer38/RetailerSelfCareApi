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

using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel.Survey
{
    public class SurveyResponseRequest : RetailerRequest
    {
        [Required]
        public int surveyId { get; set; }

        [Required]
        public bool isShortSurvey { get; set; }

        [Required]
        public string answer { get; set; }
    }
}