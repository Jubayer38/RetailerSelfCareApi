///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Log Write Model
///	Creation Date :	14-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.ViewModel.LogModels
{
    public class LogModel
    {
        public long logId { get; set; } = DateTime.Now.Ticks;

        [MaxLength(13), MinLength(3)]
        public string retailerCode { get; set; }

        public int isSuccess { get; set; }
        public string errorMessage { get; set; }

        public string methodName { get; set; } = string.Empty;

        public string requestBody { get; set; } = string.Empty;

        public string responseBody { get; set; } = string.Empty;
        public string iTopUpNumber { get; set; }
        public DateTime apiStartTime { get; set; } = DateTime.Now;
        public DateTime apiEndTime { get; set; }
        public double totalApiTimeInS { get; set; }
        public string hostAndOtherInfo { get; set; }
        public string userAgentNdIP { get; set; }

        [JsonIgnore]
        public string deviceId { get; set; }

        [JsonIgnore]
        public string lan { get; set; }

        public string sessionToken { get; set; } = string.Empty;
    }
}