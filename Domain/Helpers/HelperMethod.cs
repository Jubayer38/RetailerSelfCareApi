///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Helper Method
///	Creation Date :	13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.RequestModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Mapster;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Domain.Helpers
{
    public sealed class HelperMethod
    {
        public static string SubStrString(string str, int length = 1000)
        {
            string retString = "";

            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return "";
                }
                retString = str.Substring(0, Math.Min(str.Length, length));
            }
            catch
            { }

            return retString;
        }


        /// <summary>
        /// Generate exception message by adding _skip_ format
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static string ExMsgBuild(Exception ex, string methodName)
        {
            string retString = "";
            try
            {
                if (ex.InnerException != null)
                {
                    retString = ex.InnerException.Message + "_split_" + methodName;
                }
                else
                {
                    retString += ex.Message + "_split_" + methodName;
                }
            }
            catch
            { }

            return retString;
        }


        public static string ExMsgSubString(Exception ex, string methodName, int length = 1000)
        {
            string retString = "";
            try
            {
                string errMsg = "";
                if (ex.InnerException != null)
                {
                    if (string.IsNullOrEmpty(methodName))
                        errMsg = ex.InnerException.Message;
                    else
                        errMsg = methodName + " || " + ex.InnerException.Message;

                    retString = errMsg.Substring(0, Math.Min(errMsg.Length, length));
                }
                else
                {
                    if (string.IsNullOrEmpty(methodName))
                        errMsg = ex.Message;
                    else
                        errMsg = methodName + " || " + ex.Message;

                    retString = errMsg.Substring(0, Math.Min(errMsg.Length, length));
                }
            }
            catch
            { }

            return retString;
        }


        /// <summary>
        /// Get error message when binding a model. This function only return a object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T ModelBinding<T>(DataRow dr, string extLabel = "", string lan = "")
        {
            string modelName = typeof(T).Name;
            try
            {
                if (string.IsNullOrEmpty(lan))
                {
                    return (T)Activator.CreateInstance(typeof(T), new object[] { dr });
                }
                else
                {
                    return (T)Activator.CreateInstance(typeof(T), new object[] { dr, lan });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw new Exception(ex.InnerException.ToString() + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
                }
                throw new Exception(ex.Message + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
            }
        }


        /// <summary>
        /// Get error message when binding a model. This function only return a object.
        /// This function take another Generic Model as Parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="dr"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T ModelBinding<T, S>(DataRow dr, S item, string extLabel = "")
        {
            string modelName = typeof(T).Name;
            try
            {
                return (T)Activator.CreateInstance(typeof(T), new object[] { dr, item });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw new Exception(ex.InnerException.ToString() + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
                }
                throw new Exception(ex.Message + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
            }
        }


        /// <summary>
        /// Get error message when binding a model. This function only return a object.
        /// This function take another Generic List Model as Parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="dr"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T ModelBinding<T, S>(DataRow dr, List<S> items, string extLabel = "")
        {
            string modelName = typeof(T).Name;
            try
            {
                return (T)Activator.CreateInstance(typeof(T), new object[] { dr, items });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw new Exception(ex.InnerException.ToString() + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
                }
                throw new Exception(ex.Message + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
            }
        }


        /// <summary>
        /// Get error message when binding a model. This function only return a object. and received a DataTable as param.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="extLabel"></param>
        /// <param name="lan"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T ModelBinding<T>(DataTable dt, string extLabel = "", string lan = "")
        {
            try
            {
                string modelName = typeof(T).Name;

                try
                {
                    if (string.IsNullOrEmpty(lan))
                    {
                        return (T)Activator.CreateInstance(typeof(T), new object[] { dt });
                    }
                    else
                    {
                        return (T)Activator.CreateInstance(typeof(T), new object[] { dt, lan });
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        throw new Exception(ex.InnerException.ToString() + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
                    }
                    throw new Exception(ex.Message + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    new Exception(ex.InnerException.ToString() + "_split_GenericTypeGetFaild");
                }
                throw new Exception(ex.Message + "_split_GenericTypeGetFaild");
            }
        }


        public static string AddSpaceInCapitalChar(string valueStr, string lan)
        {
            if (lan == "bn") return valueStr;
            if (string.IsNullOrEmpty(valueStr)) return valueStr;

            string newStr = Regex.Replace(valueStr, "(\\B[A-Z])", " $1");
            return newStr;
        }


        public static string BuildTraceMessage(string oldMsg, string newMsg, Exception ex)
        {
            if (ex != null)
            {
                if (ex.InnerException != null)
                    newMsg += " || " + ex.InnerException.Message;
                else
                    newMsg += " || " + ex.Message;
            }

            if (string.IsNullOrEmpty(oldMsg))
            {
                return newMsg;
            }
            else
            {
                return oldMsg + " || " + newMsg;
            }
        }


        public static string GetOnlyNumeric(string strValue)
        {
            Regex rgx = new(@"[^0-9০-৯]");
            string newStr = rgx.Replace(strValue, "");
            return newStr;
        }


        public static void AllowAllSecurityPrototols()
        {
            Array types;
            SecurityProtocolType combined;

            types = Enum.GetValues(typeof(SecurityProtocolType));
            combined = (SecurityProtocolType)types.GetValue(0);

            int[] arr = Enumerable.Range(0, types.Length).ToArray();
            for (int i = 1; i < arr.Length; i += 1)
            { combined |= (SecurityProtocolType)types.GetValue(i); }

            ServicePointManager.SecurityProtocol = combined;
        }


        public static string BuildErrorDetails(Exception ex, string methodName)
        {
            string errString = string.Empty;
            try
            {
                string errMsg = string.Empty;
                if (ex.InnerException != null)
                {
                    if (string.IsNullOrEmpty(methodName))
                        errMsg = ex.InnerException.Message;
                    else
                        errMsg = methodName + " || " + ex.InnerException.Message;
                }
                else
                {
                    if (string.IsNullOrEmpty(methodName))
                        errMsg = ex.Message;
                    else
                        errMsg = methodName + " || " + ex.Message;
                }

                errString = BuildTraceMessage(errMsg, ex.StackTrace, null);
            }
            catch
            { }

            return errString;
        }


        /// <summary>
        /// Get milisecond from DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeInMilisecond(DateTime? dateTime)
        {
            DateTime _dateTime = DateTime.Now;
            if (dateTime.HasValue) _dateTime = dateTime.Value;

            DateTimeOffset now = _dateTime;
            long milisecond = now.ToUnixTimeMilliseconds();

            return milisecond;
        }


        public static string GetInvalidParameterMsg(string paramName)
        {
            return "Invalid Parameter: " + paramName + " is required.";
        }


        public static string FormattedExceptionMsg(Exception ex)
        {
            string msg;
            if (ex.InnerException != null)
            {
                msg = ex.InnerException.Message;
            }
            else
            {
                msg = ex.Message;
            }

            return msg;
        }


        public static string FormattedRedisError(Exception ex, string methodName, string CollectionName = "", string key = "", string dataStr = "")
        {
            string errString = "";
            try
            {
                string errMsg = "";
                if (ex.InnerException != null)
                {
                    errMsg = ex.InnerException.Message;
                }
                else
                {
                    errMsg = ex.Message;
                }

                errString = methodName;

                if (!string.IsNullOrEmpty(CollectionName))
                {
                    errString = errString + " || " + CollectionName;
                }

                if (!string.IsNullOrEmpty(key))
                {
                    errString = errString + " || " + key;
                }

                if (!string.IsNullOrEmpty(dataStr))
                {
                    errString = errString + " || " + dataStr;
                }

                errString = errString + " || " + errMsg;
            }
            catch
            { }

            return errString;
        }


        /// <summary>
        /// Parse string datetime at specific datetime format
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime ParseExactDate(string dateTime, string format)
        {
            DateTime formatedDateTime = DateTime.ParseExact(dateTime, format, CultureInfo.InvariantCulture);
            return formatedDateTime;
        }


        /// <summary>
        /// add + or - sign before number string and parse it to integer.
        /// </summary>
        /// <param name="numberStr"></param>
        /// <param name="positiveNegativeIndicator"></param>
        /// <returns></returns>
        public static int ParseNumberString(string numberStr, int positiveNegativeIndicator = 1)
        {
            string plusMinusSign = positiveNegativeIndicator == 1 ? "+" : "-";
            string numberString = plusMinusSign + numberStr;
            int.TryParse(numberString, out int result);
            return result;
        }


        /// <summary>
        /// Check weather a json string is a valid json or not.
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    JsonConvert.DeserializeObject<RetailerRequest>(strInput);
                    return true;
                }
                catch { return false; }
            }
            else
            {
                return false;
            }
        }


        public static string GetIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string interNetworkAddress = "";
            for (int i = 0; i < host.AddressList.Length; i++)
            {
                IPAddress ip = host.AddressList[i];
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    interNetworkAddress = "Local Ip: " + ip.ToString();
                else
                    interNetworkAddress = "All Ip: " + string.Join(", ", host.AddressList.ToList());
            }

            return interNetworkAddress;
        }


        public static string GetOnlyIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string interNetworkAddress = "";
            for (int i = 0; i < host.AddressList.Length; i++)
            {
                IPAddress ip = host.AddressList[i];
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    interNetworkAddress = ip.ToString();
                else
                    interNetworkAddress = host.AddressList.LastOrDefault().ToString();
            }

            return interNetworkAddress;
        }


        /// <summary>
        /// Get error message when binding a model. This function only return a object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T ModelBinding<T>(object dr, string extLabel = "", string lan = "")
        {
            string modelName = typeof(T).Name;
            try
            {
                if (string.IsNullOrEmpty(lan))
                {
                    return (T)Activator.CreateInstance(typeof(T), new object[] { dr });
                }
                else
                {
                    return (T)Activator.CreateInstance(typeof(T), new object[] { dr, lan });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw new Exception(ex.InnerException.ToString() + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
                }
                throw new Exception(ex.Message + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
            }
        }


        public static Dictionary<string, object> GetStaticPropertyDict(Type type)
        {
            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var map = new Dictionary<string, object>();
            PropertyInfo[] props = type.GetProperties(flags);

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];
                map[prop.Name] = prop.GetValue(null, null);
            }

            return map;
        }


        public static T ModelBinding<T>(object dr, bool isTrue, string extLabel = "", string lan = "")
        {
            string modelName = typeof(T).Name;
            try
            {
                if (string.IsNullOrEmpty(lan) && isTrue)
                {
                    return (T)Activator.CreateInstance(typeof(T), new object[] { dr, isTrue });
                }
                else
                {
                    return (T)Activator.CreateInstance(typeof(T), new object[] { dr, lan });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw new Exception(ex.InnerException.ToString() + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
                }
                throw new Exception(ex.Message + "_split_" + modelName + "_Binding" + (string.IsNullOrEmpty(extLabel) ? "" : "_" + extLabel));
            }
        }


        /// <summary>
        /// Validate token for prevent login from blocked version
        /// </summary>
        /// <param name="versionName"></param>
        /// <param name="appToken"></param>
        /// <returns></returns>
        public static bool IsTokenValid(int versionName, string appToken)
        {
            bool isTokenSuccess;
            if (!string.IsNullOrWhiteSpace(appToken))
            {
                Dictionary<string, object> propertyDict = GetStaticPropertyDict(typeof(AndroidAppTokens));
                string dictKey = $"V{versionName}";

                bool haskey = propertyDict.TryGetValue(dictKey, out object proprtyValue);
                if (!haskey) isTokenSuccess = false;
                else
                {
                    string versionWiseToken = proprtyValue as string;
                    isTokenSuccess = versionWiseToken.Equals(appToken);
                }
            }
            else
            {
                isTokenSuccess = false;
            }

            return isTokenSuccess;
        }


        /// <summary>
        /// This LogModelBind method made for EVLog and IRISLog Table.
        /// <para>If need for more method then need to test by criteria, Or add new one</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="controllerContext"></param>
        public static L LogModelBind<T, L>(T source, L destination, string methodName, string userAgent)
        {
            var instance = source.Adapt<L>();
            DateTime startDT = DateTime.Now;
            string hostIp = GetIPAddress();

            InitModelValue<L>(instance, "startTime", startDT);
            InitModelValue<L>(instance, "isSuccess", 1);
            InitModelValue<L>(instance, "errorMessage", string.Empty);
            InitModelValue<L>(instance, "reqBodyStr", source.ToJsonString());
            InitModelValue<L>(instance, "resBodyStr", string.Empty);
            InitModelValue<L>(instance, "methodName", methodName);
            InitModelValue<L>(instance, "ipAddress", hostIp);
            InitModelValue<L>(instance, "userAgentNdIp", userAgent);

            return instance;
        }


        /// <summary>
        /// This function set value in a Model Class for RSL.Entity project.
        /// <para>If want to use for another project then you can achived that by sending project name by param and use by below</para>
        /// <code>Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains(projectName)).FirstOrDefault();</code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        public static void InitModelValue<T>(object instance, string propName, dynamic value)
        {
            // Get the type contained in the name string
            Type type = instance.GetType();

            // Get a property on the type that is stored in the 
            // property string
            PropertyInfo prop = type.GetProperty(propName);

            // Set the value of the given property on the given instance
            if (prop != null)
            {
                prop.SetValue(instance, value, null);
            }
        }


        public static async Task SendEMail(EmailModelV2 mailModel)
        {
            mailModel.SenderEmail = EmailKeys.SenderEmail;
            mailModel.SenderPassword = EmailKeys.SenderCred;
            mailModel.SmtpHost = EmailKeys.Host;
            mailModel.SmtpPort = EmailKeys.Port;

            MailMessage mail = new();
            mail.To.Add(mailModel.ReceiverEmail);
            mail.Subject = mailModel.Subject;
            mail.IsBodyHtml = mailModel.IsBodyHtml;

            if (mailModel.Attachment != null)
                mail.Attachments.Add(mailModel.Attachment);

            string mailBody = string.Empty;

            if (!string.IsNullOrEmpty(mailModel.Body))
            {
                mailBody += "​<br/><br/>" + mailModel.Body;
            }

            if (mailModel.Attachment != null)
                mailBody += "<i>Please check attatch file.</i>";

            if (!string.IsNullOrEmpty(mailModel.Regards))
            {
                string[] regardsLines = mailModel.Regards.Split('~');

                if (regardsLines.Length > 0)
                {
                    mailBody += "​<br/><br/><br/><i><small>With Best regards,</small></i>";

                    for (int i = 0; i < regardsLines.Length; i++)
                    {
                        string text = regardsLines[i];
                        if (i == 0)
                        {
                            string titleCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
                            mailBody += "<br/><b><i>" + titleCase + "</i></b>";
                        }
                        else
                        {
                            mailBody += "<br/><i>" + text + "</i>";
                        }
                    }
                }
            }

            mail.Body = mailBody;

            NetworkCredential credentials = new(mailModel.SenderEmail, mailModel.SenderPassword);

            using (var smtp = new SmtpClient(mailModel.SmtpHost, mailModel.SmtpPort))
            {
                smtp.UseDefaultCredentials = mailModel.UseDefaultCred;
                smtp.Credentials = credentials;
                smtp.EnableSsl = mailModel.EnableSsl;

                await smtp.SendMailAsync(mail);
                await Task.FromResult(0);
            };
        }


        public static string GetRespMsgWithLengthLimit(string msg, int stringLenLimit)
        {
            string message = "";

            if (!string.IsNullOrEmpty(msg))
            {
                if (msg.Length >= stringLenLimit)
                    message = msg.Substring(0, stringLenLimit);
                else
                    message = msg;
            }

            return message;
        }


        public static string GetRespMsgWithLengthLimit(Exception ex, int stringLenLimit)
        {
            string message = "";

            if (ex.InnerException != null)
            {
                string errMsg = ex.InnerException.Message;
                if (errMsg.Length >= stringLenLimit)
                    message = errMsg.Substring(0, stringLenLimit);
                else
                    message = errMsg;
            }
            else
            {
                string errMsg = ex.Message;
                if (errMsg.Length >= stringLenLimit)
                    message = errMsg.Substring(0, stringLenLimit);
                else
                    message = errMsg;
            }

            return message;
        }


        /// <summary>
        /// Parse message and get latest amount, which was getting after recharge
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string FormatEvBalanceResponse(string msg)
        {
            if (msg != null)
            {
                int index1 = msg.ToLower().LastIndexOf("tk") + 3;
                int index2 = msg.Length - index1;
                string amount = msg.Substring(index1, index2);
                amount = amount.Substring(0, amount.Length - 1);

                return amount;
            }
            else
            {
                throw new Exception("No Balance Data Found.");
            }
        }


        public static CultureInfo GetCultureInfo(string cultureCode)
        {
            CultureInfo customCulture = CultureInfo.GetCultureInfo(cultureCode);
            return customCulture;
        }


        public static bool CheckSalesSummaryDbCallEligibility(string existKeyStr, string newKeystr)
        {
            string existDateStr = existKeyStr.Split('_')[1];
            string newDateStr = newKeystr.Split('_')[1];
            _ = long.TryParse(newDateStr, out long _newDateInt);
            _ = long.TryParse(existDateStr, out long _existDateInt);

            DateTime _newKeyDate = new(_newDateInt);
            DateTime _existKeyDate = new(_existDateInt);

            int result = (int)(_newKeyDate - _existKeyDate).TotalMinutes;
            return result > 15;
        }


        public static int GenerateSecureRandomPassword()
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] randomNumber = new byte[4];
            rng.GetBytes(randomNumber);
            int generatedValue = BitConverter.ToInt32(randomNumber, 0);

            // Ensure the value is within the desired range (111111 to 999999)
            return Math.Abs(generatedValue % (999999 - 111111 + 1)) + 111111;
        }
    }
}