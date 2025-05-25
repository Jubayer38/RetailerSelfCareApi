///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	User Validation Response Model
///	Creation Date :	11-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Data;

namespace Domain.ResponseModel
{
    public class RechargePackageModel
    {
        public string title { get; set; }

        public string offerType { get; set; }

        public string category { get; set; }

        public string dataPack { get; set; }

        public string talkTime { get; set; }

        public string sms { get; set; }

        public string toffee { get; set; }

        public string validity { get; set; }

        public int commission { get; set; }

        public int amount { get; set; }

        public string rechargeType { get; set; }

        public RechargePackageModel() { }

        public RechargePackageModel(DataRow dr, string lan)
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