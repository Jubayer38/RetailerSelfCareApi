///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	08-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.ResponseModel
{
    /// <summary>
    /// Class that includs common response property of reseller app. 
    /// </summary>
    public class RACommonResponse
    {
        /// <summary>
        /// Data contains if api request success or not!
        /// </summary>
        public bool result { get; set; }
        /// <summary>
        /// Data contains api request result's message (i.e. "Success", "Security token invalid!")
        /// </summary>
        public string message { get; set; }

        public object ErrorDetails { get; set; }
    }
}
