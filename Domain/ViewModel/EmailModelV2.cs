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

using System.Net.Mail;

namespace Domain.ViewModel
{
    public class EmailModelV2
    {
        public string SenderEmail { get; set; }

        public string SenderPassword { get; set; }

        public string ReceiverEmail { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpHost { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool EnableSsl { get; set; }

        public bool UseDefaultCred { get; set; }

        public bool IsBodyHtml { get; set; }

        public Attachment Attachment { get; set; }

        public string Regards { get; set; }
    }
}
