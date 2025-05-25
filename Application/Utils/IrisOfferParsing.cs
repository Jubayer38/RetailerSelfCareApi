///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Basher Sarkar
///	Purpose	            :	
///	Creation Date       :   29-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     29-Jan-2024		Basher Sarkar	7.0.0		upgrade project to .Net 8
///	 ----------------------------------------------------------------
///	*****************************************************************

using Domain.ViewModel;
using System.Text.RegularExpressions;

namespace Application.Utils
{
    public sealed class IrisOfferParsing
    {
        public static string ParseAmount(string offer)
        {
            Regex regex = new(@"(\d+)(?=tk(-|_|\+))", RegexOptions.IgnoreCase);
            string tkString = regex.Match(offer).Value;

            tkString = string.IsNullOrWhiteSpace(tkString) ? "00" : tkString;
            return tkString;
        }


        /// <summary>
        /// Tuple first item is talktime value and 2nd item is offertype
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="isRateCutter"></param>
        /// <returns></returns>
        public static (string, string) ParseIRISVoiceOffer(string offer, ref bool isRateCutter)
        {
            string offerType = string.Empty;
            Regex regex = new(@"\d+\.?\d*(min|p\/min|p\/sec)");
            Match match = regex.Match(offer);
            string voiceOffer = match.Value;

            Regex ratecutterPattern = new(@"p\/min|p\/sec");
            Match match2 = ratecutterPattern.Match(voiceOffer);

            isRateCutter = false;
            string talkTime;
            if (match2.Success)
            {
                talkTime = voiceOffer.Replace("p/m", " P/M").Replace("p/s", " P/S");
                isRateCutter = true;
                offerType = " Rate Cutter";
            }
            else
            {
                talkTime = voiceOffer.Replace("m", " M");
                offerType = " Voice";
            }

            talkTime = string.IsNullOrWhiteSpace(talkTime) ? "0" : talkTime;
            return (talkTime, offerType);
        }


        public static string ParseSMS(string offer)
        {
            Regex smsRegex = new(@"(\d+)sms", RegexOptions.IgnoreCase);
            string smsString = smsRegex.Match(offer).Value;
            string sms = smsString == string.Empty ? "0" : Regex.Replace(smsString, @"(sms|SMS)", " SMS");
            return sms;
        }


        public static IrisDataOfferParseVM ParseIRISDataOffer(string offer)
        {
            Regex datapackRegex = new(@"(?<=-)(\*)?(\d+\.?\d*)(gb|mb)(?=_regular|\+)?", RegexOptions.IgnoreCase);
            Regex perdayDataPackRegex = new(@"(\d+\.?\d*)(gb|mb)/day");
            Regex toffeeRegex = new(@"(?<=\+)(\d+\.?\d*)(gb|mb)(?=_toffee)");
            Regex streamingRegex = new(@"(\d+\.?\d*)(gb|mb)(?=_Streaming)", RegexOptions.IgnoreCase);

            string _dataPack = datapackRegex.Match(offer).Value;
            string _perDayData = perdayDataPackRegex.Match(offer).Value;
            string _toffee = toffeeRegex.Match(offer).Value;
            string _streaming = streamingRegex.Match(offer).Value;

            IrisDataOfferParseVM model = new()
            {
                dataPack = _dataPack == string.Empty ? "0" : _dataPack.Replace("*", "").ToUpper(),
                perDayData = _perDayData == string.Empty ? "0" : _perDayData.ToUpper(),
                toffee = _toffee == string.Empty ? "0" : _toffee.ToUpper(),
                streamingPack = string.IsNullOrWhiteSpace(_streaming) ? "0" : _streaming.ToUpper()
            };

            model.offerType = (model.toffee != "0" && model.dataPack != "0") ? " Data Toffee"
                : (model.toffee != "0" && model.dataPack == "0") ? " Toffee"
                : model.dataPack != "0" ? " Data" : model.offerType;

            if (model.streamingPack is not "0")
            {
                model.hasStreamingPack = true;
                model.offerType += " Streaming";
            }

            return model;
        }


        public static string ParseValidity(string offer)
        {
            string pattern = @"(\d+\.?\d*)(hr|d)";
            Regex regex = new(pattern);
            Match match = regex.Match(offer);

            string unit = match.Groups[2].Value;
            string digits = match.Groups[1].Value;
            _ = int.TryParse(digits, out int _digit);

            string validity;
            if (unit.Equals("d"))
                validity = digits + (_digit > 1 ? " Days" : " Day");
            else if (string.IsNullOrWhiteSpace(unit))
                validity = "0";
            else
                validity = digits + (_digit > 1 ? " Hours" : " Hour");

            return validity;
        }


        public static string ParseCommissionAmount(string offerDisplayName)
        {
            Regex commRegex = new(@"(?<=\sc\-)\d+(?<!tk)", RegexOptions.IgnoreCase);
            string commissionAmt = commRegex.Match(offerDisplayName).Value;
            string commission = string.IsNullOrWhiteSpace(commissionAmt) ? "0" : commissionAmt;
            return commission;
        }


        /// <summary>
        /// Tuple first item is talktime value and 2nd item is offertype. This v2 added for tk/min
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="isRateCutter"></param>
        /// <returns></returns>
        public static (string, string) ParseIRISVoiceOfferV2(string offer, ref bool isRateCutter)
        {
            string offerType = string.Empty;
            Regex regex = new(@"\d+\.?\d*(min|p\/min|p\/sec|tk\/min)");
            Match match = regex.Match(offer);
            string voiceOffer = match.Value;

            Regex ratecutterPattern = new(@"p\/min|p\/sec|tk\/min");
            Match match2 = ratecutterPattern.Match(voiceOffer);

            isRateCutter = false;
            string talkTime;
            if (match2.Success)
            {
                talkTime = voiceOffer.Replace("p/m", " P/M").Replace("p/s", " P/S").Replace("tk/m", "Tk/M");
                isRateCutter = true;
                offerType = " Rate Cutter";
            }
            else
            {
                talkTime = voiceOffer.Replace("m", " M");
                offerType = " Voice";
            }

            talkTime = string.IsNullOrWhiteSpace(talkTime) ? "0" : talkTime;
            return (talkTime, offerType);
        }

    }
}