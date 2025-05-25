///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Basher Sarkar
///	Purpose	            :	
///	Creation Date       :   07-Feb-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     07-Feb-2024		Basher Sarkar	7.0.0		upgrade project to .Net 8
///	 ----------------------------------------------------------------
///	*****************************************************************

using Domain.Helpers;

namespace Domain.DummyAmarOffer
{
    public sealed class AmarOffersGenerate
    {
        public static string GenerateAmarOfferResponseStr()
        {
            List<OfferModel> offerList = BuildOfferList();
            string offerListStr = offerList.ToJsonString();

            IRISOfferResponse offerResponse = new()
            {
                offersList = offerListStr,
                statusCode = "0",
                statusMessage = "Success",
                transactionId = $"{DateTime.Now.Ticks}"
            };

            var data = new
            {
                response = offerResponse
            };

            return data.ToJsonString();
        }


        public static dynamic ErrorResponse()
        {
            var offerResponse = new
            {
                transactionId = "R240307.1608.210001",
                statusCode = "206",
                statusMessage = "17017:Your request cannot be processed at this time, please try again later"
            };

            return offerResponse;
        }


        private static List<OfferModel> BuildOfferList()
        {
            List<OfferModel> offerList =
                [
                    new OfferModel()
                    {
                        offerCommission = "0",
                        offerName = "STAR_99tk-2GB_regular+2GB_Streaming-7d_C0",
                        rechargeAmount = "99",
                        sno = "101",
                        offerID = "OFF00101",
                        offerDisplayName = "STAR_99tk-2GB_regular+2GB_Streaming-7d"
                    },
                    new OfferModel()
                    {
                        offerCommission = "9",
                        offerName = "STAR_209tk-*2.5gb-30d_C9",
                        rechargeAmount = "209",
                        sno = "102",
                        offerID = "OFF00102",
                        offerDisplayName = "STAR_209tk-*2.5gb-30d C-9tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "1",
                        offerName = "STAR_39tk-0.9p/sec-30d_C1",
                        rechargeAmount = "39",
                        sno = "103",
                        offerID = "OFF00103",
                        offerDisplayName = "STAR_39tk-0.9p/sec-30d C-1tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "120",
                        offerName = "798tk-800min-40gb-30d_Sprint_C120",
                        rechargeAmount = "798",
                        sno = "104",
                        offerID = "OFF00104",
                        offerDisplayName = "798tk-800min-40gb-30d C-120tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "70",
                        offerName = "499tk-35gb+5.5gb_toffee-30d_Sprint_C70",
                        rechargeAmount = "499",
                        sno = "105",
                        offerID = "OFF00105",
                        offerDisplayName = "499tk-35gb+5.5gb_toffee-30d C-70tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "10",
                        offerName = "137tk+10GB_Streaming-15d_C10",
                        rechargeAmount = "137",
                        sno = "106",
                        offerID = "OFF00106",
                        offerDisplayName = "137tk-+10GB_Streaming-15d C-10tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "0",
                        offerName = "60tk-500sms-30d_U_IRIS_C0",
                        rechargeAmount = "60",
                        sno = "107",
                        offerID = "OFF00107",
                        offerDisplayName = "60tk-500sms-30d"
                    },
                    new OfferModel()
                    {
                        offerCommission = "5",
                        offerName = "STAR_109tk-5gb+5gb_toffee-15d_IRIS_C5",
                        rechargeAmount = "109",
                        sno = "108",
                        offerID = "OFF00108",
                        offerDisplayName = "STAR_109tk-5gb+5gb_toffee-15d C-5tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "60",
                        offerName = "407tk-670min-30d_BI_C60",
                        rechargeAmount = "407",
                        sno = "109",
                        offerID = "OFF00109",
                        offerDisplayName = "407tk-670min-30d C-60tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "20",
                        offerName = "STAR_348tk-300min-10gb-30d_C20.",
                        rechargeAmount = "348",
                        sno = "110",
                        offerID = "OFF00110",
                        offerDisplayName = "STAR_348tk-300min-10gb-30d C-20tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "0",
                        offerName = "9tk-10min-3hr_BI_C0",
                        rechargeAmount = "9",
                        sno = "111",
                        offerID = "OFF00111",
                        offerDisplayName = "9tk-10min-3hr"
                    },
                    new OfferModel()
                    {
                        offerCommission = "0",
                        offerName = "79tk-69p/min-10d",
                        rechargeAmount = "79",
                        sno = "112",
                        offerID = "OFF00112",
                        offerDisplayName = "79tk-69p/min-10d"
                    },
                    new OfferModel()
                    {
                        offerCommission = "1",
                        offerName = "STAR_27tk-*45min-2d_IRIS_C1",
                        rechargeAmount = "27",
                        sno = "113",
                        offerID = "OFF00113",
                        offerDisplayName = "STAR_27tk-*45min-2d C-1tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "0",
                        offerName = "Pop Up",
                        rechargeAmount = "0",
                        sno = "114",
                        offerID = "OFF00114",
                        offerDisplayName = "Free USIM change customer"
                    },
                    new OfferModel()
                    {
                        offerCommission = "20",
                        offerName = "338tk-1.5gb/day-30d_C20",
                        rechargeAmount = "27",
                        sno = "115",
                        offerID = "OFF00115",
                        offerDisplayName = "338tk-1.5gb/day-30d C-20tk"
                    },
                    new OfferModel()
                    {
                        offerCommission = "0",
                        offerName = "21tk-5gb-10hr_BI_C0",
                        rechargeAmount = "21",
                        sno = "116",
                        offerID = "OFF00116",
                        offerDisplayName = "21tk-5gb-10hr"
                    },
                    new OfferModel()
                    {
                        offerCommission = "0",
                        offerName = "41tk-20min-3gb-15hr_BI_C0",
                        rechargeAmount = "41",
                        sno = "117",
                        offerID = "OFF00117",
                        offerDisplayName = "41tk-20min-3gb-15hr"
                    }
                ];

            return offerList;
        }
    }
}