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
using System.Text.Json.Serialization;


namespace Domain.RequestModel
{
    public class EVPinResetStatusRequest
    {
        [Required]
        public string userName { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public long resetReqId { get; set; }

        [Required]
        public string iTopUpNumber { get; set; }

        [Required]
        public int status { get; set; }

        [JsonIgnore]
        public long userId { get; set; }
    }
}
