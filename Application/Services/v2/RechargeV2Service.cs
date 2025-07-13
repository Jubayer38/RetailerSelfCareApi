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

using Application.Utils;
using Domain.Helpers;
using Domain.RequestModel;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Domain.ViewModel.LogModels;
using Infrastracture.Repositories.v2;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using static Domain.Enums.EnumCollections;

namespace Application.Services.v2
{
    public class RechargeV2Service : IDisposable
    {
        private readonly RechargeV2Repository _repo;

        public RechargeV2Service()
        {
            _repo = new();
        }

        public RechargeV2Service(string connectionString)
        {
            _repo = new(connectionString);
        }


        #region==========|  Dispose Method  |==========
        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                _repo.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========


        public async Task<DataTable> GetRechargeOffers(OfferRequest request)
        {
            try
            {
                var result = await _repo.GetRechargeOffers(request);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "v2/GetRechargeOffers"));
            }
        }


        public EvXmlResponse EvRecharge(ItopUpXmlRequest xmlRequest, EVRechargeRequest request, string methodName, string userAgent)
        {
            LoggerService loggerService = new();
            EvLogViewModel evLog = new();
            evLog = HelperMethod.LogModelBind(xmlRequest, evLog, methodName, userAgent);

            string xmlReq = "";
            string responseXML = "";

            try
            {
                xmlReq = XMLService.GetItopUpReqXML(xmlRequest);

                RechargeRequestVM evRechargeXmlVM = new()
                {
                    RetailerCode = request.retailerCode,
                    RequestBody = xmlReq,
                    RequestUrl = xmlRequest.Url,
                    OriginMethodName = methodName
                };

                responseXML = XMLService.PostXMLDataV2(evRechargeXmlVM);
                EvXmlResponse evXmlResponse = (EvXmlResponse)XMLService.ParseXML(responseXML, typeof(EvXmlResponse));

                bool isTranSuccess = CheckEVSuccessResp(evXmlResponse);
                evLog.isTranSuccess = Convert.ToInt32(isTranSuccess);

                evLog.tranMsg = HelperMethod.GetRespMsgWithLengthLimit(evXmlResponse.message, 1000);
                if (!isTranSuccess)
                {
                    string message = EvIrisMessageParsing.ParseMessage(evXmlResponse.message);
                    evXmlResponse.message = message;
                }

                return evXmlResponse;
            }
            catch (Exception ex)
            {
                evLog.isSuccess = 0;
                evLog.isTranSuccess = 0;
                evLog.errorMessage = HelperMethod.ExMsgSubString(ex, "v2/EvRecharge");
                string errMsg = HelperMethod.GetRespMsgWithLengthLimit(ex, 1000);
                return new EvXmlResponse() { message = errMsg };
            }
            finally
            {
                #region================|| LOG ================
                evLog.reqBodyStr = xmlReq;
                evLog.resBodyStr = responseXML;

                if (TextLogging.IsEnableEVTextLog)
                {
                    evLog.endTime = DateTime.Now;
                    loggerService.WriteEVLogInText(evLog);

                }
                #endregion================ || LOG || ================
            }
        }


        public async Task<bool> SaveTransactionLog(TransactionLogVM log)
        {
            return await _repo.SaveTransactionLog(log);
        }


        public async Task<IrisRechargeResponse> IrisRechargeRequest(string irisUrl, IrisContinueRechargeRequest irisRechargeRequest, IrisRechargeRequest apiRequest, string methodName, string userAgent)
        {
            IRISLogViewModel irisLog = new();
            irisLog = HelperMethod.LogModelBind(irisRechargeRequest, irisLog, methodName, userAgent);
            IrisRechargeResponse irisResponse = new();

            try
            {
                RechargeRequestVM rechargeXmlVM = new()
                {
                    RetailerCode = apiRequest.retailerCode,
                    RequestBody = irisRechargeRequest.ToJsonString(),
                    RequestUrl = irisUrl,
                    OriginMethodName = methodName
                };

                using (HttpService httpService = new())
                {
                    irisResponse = await httpService.IRISRechargeRequest(rechargeXmlVM);
                }

                if (irisResponse?.response?.statusCode is "0")
                {
                    irisLog.isTranSuccess = 1;
                    irisLog.isSuccess = 1;

                    string irisRespMsg = irisResponse.response.statusMessage;
                    irisLog.tranMsg = GetRespMsgWithLengthLimit(irisRespMsg, 500);
                }
                else
                {
                    string respMsg = irisResponse?.response?.statusMessage;
                    respMsg = string.IsNullOrWhiteSpace(respMsg) ? Message.NoResponse : respMsg;
                    string parsedMsg = EvIrisMessageParsing.ParseMessage(respMsg);

                    if (irisResponse?.response is not null)
                    {
                        irisResponse.response.statusMessage = parsedMsg;
                    }

                    irisLog.tranMsg = respMsg;
                    throw new Exception(parsedMsg);
                }
            }
            catch (Exception ex)
            {
                irisLog.isSuccess = 0;
                irisLog.errorMessage = HelperMethod.ExMsgSubString(ex, methodName, 256);
                irisLog.isTranSuccess = 0;
            }
            finally
            {
                #region==========================|| LOG ||=============================
                irisLog.resBodyStr = irisResponse.ToJsonString();

                if (TextLogging.IsEnableIrisTextLog)
                {
                    irisLog.endTime = DateTime.Now;
                    LoggerService.WriteIRISLogInText(irisLog);
                }
                #endregion================================================================
            }

            return irisResponse;
        }


        public async Task<OfferResponseModelNew> IRISOfferRequest(IrisOfferRequestNew irisOffer)
        {
            IRISJSONRequestModel reqJSON = new();

            string nowDateTimeStr = DateTime.Now.ToEnUSDateString(".yyyyMMdd.HHmmssffffff");
            IRISOfferRequest irisRequest = new()
            {
                username = ExternalKeys.IrisUsername,
                password = ExternalKeys.IrisCred,
                retailerMsisdn = irisOffer.iTopUpNumber.Substring(1),
                subscriberMsisdn = irisOffer.subscriberNo.Substring(1),
                channel = ExternalKeys.Irischannel,
                gatewayCode = ExternalKeys.IrisGatewayCode,
                transactionID = irisOffer.retailerCode.Substring(1) + nowDateTimeStr,
                rechargeAmount = irisOffer.amount
            };
            reqJSON.request = irisRequest;

            string irisUrl = ExternalKeys.IrisUrl + "getDigitalOffer";

            try
            {
                OfferResponseModelNew irisOffersModel = await GetIRISOffers(reqJSON, irisUrl, irisOffer);
                return irisOffersModel;
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "v2/IRISOfferRequest");
                throw new Exception(errMsg);
            }
        }


        public async Task<EvPinChangeXMLResponse> SubmitEvPinChangeReq(EvPinChangeXMLRequest model, string userAgent)
        {
            LoggerService loggerService = new();
            EvLogViewModel evLog = new();
            evLog = HelperMethod.LogModelBind(model, evLog, "v2/SubmitEvPinChangeReq", userAgent);
            string xmlReq = string.Empty;
            string responseXML = string.Empty;

            try
            {
                EvPinChangeXMLResponse evPinChangeResponse = new();

                xmlReq = XMLService.GetEvPinChangeReqXML(model);
                responseXML = await XMLService.PostXMLDataAsync(model.url, xmlReq);
                evPinChangeResponse = (EvPinChangeXMLResponse)XMLService.ParseXML(responseXML, typeof(EvPinChangeXMLResponse));

                evLog.isTranSuccess = evPinChangeResponse.txnStatus == "200" ? 1 : 0;
                evLog.tranMsg = evPinChangeResponse.message;
                evPinChangeResponse.message = EvIrisMessageParsing.ParseMessage(evPinChangeResponse.message);

                return evPinChangeResponse;
            }
            catch (Exception ex)
            {
                evLog.isSuccess = 0;
                evLog.errorMessage = HelperMethod.ExMsgSubString(ex, "v2/SubmitEvPinChangeReq", 256);

                evLog.isTranSuccess = 0;
                string errMsg = HelperMethod.GetRespMsgWithLengthLimit(ex, 500);
                return new EvPinChangeXMLResponse() { message = errMsg };
            }
            finally
            {
                evLog.retMSISDN = model.msisdn;
                evLog.reqBodyStr = xmlReq;
                evLog.resBodyStr = responseXML;

                if (TextLogging.IsEnableEVTextLog)
                {
                    evLog.endTime = DateTime.Now;
                    loggerService.WriteEVLogInText(evLog);
                }
            }
        }


        public async Task<(bool, string)> UpdateEVPinStatus(EVPinResetStatusRequest model)
        {
            try
            {
                return await _repo.UpdateEVPinStatus(model);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.FormattedExceptionMsg(ex);
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(model, "v2/UpdateEVPinStatus", HelperMethod.FormattedExceptionMsg(ex));

                return (false, errMsg);
            }
        }


        public async Task<DataTable> GetRetailerEvPinResetHistory(HistoryPageRequestModel reqModel)
        {
            return await _repo.GetRetailerEvPinResetHistory(reqModel);
        }


        public async Task<long> SaveResetEVPinReqLog(EvPinResetRequest evPinReset)
        {
            return await _repo.SaveResetEVPinReqLog(evPinReset);
        }


        public async Task UpdateEvPinResetSuccessDate(ChangeEvPinRequest model, DateTime changeDate)
        {
            await _repo.UpdateEvPinResetSuccessDate(model, changeDate);
        }


        public async Task<InternalResponse> DigitalServiceEvRecharge(DigitalServiceSubmitRequest dsRequest, string userAgent)
        {
            string traceMsg = string.Empty;
            if (dsRequest.paymentType == 0) dsRequest.paymentType = 1;

            EVRechargeRequest rechargeRequest = new()
            {
                sessionToken = dsRequest.sessionToken,
                retailerCode = dsRequest.retailerCode,
                amount = dsRequest.amount,
                subscriberNo = dsRequest.subscriberNumber,
                userPin = dsRequest.userPin,
                paymentType = dsRequest.paymentType
            };

            StockV2Service stockService;
            DataTable retailer = new();
            try
            {
                using (stockService = new())
                {
                    retailer = await stockService.GetRetailerByCode(rechargeRequest.retailerCode);
                }
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "v2/GetRetailerByCode");
                throw new Exception(errMsg);
            }

            string retMsisdn;
            using (stockService = new())
            {
                retMsisdn = stockService.GetRetailerMSISDN(retailer);
            }
            string evUrl = ExternalKeys.EvURL;
            string paymentType = rechargeRequest.paymentType == (int)PaymentType.prepaid ? "EXRCTRFREQ" : "EXPPBREQ";

            RechargeV2Service rechargeService;

            ItopUpXmlRequest xmlRequest = new()
            {
                Url = evUrl,
                Type = paymentType,
                Date = "",
                Extnwcode = "BD",
                Msisdn = retMsisdn,
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
                using(rechargeService = new())
                {
                    evResponse = rechargeService.EvRecharge(xmlRequest, rechargeRequest, "DigitalServiceEvRecharge", userAgent);
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

            string loginProvider = new HttpContextAccessor().HttpContext.Items["loginProviderId"] as string;
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
                retMsisdn = "0" + retMsisdn,
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
                    using(rechargeService = new())
                    {
                        logStatus = await rechargeService.SaveTransactionLog(transObj);
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "SaveTransactionLog");
                    throw new Exception(errMsg);
                }

                // EV response message and datetime parsing
                string amount = "", updateTime = "";
                try
                {
                    stockService = new();
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
                        ItopUpNumber = retMsisdn,
                        RetailerCode = rechargeRequest.retailerCode,
                        NewBalance = Convert.ToDouble(amount),
                        UpdateTime = updateTime
                    };

                    int res;
                    using (stockService = new())
                    {
                        res = await stockService.UpdateItopUpBalance(model);
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
                _logger.WriteTraceMessageInText(dsRequest, "DigitalServiceEvRecharge", traceMsg);
            }

            return new InternalResponse()
            {
                isSuccess = true,
                message = traceMsg
            };
        }


        /// <summary>
        /// This method is applicable from app version v6.4.0
        /// </summary>
        /// <param name="irisOffer"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<OfferResponseModelNew> IRISOfferRequestV2(IrisOfferRequestNew irisOffer)
        {
            IRISJSONRequestModel reqJSON = new();

            string nowDateTimeStr = DateTime.Now.ToEnUSDateString(".yyyyMMdd.HHmmssffffff");
            IRISOfferRequest irisRequest = new()
            {
                username = ExternalKeys.IrisUsername,
                password = ExternalKeys.IrisCred,
                retailerMsisdn = irisOffer.iTopUpNumber.Substring(1),
                subscriberMsisdn = irisOffer.subscriberNo.Substring(1),
                channel = ExternalKeys.Irischannel,
                gatewayCode = ExternalKeys.IrisGatewayCode,
                transactionID = irisOffer.retailerCode.Substring(1) + nowDateTimeStr,
                rechargeAmount = irisOffer.amount
            };
            reqJSON.request = irisRequest;

            string irisUrl = ExternalKeys.IrisUrl + "getDigitalOffer";

            try
            {
                OfferResponseModelNew irisOffersModel = await GetIRISOffersV2(reqJSON, irisUrl, irisOffer);
                return irisOffersModel;
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "v2/IRISOfferRequestV2");
                throw new Exception(errMsg);
            }
        }


        public async Task<bool> StarTrekStatusCheck(StarTrekStatusCheckRequest model)
        {
            string msisdn = model.subscriberNumber.Prepend("88");
            string url = string.Format(ExternalKeys.StarTrekUrl, msisdn);
            bool isStartTrek = false;

            HttpRequestModel httpModel = new()
            {
                requestBody = model,
                requestUrl = url,
                requestMethod = "StarTrekStatusCheck"
            };

            StarTrekResponse resp;
            using (HttpService httpService = new())
            {
                resp = await httpService.StarTrekStatusCheckingRequest(httpModel);
            }
            if (resp?.included.Count <= 0)
                return isStartTrek;

            var code = resp?.included[0]?.attributes?.code ?? string.Empty;
            var starTrekFlagsSet = new HashSet<string>(ExternalKeys.StarTrekFlag.Split(','), StringComparer.OrdinalIgnoreCase);

            //Regex regex = new(@"(startrek|\*)", RegexOptions.IgnoreCase);
            //Match starTrekMatch = regex.Match(code);
            if (starTrekFlagsSet.Contains(code))
                isStartTrek = true;

            return isStartTrek;
        }


        #region================ || Private Methods || ================

        private static async Task<OfferResponseModelNew> GetIRISOffers(IRISJSONRequestModel irisRequestParam, string irisUrl, IrisOfferRequestNew offerRequest)
        {
            IRISLogViewModel irisLog = new();
            string thisMethodName = "v2/GetIRISOffers";
            irisLog = HelperMethod.LogModelBind(irisRequestParam, irisLog, thisMethodName, offerRequest.userAgent);
            IRISResponseModel irisResponse = new();
            OfferResponseModelNew irisOfferResponse = new();

            try
            {
                RechargeRequestVM rechargeXmlVM = new()
                {
                    RetailerCode = offerRequest.retailerCode,
                    RequestBody = irisRequestParam.ToJsonString(),
                    RequestUrl = irisUrl,
                    OriginMethodName = thisMethodName
                };

                using(HttpService httpService = new())
                {
                    irisResponse = await httpService.GetIRISOffers(rechargeXmlVM);
                }

                if (irisResponse?.response?.statusCode == "0")
                {
                    List<IRISOfferResponseList> irisOfferList = JsonConvert.DeserializeObject<List<IRISOfferResponseList>>(irisResponse.response.OffersList)!;
                    irisOfferResponse = FormatIRISOfferData(irisOfferList, irisResponse);

                    return irisOfferResponse;
                }
                else
                {
                    string responseMsg = string.Empty;
                    if (irisResponse == null || irisResponse.response == null)
                    {
                        responseMsg = SharedResource.GetLocal("NoAmarOfferAvailable", Message.NoAmarOfferAvailable);
                    }
                    else
                    {
                        responseMsg = irisResponse.response.statusMessage;
                    }

                    string message = EvIrisMessageParsing.ParseMessage(responseMsg);

                    irisOfferResponse = new OfferResponseModelNew
                    {
                        statusCode = irisResponse?.response?.statusCode,
                        statusMessage = message,
                        transactionId = irisResponse?.response?.transactionId,
                    };

                    return irisOfferResponse;
                }
            }
            catch (Exception ex)
            {
                irisLog.isSuccess = 0;
                irisLog.errorMessage = HelperMethod.ExMsgSubString(ex, thisMethodName, 256);
                irisOfferResponse.statusMessage = HelperMethod.ExMsgSubString(ex, "", 256);

                return irisOfferResponse;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                irisLog.resBodyStr = irisResponse.ToJsonString();

                if (TextLogging.IsEnableIrisTextLog)
                {
                    irisLog.endTime = DateTime.Now;
                    LoggerService.WriteIRISLogInText(irisLog);
                }
                #endregion
            }
        }


        private static OfferResponseModelNew FormatIRISOfferData(List<IRISOfferResponseList> irisResponseList, IRISResponseModel irisResponse)
        {
            List<OfferModelNew> irisViewModels = [];

            OfferResponseModelNew iRISOfferResponse = new()
            {
                statusCode = irisResponse.response.statusCode,
                statusMessage = irisResponse.response.statusMessage,
                transactionId = irisResponse.response.transactionId,
                OfferList = irisViewModels
            };

            foreach (var iris in irisResponseList)
            {
                if (iris.offerName.ToLower().Contains("pop up") && iris.offerCommission == "0" && iris.rechargeAmount == "0")
                {
                    iRISOfferResponse.isUSIM = true;
                    iRISOfferResponse.statusMessage = iris.offerDisplayName;
                }
                else if (string.IsNullOrWhiteSpace(iris.offerDisplayName))
                    continue;
                else if (!iris.offerDisplayName.Any(char.IsDigit))
                    continue;
                else
                {
                    OfferModelNew formatedModel = IRISOfferModelMaker(iris, irisResponse.response.transactionId);
                    irisViewModels.Add(formatedModel);
                }
            }

            return iRISOfferResponse;
        }


        private static OfferModelNew IRISOfferModelMaker(IRISOfferResponseList iris, string tranId)
        {
            string ussd = iris.sno.ToString();
            string offerId = iris.offerID;
            bool asteriskContains = false;

            string offerDisplayName = iris.offerDisplayName.ToLower();
            string[] irisSplit = offerDisplayName.Contains("default") ? offerDisplayName.Split([' '], 2) : offerDisplayName.Split([' '], 3);
            string offer = irisSplit[0];

            //Regex regex = new(@"(\*)(\d+)(gb|mb)");
            // Check star/* in offer string. Implement at 29-Jan-2024
            Regex regex = new(@"(star|\*)", RegexOptions.IgnoreCase);
            Match starMatch = regex.Match(offer);
            if (starMatch.Success)
                asteriskContains = true;

            try
            {
                offer = Regex.Replace(offer, "[&#;$]", string.Empty, RegexOptions.IgnoreCase);
                offer = (offer.Substring(0, 1) == "*" && offer.Length > 0) ? offer.Substring(1, offer.Length - 1) : offer;
                offer = (offer.Substring(offer.Length - 2) == "10") ? offer.Substring(0, offer.Length - 2) : offer;
            }
            catch (Exception ex)
            {
                offer = HelperMethod.SubStrString(ex.Message, 50);
            }

            bool isRateCutter = false;
            string _offerType = string.Empty;
            string amount = IrisOfferParsing.ParseAmount(offer);
            var voiceParseResult = IrisOfferParsing.ParseIRISVoiceOffer(offer, ref isRateCutter);
            _offerType = voiceParseResult.Item2;

            string sms = IrisOfferParsing.ParseSMS(offer);
            _offerType = sms is not "0" ? " SMS" : _offerType;

            IrisDataOfferParseVM dataOffer = IrisOfferParsing.ParseIRISDataOffer(offer);
            _offerType = string.IsNullOrWhiteSpace(dataOffer.offerType) ? _offerType : dataOffer.offerType;

            string validity = IrisOfferParsing.ParseValidity(offer);
            string commission = IrisOfferParsing.ParseCommissionAmount(offerDisplayName);

            _ = int.TryParse(amount, out int _amount);
            _ = int.TryParse(commission, out int _commission);

            OfferModelNew offerData = new()
            {
                ussdCode = ussd,
                description = iris.offerDisplayName,
                amount = _amount,
                commission = _commission,
                tranId = tranId,
                dataPack = dataOffer.dataPack,
                perDayDataPack = dataOffer.perDayData,
                streamingPack = dataOffer.streamingPack,
                talkTime = voiceParseResult.Item1,
                sms = sms,
                toffee = dataOffer.toffee,
                validity = validity,
                offerId = offerId,
                hasStar = asteriskContains,
                offerType = "IRIS" + _offerType,
                hasDataPack = dataOffer.dataPack is not "0",
                hasVoicePack = voiceParseResult.Item1 is not "0",
                hasToffePack = dataOffer.toffee is not "0",
                hasRateCutterPack = isRateCutter,
                hasSMSPack = sms is not "0",
                hasStreamingPack = dataOffer.hasStreamingPack
            };

            offerData.SetOfferYype();
            return offerData;
        }


        private static bool CheckEVSuccessResp(EvXmlResponse resp)
        {
            bool isSuccess = false;

            if (resp != null)
            {
                if (resp.txnStatus.Equals("200"))
                {
                    isSuccess = true;
                }
            }

            return isSuccess;
        }


        private static string GetRespMsgWithLengthLimit(string msg, int stringLenLimit)
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


        private static async Task<OfferResponseModelNew> GetIRISOffersV2(IRISJSONRequestModel irisRequestParam, string irisUrl, IrisOfferRequestNew offerRequest)
        {
            IRISLogViewModel irisLog = new();
            string thisMethodName = "v2/GetIRISOffersV2";
            irisLog = HelperMethod.LogModelBind(irisRequestParam, irisLog, thisMethodName, offerRequest.userAgent);
            IRISResponseModel irisResponse = new();
            OfferResponseModelNew irisOfferResponse = new();

            try
            {
                RechargeRequestVM rechargeXmlVM = new()
                {
                    RetailerCode = offerRequest.retailerCode,
                    RequestBody = irisRequestParam.ToJsonString(),
                    RequestUrl = irisUrl,
                    OriginMethodName = thisMethodName
                };

                using(HttpService httpService = new())
                {
                    irisResponse = await httpService.GetIRISOffers(rechargeXmlVM);
                }

                if (irisResponse?.response?.statusCode == "0")
                {
                    List<IRISOfferResponseList> irisOfferList = JsonConvert.DeserializeObject<List<IRISOfferResponseList>>(irisResponse.response.OffersList)!;
                    irisOfferResponse = FormatIRISOfferDataV2(irisOfferList, irisResponse);

                    return irisOfferResponse;
                }
                else
                {
                    string responseMsg = string.Empty;
                    if (irisResponse == null || irisResponse.response == null)
                    {
                        responseMsg = SharedResource.GetLocal("NoAmarOfferAvailable", Message.NoAmarOfferAvailable);
                    }
                    else
                    {
                        responseMsg = irisResponse.response.statusMessage;
                    }

                    string message = EvIrisMessageParsing.ParseMessage(responseMsg);

                    irisOfferResponse = new OfferResponseModelNew
                    {
                        statusCode = irisResponse?.response?.statusCode,
                        statusMessage = message,
                        transactionId = irisResponse?.response?.transactionId,
                    };

                    return irisOfferResponse;
                }
            }
            catch (Exception ex)
            {
                irisLog.isSuccess = 0;
                irisLog.errorMessage = HelperMethod.ExMsgSubString(ex, thisMethodName, 256);
                irisOfferResponse.statusMessage = HelperMethod.ExMsgSubString(ex, "", 256);

                return irisOfferResponse;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                irisLog.resBodyStr = irisResponse.ToJsonString();

                if (TextLogging.IsEnableIrisTextLog)
                {
                    irisLog.endTime = DateTime.Now;
                    LoggerService.WriteIRISLogInText(irisLog);
                }
                #endregion
            }
        }


        private static OfferResponseModelNew FormatIRISOfferDataV2(List<IRISOfferResponseList> irisResponseList, IRISResponseModel irisResponse)
        {
            List<OfferModelNew> irisViewModels = [];

            OfferResponseModelNew iRISOfferResponse = new()
            {
                statusCode = irisResponse.response.statusCode,
                statusMessage = irisResponse.response.statusMessage,
                transactionId = irisResponse.response.transactionId,
                OfferList = irisViewModels
            };

            foreach (var iris in irisResponseList)
            {
                if (iris.offerName.ToLower().Contains("pop up") && iris.offerCommission == "0" && iris.rechargeAmount == "0")
                {
                    iRISOfferResponse.isUSIM = true;
                    iRISOfferResponse.statusMessage = iris.offerDisplayName;
                }
                else if (string.IsNullOrWhiteSpace(iris.offerDisplayName))
                    continue;
                else if (!iris.offerDisplayName.Any(char.IsDigit))
                    continue;
                else
                {
                    OfferModelNew formatedModel = IRISOfferModelMakerV2(iris, irisResponse.response.transactionId);
                    irisViewModels.Add(formatedModel);
                }
            }

            return iRISOfferResponse;
        }


        private static OfferModelNew IRISOfferModelMakerV2(IRISOfferResponseList iris, string tranId)
        {
            string ussd = iris.sno.ToString();
            string offerId = iris.offerID;
            bool asteriskContains = false;

            string offerDisplayName = iris.offerDisplayName.ToLower();
            string[] irisSplit = offerDisplayName.Contains("default") ? offerDisplayName.Split([' '], 2) : offerDisplayName.Split([' '], 3);
            string offer = irisSplit[0];

            //Regex regex = new(@"(\*)(\d+)(gb|mb)");
            // Check star/* in offer string. Implement at 29-Jan-2024
            Regex regex = new(@"(star|\*)", RegexOptions.IgnoreCase);
            Match starMatch = regex.Match(offer);
            if (starMatch.Success)
                asteriskContains = true;

            try
            {
                offer = Regex.Replace(offer, "[&#;$]", string.Empty, RegexOptions.IgnoreCase);
                offer = (offer.Substring(0, 1) == "*" && offer.Length > 0) ? offer.Substring(1, offer.Length - 1) : offer;
                offer = (offer.Substring(offer.Length - 2) == "10") ? offer.Substring(0, offer.Length - 2) : offer;
            }
            catch (Exception ex)
            {
                offer = HelperMethod.SubStrString(ex.Message, 50);
            }

            bool isRateCutter = false;
            string _offerType = string.Empty;
            string amount = IrisOfferParsing.ParseAmount(offer);
            var voiceParseResult = IrisOfferParsing.ParseIRISVoiceOfferV2(offer, ref isRateCutter);
            _offerType = voiceParseResult.Item2;

            string sms = IrisOfferParsing.ParseSMS(offer);
            _offerType = sms is not "0" ? " SMS" : _offerType;

            IrisDataOfferParseVM dataOffer = IrisOfferParsing.ParseIRISDataOffer(offer);
            _offerType = string.IsNullOrWhiteSpace(dataOffer.offerType) ? _offerType : dataOffer.offerType;

            string validity = IrisOfferParsing.ParseValidity(offer);
            string commission = IrisOfferParsing.ParseCommissionAmount(offerDisplayName);

            _ = int.TryParse(amount, out int _amount);
            _ = int.TryParse(commission, out int _commission);

            OfferModelNew offerData = new()
            {
                ussdCode = ussd,
                description = iris.offerDisplayName,
                amount = _amount,
                commission = _commission,
                tranId = tranId,
                dataPack = dataOffer.dataPack,
                perDayDataPack = dataOffer.perDayData,
                streamingPack = dataOffer.streamingPack,
                talkTime = voiceParseResult.Item1,
                sms = sms,
                toffee = dataOffer.toffee,
                validity = validity,
                offerId = offerId,
                hasStar = asteriskContains,
                offerType = "IRIS" + _offerType,
                hasDataPack = dataOffer.dataPack is not "0",
                hasVoicePack = voiceParseResult.Item1 is not "0",
                hasToffePack = dataOffer.toffee is not "0",
                hasRateCutterPack = isRateCutter,
                hasSMSPack = sms is not "0",
                hasStreamingPack = dataOffer.hasStreamingPack
            };

            offerData.SetOfferYype();
            return offerData;
        }

        #endregion================ || Private Methods || ================

    }
}