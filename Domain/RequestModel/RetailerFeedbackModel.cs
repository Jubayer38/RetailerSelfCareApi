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


namespace Domain.RequestModel
{
    public class RetailerFeedbackModel
    {
        public int id { get; set; }
        public string retailerCode { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public List<string> imageList { get; set; }
        public string imagePath { get; set; }
        public string imageNames { get; set; }
        public int categoryId { get; set; }
        public int operatorId { get; set; }
        public int userId { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }
    }
}
