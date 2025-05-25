///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
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
    public class TarVsAchvRequest
    {
        public string sessionToken { get; set; }
        public string retailerCode { get; set; }

        private string _lan = "en";
        public string lan { get { return _lan; } set { _lan = string.IsNullOrEmpty(value) ? _lan : value; } }

        public int kpiInt;
        public string kpiId { get { return kpiInt.ToString(); } set { kpiInt = Convert.ToInt32(value); } }
    }

    /// <summary>
    /// Added By: Basher
    /// Last Modified: 20-Feb-2023
    /// </summary>
    public class TarVsAchvRequestV2 : RetailerRequest
    {
        public int kpiInt;
        public string kpiId { get { return kpiInt.ToString(); } set { kpiInt = Convert.ToInt32(value); } }
    }
}
