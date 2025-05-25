///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	LoginSmartPos Controller
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************
///	
namespace Domain.StaticClass
{
    public class MimeTypes
    {
        /// <summary>
        /// This Prop return "application/json"
        /// </summary>
        public static string Json { get { return "application/json"; } }

        /// <summary>
        /// This Prop return "application/x-www-form-urlencoded"
        /// </summary>
        public static string X_WwW_Form_UrlEncoded { get { return "application/x-www-form-urlencoded"; } }

        public static string MultipartFormData { get { return "multipart/form-data"; } }


        /// <summary>
        /// This Prop return "application/vnd.api+json"
        /// </summary>
        public static string VndApiJson { get { return "application/vnd.api+json"; } }

        /// <summary>
        /// This Prop return "application/vnd.banglalink.apihub-v1.0+json"
        /// </summary>
        public static string VndBLApiJson { get { return "application/vnd.banglalink.apihub-v1.0+json"; } }

        public static string TextXml { get { return "text/xml; encoding='utf-8'"; } }
    }
}
