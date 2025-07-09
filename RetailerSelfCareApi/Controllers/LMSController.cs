///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	All LMS rleated api collection releated to Retailer App
///	Creation Date :	10-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Application.Services;
using Domain.Helpers;
using Domain.LMS;
using Domain.LMS.Request;
using Domain.LMS.Response;
using Domain.RequestModel;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Domain.Enums.EnumCollections;

namespace RetailerSelfCareApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class LMSController : ControllerBase
    {
        [HttpPost]
        [Route(nameof(GetLMSMemberProfile))]
        public async Task<IActionResult> GetLMSMemberProfile([FromBody] RetailerRequestV2 model)
        {
            CommonLMSRequest reqBody = new()
            {
                retailerCode = model.retailerCode,
                msisdn = "88" + model.iTopUpNumber,
                transactionID = LMSService.GetTransactionId(model.retailerCode)
            };

            string absoulateURI = string.Empty;

            HttpRequestModel httpReq = new()
            {
                requestUrl = LMSKyes.RequestBaseUrl + LmsApiEndPoints.getMemberProfile,
                requestBody = reqBody,
                requestMediaType = MimeTypes.VndBLApiJson,
                requestMethod = "GetLmsMemberProfile"
            };

            using (HttpService httpService = new())
            {
                var memberProfile = await httpService.CallExternalApi<MemberProfile>(httpReq);

                if (memberProfile is not null)
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = false,
                        data = memberProfile.Object,
                        message = SharedResource.GetLocal("Success", Message.Success)
                    });
                }
                else
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound)
                    });
                }
            }

        }

        [HttpPost]
        [Route(nameof(GetLMSTelcoOffers))]
        public async Task<IActionResult> GetLMSTelcoOffers([FromBody] RetailerRequestV2 model)
        {
            List<LMSPartner> lmsPartners = [];
            using (LMSService lmsService = new())
            {
                lmsPartners = await lmsService.GetLmsPartners(model);
            }

            LMSPartner telcoPartner = lmsPartners.Where(l => l.partnerCategory.Equals("telco", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            LMSTelcoRewardReq reqBody = new()
            {
                retailerCode = model.retailerCode,
                msisdn = "88" + model.iTopUpNumber,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                partnerID = telcoPartner.partnerID,
                partnerCategoryID = telcoPartner.partnerCategoryID
            };

            HttpRequestModel httpReq = new()
            {
                requestUrl = LMSKyes.RequestBaseUrl + LmsApiEndPoints.getRewardList,
                requestBody = reqBody,
                requestMediaType = MimeTypes.VndBLApiJson,
                requestMethod = "GetLmsTelcoOffers"
            };

            using (HttpService httpService = new())
            {
                var rewardListObj = await httpService.CallExternalApi<LMSOffers>(httpReq);

                IEnumerable<LMSOfferDetails> rewardList = rewardListObj.Object.rewardArray.Select(s => HelperMethod.ModelBinding<LMSOfferDetails>(s, true));

                if (rewardList.Any())
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = false,
                        data = rewardList,
                        message = SharedResource.GetLocal("Success", Message.Success)
                    });
                }
                else
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound),
                        data = new string[] { }
                    });
                }
            }

        }


        [HttpPost]
        [Route(nameof(GetLMSPartnerOfferCategories))]
        public async Task<IActionResult> GetLMSPartnerOfferCategories([FromBody] RetailerRequestV2 model)
        {
            LMSService lmsService = new();
            List<LMSPartner> lmsPartners = await lmsService.GetLmsPartners(model);

            var partnerCategories = lmsPartners.Select(s => new { s.partnerCategory, s.partnerCategoryID }).Where(w => w.partnerCategory.ToLower() != "telco").Distinct();

            return Ok(new ResponseMessage()
            {
                isError = false,
                data = partnerCategories,
                message = SharedResource.GetLocal("Success", Message.Success)
            });

        }


        [HttpPost]
        [Route(nameof(GetLMSRewardList))]
        public async Task<IActionResult> GetLMSRewardList([FromBody] LMSRewardListReq model)
        {
            LMSRewardReq reqBody = new()
            {
                retailerCode = model.retailerCode,
                msisdn = "88" + model.iTopUpNumber,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                partnerCategoryID = model.partnerCategoryID
            };

            HttpRequestModel httpReq = new()
            {
                requestUrl = LMSKyes.RequestBaseUrl + LmsApiEndPoints.getRewardList,
                requestBody = reqBody,
                requestMediaType = MimeTypes.VndBLApiJson,
                requestMethod = "GetRewardList"
            };

            using (HttpService httpService = new())
            {
                var rewardListObj = await httpService.CallExternalApi<LMSOffers>(httpReq);

                IEnumerable<LMSOfferDetails> rewardList = rewardListObj.Object.rewardArray.Select(s => HelperMethod.ModelBinding<LMSOfferDetails>(s));

                if (rewardList.Any())
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = false,
                        data = rewardList,
                        message = SharedResource.GetLocal("Success", Message.Success)
                    });
                }
                else
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound),
                        data = new string[] { }
                    });
                }
            }

        }


        [HttpPost]
        [Route(nameof(GetLMSPointHistory))]
        public async Task<IActionResult> GetLMSPointHistory([FromBody] LMSPointHistoryReq model)
        {
            LMSService lmsService;
            LMSPointHistory pointHistory = new();
            int currentDay = DateTime.Now.Day;

            if (currentDay <= 15)
            {
                DateTime reqMonth = DateTime.Now.AddMonths(-1);
                LMSPointHistReq reqBodyPrev = new()
                {
                    retailerCode = model.retailerCode,
                    msisdn = "88" + model.iTopUpNumber,
                    transactionID = LMSService.GetTransactionId(model.retailerCode),
                    month = reqMonth.Month.ToString(),
                    year = reqMonth.Year.ToString()
                };

                using(lmsService = new LMSService())
                {
                    pointHistory = await lmsService.PullLMSPointHistoryByMonth(reqBodyPrev);
                }
            }

            LMSPointHistReq reqBody = new()
            {
                retailerCode = model.retailerCode,
                msisdn = "88" + model.iTopUpNumber,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                month = DateTime.Now.Month.ToString(),
                year = DateTime.Now.Year.ToString()
            };

            using(lmsService = new LMSService())
            {
                LMSPointHistory pointHistoryNow = await lmsService.PullLMSPointHistoryByMonth(reqBody);

                pointHistory.earnHistory.AddRange(pointHistoryNow.earnHistory);
                pointHistory.redeemHistory.AddRange(pointHistoryNow.redeemHistory);
            }

            LMSPointHistoryVM pointHistoryVM = new();

            pointHistoryVM.points = pointHistory.earnHistory.Select(obj => HelperMethod.ModelBinding<LMSPointHist>(obj)).ToList();
            pointHistoryVM.points.AddRange(pointHistory.redeemHistory.Select(obj => HelperMethod.ModelBinding<LMSPointHist>(obj)).ToList());

            if (model.startDate == null || model.endDate == null)
            {
                pointHistoryVM.points = pointHistoryVM.points.Where(w => w.queryDate >= DateTime.Now.AddDays(-15)).OrderByDescending(o => o.queryDate).ToList();
            }
            else
            {
                pointHistoryVM.points = pointHistoryVM.points.Where(w => w.queryDate.Date >= model.startDate.Value.Date && w.queryDate.Date <= model.endDate.Value.Date).OrderByDescending(o => o.queryDate).ToList();
            }

            if (pointHistoryVM.points.Any())
            {
                return Ok(new ResponseMessage()
                {
                    isError = false,
                    data = pointHistoryVM.points,
                    message = SharedResource.GetLocal("Success", Message.Success)
                });
            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound),
                    data = new string[] { }
                });
            }
        }


        [HttpPost]
        [Route(nameof(RedeemLMSReward))]
        public async Task<IActionResult> RedeemLMSReward([FromBody] LMSRedeemRewardReq model)
        {

            LMSRedeemReward reqBody = new()
            {
                retailerCode = model.retailerCode,
                msisdn = "88" + model.iTopUpNumber,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                rewardID = model.rewardID
            };

            HttpRequestModel httpReq = new()
            {
                requestUrl = LMSKyes.RequestBaseUrl + LmsApiEndPoints.getRedeemReward,
                requestBody = reqBody,
                requestMediaType = MimeTypes.VndBLApiJson,
                requestMethod = "RedeemLMSReward"
            };

            HttpService httpService = new();
            var redeemRWDObj = await httpService.CallExternalApi<dynamic>(httpReq);
            string redeemRespString = JsonConvert.SerializeObject(redeemRWDObj.Object, Formatting.None);

            LMSRewardResp redeemResp = new();

            try
            {
                redeemResp = JsonConvert.DeserializeObject<LMSRewardResp>(redeemRespString)!;

                await Task.Factory.StartNew(async () =>
                {
                    LMSService lmsService = new();
                    await lmsService.SaveRedeemTransaction(reqBody, redeemResp);
                });
            }
            catch (Exception)
            {
                throw new Exception("LMS Response format not correct");
            }

            if (redeemResp.statusCode == "0")
            {
                return Ok(new ResponseMessage()
                {
                    isError = false,
                    data = redeemRWDObj.Object,
                    message = SharedResource.GetLocal("Success", Message.Success)
                });
            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("LMSRewardCouldNotRedeem", Message.Failed)
                });
            }

        }


        [HttpPost]
        [Route(nameof(LMSBannerPoints))]
        public async Task<IActionResult> LMSBannerPoints([FromBody] RetailerRequestV2 model)
        {
            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "LMSBannerPoints",
                retailerCode = model.retailerCode,
                appPage = LMSAppPages.Banner_Details_Page,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                msisdn = "88" + model.iTopUpNumber,
                points = LMSPoints.Banner_Check.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success)
            });
        }


        [HttpPost]
        [Route(nameof(GetLMSPointsTable))]
        public async Task<IActionResult> GetLMSPointsTable([FromBody] RetailerRequestV2 model)
        {
            Dictionary<string, object> properties = HelperMethod.GetStaticPropertyDict(typeof(LMSPoints));
            List<dynamic> pointTables = new();

            for (int i = 0; i < properties.Keys.Count(); i++)
            {
                var item = properties.ElementAt(i);
                dynamic coin = new
                {
                    CoinName = item.Key.Replace('_', ' '),
                    LMSCoin = item.Value
                };

                pointTables.Add(coin);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                data = pointTables,
                message = SharedResource.GetLocal("Success", Message.Success)
            });

        }


        [HttpPost]
        [Route(nameof(LmsTermsConditions))]
        public async Task<IActionResult> LmsTermsConditions([FromBody] RetailerRequestV2 reqModel)
        {
            int featureType = (int)LmsTermsFaq.TermsConditions;
            using (LMSService lmsService = new())
            {
                List<LmsTermsFaqs> listData = await lmsService.GetLmsTermsConditionsAndFaqs(featureType, reqModel.lan);

                return Ok(new ResponseMessage()
                {
                    isError = false,
                    data = listData,
                    message = SharedResource.GetLocal("Success", Message.Success)
                });
            }
        }


        [HttpPost]
        [Route(nameof(LmsFaqs))]
        public async Task<IActionResult> LmsFaqs([FromBody] RetailerRequestV2 reqModel)
        {
            int featureType = (int)LmsTermsFaq.Faq;
            LMSService lmsService = new();
            List<LmsTermsFaqs> listData = await lmsService.GetLmsTermsConditionsAndFaqs(featureType, reqModel.lan);

            return Ok(new ResponseMessage()
            {
                isError = false,
                data = listData,
                message = SharedResource.GetLocal("Success", Message.Success)
            });
        }

    }
}