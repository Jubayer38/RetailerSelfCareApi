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


namespace Domain.ViewModel
{
    public class Question
    {
        public int id { get; set; }
        public string ques_descrip { get; set; }
        public string input_type { get; set; }
        public int? linear_start_value { get; set; }
        public int? linear_end_value { get; set; }
        public string linear_start_level_txt { get; set; }
        public string linear_end_level_txt { get; set; }
        public int created_by { get; set; }
        public bool isRequired { get; set; }
        public int quesOrder { get; set; }

        public string optionsStr { get; set; }
        public string linearScale { get; set; }
        public string rowStr { get; set; }
        public string columnStr { get; set; }

        public virtual List<QuestionOptions> options { get; set; }
        public virtual List<Row> rows { get; set; }
        public virtual List<Column> columns { get; set; }
    }
}
