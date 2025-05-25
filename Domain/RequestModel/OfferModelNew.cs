using System.Data;
using System.Text.Json.Serialization;

namespace Domain.RequestModel
{
    public class OfferModelNew
    {
        public string title { get; set; }

        public string offerType { get; set; }

        public string category { get; set; }

        public string dataPack { get; set; }

        public string perDayDataPack { get; set; }

        public string talkTime { get; set; }

        public string sms { get; set; }

        public string toffee { get; set; }

        public string validity { get; set; }

        public int commission { get; set; }

        public int amount { get; set; }

        public string rechargeType { get; set; }
        public string ussdCode { get; set; }
        public string description { get; set; }
        public string tranId { get; set; }
        public string offerId { get; set; }
        public bool hasStar { get; set; }
        public string streamingPack { get; set; }
        public bool hasStreamingPack { get; set; }

        [JsonIgnore]
        public bool hasDataPack { get; set; }
        [JsonIgnore]
        public bool hasVoicePack { get; set; }
        [JsonIgnore]
        public bool hasSMSPack { get; set; }
        [JsonIgnore]
        public bool hasToffePack { get; set; }
        [JsonIgnore]
        public bool hasRateCutterPack { get; set; }


        public OfferModelNew() { }

        public void SetOfferYype()
        {
            int count = 0;
            if (hasDataPack) count++;
            if (hasVoicePack) count++;
            if (hasSMSPack) count++;

            offerType = count >= 2 ? "IRIS Bundle" : offerType;
        }

        public OfferModelNew(DataRow dr, string lan)
        {
            title = lan == "bn" ? dr["TITLE_BN"] as string : dr["TITLE"] as string;
            dataPack = lan == "bn" ? dr["DATAPACK_BN"] as string : dr["DATAPACK"] as string;
            talkTime = lan == "bn" ? dr["TALKTIME_BN"] as string : dr["TALKTIME"] as string;
            sms = lan == "bn" ? dr["SMS_BN"] as string : dr["SMS"] as string;
            toffee = lan == "bn" ? dr["TOFFEE_BN"] as string : dr["TOFFEE"] as string;
            offerType = lan == "bn" ? dr["OFFERTYPE_BN"] as string : dr["OFFERTYPE"] as string;
            validity = lan == "bn" ? dr["VALIDITY_BN"] as string : dr["VALIDITY"] as string;

            if (dr["COMMISSION"] != DBNull.Value)
            {
                commission = Convert.ToInt32(dr["COMMISSION"].ToString());
            }

            if (dr["AMOUNT"] != DBNull.Value)
            {
                amount = Convert.ToInt32(dr["AMOUNT"].ToString());
            }

            rechargeType = dr["RECHARGETYPE"].ToString();
            category = lan == "bn" ? dr["CATEGORY_BN"] as string : dr["CATEGORY"] as string;
        }

    }
}
