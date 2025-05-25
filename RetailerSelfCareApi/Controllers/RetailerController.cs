///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	Retailer Controller
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Application.Services;
using Application.Utils;
using Domain.Helpers;
using Domain.LMS;
using Domain.LMS.Request;
using Domain.RedisModels;
using Domain.RequestModel;
using Domain.RequestModel.Survey;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Infrastracture.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text;
using static Domain.Enums.EnumCollections;

namespace RetailerSelfCareApi.Controllers
{
    [Route("api/v2")]
    [ApiController]
    public class RetailerController : ControllerBase
    {
        #region=======================|          Login         |======================

        [HttpPost]
        [Route("LoginSmartPos")]
        public async Task<IActionResult> LoginSmartPos([FromBody] LoginRequest loginRequest)
        {
            string URL = BiometricKeys.BiometricLoginUrl;
            LogInViewModel logInResponse = await RetailerService.LogInExternal(loginRequest, URL);

            return new OkObjectResult(new ResponseMessage()
            {
                isError = !logInResponse.ISAuthenticate,
                message = logInResponse.ISAuthenticate ? SharedResource.GetLocal("Success", Message.Success) : SharedResource.GetLocal("Failed", Message.Failed),
                data = logInResponse
            });
        }

        #endregion=======================|          Login         |======================

        #region=======================|      Retailer info     |======================

        [HttpPost]
        [Route("GetRetailer")]
        public async Task<IActionResult> GetRetailerByRCode([FromBody] RetailerRequest retailer)
        {
            string traceMsg = string.Empty;
            RetailerService retailerService = new();
            DataTable retailers = new();

            try
            {
                retailers = await retailerService.GetRetailerDetails(retailer);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRetailerDetails"));
            }

            string webBaseUrl = ExternalKeys.APIBaseUrl;
            RetailerV2Model retailerModel = new(retailers.Rows[0], webBaseUrl);

            string rating;
            try
            {
                retailerService = new(Connections.RetAppDbCS);
                rating = await retailerService.RetailerRating(retailer);
            }
            catch (Exception ex)
            {
                rating = "5.0";
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "RetailerRating", ex);
            }

            retailerModel.starRating = rating;

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(retailer, "GetRetailer", traceMsg);
            }

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = retailerModel
            });
        }

        [HttpPost]
        [Route("UpdateRetailer")]
        public async Task<IActionResult> UpdateRetailer([FromBody] RetailerDetailsRequest retailer)
        {
            RetailerService retailerService = new();
            long retailers = await retailerService.UpdateRetailer(retailer);
            bool isSuccess = retailers > 0;

            return new OkObjectResult(new ResponseMessage()
            {
                isError = !isSuccess,
                message = isSuccess ? SharedResource.GetLocal("Success", Message.Success) : SharedResource.GetLocal("Failed", Message.Failed),
                data = Array.Empty<string>()
            });
        }


        [HttpPost]
        [Route("CustomerFeedback")]
        public async Task<IActionResult> CustomerFeedback([FromBody] RetailerRequest retailer)
        {
            RetailerService RetailerService = new(Connections.RetAppDbCS);
            DataTable customerFeedback = await RetailerService.CustomerFeedback(retailer);
            List<CustomerFeedback> customerFeedbacks = customerFeedback.AsEnumerable().Select(cf => new CustomerFeedback(cf)).ToList();


            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = customerFeedbacks
            });
        }

        #endregion=======================|      Retailer info     |======================

        #region=======================|      Self-services      |======================

        [HttpPost]
        [Route("GetSTK")]
        public async Task<IActionResult> GetSTK([FromBody] RetailerRequest retailerRequest)
        {
            RetailerService ewHomeService = new(Connections.RetAppDbCS);
            DataTable stk = await ewHomeService.GetSTK();
            List<STKModel> sTKModels = stk.AsEnumerable().Select(row => new STKModel(row)).ToList();

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = sTKModels
            });
        }


        [HttpPost]
        [Route("FaqList")]
        public IActionResult FaqList([FromBody] RetailerRequest retailerRequest)
        {

            RetailerService ewHomeService;
            DataTable faq = new();

            ewHomeService = new RetailerService(Connections.RetAppDbCS);
            faq = ewHomeService.GetFAQ();

            List<FAQModel> fAQModels = faq.AsEnumerable().Select(row => HelperMethod.ModelBinding<FAQModel>(row)).ToList();

            ewHomeService = new RetailerService(Connections.RetAppDbCS);
            dynamic faqs = ewHomeService.GetFAQModel(fAQModels);


            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = faqs
            });

        }

        #endregion=======================|     Self-services     |======================

        #region==========================|     Best Practice    |=========================

        [HttpPost]
        [Route("RetailerBestPractice")]
        public async Task<IActionResult> RetailerBestPractice([FromBody] ComplainRequest retailer)
        {
            string traceMsg = string.Empty;

            RetailerService retailerService = new(Connections.RetAppDbCS);
            int userId = UserSession.userId;
            int bestPracticeId = 0;

            bestPracticeId = retailerService.RetailerBestPractice(retailer, userId);


            if (retailer.images != null && retailer.images.Count > 0)
            {
                retailerService = new(Connections.RetAppDbCS);

                await retailerService.DeleteTableRows(bestPracticeId, "RSLTBLBESTPRACTICEFILES", "BEST_PRACTICE_ID");


                for (int i = 0; i < retailer.images.Count; i++)
                {
                    retailerService = new(Connections.RetAppDbCS);
                    string imgStr = retailer.images[i];

                    FileExtensionModel attatchType = SaveFileHelper.GetFileExtension(imgStr);

                    string base64Header = "data:" + attatchType.MimeType + ";base64,";
                    _ = retailerService.SaveBestPracticeImage(bestPracticeId, base64Header, imgStr);
                }
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(retailer, "RetailerBestPractice", traceMsg);
            }

            return new OkObjectResult(new ResponseMessage()
            {
                isError = bestPracticeId > 0 ? false : true,
                message = bestPracticeId > 0 ? SharedResource.GetLocal("Success", Message.Success) : SharedResource.GetLocal("Failed", Message.Failed),
            });
        }


        [HttpPost]
        [Route("GetBPImagesByID")]
        public IActionResult GetBPImagesByID([FromBody] GetBPImagesRequest model)
        {

            RetailerService NewRetailerService = new(Connections.RetAppDbCS);
            DataTable bestpractice = new();


            bestpractice = NewRetailerService.BestPracticesImages(model);


            List<string> bestpractices = new();

            for (int i = 0; i < bestpractice.Rows.Count; i++)
            {
                bestpractices.Add(bestpractice.Rows[i]["BP_IMAGE"] as string);
            }
            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = bestpractices
            });
        }

        #endregion==========================|     Best Practice    |=========================

        #region =======================| Retailer App New CR 2023 |======================

        /// <summary>
        /// This API is to get Retailer Stock. This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="retailerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetRetailerStock")]
        public async Task<IActionResult> GetRetailerStock([FromBody] RetailerRequestV2 retailerRequest)
        {
            RetailerService retailerService;
            RSOEligibility rsoEligibility;
            RetailerSessionCheck retailer;
            string loginProvider = Request.HttpContext.Items["loginProviderId"] as string;

            try
            {
                StockService newStockService = new();
                retailer = await newStockService.CheckRetailerByCode(retailerRequest.retailerCode, loginProvider);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "CheckRetailerByCode");
                throw new Exception(errMsg);
            }

            if (string.IsNullOrEmpty(loginProvider) || !retailer.isSessionValid)
            {
                return Unauthorized(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("InvalidSession", Message.InvalidSession)
                });
            }

            retailerService = new(Connections.RetAppDbCS);
            rsoEligibility = retailerService.GetITopUpStockEligibilityCheck(retailerRequest, out string msg);

            if (!string.IsNullOrWhiteSpace(msg))
                throw new Exception(msg);

            DataRow emptyDr = new DataTable().NewRow();
            StockSummaryModel stockSummary = new(emptyDr, "iTopUp");

            if (rsoEligibility.IsEligible == 1)
            {
                rsoEligibility.iTopUpNumber = retailerRequest.iTopUpNumber;
                rsoEligibility.retailerCode = retailerRequest.retailerCode;

                string traceMsg = GetEVBalanceNdSaveToDb(rsoEligibility, ref stockSummary);

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(retailerRequest, "GetRetailerStock || GetEVBalanceNdSaveToDb", traceMsg);
                }
            }
            else if (rsoEligibility.IsEligible == 0)
            {
                stockSummary.amount = rsoEligibility.CurrentBalance.ToString();
                stockSummary.updateTime = rsoEligibility.UpdateTime;
            }
            else
            {
                throw new Exception("Eligibility unchecked");
            }
            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = stockSummary
            });
        }


        /// <summary>
        /// API to serve validate IRIS and Deno offer. This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="offerRequest"></param>
        /// <returns>Returns Offer Result</returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("ValidateIRISDenoOffer")]
        public async Task<IActionResult> ValidateIRISDenoOffer([FromBody] IrisOfferRequestNew offerRequest)
        {
            string traceMsg = string.Empty;
            StockService stockService;
            RetailerSessionCheck retailer = new();
            string loginProvider = Request.HttpContext.Items["loginProviderId"] as string;

            try
            {
                stockService = new StockService();
                retailer = await stockService.CheckRetailerByCode(offerRequest.retailerCode, loginProvider);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "CheckRetailerByCode");
                throw new Exception(errMsg);
            }

            if (string.IsNullOrEmpty(loginProvider) || !retailer.isSessionValid)
            {
                return Unauthorized(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("InvalidSession", Message.InvalidSession),
                });
            }

            RetailerRequestV2 retailerRequest = offerRequest.Adapt<RetailerRequestV2>();
            Dictionary<string, string> result = [];

            //Pull Stock Details
            RetailerService retailerService = new(Connections.RetAppDbCS);
            RSOEligibility rsoEligibility = new();
            rsoEligibility = retailerService.GetITopUpStockEligibilityCheck(retailerRequest, out string msg);

            traceMsg = HelperMethod.BuildTraceMessage(traceMsg, msg, null);

            DataRow emptyDr = new DataTable().NewRow();
            StockSummaryModel stockSummary = new(emptyDr, "iTopUp");

            if (rsoEligibility.IsEligible == 1)
            {
                retailerService = new RetailerService(Connections.RetAppDbCS);
                rsoEligibility.iTopUpNumber = retailerRequest.iTopUpNumber;
                rsoEligibility.retailerCode = retailerRequest.retailerCode;

                traceMsg = GetEVBalanceNdSaveToDb(rsoEligibility, ref stockSummary);

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(retailerRequest, "ValidateIRISDenoOffer || GetEVBalanceNdSaveToDb", traceMsg);
                }
            }
            else if (rsoEligibility.IsEligible == 0)
            {
                stockSummary.amount = rsoEligibility.CurrentBalance.ToString();
                stockSummary.updateTime = rsoEligibility.UpdateTime;
            }
            else
            {
                throw new Exception("Eligibility unchecked");
            }

            result.Add("availableBalance", stockSummary.amount);
            if (offerRequest.isAmarOffer)
            {
                IrisOfferRequestNew irisOfferReq = offerRequest.Adapt<IrisOfferRequestNew>();
                OfferResponseModelNew irisOffersModel = new();

                try
                {
                    RechargeService rechargeService = new(Connections.RetAppDbCS);
                    irisOffersModel = await rechargeService.IRISOfferRequest(irisOfferReq);
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "ValidateIRISDenoOffer || IRISOfferRequestNew");
                    throw new Exception(errMsg);
                }

                var offers = irisOffersModel.OfferList.Where(o => o.amount.ToString() == offerRequest.amount).ToList();

                if (offers.Count > 1)
                {
                    string errMsg = SharedResource.GetLocal("MultipleAmarOfferAvailable", Message.MultipleAmarOfferAvailable);
                    throw new Exception(errMsg);
                }

                if (offers.Count > 0)
                {
                    OfferModelNew model = offers[0];
                    result.Add("ussdCode", model.ussdCode);
                    result.Add("tranId", model.tranId);
                    result.Add("offerId", model.offerId);
                    result.Add("amount", model.amount.ToString());

                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = false,
                        message = SharedResource.GetLocal("Success", Message.Success),
                        data = result
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("InvalidAmarOffer", Message.InvalidAmarOffer)
                    });
                }
            }
            else
            {
                result.Add("ussdCode", string.Empty);
                result.Add("tranId", string.Empty);
                result.Add("offerId", string.Empty);
                result.Add("amount", string.Empty);

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = result
                });
            }
        }


        /// <summary>
        /// This API is to get RSO Number to Chat with RSO. This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="retailerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetRSONumber")]
        public async Task<IActionResult> GetRSONumber([FromBody] RetailerRequestV2 retailerRequest)
        {
            RetailerService retailerService = new();
            string rsoNumber = await retailerService.GetRSONumber(retailerRequest.retailerCode);
            var resp = new { rsoNumber };

            if (!string.IsNullOrEmpty(rsoNumber))
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    data = resp,
                    message = SharedResource.GetLocal("Success", Message.Success)
                });
            }
            else
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = true,
                    data = resp,
                    message = Message.NoRsoFound
                });
            }
        }


        /// <summary>
        /// Get Lifting(EV, SIM, SC), OTF(EV, IRIS), Commission data from Kafka, DMS, Salescom.
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="C2CTransactions"></param>
        /// <returns>Returing transaction results with filtering request</returns>
        [HttpPost]
        [Route("C2CTransactions")]
        public async Task<IActionResult> C2CTransactions([FromBody] TransactionsRequest tranRequest)
        {
            string traceMsg = string.Empty;
            if (!string.IsNullOrEmpty(tranRequest.sortByAmt))
            {
                tranRequest.sortByAmt = tranRequest.sortByAmt.ToUpper();
            }

            if (!string.IsNullOrEmpty(tranRequest.sortByInOut))
            {
                tranRequest.sortByInOut = tranRequest.sortByInOut.ToUpper();
                tranRequest.sortByAmt = "ASC";
            }

            if (!string.IsNullOrEmpty(tranRequest.sortByDate))
            {
                tranRequest.sortByDate = tranRequest.sortByDate.ToUpper();
            }
            else if (string.IsNullOrEmpty(tranRequest.sortByAmt) && string.IsNullOrEmpty(tranRequest.sortByInOut))
            {
                tranRequest.sortByDate = "DESC";
            }

            #region Get data from Oracle
            DataTable dataTable = new();

            RetailerService retailerService = new(Connections.RetAppDbCS);

            try
            {
                dataTable = retailerService.GetC2CTransactions(tranRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "C2CTransactions"));
            }

            #endregion

            #region Get OTF Data from MySql

            DataTable otfTrans = new();
            try
            {
                retailerService = new();
                otfTrans = await retailerService.GetC2SOtfTransactions(tranRequest);
                dataTable.Merge(otfTrans, true, MissingSchemaAction.Ignore);
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "GetC2SPostpaidTransactions", ex);
            }

            #endregion

            #region Get data from API

            string retailerNumber = tranRequest.iTopUpNumber.Substring(1);
            retailerService = new();
            string rsoNumber = await retailerService.GetRSONumber(tranRequest.retailerCode);

            C2CRechargeHistReq xmlRequest = new()
            {
                Url = ExternalKeys.EvURL,
                Type = "EXLST3TRFREQ",
                Date = "",
                Extnwcode = "BD",
                Msisdn = "",
                Pin = "",
                Loginid = "",
                Password = "",
                Extcode = rsoNumber,
                Extrefnum = "122345",
                Language1 = "0",
                NumberOfLastXTxn = "7",
                Receiver_Msisdn = retailerNumber
            };

            retailerService = new();

            var resp = await retailerService.GetC2CRechrgHist(xmlRequest, tranRequest);

            List<C2CRechrgHistResp> rechargeHistToday = resp.Item1.Where(r => !string.IsNullOrWhiteSpace(r.date) && DateTime.ParseExact(r.date, "dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture).Date == DateTime.Today).ToList();

            traceMsg = HelperMethod.BuildTraceMessage(traceMsg, resp.Item2, null);

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(tranRequest, "C2CTransactions", traceMsg);
            }

            #endregion Get data from API

            List<TransactionsModel> allTransactions = dataTable.AsEnumerable().Select(row => HelperMethod.ModelBinding<TransactionsModel>(row)).ToList();

            if (rechargeHistToday.Any())
            {
                List<TransactionsModel> c2cRechargeHist = rechargeHistToday.Select(s => HelperMethod.ModelBinding<TransactionsModel>(s)).ToList();

                allTransactions.AddRange(c2cRechargeHist);
            }

            List<TransactionsModel> finalTransactionDataList = TransactionsModel.OrderingTransactionData(allTransactions, tranRequest);

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = finalTransactionDataList
            });
        }

        #endregion

        #region Dynamic_Advertisement

        /// <summary>
        /// API method for getting Advertisement data and applicable from APK v6.0.0
        /// </summary>
        /// <param name="retailerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AppAdvertisement")]
        public async Task<IActionResult> AppAdvertisement([FromBody] RetailerRequestV2 retailerRequest)
        {
            string traceMsg = string.Empty;
            try
            {
                RedisCache redis;
                List<AdvertisementDetailsRedis> redisAppAdvertise = new();

                try
                {
                    redis = new RedisCache();
                    var advertisementDetailsStr = await redis.GetCacheAsync(RedisCollectionNames.AdvertisementDetails);
                    redisAppAdvertise = JsonConvert.DeserializeObject<List<AdvertisementDetailsRedis>>(advertisementDetailsStr)!;
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "AdvertModelParsing", ex);
                }

                List<long> advertisementIds = new();

                try
                {
                    redis = new RedisCache();
                    string hasAdvIdsStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerAdvertisementIds, retailerRequest.retailerCode);
                    string hasAdvIds = JsonConvert.DeserializeObject<dynamic>(hasAdvIdsStr)!;
                    if (!string.IsNullOrEmpty(hasAdvIds))
                    {
                        advertisementIds = hasAdvIds.Split(',').Select(s => Convert.ToInt64(s)).ToList();
                    }
                }
                catch (Exception ex)
                {
                    string _msg = "RetailerAdvertisementIds";
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                }

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(retailerRequest, "AppAdvertisement", traceMsg);
                }

                List<AdvertisementDetailsRedis> appAdvertisement = redisAppAdvertise.Where(w => advertisementIds.Any(a => a == w.advertisementId)).ToList();

                if (appAdvertisement.Count > 0)
                {
                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = false,
                        message = SharedResource.GetLocal("Success", Message.Success),
                        data = appAdvertisement[0]
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = false,
                        message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "AppAdvertisement"));
            }
        }

        #endregion

        #region Raise_Complaint

        /// <summary>
        /// This api is applicable from APK v6.0.0
        /// </summary>
        /// <param name="RetailerRequest"></param>
        /// <returns>Get Complain Type List</returns>
        [HttpPost]
        [Route("ComplaintTypeList")]
        public async Task<IActionResult> ComplaintTypeList([FromBody] RetailerRequestV2 reqModel)
        {
            try
            {
                List<Categories> superCategories = new();
                RetailerService retailerService;
                DataTable dt = new();

                try
                {
                    retailerService = new RetailerService();
                    dt = await retailerService.GetComplaintTypeList();
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "ComplaintTypeList");
                    throw new Exception(errMsg);
                }

                List<ComplaintTypeResponse> complaintTypes = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<ComplaintTypeResponse>(row, string.Empty, reqModel.lan)).ToList();

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = complaintTypes
                });
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ComplaintTypeListV2"));
            }
        }


        /// <summary>
        /// This api is applicable from APK v6.0.0
        /// </summary>
        /// <param name="ComplainTitleRequest"></param>
        /// <returns>Get Complain Type List</returns>
        [HttpPost]
        [Route("ComplaintTitleList")]
        public async Task<IActionResult> ComplaintTitleList([FromBody] ComplaintTitleRequest reqModel)
        {
            try
            {
                RetailerService retailerService = new();
                DataTable dt = new();

                try
                {
                    dt = await retailerService.GetComplaintTitleList(reqModel);
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "GetComplaintTitleList");
                    throw new Exception(errMsg);
                }

                List<ComplaintTitleResponse> complaintTypes = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<ComplaintTitleResponse>(row, string.Empty, reqModel.lan)).ToList();

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = complaintTypes
                });
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ComplaintTitleList"));
            }
        }


        /// <summary>
        /// This api is applicable from APK v6.0.0
        /// </summary>
        /// <param name="HistoryPageRequestModelV2"></param>
        /// <returns>Get Raise Complain History</returns>
        [HttpPost]
        [Route("RaiseComplaintHistory")]
        public async Task<IActionResult> RaiseComplaintHistory([FromBody] HistoryPageRequestModelV2 model)
        {
            string traceMsg = string.Empty;

            RetailerService retailerService = new();
            DataTable soUpdateStatusDT = new();

            try
            {
                soUpdateStatusDT = await retailerService.GetSOUpdateStatus(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSOUpdateStatus"));
            }

            List<RCSOTickets> soUpdateStatusList = soUpdateStatusDT.AsEnumerable().Select(us => HelperMethod.ModelBinding<RCSOTickets>(us)).ToList();
            StringBuilder soErr = new();

            int sofStsUpdInerval = ExternalKeys.SuperOfficeStatusUpdateInterval;
            DateTime eligibleTime = DateTime.Now.AddMinutes(-sofStsUpdInerval);

            for (int i = 0; i < soUpdateStatusList.Count; i++)
            {
                RCSOTickets item = soUpdateStatusList[i];
                try
                {
                    if (item.SO_Update_Time < eligibleTime && item.IsTicketOpen)
                    {
                        SuperOfficeService superOffice = new();
                        SOTicket ticket = await superOffice.GetSOTicketByID(model.retailerCode, item.SO_ID);

                        if (ticket.tickets[0].ticketId > 0)
                        {
                            retailerService = new();
                            long updateStatus = await retailerService.UpdateSOTicketStatus(ticket.tickets[0]);
                        }
                    }
                }
                catch (Exception)
                {
                    soErr.Append($"{item.SO_ID},");
                    continue;
                }
            }

            traceMsg = soErr.Length > 0 ? soErr.Prepend("Unable get update status for Ids: ").ToString() : string.Empty;


            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(model, "RaiseComplaintHistory", traceMsg);
            }

            DataTable dt = new();

            try
            {
                retailerService = new();
                dt = await retailerService.GetRaiseComplaintHistory(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRaiseComplaintHistory"));
            }

            List<RaiseComplaintHistoryModel> complaintHistory = dt.AsEnumerable().Select(cf => HelperMethod.ModelBinding<RaiseComplaintHistoryModel>(cf, string.Empty, model.lan)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = complaintHistory.Count > 0 ? complaintHistory : new List<RaiseComplaintHistoryModel>()
            });
        }


        #endregion

        #region Rso Rating

        /// <summary>
        /// This api is applicable from APK v6.0.0
        /// </summary>
        /// <param name="retailerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetRSOInformation")]
        public async Task<IActionResult> GetRSOInformation([FromBody] RetailerRequestV2 retailerRequest)
        {
            try
            {
                DataTable dt = new();
                try
                {
                    RetailerService retailerService = new();
                    dt = await retailerService.GetRSOInformation(retailerRequest);
                    RSOProfile profile = dt.Rows.Count > 0 ? new RSOProfile(dt.Rows[0]) : null;

                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = profile == null,
                        message = profile == null ? SharedResource.GetLocal("NoDataFound", Message.NoDataFound) : SharedResource.GetLocal("Success", Message.Success),
                        data = profile
                    });
                }
                catch (Exception ex)
                {
                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong),
                        ErrorDetails = ex.ToJsonString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSOInformation"));
            }
        }


        /// <summary>
        /// This api is applicable from APK v6.0.0
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SubmitRSORating")]
        public async Task<IActionResult> SubmitRSORating([FromBody] RSORatingReqModel reqModel)
        {
            try
            {
                RetailerService retailerService;
                int userId = UserSession.userId;
                long insertId = 0;

                try
                {
                    retailerService = new RetailerService();
                    reqModel.status = "Pending";
                    insertId = await retailerService.SaveRSORating(reqModel, userId);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitRSORating"));
                }

                if (insertId == 0)
                {
                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong)
                    });
                }

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success)
                });
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitRSORating"));
            }
        }


        /// <summary>
        /// This api is applicable from APK v6.0.0
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RSORatingHistory")]
        public async Task<IActionResult> RSORatingHistory([FromBody] HistoryPageRequestModelV2 model)
        {
            try
            {
                DataTable dt = new();

                try
                {
                    RetailerService retailerService = new();
                    dt = await retailerService.GetRSORatingHistory(model);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSORatingHistory"));
                }

                List<RSORatingHistoryModel> ratingHistory = dt.AsEnumerable().Select(cf => HelperMethod.ModelBinding<RSORatingHistoryModel>(cf, string.Empty)).ToList();

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = ratingHistory
                });
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "RSORatingHistory"));
            }
        }

        #endregion

        #region Notifications

        /// <summary>
        /// API to serve all notification to user with flash/popup and survey.
        /// This api is applicable from APK v6.0.0
        /// </summary>
        /// <param name="notificationRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Notifications")]
        public async Task<IActionResult> Notifications([FromBody] RetailerRequestV2 notificationRequest)
        {
            try
            {
                RetailerService ewHomeService = new();
                DataTable dataTable = new();

                try
                {
                    dataTable = await ewHomeService.GetNotifications(notificationRequest);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "GetNotifications"));
                }

                string fileBaseUrl = ExternalKeys.ImageVirtualDirPath;
                List<NewNotificationsModelV2> notifications = dataTable.AsEnumerable().Select(row => HelperMethod.ModelBinding<NewNotificationsModelV2>(row, "", fileBaseUrl)).ToList();

                if (notifications.Count > 0)
                {
                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = false,
                        message = SharedResource.GetLocal("Success", Message.Success),
                        data = notifications
                    });
                }
                else
                {
                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = false,
                        message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound),
                        data = new string[] { }
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "Notifications"));
            }
        }


        /// <summary>
        /// This api is applicable from APK v6.0.0
        /// API to serve all Flash-PopUp to user.
        /// </summary>
        /// <param name="FlashPopUpRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FlashPopUp")]
        public async Task<IActionResult> FlashPopUp([FromBody] RetailerRequestV2 flashPopUpRequest)
        {
            string traceMsg = string.Empty;
            try
            {
                bool isEnable = FeatureStatus.IsNewFlashPopUpEnable;

                if (isEnable)
                {
                    RedisCache redis;
                    List<NotificationDetailsRedis> flashPopUpsRedis = [];

                    try
                    {
                        redis = new RedisCache();
                        string flashPopUpRedis = await redis.GetCacheAsync(RedisCollectionNames.FlashPopUpDetails);
                        flashPopUpsRedis = JsonConvert.DeserializeObject<List<NotificationDetailsRedis>>(flashPopUpRedis)!;
                    }
                    catch (Exception ex)
                    {
                        traceMsg = HelperMethod.ExMsgSubString(ex, "Get FlashPopUp data from Redis and Deserialize", 400);
                    }

                    List<string> flashPopUpIds = new();

                    try
                    {
                        redis = new RedisCache();
                        string hasFlashPopUpsIdsStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerFlashPopUpIds, flashPopUpRequest.retailerCode);
                        string hasFlashPopUpIds = JsonConvert.DeserializeObject<dynamic>(hasFlashPopUpsIdsStr)!;
                        if (!string.IsNullOrWhiteSpace(hasFlashPopUpIds))
                        {
                            flashPopUpIds = hasFlashPopUpIds.Split(',').ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        string _msg = "RetailerFlashPopUpIds";
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                    }

                    if (!string.IsNullOrWhiteSpace(traceMsg))
                    {
                        LoggerService _logger = new();
                        _logger.WriteTraceMessageInText(flashPopUpRequest, "FlashPopUp", traceMsg);
                    }

                    List<NotificationDetailsRedis> appFlashPopUps = flashPopUpsRedis.Where(w => flashPopUpIds.Any(f => f == w.id.ToString())).ToList();

                    var _data = new
                    {
                        PopUpData = appFlashPopUps,
                        LastSyncDateTime = DateTime.Now.ToEnUSDateString("dd MMM yyyy HH:mm")
                    };

                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = false,
                        message = SharedResource.GetLocal("Success", Message.Success),
                        data = _data
                    });
                }
                else
                {
                    var _data = new
                    {
                        PopUpData = new string[] { },
                        LastSyncDateTime = DateTime.Now.ToEnUSDateString("dd MMM yyyy HH:mm")
                    };

                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = false,
                        message = SharedResource.GetLocal("Success", Message.Success),
                        data = _data
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "FlashPopUp"));
            }
        }

        #endregion

        #region Deno Report

        /// <summary>
        /// API method for getting retailers top three recharge amount from D-2 to current.
        /// Applicable from APK v6.0.0
        /// </summary>
        /// <param name="retailerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RetailersTopThreeDeno")]
        public IActionResult GetRetailersTopThreeDeno([FromBody] RetailerRequestV2 retailerRequest)
        {
            try
            {
                RetailerService retailerService = new(Connections.RetAppDbCS);
                string[] denoList = retailerService.GetRetailersTopThreeDeno(retailerRequest);

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = denoList
                });
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong),
                    ErrorDetails = ex.ToString()
                });
            }
        }


        /// <summary>
        /// API method for getting retailers top three recharge amount from last 3days.
        /// Applicable from APK v6.0.0
        /// </summary>
        /// <param name="retailerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DenoReportDetails")]
        public IActionResult DenoReportDetails([FromBody] SearchRequestV2 searchRequest)
        {
            try
            {
                RetailerService retailerService = new(Connections.RetAppDbCS);
                DataTable dataTable = retailerService.RetailerDenoReport(searchRequest);

                DenoDetailsModel denoDetails = new();
                if (dataTable.Rows.Count > 0)
                {
                    denoDetails = new DenoDetailsModel(dataTable.Rows[0]);
                }

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = denoDetails
                });
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong),
                    ErrorDetails = ex.ToString()
                });
            }
        }

        #endregion

        [HttpPost]
        [Route("InstallationProcess")]
        public async Task<IActionResult> InstallationProcess([FromBody] BiometricApkUrlRequest apkLinkRequest)
        {
            try
            {
                string virtualDirPath = ExternalKeys.ImageVirtualDirPath;
                string tCapUrl = Path.Combine(virtualDirPath, "Apk/TCap.apk");

                string biometricApkRequestUrl = BiometricKeys.BiometricAppUrl;
                string biometriccurrentApkVersion = BiometricKeys.BiometricCurrentApkVersion.ToString();

                BimetricExtranalRequest reqBody = new()
                {
                    username = apkLinkRequest.iTopUpNumber,
                    appVersion = biometriccurrentApkVersion
                };

                HttpRequestModel httpModel = new()
                {
                    requestBody = reqBody,
                    requestMediaType = MimeTypes.Json,
                    requestUrl = biometricApkRequestUrl,
                    requestMethod = "InstallationProcess"
                };

                HttpService httpService = new();
                BiometricAppInfo resp = await httpService.GetBiometricAppLatestUrl<dynamic>(httpModel);

                var data = new
                {
                    tCapUrl,
                    biometricUrl = resp.app_update_info.update_url
                };

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong),
                    ErrorDetails = ex.ToString()
                });
            }
        }


        [HttpPost]
        [Route(nameof(SaveContact))]
        public async Task<IActionResult> SaveContact([FromBody] ContactSaveRequest contactModel)
        {
            RetailerService rechargeService = new();
            long res = await rechargeService.SaveContact(contactModel);

            if (res > 0)
            {
                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("SaveSuccess", Message.SaveSuccess)
                });
            }
            else if (res < 0)
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("ContactSaveFailed", Message.ContactSaveFailed)
                });
            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("ContactExist", Message.ContactExist)
                });
            }
        }


        [HttpPost]
        [Route(nameof(VORInsert))]
        public async Task<IActionResult> VORInsert([FromBody] VorRequest vorRequest)
        {
            string traceMsg = string.Empty;

            RetailerService retailerService = new(Connections.RetAppDbCS);

            if (string.IsNullOrEmpty(vorRequest.feedbackTypeId))
            {
                vorRequest.feedbackTypeId = "0";
            }

            if (string.IsNullOrEmpty(vorRequest.feedbackOnCompanyId))
            {
                vorRequest.feedbackOnCompanyId = "0";
            }

            VORModel model = new()
            {
                retailerCode = vorRequest.retailerCode,
                title = vorRequest.title,
                description = vorRequest.description,
                categoryId = Convert.ToInt32(vorRequest.feedbackTypeId),
                operatorId = Convert.ToInt32(vorRequest.feedbackOnCompanyId),
                userId = UserSession.userId,
                imageList = vorRequest.imageList
            };

            int vorInsertResult = 0;

            try
            {
                vorInsertResult = await retailerService.VORByRetailer(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "VORByRetailer"));
            }

            if (vorInsertResult > 0)
            {
                model.id = vorInsertResult;

                if (model.imageList != null && model.imageList.Count > 0)
                {
                    retailerService = new(Connections.RetAppDbCS);
                    try
                    {
                        await retailerService.DeleteTableRows(model.id, "RSLTBLVORFILES", "VOR_ID");
                    }
                    catch (Exception ex)
                    {
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "DeleteTableRows || ID: " + model.id.ToString(), ex);
                    }

                    for (int i = 0; i < model.imageList.Count; i++)
                    {
                        string imgStr = model.imageList[i];
                        FileExtensionModel attatchType = SaveFileHelper.GetFileExtension(imgStr);

                        string base64Header = "data:" + attatchType.MimeType + ";base64,";
                        int res = 0;

                        try
                        {
                            retailerService = new(Connections.RetAppDbCS);
                            res = await retailerService.SaveVorImage(model.id, base64Header, imgStr);
                        }
                        catch (Exception ex)
                        {
                            traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "SaveVorImage", ex);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(vorRequest, "VORInsert", traceMsg);
                }

                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("YourVoiceSavedSuccessfully", Message.YourVoiceSavedSuccessfully)
                });
            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("VoiceNotSaved", Message.VoiceNotSaved)
                });
            }

        }


        [HttpPost]
        [Route(nameof(GetRSOMemo))]
        public async Task<IActionResult> GetRSOMemo([FromBody] RsoMemoRequest model)
        {

            RetailerService retailerService = new(Connections.DMSCS);
            int userId = UserSession.userId;
            DataTable memoDT = new();

            try
            {
                memoDT = await retailerService.GetRSOMemo(model, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSOMemo"));
            }

            List<MemoDate> memoDate = memoDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<MemoDate>(row)).ToList();

            var distinctDateList = memoDate.Select(s => new { s.memoDate }).Distinct().ToList();

            List<RSOMemoVM> memoList = memoDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<RSOMemoVM>(row)).ToList();

            List<RSOMemoModel> rsoMemo = new();

            try
            {
                rsoMemo = distinctDateList.AsEnumerable().Select(item => new RSOMemoModel(item.memoDate, memoList)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "RSOMemoModel_Binding"));
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = rsoMemo
            });

        }


        [HttpPost]
        [Route(nameof(GetRSOFeedback))]
        public async Task<IActionResult> GetRSOFeedback([FromBody] RsoMemoRequest model)
        {
            RetailerService NewRetailerService = new(Connections.DMSCS);

            int userId = UserSession.userId;

            DataTable feedbackDT = new();

            try
            {
                feedbackDT = await NewRetailerService.GetRSOFeedback(model, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSOFeedback"));
            }

            List<RSOFeedbackModel> feedbackList = feedbackDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<RSOFeedbackModel>(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = feedbackList
            });
        }


        [HttpPost]
        [Route(nameof(GetFeedbackCategories))]
        public async Task<IActionResult> GetFeedbackCategories([FromBody] RetailerRequest model)
        {
            RetailerService NewRetailerService = new(Connections.RetAppDbCS);
            int userId = UserSession.userId;
            DataTable operatorListDT = new();

            try
            {
                operatorListDT = await NewRetailerService.GetFeedbackCategoryList(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetFeedbackCategoryList"));
            }

            List<FeedbackCategoryModel> list = operatorListDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<FeedbackCategoryModel>(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = list
            });

        }


        [HttpPost]
        [Route(nameof(GetOperatorList))]
        public async Task<IActionResult> GetOperatorList([FromBody] RetailerRequest model)
        {
            RetailerService retailerService = new(Connections.RetAppDbCS);
            int userId = UserSession.userId;
            DataTable operatorListDT = new();

            try
            {
                operatorListDT = await retailerService.GetOperatorList(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetOperatorList"));
            }

            List<OperatorsModel> operatorList = operatorListDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<OperatorsModel>(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = operatorList
            });
        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="ProductRatingRequest"></param>
        /// <returns>Returns List Of Product Review and Rating List</returns>
        [HttpPost]
        [Route(nameof(ProductRatingList))]
        public async Task<IActionResult> ProductRatingList([FromBody] ProductRatingRequest model)
        {
            model.userId = UserSession.userId;

            RetailerService NewRetailerService = new(Connections.RetAppDbCS);
            DataTable dt = new();

            try
            {
                dt = await NewRetailerService.GetProductRatingList(model);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "GetProductRatingList");
                throw new Exception(errMsg);
            }

            List<ProductRatingResponse> rechargePackageList = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<ProductRatingResponse>(row, string.Empty, model.lan)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = rechargePackageList
            });

        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="IrisOfferRequestV3"></param>
        /// <returns>Returns List Of IRIS Product Review and Rating List</returns>
        [HttpPost]
        [Route(nameof(IRISProductRatingList))]
        public async Task<IActionResult> IRISProductRatingList([FromBody] IrisOfferRequestNew model)
        {
            int userId = UserSession.userId;
            string userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
            model.userAgent = userAgent;

            RechargeService rechargeService = new();
            OfferResponseModelNew irisOffers = await rechargeService.IRISOfferRequest(model);

            RetailerService retailerService = new(Connections.RetAppDbCS);
            DataTable dt = await retailerService.GetAllIRISProductRating(model, userId);

            List<ProductRatingVM> allRating = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<ProductRatingVM>(row, string.Empty, model.lan)).ToList();

            List<IRISRatingResponse> iRISProductList = [];

            foreach (OfferModelNew iris in irisOffers.OfferList)
            {
                IRISRatingResponse iRISProduct = new()
                {
                    title = iris.description,
                    dataPack = iris.dataPack,
                    talkTime = iris.talkTime,
                    sms = iris.sms,
                    toffee = iris.toffee,
                    validity = iris.validity,
                    commission = iris.commission,
                    amount = iris.amount
                };

                ProductRatingVM ratings = allRating.FirstOrDefault(ar => ar.productName == iris.amount.ToString());

                if (ratings != null && ratings.isRated)
                {
                    iRISProduct.rating = Convert.ToInt32(ratings.rating);
                    iRISProduct.isRated = true;
                    iRISProduct.ratingDate = ratings.ratingDate;
                }

                iRISProductList.Add(iRISProduct);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = iRISProductList
            });
        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="RetailerRequest"></param> 
        /// <returns>Get Product Type List</returns>
        [HttpPost]
        [Route(nameof(ProductTypeList))]
        public async Task<IActionResult> ProductTypeList([FromBody] RetailerRequest model)
        {
            RetailerService retailerService = new(Connections.RetAppDbCS);
            DataTable dt = new();
            ProductTypeModel type = new();
            List<ProductTypeModel> types = [];

            ProductTypeModel allProduct = new() { ProductTypeName = "All", ProductTypeNameBN = "সব" };
            types.Add(allProduct);

            ProductTypeModel amarOffer = new() { ProductTypeName = "Amar Offer", ProductTypeNameBN = "আমার অফার" };
            types.Add(amarOffer);

            try
            {
                dt = await retailerService.GetProductTypeList();
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "GetProductTypeList");
                throw new Exception(errMsg);
            }

            foreach (DataRow row in dt.Rows)
            {
                ProductTypeModel productTypes = new()
                {
                    ProductTypeName = row["PR_TYPE"] as string,
                    ProductTypeNameBN = row["PR_TYPE_BN"] as string
                };
                types.Add(productTypes);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = types
            });

        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="PRHistoryRequest"></param>
        /// <returns>Get Rating History</returns>
        [HttpPost]
        [Route(nameof(ProductRatingHistory))]
        public async Task<IActionResult> ProductRatingHistory([FromBody] HistoryPageRequestModel model)
        {
            RetailerService retailerService = new(Connections.RetAppDbCS);
            DataTable dt = new();

            try
            {
                dt = await retailerService.GetProductRatingHistory(model);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "GetProductRatingHistory");
                throw new Exception(errMsg);
            }

            List<PRHistoryVM> ratingHistory = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<PRHistoryVM>(row, string.Empty, model.lan)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = ratingHistory
            });

        }


        [HttpPost]
        [Route(nameof(GetBTSInfo))]
        public async Task<IActionResult> GetBTSInfo([FromBody] BTSInfoRequest model)
        {
            int cid = 0, lac = 0;
            DataTable dt = new();

            cid = Convert.ToInt32(model.id.Split('~')[0]);
            lac = Convert.ToInt32(model.id.Split('~')[1]);

            try
            {
                RetailerService reService = new(Connections.RetAppDbCS);
                dt = await reService.GetBTSLocationDetails(lac, cid);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetBTSLocationDetails"));
            }

            BTSInfoModel tempBTSInfo = HelperMethod.ModelBinding<BTSInfoModel>(dt.Rows[0]);

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = tempBTSInfo
            });

        }


        [HttpPost]
        [Route("POSMQRUpdate")]
        public async Task<IActionResult> SubmitPOSMQRUpdate([FromBody] POSMQRRequest reqModel)
        {
            Response<ExternalApiResponse> resp;

            try
            {
                POSMQModel model = new()
                {
                    retailerId = Convert.ToInt32(reqModel.retailerCode.Substring(1)),
                    retailerCode = reqModel.retailerCode,
                    productCode = reqModel.productCode,
                    issue_date = reqModel.issueDate.Value,
                    key = ExternalKeys.POSMQKey,
                    deviceid = reqModel.deviceId
                };

                HttpRequestModel httpModel = new()
                {
                    requestUrl = ExternalKeys.POSMQUrl,
                    requestBody = model,
                    requestMediaType = MimeTypes.Json,
                    requestMethod = "POSMQRUpdate"
                };

                HttpService httpService = new();
                resp = await httpService.CallDmsExternalApi<ExternalApiResponse>(httpModel);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "CallExternalApi"));
            }

            if (resp.Object.success)
            {
                string msg = resp.Object.message ?? "Success";
                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal(msg, msg)
                });
            }
            else
            {
                string msg = resp.Object.message ?? "SomethingWentWrong";
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal(msg, msg)
                });
            }

        }


        [HttpPost]
        [Route(nameof(GetContactList))]
        public async Task<IActionResult> GetContactList([FromBody] RetailerRequest retailerRequest)
        {
            RetailerService retailerService = new();
            DataTable contact = await retailerService.GetContactList(retailerRequest.retailerCode);
            List<ContactModel> ContactList = contact.AsEnumerable().Select(row => new ContactModel(row)).ToList();

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = ContactList
            });
        }


        /// <summary>
        /// Get Deno offers with recharge type. Applicable from APK v6.0.0 
        /// </summary>
        /// <param name="OfferRequestV2"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetDenoOffers))]
        public async Task<IActionResult> GetDenoOffers([FromBody] OfferRequest deno)
        {
            string traceMsg = string.Empty;
            RedisCache redis;

            List<RechargePackagesRedis> allDenoPackages = new();

            try
            {
                redis = new RedisCache();
                string allDenoPackagesStr = await redis.GetCacheAsync(RedisCollectionNames.DenoDriveDetails);
                allDenoPackages = JsonConvert.DeserializeObject<List<RechargePackagesRedis>>(allDenoPackagesStr)!;
            }
            catch (Exception ex)
            {
                traceMsg = ex.Message;
            }

            List<long> denoIds = new();

            try
            {
                redis = new RedisCache();
                string hasDenoIdsStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerDenoDriveIds, deno.retailerCode);
                string hasDenoIds = JsonConvert.DeserializeObject<dynamic>(hasDenoIdsStr)!;
                if (!string.IsNullOrEmpty(hasDenoIds))
                {
                    denoIds = hasDenoIds.Split(',').Select(s => Convert.ToInt64(s)).ToList();
                }
            }
            catch (Exception ex)
            {
                string _msg = "RetailerDenoIds";
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
            }

            List<RechargePackagesRedis> sortedDenoPackages = allDenoPackages.Where(w => denoIds.Any(a => a == w.id)).ToList();

            List<RechargePackageModel> denoPackages = new();

            Parallel.ForEach(sortedDenoPackages, item =>
            {
                RechargePackageModel pkgInst = new()
                {
                    title = deno.lan == "bn" ? item.titleBn : item.title,
                    offerType = deno.lan == "bn" ? item.offerTypeBn : item.offerType,
                    category = deno.lan == "bn" ? item.categoryBn : item.category,
                    dataPack = deno.lan == "bn" ? item.dataPackBn : item.dataPack,
                    talkTime = deno.lan == "bn" ? item.talkTimeBn : item.talkTime,
                    sms = deno.lan == "bn" ? item.smsBn : item.sms,
                    toffee = deno.lan == "bn" ? item.toffeeBn : item.toffee,
                    validity = deno.lan == "bn" ? item.validityBn : item.validity,
                    commission = item.commission,
                    amount = item.amount,
                    rechargeType = item.rechargeType
                };

                denoPackages.Add(pkgInst);
            });

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(deno, "GetDenoOffers", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = denoPackages
            });
        }


        /// <summary>
        /// API to get Quick Access data. Applicable from APK v6.0.0
        /// Redis DB used only
        /// </summary>
        /// <param name="QuickAccessRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetQuickAccessList))]
        public async Task<IActionResult> GetQuickAccessList([FromBody] QuickAccessRequest quickAccessRequest)
        {
            string traceMsg = string.Empty;

            RedisCache redis;
            IEnumerable<QuickAccessModel> quickAccessList = new List<QuickAccessModel>();
            QuickAccessListModel widgetList = new();

            try
            {
                redis = new RedisCache();
                string QAListStr = await redis.GetCacheAsync(RedisCollectionNames.QuickAccessList);
                IEnumerable<QuickAccessRedisModel> quickAccessRedis = JsonConvert.DeserializeObject<IEnumerable<QuickAccessRedisModel>>(QAListStr);
                quickAccessList = quickAccessRedis.Select(q => q.Adapt<QuickAccessModel>().SetIcon(quickAccessRequest.isDark));
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "GetCacheAsync || DeserializeObject", ex);
            }

            RetailerQAListRedis qaList = new();
            string retailerQAListStr = string.Empty;
            try
            {
                try
                {
                    redis = new RedisCache();
                    retailerQAListStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerQuickAccess, quickAccessRequest.deviceId);
                    if (!string.IsNullOrWhiteSpace(retailerQAListStr))
                    {
                        qaList = JsonConvert.DeserializeObject<RetailerQAListRedis>(retailerQAListStr)!;
                    }
                }
                catch (Exception ex)
                {
                    string _msg = "RetailerQuickAccess";
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                }

                if (string.IsNullOrWhiteSpace(qaList.ActiveQAList) || string.IsNullOrWhiteSpace(qaList.InActiveQAList))
                {
                    RetailerService retailerService = new(Connections.RetAppDbCS);

                    //pulling retailer wise active QA string
                    try
                    {
                        qaList.ActiveQAList = await retailerService.GetActiveQAListIDs(quickAccessRequest);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(ex, "GetActiveQAList"));
                    }

                    retailerService = new(Connections.RetAppDbCS);

                    //pulling retailer wise inactive QA list
                    try
                    {
                        qaList.InActiveQAList = await retailerService.GetInActiveQAListIDs(quickAccessRequest);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(ex, "GetInActiveQAList"));
                    }

                    redis = new RedisCache();
                    await redis.SetCacheAsync(RedisCollectionNames.RetailerQuickAccess, quickAccessRequest.deviceId, qaList.ToJsonString());
                }
                else
                {
                    bool hasUpdate = false;

                    //Filter active & inactivate 

                    List<string> qaCombined = (qaList.ActiveQAList + qaList.InActiveQAList).Split(',').ToList();

                    List<string> allQaids = quickAccessList.Select(s => s.id).ToList();

                    List<string> inactiveQAids = qaList.ActiveQAList.Split(',').ToList().Except(allQaids).ToList();
                    List<string> moreInactiveQAids = qaList.InActiveQAList.Split(',').ToList().Except(allQaids).ToList();
                    inactiveQAids.AddRange(moreInactiveQAids);

                    List<string> allInternalQAIds = new();

                    //remove 10 id if not primary

                    if (!quickAccessRequest.isPrimary)
                    {
                        allInternalQAIds = quickAccessList.Where(w => Convert.ToInt32(w.id) < 100 && Convert.ToInt32(w.id) != 10).Select(s => s.id).ToList();
                    }
                    else
                    {
                        allInternalQAIds = quickAccessList.Where(w => Convert.ToInt32(w.id) < 100).Select(s => s.id).ToList();
                    }

                    List<string> allQaExternalIds = quickAccessList.Where(q => Convert.ToInt32(q.id) > 100).Select(s => s.id).ToList();


                    if (inactiveQAids.Count() > 0)
                    {
                        hasUpdate = true;

                        qaList.ActiveQAList = string.Join(',', qaList.ActiveQAList.Split(',').ToList().Intersect(allQaids));
                        qaList.InActiveQAList = string.Join(',', qaList.InActiveQAList.Split(',').ToList().Intersect(allQaids));
                    }

                    List<string> selectedExternalQAList = qaList.ActiveQAList.Split(',').Where(id => Convert.ToInt32(id) > 100).Select(s => s).ToList();
                    List<string> moreSelectedExternalQAs = qaList.InActiveQAList.Split(',').Where(id => Convert.ToInt32(id) > 100).Select(s => s).ToList();
                    selectedExternalQAList.AddRange(moreSelectedExternalQAs);

                    List<string> selectedInternalQAList = qaList.ActiveQAList.Split(',').Where(id => Convert.ToInt32(id) < 100).Select(s => s).ToList();
                    List<string> moreSelectedInternalQAs = qaList.InActiveQAList.Split(',').Where(id => Convert.ToInt32(id) < 100).Select(s => s).ToList();
                    selectedInternalQAList.AddRange(moreSelectedInternalQAs);

                    //Add new Internal QA
                    if (allInternalQAIds.Count() != selectedInternalQAList.Count())
                    {
                        hasUpdate = true;
                        List<string> newInternalQA = allInternalQAIds.Except(selectedInternalQAList).ToList();

                        foreach (string qid in newInternalQA)
                        {
                            QuickAccessModel qa = quickAccessList.FirstOrDefault(f => f.id == qid);

                            if (qa.isWidgetActive)
                            {
                                qaList.ActiveQAList = qaList.ActiveQAList + "," + qid;
                            }
                            else
                            {
                                qaList.InActiveQAList = qaList.InActiveQAList + "," + qid;
                            }
                        }
                    }


                    //Add new External QA 
                    if (allQaExternalIds.Count() != selectedExternalQAList.Count())
                    {
                        hasUpdate = true;

                        List<string> newQas = allQaExternalIds.Except(selectedExternalQAList).ToList();

                        foreach (string qid in newQas)
                        {
                            QuickAccessModel qa = quickAccessList.FirstOrDefault(f => f.id == qid);

                            if (qa.isWidgetActive)
                            {
                                qaList.ActiveQAList = qaList.ActiveQAList + "," + qid;
                            }
                            else
                            {
                                qaList.InActiveQAList = qaList.InActiveQAList + "," + qid;
                            }
                        }
                    }

                    if (hasUpdate)
                    {
                        redis = new RedisCache();
                        await redis.SetCacheAsync(RedisCollectionNames.RetailerQuickAccess, quickAccessRequest.deviceId, qaList.ToJsonString());
                    }

                }
            }
            catch (Exception ex)
            {
                string _msg = "RetailerQuickAccess_FromDB";
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
            }

            string[] activeList = qaList.ActiveQAList.Split(',');
            string[] inActiveList = qaList.InActiveQAList.Split(',');

            widgetList.activeWidgetList = quickAccessList.Where(w => activeList.Contains(w.id)).ToList();
            widgetList.inactiveWidgetList = quickAccessList.Where(w => inActiveList.Contains(w.id)).ToList();

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(quickAccessRequest, "GetQuickAccessList", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = widgetList
            });
        }


        /// <summary>
        /// Applicable from APK v6.0.0
        /// </summary>
        /// <param name="QuickAccessUpdateRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(UpdateQuickAccessListOrder))]
        public async Task<IActionResult> UpdateQuickAccessListOrder([FromBody] QuickAccessUpdateRequest model)
        {
            string traceMsg = string.Empty;
            model.userId = UserSession.userId;

            List<string> inactivateQAs = model.inactiveWidgetList.Split(",").ToList().Except(model.activeWidgetList.Split(",").ToList()).ToList();

            model.inactiveWidgetList = string.Join(",", inactivateQAs);

            if (!string.IsNullOrWhiteSpace(model.activeWidgetList))
            {
                model.activeWeidgets = model.activeWidgetList;
            }
            else
            {
                model.activeWeidgets = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(model.inactiveWidgetList))
            {
                model.inactiveWeidgets = model.inactiveWidgetList;
            }
            else
            {
                model.inactiveWeidgets = string.Empty;
            }

            RetailerQAListRedis retQAList = new()
            {
                ActiveQAList = model.activeWeidgets,
                InActiveQAList = model.inactiveWeidgets
            };

            try
            {
                RedisCache redis = new();
                bool result = await redis.SetCacheAsync(RedisCollectionNames.RetailerQuickAccess, model.deviceId, retQAList.ToJsonString());
            }
            catch (Exception ex)
            {
                traceMsg = ex.Message;
            }


            RetailerService retailerService = new(Connections.RetAppDbCS);
            int insertResult = 0;

            try
            {
                insertResult = await retailerService.UpdateQuickAccessOrder(model);
            }
            catch (Exception ex)
            {
                string _msg = "UpdateQuickAccessOrder";
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(model, "UpdateQuickAccessListOrder", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = insertResult > 0 ? false : true,
                message = insertResult > 0 ? SharedResource.GetLocal("Success", Message.Success) : SharedResource.GetLocal("Failed", Message.Failed)
            });
        }


        /// <summary>
        /// API to search Deno offer
        /// </summary>
        /// <param name="GlobalOfferSearch">Requesting with OfferSearchRequest's Model"</param>
        /// <returns>Returing RechargePackages data after filter as per request</returns>
        [HttpPost]
        [Route(nameof(GlobalOfferSearch))]
        public async Task<IActionResult> GlobalOfferSearch([FromBody] SearchRequestV2 searchRequest)
        {
            if (string.IsNullOrEmpty(searchRequest.lan))
            {
                searchRequest.lan = "en";
            }
            else
            {
                searchRequest.lan = searchRequest.lan.ToLower();
            }

            RechargeService rechargeService = new(Connections.RetAppDbCS);
            DataTable offers = await rechargeService.SearchOffers(searchRequest);

            List<RechargePackageModel> rechargePackageModels = offers.AsEnumerable().Select(row => HelperMethod.ModelBinding<RechargePackageModel>(row, "GlobalOfferSearchV2", searchRequest.lan)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = rechargePackageModels
            });

        }


        /// <summary>
        /// API to serve archive data(Banner, Notification, FlashPopUp, Communication) along with filtering options
        /// </summary>
        /// <param name="model">Requesting with ArchivedRequest's Model</param>
        /// <returns>Returing Archived data after filter as per request</returns>
        [HttpPost]
        [Route(nameof(GetArchivedData))]
        public async Task<IActionResult> GetArchivedData([FromBody] ArchivedRequest requestModel)
        {
            if (!string.IsNullOrEmpty(requestModel.sortType))
            {
                requestModel.sortType = requestModel.sortType.ToUpper();
            }

            RetailerService ewHomeService = new(Connections.RetAppDbCS);
            DataTable datTable = new();

            try
            {
                datTable = await ewHomeService.GetArchivedData(requestModel);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "GetArchivedData");
                throw new Exception(errMsg);
            }

            string imageBasePath = ExternalKeys.ImageVirtualDirPath;
            List<CommunicationModel> communicaions = datTable.AsEnumerable().Select(row => HelperMethod.ModelBinding<CommunicationModel>(row, "CommunicationModel_Binding", imageBasePath)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = communicaions
            });

        }


        [HttpPost]
        [Route(nameof(CampCompareDetailList))]
        public async Task<IActionResult> CampCompareDetailList([FromBody] CampCompareDetailRequest model)
        {
            string campCat = "";
            model.userId = UserSession.userId;
            DateTime updateTill = DateTime.MinValue;
            CampCompareDetailModel campDetailsModel = new();

            foreach (var camp in model.idList)
            {
                if (camp.campaignCategory.ToLower() == "ext")
                {
                    try
                    {
                        CampaignRequestV2 request = new()
                        {
                            campaignId = camp.campaignId,
                            retailerCode = model.retailerCode,
                            userId = UserSession.userId
                        };

                        RetailerService retailerService = new();
                        DataTable kpidt = new();

                        try
                        {
                            kpidt = await retailerService.GetCampaignKPIList(request);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampaignKPIList"));
                        }

                        List<CampaignKPIListModel> campKpiList = kpidt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignKPIListModel>(row)).ToList();

                        retailerService = new();
                        DataTable rewardt = new();

                        try
                        {
                            rewardt = await retailerService.GetCampaignRewardList(request);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampaignRewardListV2"));
                        }

                        string webUrl = ExternalKeys.WebPortalBaseUrl;
                        List<CampaignRewardListModel> campRewardList = rewardt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignRewardListModel>(row, "", webUrl)).ToList();

                        retailerService = new();
                        DataTable campMoreDetails = new();

                        try
                        {
                            campMoreDetails = await retailerService.GetCampFurtherDetails(request);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampFurtherDetailsV2"));
                        }

                        CampCompareDetails campaignDetails = new();

                        campaignDetails.campaignType = "Admin";
                        campaignDetails.campaignTitle = campMoreDetails.Rows[0]["TITLE"] as string;
                        campaignDetails.campaignCategory = "Ext";
                        campaignDetails.campaignDuration = campMoreDetails.Rows[0]["DURATION"] as string;
                        campaignDetails.startDate = campMoreDetails.Rows[0]["START_DATE"] as string;
                        campaignDetails.endDate = campMoreDetails.Rows[0]["END_DATE"] as string;
                        campaignDetails.campaignEndDate = campMoreDetails.Rows[0]["END_DATE"] as string;

                        campaignDetails.kpiTargetList = campKpiList;
                        campaignDetails.rewardList = campRewardList;

                        campDetailsModel.detailList.Add(campaignDetails);

                        if (updateTill == DateTime.MinValue || updateTill < Convert.ToDateTime(campMoreDetails.Rows[0]["UPDATE_TILL_DATE"]))
                        {
                            updateTill = Convert.ToDateTime(campMoreDetails.Rows[0]["UPDATE_TILL_DATE"]);
                            campDetailsModel.updateTill = campMoreDetails.Rows[0]["UPDATE_TILL"] as string;
                        }
                    }
                    catch (Exception ex)
                    {
                        campCat = camp.campaignCategory;
                        throw;
                    }
                }
                else if (camp.campaignCategory.ToLower() == "self")
                {
                    try
                    {
                        CampaignRequestV2 request = new()
                        {
                            campaignId = camp.campaignId,
                            retailerCode = model.retailerCode,
                            userId = UserSession.userId
                        };

                        RetailerService retailerService = new();
                        DataTable kpidt = new();

                        try
                        {
                            kpidt = await retailerService.GetSelfCampaignKPIList(request);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampaignKPIList"));
                        }

                        List<CampaignKPIListModel> campKpiList = kpidt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignKPIListModel>(row)).ToList();

                        retailerService = new();
                        DataTable rewardt = new();

                        try
                        {
                            rewardt = await retailerService.GetSelfCampaignRewardList(request);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampaignRewardList"));
                        }

                        List<CampaignRewardListModel> campRewardList = rewardt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignRewardListModel>(row, "")).ToList();

                        retailerService = new();
                        DataTable campMoreDetails = new();

                        try
                        {
                            campMoreDetails = await retailerService.GetSelfCampFurtherDetails(request);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampFurtherDetails"));
                        }

                        CampCompareDetails campaignDetails = new();

                        campaignDetails.campaignType = campMoreDetails.Rows[0]["CAMPAIGN_TYPE"] as string;
                        campaignDetails.campaignTitle = campMoreDetails.Rows[0]["TITLE"] as string;
                        campaignDetails.campaignCategory = "Self";
                        campaignDetails.campaignDuration = campMoreDetails.Rows[0]["DURATION"] as string;
                        campaignDetails.startDate = campMoreDetails.Rows[0]["START_DATE"] as string;
                        campaignDetails.endDate = campMoreDetails.Rows[0]["END_DATE"] as string;
                        campaignDetails.campaignEndDate = campMoreDetails.Rows[0]["END_DATE"] as string;

                        campaignDetails.kpiTargetList = campKpiList;
                        campaignDetails.rewardList = campRewardList;

                        campDetailsModel.detailList.Add(campaignDetails);

                        if (updateTill == DateTime.MinValue || updateTill < Convert.ToDateTime(campMoreDetails.Rows[0]["UPDATE_TILL_DATE"]))
                        {
                            updateTill = Convert.ToDateTime(campMoreDetails.Rows[0]["UPDATE_TILL_DATE"]);
                            campDetailsModel.updateTill = campMoreDetails.Rows[0]["UPDATE_TILL"] as string;
                        }
                    }
                    catch (Exception ex)
                    {
                        campCat = camp.campaignCategory;
                        throw;
                    }
                }
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = campDetailsModel
            });

        }


        [HttpPost]
        [Route(nameof(GetCampCompareList))]
        public async Task<IActionResult> GetCampCompareList([FromBody] CampaignRequestV3 model)
        {
            model.userId = UserSession.userId;
            RetailerService retailerService = new();
            DataTable campaigns = new();

            try
            {
                campaigns = await retailerService.GetCampaignList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampaignList"));
            }

            retailerService = new();
            DataTable selfCampaign = new();

            try
            {
                selfCampaign = await retailerService.GetSelfCampaignList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampaignList"));
            }

            List<CampCompareTitleList> campaignList = campaigns.AsEnumerable().Where(row => Convert.ToBoolean(row["IS_ENROLLED"]) == true).Select(srow => HelperMethod.ModelBinding<CampCompareTitleList>(srow)).ToList();

            campaignList.AddRange(selfCampaign.AsEnumerable().Where(row => Convert.ToBoolean(row["IS_ENROLLED"]) == true).Select(srow => HelperMethod.ModelBinding<CampCompareTitleList>(srow)).ToList());

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = campaignList
            });


        }


        [HttpPost]
        [Route(nameof(GetSelfCampDates))]
        public async Task<IActionResult> GetSelfCampDates([FromBody] SelfCampDatesRequest model)
        {

            RetailerService NewRetailerService = new();

            string ids = string.Join(",", model.targetIdList);
            DataTable dt = new();

            try
            {
                dt = await NewRetailerService.GetCampRetailerDates(model, ids);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampRetailerDates"));
            }

            RetailerCampDuration results = new();

            results.minStartDate = dt.Rows[0]["MIN_START_DATE"] as string;
            results.minEndDate = dt.Rows[0]["MIN_END_DATE"] as string;

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = results
            });

        }


        [HttpPost]
        [Route(nameof(GetSelfRewardList))]
        public async Task<IActionResult> GetSelfRewardList([FromBody] SelfCampaignRewardRequest model)
        {
            RetailerService retailerService = new();
            DataTable rewardDT = new();

            try
            {
                rewardDT = await retailerService.GetSelfRewardList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfRewardList"));
            }

            string baseURL = ExternalKeys.ImageVirtualDirPath;
            List<SelfCampaignRewardList> rewardList = rewardDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<SelfCampaignRewardList>(row, "", baseURL)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = rewardList
            });

        }


        [HttpPost]
        [Route(nameof(GetSelfCampDayList))]
        public async Task<IActionResult> GetSelfCampDayList([FromBody] GetSelfCampDayListRequest model)
        {
            RetailerService retailerService = new();
            string ids = string.Join(",", model.targetIdList);
            DataTable dt = new();

            try
            {
                dt = await retailerService.GetSelfCampDayList(model.retailerCode, ids);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampDayList"));
            }

            List<CampDurationListObject> daysList = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampDurationListObject>(row)).ToList();
            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = daysList
            });

        }


        [HttpPost]
        [Route(nameof(GetSelfKPIList))]
        public async Task<IActionResult> GetSelfKPIList([FromBody] SelfKPIListRequest model)
        {
            RetailerService retailerService = new();
            DataTable kpiDT = new();

            try
            {
                kpiDT = await retailerService.GetCampKPIList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampKPIList"));
            }

            List<SelfCampaignKPIListModel> kpiList = kpiDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<SelfCampaignKPIListModel>(row, "", model.lan)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = kpiList
            });
        }


        [HttpPost]
        [Route(nameof(GetCampaignDetails))]
        public async Task<IActionResult> GetCampaignDetails([FromBody] CampaignRequestV2 model)
        {
            model.userId = UserSession.userId;

            switch (model.campaignCategory)
            {
                case "Ext":

                    try
                    {
                        RetailerService retailerService = new();
                        DataTable kpidt = new();

                        try
                        {
                            kpidt = await retailerService.GetCampaignKPIList(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampaignKPIList"));
                        }

                        List<CampaignKPIListModel> campKpiList = kpidt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignKPIListModel>(row)).ToList();

                        retailerService = new();
                        DataTable rewardt = new();

                        try
                        {
                            rewardt = await retailerService.GetCampaignRewardList(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampaignRewardList"));
                        }

                        string baseURL = ExternalKeys.ImageVirtualDirPath;
                        List<CampaignRewardListModel> campRewardList = rewardt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignRewardListModel>(row, "", baseURL)).ToList();

                        retailerService = new();
                        DataTable campMoreDetails = new();

                        try
                        {
                            campMoreDetails = await retailerService.GetCampFurtherDetails(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampFurtherDetails"));
                        }

                        CampaignDetailsModel campaignDetails = new();

                        campaignDetails.campaignId = model.campaignId;
                        campaignDetails.kpiTargetList = campKpiList;
                        campaignDetails.rewardList = campRewardList;
                        campaignDetails.campaignCategory = "Ext";
                        if (campMoreDetails.Rows.Count > 0)
                        {
                            campaignDetails.enrollTypeId = campMoreDetails.Rows[0]["ENROLL_TYPE_ID"] as string;
                            campaignDetails.enrollType = campMoreDetails.Rows[0]["ENROLL_TYPE"] as string;
                            campaignDetails.isEnrolled = Convert.ToBoolean(campMoreDetails.Rows[0]["IS_ENROLLED"]);
                            campaignDetails.updateTill = campMoreDetails.Rows[0]["UPDATE_TILL"] as string;
                            campaignDetails.ussd = campMoreDetails.Rows[0]["USSD"] as string;
                        }
                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("Success", Message.Success),
                            data = campaignDetails
                        });
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                case "Self":
                    try
                    {
                        RetailerService NewRetailerService = new();
                        DataTable kpidt = new();

                        try
                        {
                            kpidt = await NewRetailerService.GetSelfCampaignKPIList(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampaignKPIList"));
                        }

                        List<CampaignKPIListModel> campKpiList = kpidt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignKPIListModel>(row)).ToList();

                        NewRetailerService = new();
                        DataTable rewardt = new();

                        try
                        {
                            rewardt = await NewRetailerService.GetSelfCampaignRewardList(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampaignRewardList"));
                        }

                        List<CampaignRewardListModel> campRewardList = rewardt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignRewardListModel>(row, "")).ToList();

                        NewRetailerService = new();
                        DataTable campMoreDetails = new();

                        try
                        {
                            campMoreDetails = await NewRetailerService.GetSelfCampFurtherDetails(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampFurtherDetails"));
                        }

                        CampaignDetailsModel campaignDetails = new();

                        campaignDetails.campaignId = model.campaignId;
                        campaignDetails.kpiTargetList = campKpiList;
                        campaignDetails.rewardList = campRewardList;
                        campaignDetails.campaignCategory = "Self";

                        if (campMoreDetails.Rows.Count > 0)
                        {
                            campaignDetails.enrollTypeId = campMoreDetails.Rows[0]["ENROLL_TYPE_ID"] as string;
                            campaignDetails.enrollType = campMoreDetails.Rows[0]["ENROLL_TYPE"] as string;
                            campaignDetails.isEnrolled = Convert.ToBoolean(campMoreDetails.Rows[0]["IS_ENROLLED"]);
                            campaignDetails.updateTill = campMoreDetails.Rows[0]["UPDATE_TILL"] as string;
                            campaignDetails.ussd = campMoreDetails.Rows[0]["USSD"] as string;
                        }

                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("Success", Message.Success),
                            data = campaignDetails
                        });
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                default:

                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("InvalidOperationType", Message.InvalidOperationType),
                    });
            }
        }


        /// <summary>
        /// API to Enroll Campaign Created by Admin
        /// </summary>
        /// <param name="CampaignEnroll"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(CampaignEnroll))]
        public async Task<IActionResult> CampaignEnroll([FromBody] CampaignEnrollOrCancelRequest model)
        {
            model.userId = UserSession.userId;

            switch (model.campaignCategory)
            {
                case "Ext":
                    try
                    {
                        RetailerService retailerService = new();
                        long result = 0;

                        try
                        {
                            result = await retailerService.EnrollExtCampaign(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "EnrollExtCampaign"));
                        }

                        return Ok(new ResponseMessage()
                        {
                            isError = result > 0 ? false : true,
                            message = result > 0 ? SharedResource.GetLocal("CampaignEnrollSuccessful", Message.CampaignEnrollSuccessful) : result == -1 ? SharedResource.GetLocal("CampaingKPIOverlap", Message.CampaingKPIOverlap) : SharedResource.GetLocal("Failed", Message.Failed),
                        });
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                case "Self":
                    try
                    {
                        RetailerService NewRetailerService = new();
                        long result = 0;

                        try
                        {
                            result = await NewRetailerService.EnrollSelfCampaign(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "EnrollSelfCampaign"));
                        }

                        if (result != -999)
                        {
                            return Ok(new ResponseMessage()
                            {
                                isError = result > 0 ? false : true,
                                message = result > 0 ? SharedResource.GetLocal("CampaignEnrollSuccessful", Message.CampaignEnrollSuccessful) : SharedResource.GetLocal("Failed", Message.Failed),
                            });
                        }
                        else
                        {
                            return Ok(new ResponseMessage()
                            {
                                isError = true,
                                message = SharedResource.GetLocal("YouAlreadyHaveEnrolledForMaximumTimeForThisCampaign", Message.YouAlreadyHaveEnrolledForMaximumTimeForThisCampaign)
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                default:

                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("InvalidOperationType", Message.InvalidOperationType),
                    });
            }
        }


        /// <summary>
        /// API to Cancel Campaign Enrollment 
        /// </summary>
        /// <param name="CampaignCancel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(CancelCampEnrollment))]
        public async Task<IActionResult> CancelCampEnrollment([FromBody] CampaignEnrollOrCancelRequest model)
        {
            model.userId = UserSession.userId;

            switch (model.campaignCategory)
            {
                case "Ext":
                    try
                    {
                        RetailerService retailerService = new();
                        int result = 0;

                        try
                        {
                            result = await retailerService.CancelExtCampaignEnroll(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "CancelExtCampaignEnroll"));
                        }

                        return Ok(new ResponseMessage()
                        {
                            isError = result > 0 ? false : true,
                            message = result > 0 ? SharedResource.GetLocal("CancelCampaignEnrollmentSuccessful", Message.CancelCampaignEnrollmentSuccessful) : SharedResource.GetLocal("Failed", Message.Failed),
                        });
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                case "Self":
                    try
                    {
                        RetailerService NewRetailerService = new();
                        int result = 0;

                        try
                        {
                            result = await NewRetailerService.CancelSelfCampaignEnroll(model);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(ex, "CancelSelfCampaignEnroll"));
                        }

                        return Ok(new ResponseMessage()
                        {
                            isError = result > 0 ? false : true,
                            message = result > 0 ? SharedResource.GetLocal("CancelCampaignEnrollmentSuccessful", Message.CancelCampaignEnrollmentSuccessful) : SharedResource.GetLocal("Failed", Message.Failed)
                        });
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                default:

                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("InvalidOperationType", Message.InvalidOperationType),
                    });
            }
        }


        /// <summary>
        /// API to get Campaign History
        /// </summary>
        /// <param name="CampaignHistory"></param>
        /// <returns>Status</returns>
        [HttpPost]
        [Route(nameof(CampaignHistory))]
        public async Task<IActionResult> CampaignHistory([FromBody] CampaignHistoryRequest model)
        {
            DateTime today = DateTime.Now;
            DateTime fromDate = new(today.Year, today.Month, 1);
            DateTime tillDate = new(today.Year, today.Month, today.Day);

            if (model.startDate == null)
            {
                model.startDate = fromDate;
                model.endDate = tillDate;
            }

            RetailerService retailerService = new();
            DataTable kpidt = new();

            try
            {
                kpidt = await retailerService.GetCampHistoryKPIList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampHistoryKPIList"));
            }

            List<CampaignHistoryKPI> campKpiList = kpidt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignHistoryKPI>(row)).ToList();

            retailerService = new();
            DataTable campDT = new();

            try
            {
                campDT = await retailerService.GetCampHistoryList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampHistoryList"));
            }

            List<CampaignHistoryModel> campHistoryList = campDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignHistoryModel, CampaignHistoryKPI>(row, campKpiList)).ToList();

            retailerService = new();
            string historyUpdateTill = "";

            try
            {
                historyUpdateTill = await retailerService.GetCampHistoryUpdateTill(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampHistoryUpdateTill"));
            }

            //Self Campaign Section
            RetailerService reService = new();
            DataTable selfKPIdt = new();

            try
            {
                selfKPIdt = await reService.GetSelfCampHistoryKPIList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampHistoryKPIList"));
            }

            List<CampaignHistoryKPI> campSelfKpiList = selfKPIdt.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignHistoryKPI>(row)).ToList();

            if (model.dateField == "START_DATE") model.dateField = "FROM_DATE";
            if (model.dateField == "END_DATE") model.dateField = "TO_DATE";

            reService = new();
            DataTable selfCampDT = new();

            try
            {
                selfCampDT = await reService.GetSelfCampHistoryList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampHistoryList"));
            }

            List<CampaignHistoryModel> selfCampHistoryList = selfCampDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignHistoryModel, CampaignHistoryKPI>(row, campSelfKpiList)).ToList();

            campHistoryList.AddRange(selfCampHistoryList);
            reService = new();

            string selfHistoryUpdateTill = "";

            try
            {
                selfHistoryUpdateTill = await reService.GetSelfCampHistoryUpdateTill(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampHistoryUpdateTill"));
            }

            string maxUpdateTill = string.Empty;

            if (!string.IsNullOrEmpty(historyUpdateTill) && !string.IsNullOrEmpty(selfHistoryUpdateTill))
            {
                maxUpdateTill = Convert.ToDateTime(historyUpdateTill) > Convert.ToDateTime(selfHistoryUpdateTill) ? historyUpdateTill : selfHistoryUpdateTill;

                DateTime selfCampUpdateTill = Convert.ToDateTime(selfHistoryUpdateTill);
            }
            else if (!string.IsNullOrEmpty(historyUpdateTill) || !string.IsNullOrEmpty(selfHistoryUpdateTill))
            {
                if (!string.IsNullOrEmpty(historyUpdateTill))
                {
                    maxUpdateTill = historyUpdateTill;
                }
                else
                {
                    maxUpdateTill = selfHistoryUpdateTill;
                }
            }

            CampaignHistory histories = new()
            {
                updateTill = maxUpdateTill,
                campaignList = campHistoryList
            };

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = histories
            });

        }


        /// <summary>
        ///  This API applicable from v6.0.0
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetSelfKPIDetails))]
        public async Task<IActionResult> GetSelfKPIDetails([FromBody] CampaignKPIRequest model)
        {
            RetailerService NewRetailerService = new();
            string ids = string.Join(",", model.targetIdList);
            DataTable kpiDT = new();

            try
            {
                kpiDT = await NewRetailerService.GetCampKPIDetails(model, ids);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "GetCampKPIDetailsV2");
                throw new Exception(errMsg);
            }

            List<SelfCampaignKPIDetailsModelV2> kpiList = kpiDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<SelfCampaignKPIDetailsModelV2>(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = kpiList
            });

        }


        /// <summary>
        /// API to Create Campaign by Admin. This API applicable from v6.0.0
        /// </summary>
        /// <param name="CreateCampaignByRetailerV2"></param>
        /// <returns>Status</returns>
        [HttpPost]
        [Route(nameof(CreateCampaignByRetailer))]
        public async Task<IActionResult> CreateCampaignByRetailer([FromBody] CreateCampaignByRetailerRequest model)
        {
            string traceMsg = string.Empty;
            RetailerService retailerService = new();

            model.userId = UserSession.userId;

            string campInsertResult = "";

            try
            {
                campInsertResult = await retailerService.CampaignByRetailer(model);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "CampaignByRetailer");
                throw new Exception(errMsg);
            }

            string[] campResultSplit = new string[3];

            if (!campInsertResult.ToLower().Contains("failed"))
            {
                campResultSplit = campInsertResult.Split(',');
            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("Failed", Message.Failed)
                });
            }


            for (var i = 0; i < model.targets.Count(); i++)
            {
                RetailerService rs = new();
                CampaignTargetListRequest target = model.targets[i];

                CampaignTargetRequestModel campTarget = new();
                campTarget.retailerCode = model.retailerCode;
                campTarget.campaignId = Convert.ToInt32(campResultSplit[0]);
                campTarget.campEnrollId = Convert.ToInt32(campResultSplit[1]);
                campTarget.kpiId = target.kpiId;
                campTarget.kpiTarget = target.kpiTarget;
                campTarget.targetUnit = target.targetUnit;
                campTarget.userId = UserSession.userId;
                campTarget.kpiConfigId = target.kpiConfigId;

                long? targetResult = 0;

                try
                {
                    targetResult = await rs.InsertRetailerCampTarget(campTarget);
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "InsertRetailerCampTarget");
                    throw new Exception(errMsg);
                }

                if (targetResult < 0)
                {
                    rs = new();

                    try
                    {
                        await rs.DeleteInsertedCampaign(campTarget.campaignId, campTarget.campEnrollId, Convert.ToInt32(campResultSplit[2]));
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            traceMsg += "DeleteInsertedCampaign || ID:" + campTarget.campaignId.ToString() + " || " + ex.InnerException.ToString();
                        }
                        traceMsg += "DeleteInsertedCampaign || ID:" + campTarget.campaignId.ToString() + " || " + ex.Message;
                    }

                    if (!string.IsNullOrWhiteSpace(traceMsg))
                    {
                        LoggerService _logger = new();
                        _logger.WriteTraceMessageInText(model, "CreateCampaignByRetailer", traceMsg);
                    }

                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("Failed", Message.Failed)
                    });
                }
            }

            return Ok(new ResponseMessage()
            {
                isError = Convert.ToInt32(campResultSplit[0]) > 0 ? false : true,
                message = Convert.ToInt32(campResultSplit[0]) > 0 ? SharedResource.GetLocal("SaveSuccessful", Message.SaveSuccess) : SharedResource.GetLocal("Failed", Message.Failed)
            });
        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="DigitalServiceHistoryRequest"></param>
        /// <returns>SC Sales History</returns>
        [HttpPost]
        [Route(nameof(GetDigitalServiceHistory))]
        public async Task<IActionResult> GetDigitalServiceHistory([FromBody] DigitalServiceHistoryRequest model)
        {
            RetailerService retailerService = new();

            DataTable dt = new();

            try
            {
                dt = await retailerService.GetDigitalServiceHistory(model);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "GetDigitalServiceHistory");
                throw new Exception(errMsg);
            }

            List<DigitalServiceHistoryResponse> digitalServiceHistoryResponse = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<DigitalServiceHistoryResponse>(row, string.Empty, model.lan)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = digitalServiceHistoryResponse
            });

        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="DigitalProductRequest"></param>
        /// <returns>SC Sales History</returns>
        [HttpPost]
        [Route(nameof(GetDigitalProductList))]
        public async Task<IActionResult> GetDigitalProductList([FromBody] DigitalProductRequest model)
        {
            RetailerService retailerService = new();
            DataTable dt = new();

            try
            {
                dt = await retailerService.GetDigitalProductList();
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "GetDigitalProductList");
                throw new Exception(errMsg);
            }

            List<DigitalProductResponse> digitalProduct = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<DigitalProductResponse>(row, string.Empty, model.lan)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = digitalProduct
            });
        }


        [HttpPost]
        [Route(nameof(SubmitShortSurveyResponse))]
        public async Task<IActionResult> SubmitShortSurveyResponse([FromBody] SurveyResponseRequest model)
        {
            SurveyResponse responseModel = new()
            {
                surveyId = model.surveyId,
                answer = model.answer,
                isShortSurvey = Convert.ToInt32(model.isShortSurvey),
                retailerCode = model.retailerCode
            };

            SurveyService surveyService = new();

            int insertResult = 0;

            try
            {
                insertResult = await surveyService.InsertShortSurveyResponse(responseModel);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "InsertShortSurveyResponse"));
            }

            if (insertResult <= 0)
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("UnableToSaveResponse", Message.UnableToSaveResponse)
                });
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("SaveSuccessful", Message.SaveSuccess),
            });
        }


        /// <summary>
        /// Save or update firebase token for specific device
        /// </summary>
        /// <param name="deviceTokenRequest"></param>
        /// <returns>Returns ResponseMessage</returns>
        [HttpPost]
        [Route(nameof(SaveDeviceToken))]
        public async Task<IActionResult> SaveDeviceToken([FromBody] DeviceTokenRequest deviceTokenRequest)
        {
            RetailerService retailerService = new();
            long tokenID = 0;

            try
            {
                tokenID = await retailerService.SaveDeviceTokens(deviceTokenRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveDeviceTokens"));
            }

            bool isSuccess = tokenID > 0 ? true : false;

            return Ok(new ResponseMessage()
            {
                isError = isSuccess ? false : true,
                message = isSuccess ? SharedResource.GetLocal("Success", Message.Success) : SharedResource.GetLocal("Failed", Message.Failed)
            });
        }


        [HttpPost]
        [Route(nameof(GetPopUpSettings))]
        public async Task<IActionResult> GetPopUpSettings([FromBody] RetailerRequestV2 retailerRequest)
        {
            if (string.IsNullOrWhiteSpace(retailerRequest.iTopUpNumber))
            {
                string msg = HelperMethod.GetInvalidParameterMsg("iTopUpNumber");
                throw new Exception(msg);
            }

            RetailerService retailerService = new();
            string callingTime = await retailerService.GetRegionWisePopupCallingTime(retailerRequest.iTopUpNumber);

            var data = new
            {
                PopUpMethodsCallingTime = callingTime,
                FeatureStatus.PopUpMethodsRestrictTime,
                FeatureStatus.PopUpMethodsCallingSlot
            };

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = data
            });
        }


        /// <summary>
        /// Direct Logout from device
        /// </summary>
        /// <param name="Logout"></param>
        /// <returns>Return ResponseMessage</returns>
        [HttpPost]
        [Route(nameof(LogOut))]
        public async Task<IActionResult> LogOut([FromBody] LogoutRequest logout)
        {
            DeviceStatusRequest model = new()
            {
                retailerCode = logout.retailerCode,
                operationalDeviceId = logout.deviceId,
                userId = logout.userId
            };

            RetailerService retailerService = new();
            var result = await retailerService.LogoutDevice(model);

            if (result > 0)
            {
                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("LogoutSuccess", Message.LogoutSuccess)
                });
            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("LogoutFailed", Message.LogoutFailed)
                });
            }
        }


        /// <summary>
        /// API to serve registered secondary list only to primary device [true/false]
        /// </summary>
        /// <param name="GetSecondaryDeviceList">Requesting parameter with retailerCode and deviceId</param>
        /// <returns>Return reuslt of device list that are registered as secondary device</returns>
        [HttpPost]
        [Route(nameof(GetSecondaryDeviceList))]
        public async Task<IActionResult> GetSecondaryDeviceList([FromBody] RetailerRequest getSecondaryDeviceListRequest)
        {
            RetailerService retailerService = new();
            DataTable deviceList = new();

            try
            {
                deviceList = await retailerService.SecondaryDeviceList(getSecondaryDeviceListRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SecondaryDeviceList"));
            }

            List<SecondaryDeviceListModel> secondaryDeviceListModel = deviceList.AsEnumerable().Select(row => HelperMethod.ModelBinding<SecondaryDeviceListModel>(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = secondaryDeviceListModel
            });
        }


        /// <summary>
        /// API to change device type
        /// </summary>
        /// <param name="SecondaryDeviceSettings">Requesting parameter with DeviceStatusRequest's model</param>
        /// <returns>Return reuslt of device status change result with operationType and operationalDeviceId</returns>
        [HttpPost]
        [Route(nameof(SecondaryDeviceSettings))]
        public async Task<IActionResult> SecondaryDeviceSettings([FromBody] DeviceStatusRequest deviceStatusRequest)
        {
            deviceStatusRequest.userId = UserSession.userId;

            RetailerService NewRetailerService = new();

            switch (deviceStatusRequest.operationType)
            {
                case "Deregister":

                    try
                    {
                        Dictionary<string, string> responseDictionary = new()
                        {
                                { "operationType", "Deregister" }
                            };

                        long derestrationResult = await NewRetailerService.DeregisterDevice(deviceStatusRequest);

                        List<string> keyList = new() { deviceStatusRequest.retailerCode + "_" + deviceStatusRequest.operationalDeviceId };
                        RedisCache redis = new();
                        await redis.RemoveLoginProviderFromRedis(RedisCollectionNames.RetailerChkInGuids, keyList);

                        bool isSuccess = derestrationResult > 0 ? true : false;

                        responseDictionary.Add("success", isSuccess.ToString());

                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("Success", Message.Success),
                            data = responseDictionary
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(ex, "DeregisterDevice"));
                    }

                case "SecondaryToPrimary":

                    try
                    {
                        Dictionary<string, string> responseDictionary = new()
                        {
                                { "operationType", "SecondaryToPrimary" },
                                { "operationalDeviceId", deviceStatusRequest.operationalDeviceId }
                            };

                        long result = await NewRetailerService.ChangeDeviceType(deviceStatusRequest);

                        bool isSuccess = false;

                        if (result == 99)
                        {
                            isSuccess = false;

                            responseDictionary.Add("success", isSuccess.ToString());


                            return Ok(new ResponseMessage()
                            {
                                isError = false,
                                message = SharedResource.GetLocal("DeviceNotEnabled", Message.DeviceNotEnabled),
                                data = responseDictionary
                            });
                        }
                        else
                        {
                            isSuccess = result > 0 ? true : false;

                            responseDictionary.Add("success", isSuccess.ToString());

                            return Ok(new ResponseMessage()
                            {
                                isError = false,
                                message = SharedResource.GetLocal("Success", Message.Success),
                                data = responseDictionary
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(ex, "ChangeDeviceType"));
                    }

                case "Enable":

                    try
                    {
                        Dictionary<string, string> responseDictionary = new();

                        responseDictionary.Add("operationType", "Enable");

                        deviceStatusRequest.deviceStatus = 1;

                        var result = await NewRetailerService.EnableDisableDevice(deviceStatusRequest);

                        bool isSuccess = result > 0 ? true : false;

                        responseDictionary.Add("success", isSuccess.ToString());

                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("Success", Message.Success),
                            data = responseDictionary
                        });

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(ex, "EnableDisableDevice"));
                    }

                case "Disable":

                    try
                    {
                        Dictionary<string, string> responseDictionary = new();

                        responseDictionary.Add("operationType", "Disable");

                        deviceStatusRequest.deviceStatus = 0;

                        var result = await NewRetailerService.EnableDisableDevice(deviceStatusRequest);

                        bool isSuccess = result > 0 ? true : false;

                        responseDictionary.Add("success", isSuccess.ToString());

                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("Success", Message.Success),
                            data = responseDictionary
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(ex, "EnableDisableDevice"));
                    }

                default:
                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("InvalidOperationType", Message.InvalidOperationType),
                    });
            }
        }


        /// <summary>
        /// Applicable from APK v6.0.0
        /// API to serve all PopUp survey to user.
        /// </summary>
        /// <param name="FlashPopUpRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetPopUpSurvey))]
        public async Task<IActionResult> GetPopUpSurvey([FromBody] RetailerRequestV2 surveyRequest)
        {
            string traceMsg = string.Empty;

            bool isEnablePopUpSurvey = FeatureStatus.IsNewPopUpSurveyEnable;

            if (isEnablePopUpSurvey)
            {
                RedisCache redis;
                List<PopUpSurveyDetailsRedis> popUpSurveyRedis = new();

                try
                {
                    redis = new RedisCache();
                    string popUpSurveyRedisStr = await redis.GetCacheAsync(RedisCollectionNames.PopUpSurveyDetails);
                    popUpSurveyRedis = JsonConvert.DeserializeObject<List<PopUpSurveyDetailsRedis>>(popUpSurveyRedisStr)!;
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.ExMsgSubString(ex, "Get PopUp Survey data from Redis and Deserialize", 400);
                }

                List<string> popUpSurveyIds = new();

                try
                {
                    redis = new RedisCache();
                    string hasPopUpSurveyIdsStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerPopUpSurveyIds, surveyRequest.retailerCode);
                    string hasPopUpSurveyIds = JsonConvert.DeserializeObject<dynamic>(hasPopUpSurveyIdsStr)!;
                    if (!string.IsNullOrWhiteSpace(hasPopUpSurveyIds))
                    {
                        popUpSurveyIds = hasPopUpSurveyIds.Split(',').ToList();
                    }
                }
                catch (Exception ex)
                {
                    string _msg = "RetailerPopUpSurveyIds";
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                }

                List<PopUpSurveyDetailsRedis> appPopUpSurveys = popUpSurveyRedis.Where(w => popUpSurveyIds.Any(f => f == w.surveyId.ToString())).ToList();
                Parallel.ForEach(appPopUpSurveys, item => item.surveyWebLink += surveyRequest.retailerCode);

                var _data = new
                {
                    PopUpData = appPopUpSurveys,
                    LastSyncDateTime = DateTime.Now.ToEnUSDateString("dd MMM yyyy HH:mm")
                };

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(surveyRequest, "GetPopUpSurvey", traceMsg);
                }

                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = _data
                });
            }
            else
            {
                var _data = new
                {
                    PopUpData = new string[] { },
                    LastSyncDateTime = DateTime.Now.ToEnUSDateString("dd MMM yyyy HH:mm")
                };

                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = _data
                });
            }
        }


        /// <summary>
        /// Bulk Enable/Disable/Deregister Deivce using a device list
        /// </summary>
        /// <param name="BulkDeviceSetting"></param>
        /// <returns>Return ResponseMessage</returns>
        [HttpPost]
        [Route(nameof(BulkDeviceSetting))]
        public async Task<IActionResult> BulkDeviceSetting([FromBody] BulkDeviceStatusRequest deviceStatusRequest)
        {
            RetailerService retailerService = new();

            RetailerRequest deviceListModed = new()
            {
                retailerCode = deviceStatusRequest.retailerCode,
                deviceId = deviceStatusRequest.deviceId
            };

            DataTable deviceList = new();

            try
            {
                deviceList = await retailerService.SecondaryDeviceList(deviceListModed);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SecondaryDeviceList"));
            }

            List<SecondaryDeviceId> secondaryDeviceList = deviceList.AsEnumerable().Select(row => HelperMethod.ModelBinding<SecondaryDeviceId>(row)).Where(ip => ip.isPrimary == false).ToList();

            switch (deviceStatusRequest.operationType)
            {
                case "EnableAll":
                    Dictionary<string, string> responseDictionary = [];

                    if (secondaryDeviceList.Count is not 0 && secondaryDeviceList is not null)
                    {
                        responseDictionary.Add("operationType", "EnableAll");

                        var resultCount = 0;

                        foreach (var id in secondaryDeviceList)
                        {
                            retailerService = new();

                            DeviceStatusRequest model = new()
                            {
                                retailerCode = deviceStatusRequest.retailerCode,
                                deviceId = deviceStatusRequest.deviceId,
                                operationalDeviceId = id.DeviecId,
                                deviceStatus = 1
                            };

                            var result = await retailerService.EnableDisableDevice(model);

                            if (result > 0)
                            {
                                resultCount++;
                            }
                        }

                        if (secondaryDeviceList.Count == resultCount)
                        {
                            responseDictionary.Add("success", "True");

                            return Ok(new ResponseMessage()
                            {
                                isError = false,
                                message = SharedResource.GetLocal("Success", Message.Success),
                                data = responseDictionary
                            });
                        }
                        else
                        {
                            responseDictionary.Add("Error", "One or more operation failed");

                            return Ok(new ResponseMessage()
                            {
                                isError = true,
                                message = SharedResource.GetLocal("FailedSteps", Message.FailedSteps),
                                data = responseDictionary
                            });
                        }
                    }
                    else
                    {
                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("NothingToExecute", Message.NothingToExecute)
                        });
                    }

                case "DisableAll":
                    responseDictionary = [];

                    if (secondaryDeviceList.Count is not 0 && secondaryDeviceList is not null)
                    {
                        responseDictionary.Add("operationType", "DisableAll");

                        var resultCount = 0;

                        foreach (var id in secondaryDeviceList)
                        {
                            retailerService = new();

                            DeviceStatusRequest model = new()
                            {
                                retailerCode = deviceStatusRequest.retailerCode,
                                deviceId = deviceStatusRequest.deviceId,
                                operationalDeviceId = id.DeviecId,
                                deviceStatus = 0
                            };

                            var result = await retailerService.EnableDisableDevice(model);

                            if (result > 0)
                            {
                                resultCount++;
                            }
                        }

                        if (secondaryDeviceList.Count == resultCount)
                        {
                            responseDictionary.Add("success", "True");

                            return Ok(new ResponseMessage()
                            {
                                isError = false,
                                message = SharedResource.GetLocal("Success", Message.Success),
                                data = responseDictionary
                            });
                        }
                        else
                        {
                            responseDictionary.Add("Error", "One or more operation failed");

                            return Ok(new ResponseMessage()
                            {
                                isError = true,
                                message = SharedResource.GetLocal("FailedSteps", Message.FailedSteps),
                                data = responseDictionary
                            });
                        }
                    }
                    else
                    {
                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("NothingToExecute", Message.NothingToExecute)
                        });
                    }

                case "DeregisterAll":
                    responseDictionary = [];

                    if (secondaryDeviceList.Count is not 0 && secondaryDeviceList is not null)
                    {
                        responseDictionary.Add("operationType", "DeregisterAll");

                        var resultCount = 0;

                        foreach (var id in secondaryDeviceList)
                        {
                            retailerService = new();

                            DeviceStatusRequest model = new()
                            {
                                retailerCode = deviceStatusRequest.retailerCode,
                                operationalDeviceId = id.DeviecId
                            };

                            var result = await retailerService.DeregisterDevice(model);

                            if (result > 0)
                            {
                                resultCount++;
                            }
                        }

                        if (secondaryDeviceList.Count == resultCount)
                        {
                            responseDictionary.Add("success", "True");

                            List<string> keyList = secondaryDeviceList.AsEnumerable().Select(x => deviceStatusRequest.retailerCode + "_" + x.DeviecId).ToList();
                            RedisCache redis = new();
                            await redis.RemoveLoginProviderFromRedis(RedisCollectionNames.RetailerChkInGuids, keyList);

                            return Ok(new ResponseMessage()
                            {
                                isError = false,
                                message = SharedResource.GetLocal("Success", Message.Success),
                                data = responseDictionary
                            });
                        }
                        else
                        {
                            responseDictionary.Add("Error", "One or more operation failed");

                            return Ok(new ResponseMessage()
                            {
                                isError = true,
                                message = SharedResource.GetLocal("FailedSteps", Message.FailedSteps),
                                data = responseDictionary
                            });
                        }
                    }
                    else
                    {
                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("NothingToExecute", Message.NothingToExecute)
                        });
                    }

                default:

                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("InvalidOperationType", Message.InvalidOperationType),
                    });
            }
        }


        [HttpPost]
        [Route(nameof(NotificationCount))]
        public async Task<IActionResult> NotificationCount([FromBody] RetailerRequest retailerRequest)
        {
            RetailerService retailerService = new();
            int notificationCount = 0;

            try
            {
                notificationCount = await retailerService.GetNotificationCount(retailerRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetNotificationCount"));
            }

            dynamic dynamicCount = new ExpandoObject();
            dynamicCount.count = notificationCount;

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = dynamicCount
            });

        }


        [HttpPost]
        [Route(nameof(NotificationStatusUpdate))]
        public async Task<IActionResult> NotificationStatusUpdate([FromBody] RetailerNotificationRequest retailerRequest)
        {
            RetailerService retailerService = new();
            int notificationCount = 0;

            try
            {
                notificationCount = await retailerService.UpdateNotoficationStatus(retailerRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateNotoficationStatusV2"));
            }

            dynamic dynamicCount = new ExpandoObject();
            dynamicCount.count = notificationCount;

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = dynamicCount
            });

        }


        /// <summary>
        /// API method for getting Banner data. Applicable from APK v6.0.0. Only Redis DB used
        /// </summary>
        /// <param name="retailerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(AppBanner))]
        public async Task<IActionResult> AppBanner([FromBody] RetailerRequestV2 retailerRequest)
        {
            string traceMsg = string.Empty;
            RedisCache redis;
            List<BannerDetailsRedis> redisAppBanners = new();

            try
            {
                redis = new RedisCache();
                var redBnrDetailsStr = await redis.GetCacheAsync(RedisCollectionNames.BannerDetailsMySQL);
                redisAppBanners = JsonConvert.DeserializeObject<List<BannerDetailsRedis>>(redBnrDetailsStr)!;
            }
            catch (Exception ex)
            {
                traceMsg = ex.Message;
            }

            List<long> bannerIds = new();

            try
            {
                redis = new RedisCache();
                string hasBnrIdsStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerBannerIdsMySQL, retailerRequest.retailerCode);
                string hasBnrIds = JsonConvert.DeserializeObject<dynamic>(hasBnrIdsStr)!;
                if (!string.IsNullOrEmpty(hasBnrIds))
                {
                    bannerIds = hasBnrIds.Split(',').Select(s => Convert.ToInt64(s)).ToList();
                }
            }
            catch (Exception ex)
            {
                string _msg = "RetailerBannerIdsMySQL";
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
            }

            List<BannerDetailsRedis> appBanners = redisAppBanners.Where(w => bannerIds.Any(a => a == w.bannerId)).ToList();

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(retailerRequest, "AppBanner", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = appBanners
            });
        }


        /// <summary>
        /// API method for getting App settings data and banner last update time. 
        /// Applicable from APK v6.0.0, new add Advertisement initiation type
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetAppSettings))]
        public async Task<IActionResult> GetAppSettings([FromBody] AppSettingsRequest model)
        {
            string traceMsg = string.Empty;

            int.TryParse(model.versionName.Replace(".", string.Empty), out int _versionName);
            if (ApiManager.IsApkVersionBlockAsync(model.versionCode, _versionName, model.appToken))
            {
                string lowerVersionBlockMessage = AppAllowedVersion.lower_version_block_message;
                string lowerVersionBlockMessageBN = AppAllowedVersion.lower_version_block_message_bn;

                return Ok(new ResponseMessageV2()
                {
                    isError = false,
                    isThisVersionBlocked = true,
                    message = model.lan.Equals("en", StringComparison.CurrentCultureIgnoreCase) ? lowerVersionBlockMessage : lowerVersionBlockMessageBN,
                });
            }
            else
            {
                model ??= new AppSettingsRequest();

                RedisCache redis;
                AppFeatureSettings appSettingsResp = new();
                bool _hasAdvert = false;

                if (!string.IsNullOrWhiteSpace(model.retailerCode))
                {
                    redis = new RedisCache();
                    string result = await redis.GetCacheAsync(RedisCollectionNames.RetailerAdvertisementIds, model.retailerCode);
                    string retailerAdvertId = result ?? JsonConvert.DeserializeObject<string>(result)!;
                    _hasAdvert = !string.IsNullOrWhiteSpace(retailerAdvertId);
                }

                try
                {
                    redis = new RedisCache();
                    string appSettingsInfo = await redis.GetCacheAsync(RedisCollectionNames.AppSettingsInfo);
                    if (!string.IsNullOrWhiteSpace(appSettingsInfo))
                    {
                        appSettingsResp = JsonConvert.DeserializeObject<AppFeatureSettings>(appSettingsInfo)!;
                    }
                    else
                    {
                        RetailerService retailerService = new();
                        DataTable dt = new();

                        try
                        {
                            dt = await retailerService.GetAppSettingsInfo();
                        }
                        catch (Exception exp)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(exp, "GetAppSettingsInfo"));
                        }

                        appSettingsResp = HelperMethod.ModelBinding<AppFeatureSettings>(dt);
                    }

                    appSettingsResp.hasAdvertisement = _hasAdvert;
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "", ex);
                    RetailerService retailerService = new();
                    DataTable dt = new();

                    try
                    {
                        dt = await retailerService.GetAppSettingsInfo();
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(exp, "GetAppSettingsInfo"));
                    }

                    appSettingsResp = HelperMethod.ModelBinding<AppFeatureSettings>(dt);
                }

                try
                {
                    redis = new RedisCache();
                    string bnrIDTimes = await redis.GetCacheAsync(RedisCollectionNames.BannerIdTimeMySQL);

                    if (!string.IsNullOrWhiteSpace(bnrIDTimes))
                    {
                        Dictionary<string, string> idTimesList = new();

                        try
                        {
                            idTimesList = JsonConvert.DeserializeObject<Dictionary<string, string>>(bnrIDTimes)!;
                        }
                        catch (Exception ex)
                        {
                            string _msg = "BannerIDTimes Deserialization";
                            traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                        }

                        Parallel.ForEach(idTimesList, item =>
                        {
                            AppBannerIDDatesVM tempInstnc = new()
                            {
                                bannerId = Convert.ToInt64(item.Key),
                                dateInMS = Convert.ToInt64(item.Value)
                            };

                            appSettingsResp.bannerUpdatedInMSList.Add(tempInstnc);
                        });
                    }
                }
                catch (Exception ex)
                {
                    string _msg = "Get BannerIDTimes";
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                }

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(model, "GetAppSettings", traceMsg);
                }

                return Ok(new ResponseMessageV2()
                {
                    isError = false,
                    isThisVersionBlocked = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = appSettingsResp
                });
            }
        }


        /// <summary>
        /// Get RetailerApp and Bar Phone transaction data by kafka service. This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="C2STransactions"></param>
        /// <returns>Returing transaction results with filtering request</returns>
        [HttpPost]
        [Route(nameof(C2STransactions))]
        public async Task<IActionResult> C2STransactions([FromBody] TransactionsRequest tranRequest)
        {
            string traceMsg = string.Empty;

            if (!string.IsNullOrEmpty(tranRequest.sortByAmt))
            {
                tranRequest.sortByAmt = tranRequest.sortByAmt.ToUpper();
            }

            if (!string.IsNullOrEmpty(tranRequest.sortByInOut))
            {
                tranRequest.sortByInOut = tranRequest.sortByInOut.ToUpper();
                tranRequest.sortByAmt = "ASC";
            }

            if (!string.IsNullOrEmpty(tranRequest.sortByDate))
            {
                tranRequest.sortByDate = tranRequest.sortByDate.ToUpper();
            }
            else if (string.IsNullOrEmpty(tranRequest.sortByAmt) && string.IsNullOrEmpty(tranRequest.sortByInOut))
            {
                tranRequest.sortByDate = "DESC";
            }

            RetailerService retailerService = new(Connections.RetAppDbCS);
            DataTable datatable = retailerService.GetC2STransactions(tranRequest);

            DataTable postpaidTrans = new();
            try
            {
                retailerService = new();
                postpaidTrans = await retailerService.GetC2SPostpaidTransactions(tranRequest);
                datatable.Merge(postpaidTrans, true, MissingSchemaAction.Ignore);
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "GetC2SPostpaidTransactions", ex);
            }

            List<TransactionsModel> transactions = datatable.AsEnumerable().Select(row => HelperMethod.ModelBinding<TransactionsModel>(row)).ToList();
            List<TransactionsModel> combineTransactions = TransactionsModel.OrderingTransactionData(transactions, tranRequest);

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "C2STransactions",
                retailerCode = tranRequest.retailerCode,
                appPage = LMSAppPages.Transactions,
                transactionID = LMSService.GetTransactionId(tranRequest.retailerCode),
                msisdn = "88" + tranRequest.iTopUpNumber,
                points = LMSPoints.Transactions.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(tranRequest, "C2STransactions", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = combineTransactions
            });
        }


        /// <summary>
        /// API to serve ticker message to user to user. This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="GetTickerMessage">Requesting parameter with RetailerRequest's Model</param>
        /// <returns>Return a list of Ticker Message</returns>
        [HttpPost]
        [Route(nameof(GetTickerMessage))]
        public async Task<IActionResult> GetTickerMessage([FromBody] RetailerRequestV2 tickerRequest)
        {
            RetailerService retailerService = new();
            DataTable dtResp = new();

            try
            {
                dtResp = await retailerService.GetTickerMessages(tickerRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetTickerMessages"));
            }

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "GetTickerMessage",
                retailerCode = tickerRequest.retailerCode,
                appPage = LMSAppPages.Home_Page,
                transactionID = LMSService.GetTransactionId(tickerRequest.retailerCode),
                msisdn = $"88{tickerRequest.iTopUpNumber}",
                points = LMSPoints.Home.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);


            if (dtResp.Rows.Count > 0)
            {
                List<string> notifications = dtResp.AsEnumerable().Select(row => row["DESCRIPTION"].ToString()).ToList();

                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = notifications
                });
            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound),
                    data = new string[] { }
                });
            }

        }


        /// <summary>
        /// API to get List of Campaign. This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="CampaignList"></param>
        /// <returns>List of campaigns</returns>
        [HttpPost]
        [Route(nameof(CampaignList))]
        public async Task<IActionResult> CampaignList([FromBody] CampaignRequestV3 model)
        {
            model.userId = UserSession.userId;
            RetailerService retailerService = new();
            DataTable campaigns = new();

            try
            {
                campaigns = await retailerService.GetCampaignList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCampaignList"));
            }

            retailerService = new();
            DataTable selfCampaign = new();

            try
            {
                selfCampaign = await retailerService.GetSelfCampaignList(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSelfCampaignList"));
            }

            string baseURL = ExternalKeys.ImageVirtualDirPath;
            List<CampaignListModel> campaignList = campaigns.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignListModel>(row, "", baseURL)).ToList();
            campaignList.AddRange(selfCampaign.AsEnumerable().Select(row => HelperMethod.ModelBinding<CampaignListModel>(row, "", baseURL)).ToList());

            campaignList = campaignList.OrderByDescending(o => o.createdDate).ToList();

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "CampaignListV2",
                retailerCode = model.retailerCode,
                appPage = LMSAppPages.Campaign,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                msisdn = $"88{model.iTopUpNumber}",
                points = LMSPoints.Campaign.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = campaignList
            });
        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="DigitalServiceSubmitRequest"></param>
        /// <returns>Submit DIGITAL SERVICE</returns>
        [HttpPost]
        [Route(nameof(SubmitDigitalService))]
        public async Task<IActionResult> SubmitDigitalService([FromBody] DigitalServiceSubmitRequest model)
        {
            string traceMsg = string.Empty;
            string rechargeResponse = string.Empty;

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "SubmitDigitalService",
                retailerCode = model.retailerCode,
                appPage = LMSAppPages.Digital_Services,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                msisdn = $"88{model.iTopUpNumber}",
                points = LMSPoints.Digital_Services.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);


            RetailerService retailerService = new();
            int userId = UserSession.userId;
            long insertId = 0;

            try
            {
                insertId = await retailerService.SaveDigitalService(model, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitDigitalService"));
            }

            if (insertId > 0 && !string.IsNullOrEmpty(model.amount) && !string.IsNullOrEmpty(model.userPin))
            {
                try
                {
                    var userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
                    RechargeService rechargeService = new();
                    var resData = await rechargeService.DigitalServiceEvRecharge(model, userAgent);
                    rechargeResponse = resData.message;
                    traceMsg = string.IsNullOrEmpty(traceMsg) ? resData.message : traceMsg + " || " + resData.message;
                }
                catch (Exception ex)
                {
                    retailerService = new(Connections.RetAppDbCS);
                    await retailerService.DeleteTableRows(insertId, "RSLTBLDIGITALSERVICE", "DIGITAL_SERVICE_ID");

                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = ex.Message
                    });
                }
            }

            if (insertId > 0)
            {
                try
                {
                    retailerService = new();
                    await retailerService.DigitalServiceSmsSendToUser(model.productId, model.subscriberNumber);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "DigitalServiceSmsSendToUser"));
                }
            }

            string _msgKey = string.IsNullOrEmpty(rechargeResponse) ? "SubmitDigitalServiceSuccessful" : rechargeResponse;
            string _msgValue = string.IsNullOrEmpty(rechargeResponse) ? Message.SubmitDigitalServiceSuccessful : rechargeResponse;

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(model, "SubmitDigitalService", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = insertId > 0 ? false : true,
                message = insertId > 0 ? SharedResource.GetLocal(_msgKey, _msgValue) : SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong)
            });
        }


        /// <summary>
        /// This api is applicable from APK v6.0.0
        /// API to serve communications data along with filtering options.
        /// </summary>
        /// <param name="Communications">Requesting with CommunicationV3Request's Model"</param>
        /// <returns>Returing communications data after filter as per request</returns>
        [HttpPost]
        [Route("Communications")]
        public async Task<IActionResult> Communications([FromBody] CommunicationRequestV4 communicationRequest)
        {
            if (!string.IsNullOrEmpty(communicationRequest.sortType))
            {
                communicationRequest.sortType = communicationRequest.sortType.ToUpper();
            }

            RetailerRequest tempRequest = new()
            {
                sessionToken = communicationRequest.sessionToken,
                retailerCode = communicationRequest.retailerCode
            };

            RetailerService retailerService = new(Connections.RetAppDbCS);
            DataTable communications = new();

            try
            {
                communications = await retailerService.GetCommunications(communicationRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetCommunicationsV3"));
            }

            string imageVirDirPath = ExternalKeys.ImageVirtualDirPath;
            List<CommunicationModel> communicaions = communications.AsEnumerable().Select(row => HelperMethod.ModelBinding<CommunicationModel>(row, "", imageVirDirPath)).ToList();


            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "Communications",
                retailerCode = communicationRequest.retailerCode,
                appPage = LMSAppPages.Communication,
                transactionID = LMSService.GetTransactionId(communicationRequest.retailerCode),
                msisdn = $"88{communicationRequest.iTopUpNumber}",
                points = LMSPoints.Communication.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = communicaions
            });
        }


        [HttpPost]
        [Route(nameof(BestPracticesHistory))]
        public async Task<IActionResult> BestPracticesHistory([FromBody] RetailerRequestV2 retailer)
        {
            RetailerService retailerService = new(Connections.RetAppDbCS);
            DataTable bestpractice = new();

            try
            {
                bestpractice = await retailerService.BestPracticesHistory(retailer);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "BestPracticesHistory"));
            }

            List<BestPractices> bestpractices = bestpractice.AsEnumerable().Select(bp => HelperMethod.ModelBinding<BestPractices>(bp)).ToList();

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "BestPracticesHistory",
                retailerCode = retailer.retailerCode,
                appPage = LMSAppPages.Best_Practice,
                transactionID = LMSService.GetTransactionId(retailer.retailerCode),
                msisdn = $"88{retailer.iTopUpNumber}",
                points = LMSPoints.Best_Practice.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = bestpractices
            });
        }


        /// <summary>
        /// This api is applicable from APK v6.0.0
        /// </summary>
        /// <param name="RaiseComplSubmitRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(SubmitRaiseComplaint))]
        public async Task<IActionResult> SubmitRaiseComplaint([FromBody] RaiseComplSubmitRequest reqModel)
        {
            string traceMsg = string.Empty;

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "SubmitRaiseComplaint",
                retailerCode = reqModel.retailerCode,
                appPage = LMSAppPages.Raise_Complaint,
                transactionID = LMSService.GetTransactionId(reqModel.retailerCode),
                msisdn = $"88{reqModel.iTopUpNumber}",
                points = LMSPoints.Raise_Complaint.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);


            RetailerService retailerService;
            if (!string.IsNullOrEmpty(reqModel.address))
            {
                reqModel.description += " | " + reqModel.address;
            }

            if (reqModel.category.Equals("superoffice", StringComparison.OrdinalIgnoreCase) && reqModel.soSubCategoryId <= 0)
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("MissingSOCategory", Message.MissingSOCategory)
                });
            }

            int userId = UserSession.userId;
            long insertId = 0;
            List<ComplaintImageVM> imageVMs = [];
            List<string> imagePaths = [];
            ComplaintSuperOfficeModel soModel = reqModel.Adapt<ComplaintSuperOfficeModel>();

            try
            {
                if (reqModel.images.Any())
                {
                    for (int i = 0; i < reqModel.images.Count; i++)
                    {
                        string image = reqModel.images[i];
                        if (string.IsNullOrWhiteSpace(image)) continue;

                        string fileExt = SaveFileHelper.GetFileExtension(image).Extension;
                        string imageName = (i + 1) + DateTime.Now.ToEnUSDateString("yyyyMMddHHmmssfff") + "." + fileExt;
                        string folderName = "RaiseComplaint" + "/" + reqModel.retailerCode;

                        ComplaintImageVM imageModel = new()
                        {
                            FileName = imageName,
                            FolderName = folderName,
                            ImageBase64 = image
                        };

                        string fileName = Path.Combine(folderName, imageName);
                        imagePaths.Add(fileName);
                        imageVMs.Add(imageModel);
                    }

                    if (reqModel.category.Equals("superoffice", StringComparison.OrdinalIgnoreCase))
                    {
                        soModel.fileName = imageVMs[0].FileName;
                        soModel.image = imageVMs[0].ImageBase64;
                    }

                    if (imagePaths.Count > 0)
                    {
                        traceMsg = reqModel.FileLocation = string.Join("|", imagePaths);
                    }
                }

                retailerService = new();
                insertId = await retailerService.SaveRaiseComplaint(reqModel, userId);

                if (insertId > 0 && imageVMs.Count > 0)
                {
                    RetailerService reService = new();
                    await reService.SaveFileToApiServer(imageVMs, reqModel);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveRaiseComplaint"));
            }

            if (insertId == 0)
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong)
                });
            }

            //================ Update RSO App / vFocus / SuperOffice about New Complaint submission =========
            bool isSuccess = false;

            if (!string.IsNullOrEmpty(reqModel.category) && reqModel.category.ToLower() == "app")
            {
                retailerService = new();
                DataTable raiseComplInfo = new();
                try
                {
                    raiseComplInfo = await retailerService.GetRaiseComplaintInfoV2(reqModel, insertId);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRaiseComplaintInfo"));
                }

                RaiseComplaintExternalSubmissionModel extSubModel;
                ExternalSubmitResponse rcExtSubmit;

                if (raiseComplInfo.Rows.Count > 0)
                {
                    extSubModel = HelperMethod.ModelBinding<RaiseComplaintExternalSubmissionModel>(raiseComplInfo.Rows[0]);
                    extSubModel.raiseComplaintID = (int)insertId;
                    extSubModel.description = reqModel.description;
                    extSubModel.retailerCode = reqModel.retailerCode;
                    extSubModel.userName = ExternalKeys.RsoUser;
                    extSubModel.password = ExternalKeys.RsoCred;

                    if (reqModel.images.Any())
                    {
                        extSubModel.images = reqModel.images;
                    }

                    string submitURL = ExternalKeys.RaiseComplaintSubmitUrlToRso;

                    try
                    {
                        if (string.IsNullOrWhiteSpace(extSubModel.preferredLevelName) && string.IsNullOrWhiteSpace(extSubModel.preferredLevelContact))
                        {
                            string msg = SharedResource.GetLocal("RsoOrZmInfoNotFound", Message.RsoOrZmInfoNotFound);

                            string errMsg = $"{extSubModel.preferredLevel} Information not found.";

                            if (!string.IsNullOrWhiteSpace(errMsg))
                            {
                                LoggerService _logger = new();
                                _logger.WriteTraceMessageInText(reqModel, "SubmitRaiseComplaint || S1", errMsg);
                            }

                            return Ok(new ResponseMessage()
                            {
                                isError = true,
                                message = string.Format(msg, extSubModel.preferredLevel)
                            });
                        }

                        HttpRequestModel httpModel = new()
                        {
                            requestBody = extSubModel,
                            requestUrl = submitURL,
                            requestMediaType = MimeTypes.Json,
                            requestMethod = "SubmitRaiseComplaint"
                        };

                        HttpService httpService = new();
                        rcExtSubmit = await httpService.SubmitExternalRequest(httpModel);

                        //once api call complete update complaint status to 1
                        if (rcExtSubmit.success == true && rcExtSubmit.statusCode == 200)
                        {
                            isSuccess = true;

                            UpdateRaiseComplaintRequest updateModel = new()
                            {
                                retailerCode = reqModel.retailerCode,
                                complaintStatus = 1,
                                raiseComplaintID = (int)insertId
                            };

                            updateModel.userName = ExternalKeys.InternalUser;

                            Tuple<bool, string> result;
                            try
                            {
                                retailerService = new();
                                result = await retailerService.UpdateRaiseComplaintStatusV2(updateModel);
                            }
                            catch (Exception ex)
                            {
                                string errMsg = HelperMethod.ExMsgBuild(ex, "UpdateRaiseComplaintStatusV2");
                                throw new Exception(errMsg);
                            }

                            if (!result.Item1)
                            {
                                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, result.Item2, null);
                            }
                        }
                        else
                        {
                            return Ok(new ResponseMessage()
                            {
                                isError = true,
                                message = rcExtSubmit.message
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        string errMsg = HelperMethod.ExMsgBuild(ex, "SubmitComplaintRequest");
                        throw new Exception(errMsg);
                    }
                }
                else
                {
                    string errMsg = "No RaiseComplaint Info found.";

                    if (!string.IsNullOrWhiteSpace(errMsg))
                    {
                        LoggerService _logger = new();
                        _logger.WriteTraceMessageInText(reqModel, "SubmitRaiseComplaint || S2", errMsg);
                    }

                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("ComplaintSubmitFailed", Message.ComplaintSubmitFailed)
                    });
                }
            }
            else if (!string.IsNullOrEmpty(reqModel.category) && reqModel.category.ToLower() == "superoffice")
            {
                SuperOfficeService superOffice = new();
                long soTicketId = await superOffice.SubmitTicketToSuperOffice(soModel);

                if (soTicketId > 0)
                {
                    isSuccess = true;

                    UpdateRaiseComplaintRequest updateModel = new()
                    {
                        retailerCode = reqModel.retailerCode,
                        complaintStatus = 2,
                        raiseComplaintID = (int)insertId
                    };

                    updateModel.userName = ExternalKeys.InternalUser;

                    try
                    {
                        retailerService = new();
                        await retailerService.UpdateRaiseComplaintStatusFromSO(updateModel, soTicketId);
                    }
                    catch (Exception ex)
                    {
                        string errMsg = HelperMethod.ExMsgBuild(ex, "UpdateRaiseComplaintStatusFromSOV2");
                        throw new Exception(errMsg);
                    }
                }
                else if (soTicketId == -1)
                {
                    string errMsg = "Unable to raise complaint at SuperOffice";

                    if (!string.IsNullOrWhiteSpace(errMsg))
                    {
                        LoggerService _logger = new();
                        _logger.WriteTraceMessageInText(reqModel, "SubmitRaiseComplaint || S3", errMsg);
                    }

                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("CanNotRaiseToSO", Message.CanNotRaiseToSO)
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(reqModel, "SubmitRaiseComplaint", traceMsg);
            }

            //================= Update RSO App / vFocus about New Complaint submission =========

            return Ok(new ResponseMessage()
            {
                isError = !isSuccess,
                message = isSuccess ? SharedResource.GetLocal("ComplaintSubmitSuccessful", Message.ComplaintSubmitSuccessful) : SharedResource.GetLocal("ComplaintSubmitFailed", Message.ComplaintSubmitFailed)
            });

        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="BTSInfoRequest"></param>
        /// <returns>Get BTS Info List</returns>
        [HttpPost]
        [Route(nameof(GetBTSInfoList))]
        public async Task<IActionResult> GetBTSInfoList([FromBody] RetailerRequestV2 model)
        {
            DataTable dataTable = new();
            List<BTSInfoModel> bTSInfoList = new();

            try
            {
                RetailerService retailerService = new(Connections.RetAppDbCS);
                dataTable = await retailerService.GetBTSLocation(model.retailerCode);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetBTSLocation"));
            }

            bTSInfoList = dataTable.AsEnumerable().Select(row => HelperMethod.ModelBinding<BTSInfoModel>(row)).ToList();

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "GetBTSInfoList",
                retailerCode = model.retailerCode,
                appPage = LMSAppPages.BTS_View,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                msisdn = $"88{model.iTopUpNumber}",
                points = LMSPoints.BTS_View.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = bTSInfoList
            });

        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="SubmitProductRating"></param>
        /// <returns>Submit Product Rating</returns>
        [HttpPost]
        [Route(nameof(SubmitProductRating))]
        public async Task<IActionResult> SubmitProductRating([FromBody] SubmitProductRating model)
        {
            model.userId = UserSession.userId;
            RetailerService retailerService = new(Connections.RetAppDbCS);
            long rated = 0;

            try
            {
                rated = await retailerService.SaveProductRating(model);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "SaveProductRating");
                throw new Exception(errMsg);
            }

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "SubmitProductRating",
                retailerCode = model.retailerCode,
                appPage = LMSAppPages.Product_Review_And_Rating,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                msisdn = $"88{model.iTopUpNumber}",
                points = LMSPoints.Product_Review_And_Rating.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            return Ok(new ResponseMessage()
            {
                isError = rated == -9999 ? true : (rated > 0 ? false : true),
                message = rated == -9999 ? SharedResource.GetLocal("RatingAlreadySubmitted", Message.SaveSuccess) : (rated > 0 ? SharedResource.GetLocal("RatingSubmitted", Message.SaveSuccess) : SharedResource.GetLocal("Failed", Message.Failed))
            });

        }


        [HttpPost]
        [Route(nameof(UploadFile))]
        public async Task<IActionResult> UploadFile([FromBody] FileSaveRequestV2 fileRequest)
        {
            string apiWebToken = ExternalKeys.RetailerApiToWebCred;
            if (!fileRequest.webToken.Equals(apiWebToken, StringComparison.Ordinal))
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = ResponseMessages.InvalidSessionMsg
                });
            }

            RetailerService retailerService = new(Connections.RetAppDbCS);
            await retailerService.SaveBase64File(fileRequest);

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = Message.Success
            });
        }


        [HttpPost]
        [Route("DisclaimerNotices")]
        public async Task<IActionResult> DisclaimerNotices([FromBody] RetailerRequestV2 disclaimerRequest)
        {
            if (!string.IsNullOrEmpty(disclaimerRequest.lan))
            {
                disclaimerRequest.lan = disclaimerRequest.lan.ToLower();
            }

            RetailerService retailerService = new(Connections.RetAppDbCS);
            DataTable dtResp = new();

            try
            {
                dtResp = await retailerService.GetDisclaimerNotices(disclaimerRequest.lan);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDisclaimerNotices"));
            }

            if (dtResp.Rows.Count > 0)
            {
                Dictionary<string, string> notices = [];
                foreach (DataRow dr in dtResp.Rows)
                {
                    string viewName = dr["VIEWNAME"].ToString();
                    notices.Add(viewName.ToCamelCase(), dr["NOTICE"].ToString());
                }

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = notices
                });
            }
            else
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound)
                });
            }

        }


        [HttpPost]
        [Route("DeleteContactNumber")]
        public async Task<IActionResult> DeleteContactNumber([FromBody] ContactDeleteRequest deleteRequest)
        {
            RetailerService retailerService = new();
            int res = await retailerService.DeleteContact(deleteRequest.contactId);

            string msg = res > 0 ?
                SharedResource.GetLocal("Success", Message.Success) :
                SharedResource.GetLocal("DeleteFailed", Message.DeleteFailed);

            return new OkObjectResult(new ResponseMessage()
            {
                isError = !(res > 0),
                message = msg
            });
        }


        /// <summary>
        /// API method for getting App settings data and banner last update time. 
        /// Applicable from APK v6.1.0, new add Advertisement initiation type
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetAppSettingsV2))]
        public async Task<IActionResult> GetAppSettingsV2([FromBody] AppSettingsRequest model)
        {
            string traceMsg = string.Empty;

            _ = int.TryParse(model.versionName.Replace(".", string.Empty), out int _versionName);
            if (ApiManager.IsApkVersionBlockAsync(model.versionCode, _versionName, model.appToken))
            {
                string lowerVersionBlockMessage = AppAllowedVersion.lower_version_block_message;
                string lowerVersionBlockMessageBN = AppAllowedVersion.lower_version_block_message_bn;

                return Ok(new ResponseMessageV2()
                {
                    isError = false,
                    isThisVersionBlocked = true,
                    message = model.lan.Equals("en", StringComparison.CurrentCultureIgnoreCase) ? lowerVersionBlockMessage : lowerVersionBlockMessageBN,
                });
            }
            else
            {
                model ??= new AppSettingsRequest();

                RedisCache redis;
                AppFeatureSettings appSettingsResp = new();
                bool _hasAdvert = false;

                if (!string.IsNullOrWhiteSpace(model.retailerCode))
                {
                    redis = new RedisCache();
                    string result = await redis.GetCacheAsync(RedisCollectionNames.RetailerAdvertisementIds, model.retailerCode);
                    string retailerAdvertId = result ?? JsonConvert.DeserializeObject<string>(result)!;
                    _hasAdvert = !string.IsNullOrWhiteSpace(retailerAdvertId);
                }

                try
                {
                    redis = new RedisCache();
                    string appSettingsInfo = await redis.GetCacheAsync(RedisCollectionNames.AppSettingsInfo);
                    if (!string.IsNullOrWhiteSpace(appSettingsInfo))
                    {
                        appSettingsResp = JsonConvert.DeserializeObject<AppFeatureSettings>(appSettingsInfo)!;
                    }
                    else
                    {
                        RetailerService retailerService = new();
                        DataTable dt = new();

                        try
                        {
                            dt = await retailerService.GetAppSettingsInfo();
                        }
                        catch (Exception exp)
                        {
                            throw new Exception(HelperMethod.ExMsgBuild(exp, "GetAppSettingsInfo"));
                        }

                        appSettingsResp = HelperMethod.ModelBinding<AppFeatureSettings>(dt);
                    }

                    appSettingsResp.hasAdvertisement = _hasAdvert;
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "", ex);
                    RetailerService retailerService = new();
                    DataTable dt = new();

                    try
                    {
                        dt = await retailerService.GetAppSettingsInfo();
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(exp, "GetAppSettingsInfo"));
                    }

                    appSettingsResp = HelperMethod.ModelBinding<AppFeatureSettings>(dt);
                }

                try
                {
                    redis = new RedisCache();
                    string bnrIDTimes = await redis.GetCacheAsync(RedisCollectionNames.BannerIdTimeMySQL);

                    if (!string.IsNullOrWhiteSpace(bnrIDTimes))
                    {
                        Dictionary<string, string> idTimesList = new();

                        try
                        {
                            idTimesList = JsonConvert.DeserializeObject<Dictionary<string, string>>(bnrIDTimes)!;
                        }
                        catch (Exception ex)
                        {
                            string _msg = "BannerIdTimes Deserialization";
                            traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                        }

                        Parallel.ForEach(idTimesList, item =>
                        {
                            AppBannerIDDatesVM tempInstnc = new()
                            {
                                bannerId = Convert.ToInt64(item.Key),
                                dateInMS = Convert.ToInt64(item.Value)
                            };

                            appSettingsResp.bannerUpdatedInMSList.Add(tempInstnc);
                        });
                    }
                }
                catch (Exception ex)
                {
                    string _msg = "Get BannerIdTimes";
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                }

                try
                {
                    redis = new RedisCache();
                    string bnrIDTimes = await redis.GetCacheAsync(RedisCollectionNames.GamificationBannerIdTime);

                    if (!string.IsNullOrWhiteSpace(bnrIDTimes))
                    {
                        Dictionary<string, string> idTimesList = [];

                        try
                        {
                            idTimesList = JsonConvert.DeserializeObject<Dictionary<string, string>>(bnrIDTimes)!;
                        }
                        catch (Exception ex)
                        {
                            string _msg = "Gamification BannerId Times Deserialization";
                            traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                        }

                        Parallel.ForEach(idTimesList, item =>
                        {
                            AppBannerIDDatesVM tempInstnc = new()
                            {
                                bannerId = Convert.ToInt64(item.Key),
                                dateInMS = Convert.ToInt64(item.Value)
                            };

                            appSettingsResp.gamificationBannerUpdatedMSList.Add(tempInstnc);
                        });
                    }
                }
                catch (Exception ex)
                {
                    string _msg = "Get Gamification BannerId Times";
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
                }

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(model, "GetAppSettingsV2", traceMsg);
                }

                return Ok(new ResponseMessageV2()
                {
                    isError = false,
                    isThisVersionBlocked = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = appSettingsResp
                });
            }
        }


        [HttpPost]
        [Route("GamificationBanners")]
        public async Task<IActionResult> GamificationBanners([FromBody] RetailerRequest reqModel)
        {
            string traceMsg = string.Empty;
            RedisCache redis = new();
            List<GamificationBannerRedis> bannerList = [];

            try
            {
                string allBannersStr = await redis.GetCacheAsync(RedisCollectionNames.GamificationBannerDetails);
                bannerList = JsonConvert.DeserializeObject<List<GamificationBannerRedis>>(allBannersStr);
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.FormattedExceptionMsg(ex);
            }

            List<long> bannerIds = [];

            try
            {
                redis = new RedisCache();
                string hasBnrIdsStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerGamificationBannerIds, reqModel.retailerCode);
                string hasBnrIds = JsonConvert.DeserializeObject<dynamic>(hasBnrIdsStr)!;
                if (!string.IsNullOrEmpty(hasBnrIds))
                {
                    bannerIds = hasBnrIds.Split(',').Select(s => Convert.ToInt64(s)).ToList();
                }
            }
            catch (Exception ex)
            {
                string _msg = "RetailerGamificationBannerIds";
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, _msg, ex);
            }

            List<GamificationBannerRedis> appBanners = bannerList.Where(w => bannerIds.Any(a => a == w.bannerId)).ToList();

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(reqModel, "GamificationBanners", traceMsg);
            }

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = appBanners
            });
        }


        [HttpPost]
        [Route(nameof(SubmitGamificationResponse))]
        public async Task<IActionResult> SubmitGamificationResponse([FromBody] GamificationResponseReq reqModel)
        {
            RetailerService retailerService = new();

            long res = await retailerService.SaveGamificationResponse(reqModel);

            return new OkObjectResult(new ResponseMessage()
            {
                isError = res <= 0,
                message = res > 0 ? SharedResource.GetLocal("SaveSuccess", Message.SaveSuccess) : SharedResource.GetLocal("Failed", Message.Failed)
            });
        }


        #region================ || Private Methods || ================

        private string GetEVBalanceNdSaveToDb(RSOEligibility rsoEligibility, ref StockSummaryModel stockSummary)
        {
            string traceMsg = string.Empty;
            ItopUpXmlRequestV2 xmlRequest = new()
            {
                Url = ExternalKeys.EvPinLessBlncURL,
                Type = "OTHERBALAN",
                Msisdn = rsoEligibility.RsoNumber,
                Pin = "",
                Loginid = "",
                Password = "",
                DateTime = "",
                Imei = "",
                Msisdn2 = rsoEligibility.iTopUpNumber,
                Language1 = "0",
                Extrefnum = "123456"
            };

            var userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
            StockService stockService = new(Connections.RetAppDbCS);
            string resp = stockService.GetITopUpBalanceNew(rsoEligibility, xmlRequest, userAgent);

            try
            {
                stockService = new StockService(Connections.RetAppDbCS);
                stockService.FormatEvBalanceResponse(ref stockSummary, resp);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "FormatEvBalanceResponse"));
            }

            _ = double.TryParse(stockSummary.amount, out double _amount);
            VMItopUpStock model = new()
            {
                ItopUpNumber = rsoEligibility.iTopUpNumber.Substring(1),
                RetailerCode = rsoEligibility.retailerCode,
                NewBalance = _amount,
                UpdateTime = stockSummary.updateTime
            };

            try
            {
                stockService = new StockService(Connections.RetAppDbCS);
                int res = stockService.UpdateItopUpBalance(model);
                if (res == 0)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "Unable to update Balance;", null);
                }
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, stockSummary.updateTime, null);
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "UpdateItopUpBalance", ex);
            }

            return traceMsg;
        }

        #endregion================ || Private Methods || ================

    }
}