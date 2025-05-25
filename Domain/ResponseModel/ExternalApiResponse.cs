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
    public class ExternalApiResponse
    {
        public bool success { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
    }

    public class Response<T>
    {
        public T Object { get; set; }
        public List<T> Objects { get; set; }
    }
}
