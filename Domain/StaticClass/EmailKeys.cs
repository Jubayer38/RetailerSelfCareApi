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


namespace Domain.StaticClass
{
    public class EmailKeys
    {
        public static string SenderEmail { get; set; }

        public static string SenderCred { get; set; }

        public static string Host { get; set; }

        public static int Port { get; set; }

        public static bool EnableSsl { get; set; }

        public static bool UseDefaultCre { get; set; }

        public static string Subject { get; set; }

        public static string Body { get; set; }

        public static bool IsHtml { get; set; }
    }
}
