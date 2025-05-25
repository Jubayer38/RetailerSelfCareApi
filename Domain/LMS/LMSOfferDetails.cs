///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	10-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Text.RegularExpressions;

namespace Domain.LMS
{
    public class LMSOfferDetails
    {
        public string rewardID { get; set; }
        public string rewardCategory { get; set; }
        public string partnerName { get; set; }
        public string title { get; set; }
        public string imageURL { get; set; }
        public string shortDescription { get; set; }
        public string longDescription { get; set; }
        public string days { get; set; }
        public string pointsToRedeem { get; set; }
        public string shopsavailable { get; set; }
        public string voucherType { get; set; }
        public string discountType { get; set; }
        public string minOrderValue { get; set; }
        public string maxDiscountAmount { get; set; }
        public string discountPercentage { get; set; }


        public LMSOfferDetails(RewardDetails obj, bool isTelco)
        {
            string _day = string.Empty;
            string _rewardName = obj.rewardName;
            string _partnerName = string.Empty;

            if (isTelco)
            {
                _partnerName = "Telecom";
                string daysPattern = @"(\d+)\|(?:DAY|DAYS)";
                _day = Regex.Match(obj.offerLongDescription1, daysPattern, RegexOptions.IgnoreCase).Groups[1].Value;
                _day += _day.Equals("1") ? " Day" : " Days";

                string voiceStr = string.Empty;
                string smsStr = string.Empty;
                string dataStr = string.Empty;
                string tarrifOnNetStr = string.Empty;
                string tarrifOffNetStr = string.Empty;
                int voiceVal = 0;
                int smsVal = 0;
                double dataVal = 0;
                double tarrifOnNetVal = 0;
                double tarrifOffNetVal = 0;

                Regex regex = new(@"(?<=VOICE\|)\d+(\.\d+)?\|MIN|(?<=SMS\|)\d+(\.\d+)?\|SMS|(?<=DATA\|)\d+(\.\d+)?\|GB|(?<=DATA\|)\d+(\.\d+)?\|MB|(?<=TARIFF\|)\d+(\.\d+)?\|P/SEC\s?ON-NET|(?<=TARIFF\|)\d+(\.\d+)?\|P/SEC\s?OFF-NET", RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(obj.offerLongDescription1);

                if (matches.Count > 0)
                {
                    Regex onlyDigit = new(@"\d+(\.\d+)?", RegexOptions.IgnoreCase);
                    voiceStr = matches[0].Value.Replace("|", " ");
                    int.TryParse(onlyDigit.Match(voiceStr).Value, out voiceVal);

                    if (matches.Count > 1)
                    {
                        smsStr = matches[1].Value.Replace("|", " ");
                        int.TryParse(onlyDigit.Match(smsStr).Value, out smsVal);
                    }

                    if (matches.Count > 2)
                    {
                        dataStr = matches[2].Value.Replace("|", " ");
                        double.TryParse(onlyDigit.Match(dataStr).Value, out dataVal);
                    }

                    if (matches.Count > 3)
                    {
                        tarrifOnNetStr = matches[3].Value.Replace("|", " ");
                        double.TryParse(onlyDigit.Match(tarrifOnNetStr).Value, out tarrifOnNetVal);
                    }

                    if (matches.Count > 4)
                    {
                        tarrifOffNetStr = matches[4].Value.Replace("|", " ");
                        double.TryParse(onlyDigit.Match(tarrifOffNetStr).Value, out tarrifOffNetVal);
                    }
                }

                _rewardName = voiceVal > 0 ? voiceStr :
                    smsVal > 0 ? smsStr :
                    dataVal > 0 ? dataStr :
                    tarrifOnNetVal > 0 ? tarrifOnNetStr :
                    tarrifOffNetVal > 0 ? tarrifOffNetStr : obj.rewardName;

                rewardCategory = obj.rewardCategory.Replace("TELCO-", "");
                shortDescription = obj.smallDescription.Replace("-", " ");
            }

            if (!isTelco)
            {
                if (obj.rewardName.Contains("at"))
                {
                    string pattern = @"(?<=\bat\b\s*)\S+.*";
                    _partnerName = Regex.Match(obj.rewardName, pattern).Value;
                }
                else
                {
                    _partnerName = obj.rewardName;
                }

                rewardCategory = obj.rewardCategory;
                shortDescription = obj.smallDescription;
            }

            rewardID = obj.rewardID;
            partnerName = _partnerName;
            title = _rewardName;
            longDescription = obj.longDescription;
            days = _day;
            pointsToRedeem = obj.pointsToRedeem;
            shopsavailable = obj.shopsavailable;
            voucherType = obj.voucherType;
            discountType = obj.discountType;
            minOrderValue = obj.minOrderValue;
            maxDiscountAmount = obj.maxDiscountAmount;
            discountPercentage = obj.discountPercentage;
            imageURL = obj.imageURL;
        }


        public LMSOfferDetails(RewardDetails obj)
        {
            string _partnerName;
            if (obj.rewardName.Contains("at"))
            {
                string pattern = @"(?<=\bat\b\s*)\S+.*";
                _partnerName = Regex.Match(obj.rewardName, pattern).Value;
            }
            else
            {
                _partnerName = obj.rewardName;
            }

            rewardCategory = obj.rewardCategory;
            shortDescription = obj.smallDescription;

            rewardID = obj.rewardID;
            partnerName = _partnerName;
            title = obj.rewardName;
            longDescription = obj.longDescription;
            days = string.Empty;
            pointsToRedeem = obj.pointsToRedeem;
            shopsavailable = obj.shopsavailable;
            voucherType = obj.voucherType;
            discountType = obj.discountType;
            minOrderValue = obj.minOrderValue;
            maxDiscountAmount = obj.maxDiscountAmount;
            discountPercentage = obj.discountPercentage;
            imageURL = obj.imageURL;
        }

    }
}
