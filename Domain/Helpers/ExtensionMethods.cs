///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Various Extension methods of to mitigate repetation
///	Creation Date :	18-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Domain.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get Enum Description from Enum
        /// </summary>
        /// <param name="GenericEnum"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (_Attribs != null && _Attribs.Count() > 0)
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }


        /// <summary>
        /// Convert any object to Json string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }


        /// <summary>
        /// Convert any object to Json string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DataTableToJsonStr(this DataTable dt)
        {
            string jsonStr = (dt.AsEnumerable().Select(s => s.ItemArray), Formatting.None).ToJsonString();
            return jsonStr;
        }


        /// <summary>
        /// Add char/string before another string
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Prepend(this string existingValue, string content)
        {
            return existingValue.Insert(0, content);
        }


        /// <summary>
        /// Add char/string before another string in stringbuilder object
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static StringBuilder Prepend(this StringBuilder sb, string content)
        {
            return sb.Insert(0, content);
        }


        public static string ToCamelCase(this string strValue)
        {
            string result = JsonNamingPolicy.CamelCase.ConvertName(strValue);
            return result;
        }


        public static string ToEnUSDateString(this DateTime dateTime, string formatStr)
        {
            string datetimeStr = dateTime.ToString(formatStr, CultureInfo.InvariantCulture);
            return datetimeStr;
        }


        public static long DBNullToLong(this object obj)
        {
            bool isNull = Convert.IsDBNull(obj);
            long result = (obj is not { } || isNull) ? 0 : Convert.ToInt64(obj.ToString());
            return result;
        }


        public static int DBNullToInteger(this object obj)
        {
            bool isNull = Convert.IsDBNull(obj);
            int result = (obj is not { } || isNull) ? 0 : Convert.ToInt32(obj.ToString());
            return result;
        }


        public static string DBNullToString(this object obj)
        {
            bool isNull = Convert.IsDBNull(obj);
            string result = (obj is not { } || isNull) ? string.Empty : obj.ToString();
            return result;
        }
    }
}