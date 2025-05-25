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


namespace Domain.ViewModel
{
    public class SMSInformationModel
    {
        public string SMSApiUrl { get; set; }
        public string SenderAddress { get; set; } = "Banglalink";
        public string ReceiverAddress { get; set; }
        public string SMSBody { get; set; }
        public int SMSCoding { get; set; }
        public int status { get; set; }
        public string remarks { get; set; }
        public bool isSuccess { get; set; }
        public DateTime deliveredOn { get; set; }
    }
}
