///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Basher Sarkar
///	Purpose	            :	
///	Creation Date       :   24-Feb-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------
///	*****************************************************************

using Domain.Resources;
using Domain.ViewModel.LogModels;
using System.Text.RegularExpressions;

namespace Domain.Helpers
{
    public sealed class EvIrisMessageParsing
    {
        public static string ParseMessage(string message)
        {
            LogModel logMOdel = new()
            {
                apiStartTime = DateTime.Now,
                isSuccess = 0,
                methodName = "ParseMessage",
                errorMessage = message,
                apiEndTime = DateTime.Now
            };
            LoggerService.WriteTraceMsg(logMOdel);

            message = message.Replace("\\n", "");
            Regex regex = new("^\\d+");
            Regex digitCheckRegex = new("\\d+");
            Regex hasDigitWithWord = new(@"\b\D+\b");

            Match match = regex.Match(message);
            if (match.Success)
                return SharedResource.GetLocal(match.Value, message);
            else if (message.Contains("404"))
                return SharedResource.GetLocal("Recharge404", message);

            Match wordDigitMatch = hasDigitWithWord.Match(message);
            Match matchDigit = digitCheckRegex.Match(message);
            if (wordDigitMatch.Success && matchDigit.Success)
            {
                string amount = matchDigit.Value;
                string resp = SharedResource.GetLocal("RechargeAmountFailed", message);
                resp = string.Format(resp, amount);
                return resp;
            }

            if (!matchDigit.Success)
            {
                // Add space after period
                string newMessage = message.Replace(".", ". ");
                // Extract first character of each word
                string wordFirstChars = string.Join("", Regex.Matches(newMessage, @"\b\w"));
                return SharedResource.GetLocal(wordFirstChars, message);
            }

            return message;
        }


        //public static string ParseMessageV2(string message)
        //{
        //    Match match = Regex.Match(message, @"(^\d+)|404|(\d+)|(\b\D+\b)|(\b\w+)");
        //    if (match.Success)
        //    {
        //        string key = match.Value switch
        //        {
        //            string n when n.All(char.IsDigit) => "RechargeAmountFailed",
        //            "404" => "Recharge404",
        //            _ => string.Join("", message.Split().Select(word => word[0])),
        //        };
        //        return SharedResource.GetLocal(key, message);
        //    }

        //    return message;
        //}
    }
}