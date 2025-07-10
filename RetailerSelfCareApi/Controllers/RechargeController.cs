///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Application.Services;
using Domain.Helpers;
using Domain.LMS;
using Domain.LMS.Request;
using Domain.RequestModel;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using static Domain.Enums.EnumCollections;

namespace RetailerSelfCareApi.Controllers
{
    [Route("api/v2")]
    [ApiController]
    public class RechargeController : ControllerBase
    {
        [HttpPost]
        [Route("RechargeOffers")]
        public IActionResult GetRechargeOffers([FromBody] OfferRequest offer)
        {
            offer.rechargeType = (int)OfferType.RechargeOffer;
            DataTable offers;
            using(RechargeService rechargeService = new(Connections.RetAppDbCS))
            {
                offers = rechargeService.GetRechargeOffers(offer);
            }

            List<RechargePackageModel> rechargePackageModels = offers.AsEnumerable().Select(row => HelperMethod.ModelBinding<RechargePackageModel>(row, "", offer.lan)).ToList();

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = rechargePackageModels
            });
        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="IrisRecharge"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(IrisRecharge))]
        public async Task<IActionResult> IrisRecharge([FromBody] IrisRechargeRequest rechargeRequest)
        {
            string traceMsg = string.Empty;
            string loginProvider = Request.HttpContext.Items["loginProviderId"] as string;
            StockService stockService;
            RetailerSessionCheck retailer = new();

            try
            {
                stockService = new();
                retailer = await stockService.CheckRetailerByCode(rechargeRequest.retailerCode, loginProvider);
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

            string logPath = TextLogging.TextLogPath;
            string irisUrl = ExternalKeys.IrisUrl + "activateDigitalOffer";

            IrisContinueRechargeRequest irisRechargeReq = new();
            IrisContinueRequest req = new()
            {
                username = ExternalKeys.IrisUsername,
                password = ExternalKeys.IrisCred,
                retailerMsisdn = retailer.msisdn,
                subscriberMsisdn = rechargeRequest.subscriberNo.Substring(1),
                rechargeAmount = rechargeRequest.amount,
                transactionID = rechargeRequest.tranId,
                offerID = rechargeRequest.offerId,
                pin = rechargeRequest.pin,
                channel = ExternalKeys.Irischannel,
                gatewayCode = ExternalKeys.IrisGatewayCode,
            };

            irisRechargeReq.request = req;

            RechargeService rechargeService = new(Connections.RetAppDbCS);
            IrisRechargeResponse irisResponse = new();

            string updateTime = "";
            try
            {
                var userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
                irisResponse = await rechargeService.IrisRechargeRequest(irisUrl, irisRechargeReq, rechargeRequest, "IrisRecharge", userAgent);
                updateTime = DateTime.Now.ToEnUSDateString("hh:mm:ss tt, dd MMM yyyy"); // This time will save in ItopUpbalance table as a latest update time.
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "IrisRechargeRequest");
                throw new Exception(errMsg);
            }

            bool status = irisResponse?.response?.statusCode is "0";

            string _respTranId = string.Empty;
            string trnMsg = "";
            if (status)
            {
                try
                {
                    trnMsg = irisResponse.response.statusMessage.ToLower();
                    string[] strList = trnMsg.Split(',');
                    var resList = strList.Where(w => w.Contains("txn number") || w.Contains("transaction id")).ToList();

                    if (resList.Count > 0)
                    {
                        string resStr = resList.FirstOrDefault().Trim();
                        _respTranId = resStr.Replace("txn number", "").Trim();

                        if (_respTranId.Contains("transaction id"))
                        {
                            string resString = _respTranId.Replace("transaction id", "").Trim().ToUpper();
                            var index = resString.IndexOf(" ");
                            _respTranId = resString.Substring(0, index);
                        }
                        if (_respTranId.EndsWith("."))
                        {
                            _respTranId = _respTranId.Substring(0, _respTranId.Length - 1).ToUpper();
                        }
                    }
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "TRANNO_ParseErr", ex);

                    if (!string.IsNullOrEmpty(trnMsg))
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, trnMsg, null);
                }
            }

            TransactionLogVM transObj = new()
            {
                rCode = rechargeRequest.retailerCode,
                tranNo = rechargeRequest.tranId,
                tranType = "ITOP'UP",
                amount = Convert.ToInt64(rechargeRequest.amount),
                tranDate = DateTime.Now,
                msisdn = "88" + rechargeRequest.subscriberNo,
                rechargeType = (int)RechargeType.IrisRecharge,
                isTranSuccess = status == true ? 1 : 0,
                tranMsg = irisResponse.response.statusMessage,
                retMsisdn = "0" + retailer.msisdn,
                loginProvider = loginProvider,
                respTranId = _respTranId,
                lat = rechargeRequest.lat,
                lng = rechargeRequest.lng,
                ipAddress = HelperMethod.GetIPAddress()
            };

            rechargeService = new(Connections.RetAppDbCS);
            bool logStatus = false;

            if (status)
            {
                try
                {
                    logStatus = await rechargeService.SaveTransactionLog(transObj);
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "SaveTransactionLog");
                    throw new Exception(errMsg);
                }

                // EV response message and datetime parsing
                string amount = "";
                try
                {
                    amount = HelperMethod.FormatEvBalanceResponse(irisResponse.response.statusMessage);
                }
                catch (Exception ex)
                {
                    string fullMsg = $"FormatEvBalanceResponse || {irisResponse.response.statusMessage}";
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, fullMsg, ex);
                }

                // Latest EV amount updating to DB
                try
                {
                    VMItopUpStock model = new()
                    {
                        ItopUpNumber = retailer.msisdn,
                        RetailerCode = rechargeRequest.retailerCode,
                        NewBalance = Convert.ToDouble(amount),
                        UpdateTime = updateTime
                    };

                    stockService = new(Connections.RetAppDbCS);
                    int res = stockService.UpdateItopUpBalance(model);
                    if (res == 0)
                    {
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "Unable to update Retailer Balance;", null);
                    }
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, updateTime, null);
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "UpdateItopUpBalance", ex);
                }
            }

            string mailMsg = string.Empty;
            if (logStatus && !string.IsNullOrWhiteSpace(transObj.email))
            {
                EmailModelV2 emailModel = new()
                {
                    ReceiverEmail = transObj.email,
                    Subject = EmailKeys.Subject,
                    Body = string.Format(EmailKeys.Body, transObj.amount, rechargeRequest.subscriberNo, transObj.msisdn, transObj.tranNo, transObj.email),
                    IsBodyHtml = true,
                    UseDefaultCred = false,
                    EnableSsl = true
                };

                try
                {
                    await HelperMethod.SendEMail(emailModel);
                    mailMsg = " Receipt send via e-mail.";
                }
                catch (Exception ex)
                {
                    mailMsg = ex.Message;
                }
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(rechargeRequest, "IrisRecharge", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = !logStatus,
                message = irisResponse.response.statusMessage + mailMsg
            });
        }


        /// <summary>
        /// API for getting IRIS offers with/without amount. Also include admin created offers when amount available.
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="offerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(ShowAllIRISOffers))]
        public async Task<IActionResult> ShowAllIRISOffers([FromBody] IrisOfferRequestNew offerRequest)
        {
            try
            {
                string userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
                offerRequest.userAgent = userAgent;

                using (RechargeService rechargeService = new(Connections.RetAppDbCS))
                {
                    OfferResponseModelNew responseModel = await rechargeService.IRISOfferRequest(offerRequest);

                    if (!string.IsNullOrWhiteSpace(offerRequest.amount))
                    {
                        List<OfferModelNew> rechargeOffers = rechargeService.GetRechargeOffersV2(offerRequest);
                        responseModel.OfferList.AddRange(rechargeOffers);
                    }

                    if (responseModel.statusCode != "0")
                    {
                        if (string.IsNullOrEmpty(responseModel.statusMessage))
                        {
                            responseModel.statusMessage = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong);
                        }

                        return new OkObjectResult(new IRISResponseMessage()
                        {
                            isError = true,
                            isUSIM = responseModel.isUSIM,
                            message = responseModel.statusMessage,
                            data = responseModel.OfferList
                        });
                    }
                    else
                    {
                        return new OkObjectResult(new IRISResponseMessage()
                        {
                            isError = false,
                            isUSIM = responseModel.isUSIM,
                            message = responseModel.statusMessage,
                            data = responseModel.OfferList
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ShowAllIRISOffers"));
            }
        }


        /// <summary>
        /// Send All recharge offer to customer msisdn. This api is applicable from app version v6.0.0
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SendIrisOfferToCustomer")]
        public async Task<IActionResult> SendIrisOfferToCustomer([FromBody] SendRechargeOfferRequest requestModel)
        {
            if (FeatureStatus.IsEnableOfferSMSSending)
            {
                string userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
                requestModel.userAgent = userAgent;
                IrisOfferRequestNew offerRequest = new();
                requestModel.Adapt(offerRequest);

                using (RechargeService rechargeService = new(Connections.RetAppDbCS))
                {
                    OfferResponseModelNew responseModel = await rechargeService.IRISOfferRequest(offerRequest);


                    if (responseModel.statusCode != "0")
                    {
                        if (string.IsNullOrEmpty(responseModel.statusMessage))
                        {
                            responseModel.statusMessage = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong);
                        }

                        return new OkObjectResult(new IRISResponseMessage()
                        {
                            isError = true,
                            isUSIM = responseModel.isUSIM,
                            message = responseModel.statusMessage
                        });
                    }

                    Regex regex = new(@"^(.*?)(C|c)-");
                    IEnumerable<string> offerList = responseModel.OfferList.Select((o, i) => (i + 1) + "." + (regex.IsMatch(o.description) ? regex.Match(o.description).Groups[1].Value.Trim() : o.description));
                    string offers = string.Join("\n", offerList);

                    using (RetailerService NewRetailerService = new(null))
                    {
                        string result = await NewRetailerService.SendSMS(requestModel, offers);
                    }
                }

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("AmarOfferSmsSuccess", Message.AmarOfferSmsSuccess)
                });
            }
            else
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = true,
                    message = "Sending Offer temporary disable."
                });
            }
        }


        [HttpPost]
        [Route(nameof(EvPinResetHistory))]
        public async Task<IActionResult> EvPinResetHistory([FromBody] HistoryPageRequestModel requestModel)
        {
            DataTable dt = new();

            try
            {
                using (RechargeService retailerService = new())
                {
                    dt = await retailerService.GetRetailerEvPinResetHistory(requestModel);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitEvPinResetRequest"));
            }

            List<EvPinResetHistoryModel> logs = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<EvPinResetHistoryModel>(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = logs
            });
        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="rechargeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(EvRecharge))]
        public async Task<IActionResult> EvRecharge([FromBody] EVRechargeRequest rechargeRequest)
        {
            string traceMsg = string.Empty;
            StockService stockService;
            RetailerSessionCheck retailer = new();
            string loginProvider = Request.HttpContext.Items["loginProviderId"] as string;
            try
            {
                using (stockService = new())
                {
                    retailer = await stockService.CheckRetailerByCode(rechargeRequest.retailerCode, loginProvider);
                }
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


            if (string.IsNullOrEmpty(rechargeRequest.subscriberNo))
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = "Invalid Parameter: rechargeRequest.subscriberNo"
                });
            }

            string evUrl = ExternalKeys.EvURL;
            string paymentType = rechargeRequest.paymentType == (int)PaymentType.prepaid ? "EXRCTRFREQ" : "EXPPBREQ";

            RechargeService rechargeService;

            ItopUpXmlRequest xmlRequest = new()
            {
                Url = evUrl,
                Type = paymentType,
                Date = "",
                Extnwcode = "BD",
                Msisdn = retailer.msisdn,
                Pin = rechargeRequest.userPin,
                Loginid = "",
                Pass = "",
                Extcode = "",
                Extrefnum = "",
                Msisdn2 = rechargeRequest.subscriberNo,
                Amount = rechargeRequest.amount,
                Language1 = "0",
                Language2 = "1",
                Selector = "1"
            };

            EvXmlResponse evResponse = new();

            try
            {
                var userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
                using (rechargeService = new(Connections.RetAppDbCS))
                {
                    // Call the EV recharge service with the XML request and recharge request
                    // The method will return an EvXmlResponse object containing the response from the EV service
                    evResponse = rechargeService.EvRecharge(xmlRequest, rechargeRequest, "EvRecharge", userAgent);
                }
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "EvRecharge");
                throw new Exception(errMsg);
            }

            bool status = false;
            if (evResponse.txnStatus == "200")
            {
                status = true;
            }
            else
            {
                throw new Exception(evResponse.message);
            }

            string _respTranId = string.Empty;
            string trnMsg = string.Empty;
            try
            {
                trnMsg = evResponse.message.ToLower();
                string[] strList = trnMsg.Split(',');
                var resList = strList.Where(w => w.Contains("txn number") || w.Contains("transaction id")).ToList();

                if (resList.Count > 0)
                {
                    string resStr = resList.FirstOrDefault().Trim();
                    _respTranId = resStr.Replace("txn number", "").Trim();

                    if (_respTranId.Contains("transaction id"))
                    {
                        string resString = _respTranId.Replace("transaction id", "").Trim().ToUpper();
                        var index = resString.IndexOf(" ");
                        _respTranId = resString.Substring(0, index);
                    }
                    if (_respTranId.EndsWith("."))
                    {
                        _respTranId = _respTranId.Substring(0, _respTranId.Length - 1).ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "TRANNO_ParseErr", ex);

                if (!string.IsNullOrEmpty(trnMsg))
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, trnMsg, null);
            }

            TransactionLogVM transObj = new()
            {
                rCode = rechargeRequest.retailerCode,
                tranNo = evResponse.txnId,
                tranType = rechargeRequest.paymentType == (int)PaymentType.prepaid ? "ITOP'UP" : "Bill Pay",
                amount = Convert.ToInt64(rechargeRequest.amount) / 100,
                tranDate = DateTime.Now,
                msisdn = rechargeRequest.subscriberNo.Length == 11 ? "88" + rechargeRequest.subscriberNo : rechargeRequest.subscriberNo,
                rechargeType = (int)RechargeType.EvRecharge,
                email = rechargeRequest.email,
                isTranSuccess = status == true ? 1 : 0,
                tranMsg = evResponse.message,
                retMsisdn = "0" + retailer.msisdn,
                loginProvider = loginProvider,
                respTranId = _respTranId,
                lat = rechargeRequest.lat,
                lng = rechargeRequest.lng,
                ipAddress = HelperMethod.GetIPAddress()
            };

            bool logStatus = false;

            if (status)
            {
                try
                {
                    using (rechargeService = new(Connections.RetAppDbCS))
                    {
                        logStatus = await rechargeService.SaveTransactionLog(transObj);
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "SaveTransactionLog");
                    throw new Exception(errMsg);
                }

                LMSPointAdjustReq pointAdjustReq = new()
                {
                    requestMethod = "EvRecharge",
                    retailerCode = rechargeRequest.retailerCode,
                    appPage = LMSAppPages.EV_Recharge,
                    transactionID = LMSService.GetTransactionId(rechargeRequest.retailerCode),
                    msisdn = "880" + retailer.msisdn,
                    points = LMSPoints.EV_Recharge.ToString(),
                    adjustmentType = nameof(LmsAdjustmentType.CREDIT)
                };

                using (LMSService lmsService = new())
                {
                    await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);
                }


                // EV response message and datetime parsing
                string amount = "", updateTime = "";
                try
                {
                    amount = HelperMethod.FormatEvBalanceResponse(evResponse.message);

                    DateTime _dateTime = DateTime.ParseExact(evResponse.date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    updateTime = _dateTime.ToEnUSDateString("hh:mm:ss tt, dd MMM yyyy");
                }
                catch (Exception ex)
                {
                    string fullMsg = $"FormatEvBalanceResponse || {evResponse.message}";
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, fullMsg, ex);
                }

                // Latest EV amount updating to DB
                try
                {
                    VMItopUpStock model = new()
                    {
                        ItopUpNumber = retailer.msisdn,
                        RetailerCode = rechargeRequest.retailerCode,
                        NewBalance = Convert.ToDouble(amount),
                        UpdateTime = updateTime
                    };

                    int res;
                    using(stockService = new(Connections.RetAppDbCS))
                    {
                        // Update the retailer's iTopUp balance with the new amount and update time
                        res = stockService.UpdateItopUpBalance(model);
                    }

                    if (res == 0)
                    {
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "Unable to update Retailer Balance;", null);
                    }
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, updateTime, null);
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "UpdateItopUpBalance", ex);
                }
            }

            string mailMsg = string.Empty;
            if (logStatus && !string.IsNullOrWhiteSpace(transObj.email))
            {
                EmailModelV2 emailModel = new()
                {
                    ReceiverEmail = transObj.email,
                    Subject = EmailKeys.Subject,
                    Body = string.Format(EmailKeys.Body, transObj.amount, rechargeRequest.subscriberNo, transObj.msisdn, transObj.tranNo, transObj.email),
                    IsBodyHtml = true,
                    UseDefaultCred = false,
                    EnableSsl = true
                };

                try
                {
                    await HelperMethod.SendEMail(emailModel);
                    mailMsg = SharedResource.GetLocal("ReceiptSendViaEmail", Message.ReceiptSendViaEmail);
                }
                catch (Exception ex)
                {
                    mailMsg = ex.Message;
                }
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(rechargeRequest, "EvRecharge", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = !logStatus,
                message = evResponse.message + mailMsg,
                data = Array.Empty<string>()
            });
        }


        /// <summary>
        /// This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="rechargeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(MultiEvRecharge))]
        public async Task<IActionResult> MultiEvRecharge([FromBody] EVRechargeRequest rechargeRequest)
        {
            string traceMsg = string.Empty;
            StockService stockService;
            RetailerSessionCheck retailer = new();
            string loginProvider = Request.HttpContext.Items["loginProviderId"] as string;

            try
            {
                using (stockService = new())
                {
                    retailer = await stockService.CheckRetailerByCode(rechargeRequest.retailerCode, loginProvider);
                }
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

            string evUrl = ExternalKeys.EvURL;

            RechargeService rechargeService;
            ItopUpXmlRequest xmlRequest = new()
            {
                Url = evUrl,
                Type = "",
                Date = "",
                Extnwcode = "BD",
                Msisdn = retailer.msisdn,
                Pin = rechargeRequest.userPin,
                Loginid = "",
                Pass = "",
                Extcode = "",
                Extrefnum = "",
                Msisdn2 = rechargeRequest.subscriberNo,
                Amount = rechargeRequest.amount,
                Language1 = "0",
                Language2 = "1",
                Selector = "1"
            };

            string respMesg = "", respDateTime = "";
            foreach (var recharge in rechargeRequest.rechargeList)
            {
                ItopUpXmlRequest xmlRequestNew = new();
                xmlRequestNew = xmlRequest;
                xmlRequestNew.Amount = recharge.amount;
                xmlRequestNew.Msisdn2 = recharge.msisdn;
                xmlRequestNew.Type = recharge.paymentType == (int)PaymentType.prepaid ? "EXRCTRFREQ" : "EXPPBREQ";

                rechargeRequest.amount = recharge.amount;

                EvXmlResponse evXmlResponse = new();

                try
                {
                    var userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
                    using (rechargeService = new(Connections.RetAppDbCS))
                    {
                        evXmlResponse = rechargeService.EvRecharge(xmlRequestNew, rechargeRequest, "MultiEvRecharge", userAgent);
                    }
                    respMesg = evXmlResponse.message;
                    respDateTime = evXmlResponse.date;
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "EvRecharge", ex);
                }

                recharge.isSuccess = false;
                if (evXmlResponse.txnStatus == "200")
                {
                    recharge.isSuccess = true;
                }

                string _respTranId = string.Empty;
                string trnMsg = string.Empty;
                try
                {
                    trnMsg = evXmlResponse.message.ToLower();
                    string[] strList = trnMsg.Split(',');
                    var resList = strList.Where(w => w.Contains("txn number") || w.Contains("transaction id")).ToList();

                    if (resList.Count > 0)
                    {
                        string resStr = resList.FirstOrDefault().Trim();
                        _respTranId = resStr.Replace("txn number", "").Trim();

                        if (_respTranId.Contains("transaction id"))
                        {
                            string resString = _respTranId.Replace("transaction id", "").Trim().ToUpper();
                            var index = resString.IndexOf(" ");
                            _respTranId = resString.Substring(0, index);
                        }
                        if (_respTranId.EndsWith("."))
                        {
                            _respTranId = _respTranId.Substring(0, _respTranId.Length - 1).ToUpper();
                        }
                    }
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "TRANNO_ParseErr", ex);

                    if (!string.IsNullOrEmpty(trnMsg))
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, trnMsg, null);
                }

                TransactionLogVM transObj = new()
                {
                    rCode = rechargeRequest.retailerCode,
                    tranNo = evXmlResponse.txnId,
                    tranType = recharge.paymentType == (int)PaymentType.prepaid ? "ITOP'UP" : "Bill Pay",
                    amount = Convert.ToInt64(recharge.amount) / 100,
                    tranDate = DateTime.Now,
                    msisdn = recharge.msisdn.Length == 11 ? "88" + recharge.msisdn : recharge.msisdn,
                    rechargeType = (int)RechargeType.EvRecharge,
                    isTranSuccess = recharge.isSuccess == true ? 1 : 0,
                    tranMsg = evXmlResponse.message,
                    retMsisdn = '0' + xmlRequest.Msisdn,
                    loginProvider = loginProvider,
                    respTranId = _respTranId,
                    lat = rechargeRequest.lat,
                    lng = rechargeRequest.lng,
                    ipAddress = HelperMethod.GetIPAddress()
                };

                if (recharge.isSuccess)
                {
                    bool logStatus = false;
                    try
                    {
                        using (rechargeService = new(Connections.RetAppDbCS))
                        {
                            logStatus = await rechargeService.SaveTransactionLog(transObj);
                        }
                        if (!logStatus) { break; }
                    }
                    catch (Exception ex)
                    {
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "SaveTransactionLog", ex);
                    }
                };
            }

            List<RechargeInfo> rechargeInfo = rechargeRequest.rechargeList;
            bool success = rechargeInfo.Where(f => f.isSuccess == true).ToList().Count() >= 1;

            if (success)
            {
                LMSPointAdjustReq pointAdjustReq = new()
                {
                    requestMethod = "MultiEvRecharge",
                    retailerCode = rechargeRequest.retailerCode,
                    appPage = LMSAppPages.Multi_EV_Recharge,
                    transactionID = LMSService.GetTransactionId(rechargeRequest.retailerCode),
                    msisdn = "880" + retailer.msisdn,
                    points = LMSPoints.EV_Recharge.ToString(),
                    adjustmentType = nameof(LmsAdjustmentType.CREDIT)
                };

                using (LMSService lmsService = new())
                {
                    await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);
                }

                // EV response message and datetime parsing
                string amount = "", updateTime = "";
                try
                {
                    amount = HelperMethod.FormatEvBalanceResponse(respMesg);

                    DateTime _dateTime = DateTime.ParseExact(respDateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    updateTime = _dateTime.ToEnUSDateString("hh:mm:ss tt, dd MMM yyyy");
                }
                catch (Exception ex)
                {
                    string fullMsg = $"FormatEvBalanceResponse || {respMesg}";
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, fullMsg, ex);
                }

                // Latest EV amount updating to DB
                try
                {
                    VMItopUpStock model = new()
                    {
                        ItopUpNumber = retailer.msisdn,
                        RetailerCode = rechargeRequest.retailerCode,
                        NewBalance = Convert.ToDouble(amount),
                        UpdateTime = updateTime
                    };

                    int res;
                    using (stockService = new(Connections.RetAppDbCS))
                    {
                        res = stockService.UpdateItopUpBalance(model);
                    }
                    if (res == 0)
                    {
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "Unable to update Retailer Balance;", null);
                    }
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, updateTime, null);
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "UpdateItopUpBalance", ex);
                }
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(rechargeRequest, "MultiEvRecharge", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = !success,
                message = success ? SharedResource.GetLocal("Success", Message.Success) : SharedResource.GetLocal("Failed", Message.Failed),
                data = rechargeInfo
            });

        }


        [HttpPost]
        [Route(nameof(ResetEVPinReq))]
        public async Task<IActionResult> ResetEVPinReq([FromBody] EvPinResetRequest reqModel)
        {
            ExternalSubmitResponse respMessage = new();
            RechargeService rechargeService;

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "ResetEVPinReq",
                retailerCode = reqModel.retailerCode,
                appPage = LMSAppPages.EV_PIN_Reset,
                transactionID = LMSService.GetTransactionId(reqModel.retailerCode),
                msisdn = $"88{reqModel.iTopUpNumber}",
                points = LMSPoints.EV_PIN_Reset_Or_Change.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            reqModel.status = 0;
            rechargeService = new();
            long insertedId = await rechargeService.SaveResetEVPinReqLog(reqModel);

            if (insertedId > 0)
            {
                reqModel.reqID = insertedId;
                string evPinResetUrl = ExternalKeys.EvPinResetUrl;

                EvPinResetReqModel externalModel = new()
                {
                    resetReqId = reqModel.reqID,
                    retailerCode = reqModel.retailerCode,
                    iTopUpNumber = reqModel.iTopUpNumber.Substring(1),
                    pinResetReason = reqModel.pinResetReason,
                    userName = ExternalKeys.RsoUser,
                    password = ExternalKeys.RsoCred
                };

                try
                {
                    HttpRequestModel httpModel = new()
                    {
                        requestBody = externalModel,
                        requestUrl = evPinResetUrl,
                        requestMediaType = MimeTypes.Json,
                        requestMethod = "ResetEVPinReq"
                    };

                    HttpService httpService = new();
                    respMessage = await httpService.SubmitExternalRequest(httpModel);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitExternalRequest"));
                }
            }

            if (respMessage.success)
            {
                EVPinResetStatusRequest updateModel = new()
                {
                    userName = ExternalKeys.RsoUser,
                    resetReqId = insertedId,
                    iTopUpNumber = reqModel.iTopUpNumber,
                    status = 1,
                    userId = UserSession.userId
                };

                rechargeService = new();
                var result = await rechargeService.UpdateEVPinStatus(updateModel);

                if (result.Item1)
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = false,
                        message = respMessage.message,
                    });
                }
            }
            else
            {
                reqModel.reqID = insertedId;

                EVPinResetStatusRequest updateModel = new()
                {
                    userName = ExternalKeys.InternalUser,
                    resetReqId = insertedId,
                    iTopUpNumber = reqModel.iTopUpNumber,
                    status = -1,
                    userId = UserSession.userId
                };

                rechargeService = new();
                var result = await rechargeService.UpdateEVPinStatus(updateModel);

                if (result.Item1)
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = respMessage.message,
                    });
                }
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = (respMessage.success == true && respMessage.statusCode == 200) ? SharedResource.GetLocal("EVPinResetReqSent", Message.Success) : respMessage.message,
            });

        }


        [HttpPost]
        [Route(nameof(ChangeEVPin))]
        public async Task<IActionResult> ChangeEVPin([FromBody] ChangeEvPinRequest model)
        {
            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "ResetEVPinReq",
                retailerCode = model.retailerCode,
                appPage = LMSAppPages.EV_PIN_Change,
                transactionID = LMSService.GetTransactionId(model.retailerCode),
                msisdn = $"88{model.iTopUpNumber}",
                points = LMSPoints.EV_PIN_Reset_Or_Change.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            using(LMSService lmsService = new())
            {
                await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);
            }

            string evPinChangeURL = ExternalKeys.EvPinChangeUrl;

            EvPinChangeXMLRequest xmlRequest = new()
            {
                url = evPinChangeURL,
                type = "EXC2SCPNREQ",
                extnwCode = "BD",
                msisdn = model.iTopUpNumber.Substring(1),
                oldPin = model.temporaryPin,
                newPin = model.newPin,
                extrefnum = "EVPinChange",
                remarks = "EvPinChangeByRetailer"
            };

            EvPinChangeXMLResponse evPinChangeResponse = new();
            DateTime changeDate = DateTime.Now;
            RechargeService rechargeService;

            try
            {
                var userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
                using(rechargeService = new())
                {
                    evPinChangeResponse = await rechargeService.SubmitEvPinChangeReq(xmlRequest, userAgent);
                }
                changeDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitEvPinChangeReq"));
            }

            bool status = evPinChangeResponse.txnStatus == "200";
            string message = status ? SharedResource.GetLocal("Success", Message.Success) : evPinChangeResponse.message;

            if (status)
            {
                // Update pin change successfull time in ev pin log table
                try
                {
                    using(rechargeService = new())
                    {
                        await rechargeService.UpdateEvPinResetSuccessDate(model, changeDate);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateEvPinResetSuccessDate"));
                }
            }

            return Ok(new ResponseMessage()
            {
                isError = !status,
                message = message,
            });
        }

    }
}