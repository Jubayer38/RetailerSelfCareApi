using Newtonsoft.Json;
///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Borhan uddin amin
///	Purpose	      :	
///	Creation Date :	30-Sep-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Domain.ResponseModel
{
    public class StarTrekResponse
    {
        public List<Included> included { get; set; } = new();
    }

    public class Included
    {
        public Attributes attributes { get; set; }
    }
    public class Attributes
    {
        public string brand { get; set; }

        [JsonProperty("business-model-type")]
        public string businessModelType { get; set; }
        public string code { get; set; } = string.Empty;
    }
}
