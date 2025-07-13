///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Domain.ViewModel.LogModels;
using ImageMagick;
using Infrastracture.Repositories.v2;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using static Domain.Enums.EnumCollections;

namespace Application.Services.v2
{
    public class RetailerV2Service : IDisposable
    {
        private readonly RetailerV2Repository _repo = new();


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


        #region Login External

        public static async Task<LogInViewModel> LogInExternal(LoginRequest loginRequest, string URL)
        {
            HttpRequestModel httpModel = new()
            {
                requestBody = loginRequest,
                requestUrl = URL,
                requestMediaType = MimeTypes.Json,
                requestMethod = "LogInExternal"
            };

            using (HttpService httpService = new())
            {
                Response<LogInViewModel> logInResponse = await httpService.CallExternalApi<LogInViewModel>(httpModel);

                if (logInResponse is null || logInResponse.Object is null)
                {
                    return new LogInViewModel() { ISAuthenticate = false, AuthenticationMessage = Message.BadRequest };
                }

                return logInResponse.Object;
            }
        }

        #endregion


        public async Task<long> UpdateRetailer(RetailerDetailsRequest retailer)
        {
            try
            {
                return await _repo.UpdateRetailer(retailer);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateRetailer"));
            }
        }


        public Task<DataTable> CustomerFeedback(RetailerRequest retailer)
        {
            var result = _repo.CustomerFeedback(retailer);
            return result;
        }


        public async Task<string> RetailerRating(RetailerRequest retailer)
        {
            var result = await _repo.RetailerRating(retailer);
            return result;
        }


        public Task<DataTable> GetSTK()
        {
            try
            {
                Task<DataTable> result = _repo.GetSTK();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSTK"));
            }
        }


        public async Task<DataTable> GetFAQ()
        {
            DataTable result = await _repo.GetFAQ();
            return result;
        }


        public dynamic GetFAQModel(List<FAQModel> model)
        {
            dynamic faqList = _repo.GetFAQModel(model);
            return faqList;
        }


        public async Task<DataTable> GetRetailerDetails(RetailerRequest retailer)
        {
            try
            {
                dynamic faqList = await _repo.GetRetailerDetails(retailer);
                return faqList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRetailerDetails"));
            }
        }


        public async Task<Tuple<bool, string>> UpdateRetailerInfoMySQL(RetailerInfoRequest model)
        {
            return await _repo.UpdateRetailerInfoMySQL(model);
        }


        public async Task<Tuple<bool, string>> UpdateDigitalServiceStatus(UpdateDigitalServiceStatus model)
        {
            return await _repo.UpdateDigitalServiceStatus(model);
        }


        public async Task<int> RetailerBestPractice(ComplainRequest model, int userId)
        {
            try
            {
                dynamic faqList = await _repo.RetailerBestPractice(model, userId);
                return faqList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "RetailerBestPractice"));
            }
        }


        public async Task DeleteTableRows(long id, string tableName, string colName)
        {
            try
            {
                await _repo.DeleteTableRows(id, tableName, colName);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "RetailerBestPractice"));
            }
        }


        public async Task<int> SaveBestPracticeImage(int bestPracticeId, string base64Header, string base64Str)
        {
            try
            {
                dynamic faqList = await _repo.SaveBestPracticeImage(bestPracticeId, base64Header, base64Str);
                return faqList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveBestPracticeImage"));
            }
        }


        public async Task<DataTable> BestPracticesImages(GetBPImagesRequest retailer)
        {
            try
            {
                dynamic faqList = await _repo.BestPracticesImages(retailer);
                return faqList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "BestPracticesImages"));
            }
        }


        public DataTable GetSIMSCStocksSummary(RetailerRequest retailer)
        {
            try
            {
                DataTable faqList = _repo.GetSIMSCStocksSummary(retailer);
                return faqList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSIMSCStocksSummary"));
            }
        }


        public async Task<string> GetActiveQAListIDs(QuickAccessRequest model)
        {
            try
            {
                string objList = await _repo.GetActiveQAListIDs(model);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetActiveQAListIDs"));
            }
        }


        public async Task<string> GetInActiveQAListIDs(QuickAccessRequest model)
        {
            try
            {
                string objList = await _repo.GetInActiveQAListIDs(model);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetInActiveQAListIDs"));
            }
        }


        public async Task<int> UpdateQuickAccessOrder(QuickAccessUpdateRequest model)
        {
            int objList = await _repo.UpdateQuickAccessOrder(model);
            return objList;
        }


        public async Task<DataTable> GetRSOAndLastTime(RetailerRequestV2 reqModel)
        {
            DataTable objList = await _repo.GetRSOAndLastTime(reqModel);
            return objList;
        }


        public DataTable TarVsAchvSummaryV4(RetailerRequestV2 retailerRequest)
        {
            DataTable objList = _repo.TarVsAchvSummaryV4(retailerRequest);
            return objList;
        }


        public DataTable TarVsAchvDeatilsV4(TarVsAchvRequestV2 tarVsAchvRequest)
        {
            DataTable objList = _repo.TarVsAchvDeatilsV4(tarVsAchvRequest);
            return objList;
        }


        public async Task<DataTable> GetArchivedData(ArchivedRequest model)
        {
            DataTable objList = await _repo.GetArchivedData(model);
            return objList;
        }


        public RSOEligibility GetITopUpStockEligibilityCheck(RetailerRequestV2 retailerRequest, out string traceMsg)
        {
            try
            {
                traceMsg = string.Empty;
                RSOEligibility rsoEligibility = new();
                DataTable dt = GetRSOAndLastTime(retailerRequest).Result;

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    rsoEligibility = HelperMethod.ModelBinding<RSOEligibility>(row);
                }
                else
                {
                    traceMsg = EvIrisMessageParsing.ParseMessage(Message.NoRsoDataFound);
                }

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(retailerRequest, "GetITopUpStockEligibilityCheck", traceMsg);
                }

                return rsoEligibility;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetITopUpStockEligibilityCheck"));
            }
        }


        public async Task<string> SendSMS(SendRechargeOfferRequest model, string offers)
        {
            string msisdn = model.subscriberNo.Prepend("0088");
            string _url = ExternalKeys.SMS_Send_Url;
            string smsGreetings = ExternalKeys.SMS_Grettings;
            smsGreetings = string.Format(smsGreetings, "\n");

            StringBuilder urlWithBody = new();
            urlWithBody.AppendFormat(_url, msisdn, offers.Prepend(smsGreetings));

            HttpService httpService = new();
            string result = await httpService.CallSMSSendAPI(model, urlWithBody);
            return result;
        }


        public async Task<List<OfferModelNew>> GetAdminsRechargeOffers(IrisOfferRequestNew irisOffer)
        {
            try
            {
                DataTable offers = new();
                OfferRequest offer = new()
                {
                    acquisition = irisOffer.acquisition,
                    simReplacement = irisOffer.simReplacement,
                    rechargeType = (int)OfferType.RechargeOffer,
                    retailerCode = irisOffer.retailerCode,
                };

                offers = await _repo.GetRechargeOffersV2(offer);
                List<OfferModelNew> rechargePackageModels = offers.AsEnumerable().Select(row => HelperMethod.ModelBinding<OfferModelNew>(row, "", offer.lan)).ToList();

                return rechargePackageModels;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetAdminsRechargeOffers"));
            }
        }


        public async Task<string> GetRSONumber(string retailerCode)
        {
            return await _repo.GetRSONumber(retailerCode);
        }


        public async Task<DataTable> GetC2STransactions(TransactionsRequest model)
        {
            try
            {
                DataTable objList = await _repo.GetC2STransactions(model);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetC2STransactions"));
            }
        }


        public async Task<DataTable> GetC2CTransactions(TransactionsRequest model)
        {
            try
            {
                DataTable objList = await _repo.GetC2CTransactions(model);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetC2CTransactions"));
            }
        }


        public async Task<Tuple<List<C2CRechrgHistResp>, string>> GetC2CRechrgHist(C2CRechargeHistReq xmlRequest, TransactionsRequest trnsReq)
        {
            LoggerService loggerService = new();
            EvLogViewModel evLog = new();
            evLog.startTime = DateTime.Now;

            string xmlReq = "";
            string responseXML = "";

            evLog.isSuccess = 1;
            evLog.errorMessage = "";
            evLog.amount = "";
            try
            {
                xmlReq = XMLService.GetC2CRechrgHistReqXML(xmlRequest);

                responseXML = await XMLService.PostXMLDataAsync(xmlRequest.Url, xmlReq);

                C2CHistMainResp rechargeHist = (C2CHistMainResp)XMLService.ParseXML(responseXML, typeof(C2CHistMainResp));

                return Tuple.Create(rechargeHist.TXNDETAILS.Transactions, string.Empty);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.FormattedExceptionMsg(ex);
                evLog.isSuccess = 0;
                evLog.errorMessage = errMsg;
                return Tuple.Create(new List<C2CRechrgHistResp>(), errMsg);
            }
            finally
            {
                #region==========================|| LOG ||=============================
                evLog.retMSISDN = xmlRequest.Msisdn;
                evLog.reqBodyStr = xmlReq;
                evLog.resBodyStr = responseXML;
                evLog.retailerCode = trnsReq.retailerCode;
                evLog.methodName = "GetC2CRechrgHist";
                evLog.sessionToken = trnsReq.sessionToken;
                evLog.subMSISDN = string.Empty;

                if (TextLogging.IsEnableEVTextLog)
                {
                    evLog.endTime = DateTime.Now;
                    loggerService.WriteEVLogInText(evLog);
                }
                #endregion=============================================================
            }
        }


        public Task<DataTable> GetComplaintTypeList()
        {
            try
            {
                Task<DataTable> objList = _repo.GetComplaintTypeList();
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetComplaintTypeList"));
            }
        }
        public Task<DataTable> GetComplaintDetailsByTitleId(RecommendedComplaintCategoryRequest reqModel, RecommendedComplaintTitle apiResModel)
        {
            try
            {
                Task<DataTable> objList = _repo.GetComplaintDetailsByTitleId(reqModel, apiResModel);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetComplaintDetailsByTitleId"));
            }
        }


        public Task<DataTable> GetComplaintTitleList(ComplaintTitleRequest model)
        {
            try
            {
                Task<DataTable> objList = _repo.GetComplaintTitleList(model);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetComplaintTitleList"));
            }
        }


        public Task<long> SaveRaiseComplaint(RaiseComplSubmitRequest model, int userId)
        {
            Task<long> objList = _repo.SaveRaiseComplaint(model, userId);
            return objList;
        }


        public async Task SaveFileToApiServer(List<ComplaintImageVM> imageVMs, string retailerCode)
        {
            for (int i = 0; i < imageVMs.Count; i++)
            {
                ComplaintImageVM imageVM = imageVMs[i];

                string serverIp = string.Empty;
                string urlRegxStr = "^https:\\/\\/[a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$";
                Regex urlRegx = new(urlRegxStr);

                string serverProtocol = ExternalKeys.ApiServersProtocol;
                string serverIPs = ExternalKeys.ApiServersIPs;
                string urlEndPoint = ExternalKeys.UrlEndPoint;

                string[] serverIPList = serverIPs.Split(',');
                if (serverIPList.Length > 0)
                {
                    for (int j = 0; j < serverIPList.Length; j++)
                    {
                        serverIp = serverIPList[j];
                        string currentServer = HelperMethod.GetIPAddress();
                        RasieComplaintFileSaveRequest fileReq = new()
                        {
                            webToken = ExternalKeys.RetailerApiToWebCred,
                            folderName = imageVM.FolderName,
                            fileName = imageVM.FileName,
                            fileExtension = Path.GetExtension(imageVM.FileName),
                            fileBase64 = imageVM.ImageBase64,
                            retailerCode = retailerCode
                        };

                        if (currentServer.Contains(serverIp))
                        {
                            await SaveBase64File(fileReq);
                        }
                        else
                        {
                            string url = serverProtocol + serverIp + urlEndPoint;

                            if (urlRegx.IsMatch(url))
                            {
                                HttpRequestModel httpReqModel = new()
                                {
                                    requestBody = fileReq,
                                    requestMediaType = MimeTypes.Json,
                                    requestUrl = url
                                };

                                HttpService httpService = new();
                                await httpService.CallExternalApi<dynamic>(httpReqModel);
                            }
                        }

                    }
                }
            }
        }


        public async Task SaveFileToApiServer(ComplaintImageVM imageVM, string retailerCode)
        {
            string serverIp = string.Empty;
            string urlRegxStr = "^https:\\/\\/[a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$";
            Regex urlRegx = new(urlRegxStr);

            string serverProtocol = ExternalKeys.ApiServersProtocol;
            string serverIPs = ExternalKeys.ApiServersIPs;
            string urlEndPoint = ExternalKeys.UrlEndPoint;

            string[] serverIPList = serverIPs.Split(',');
            if (serverIPList.Length > 0)
            {
                for (int j = 0; j < serverIPList.Length; j++)
                {
                    serverIp = serverIPList[j];
                    RasieComplaintFileSaveRequest fileReq = new()
                    {
                        webToken = ExternalKeys.RetailerApiToWebCred,
                        folderName = imageVM.FolderName,
                        fileName = imageVM.FileName,
                        fileExtension = Path.GetExtension(imageVM.FileName),
                        fileBase64 = imageVM.ImageBase64,
                        retailerCode = retailerCode
                    };

                    string url = serverProtocol + serverIp + urlEndPoint;

                    if (urlRegx.IsMatch(url))
                    {
                        HttpRequestModel httpReqModel = new()
                        {
                            requestBody = fileReq,
                            requestMediaType = MimeTypes.Json,
                            requestUrl = url
                        };

                        using(HttpService httpService = new())
                        {
                            _ = httpService.CallExternalApi<dynamic>(httpReqModel);
                        }

                    }
                }
            }

        }


        public async Task SaveBase64File(FileSaveRequestV2 model)
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string fileLocation = string.Empty;
            string path = string.Empty;
            if (isWindows)
            {
                fileLocation = Path.Combine(ExternalKeys.ImagePhisycalDirPath, model.folderName);
                path = Path.Combine(fileLocation, model.fileName);
            }
            else
            {
                fileLocation = $"{ExternalKeys.ImagePhisycalDirPath}/{model.folderName}";
                path = $"{fileLocation}/{model.fileName}";
            }


            Directory.CreateDirectory(fileLocation);

            if (File.Exists(path))
            {
                File.Delete(path);

                byte[] bytes = Convert.FromBase64String(model.fileBase64);
                await File.WriteAllBytesAsync(path, bytes);
            }
            else
            {
                byte[] bytes = Convert.FromBase64String(model.fileBase64);
                await File.WriteAllBytesAsync(path, bytes);
            }
        }


        public Task<Tuple<bool, string>> UpdateRaiseComplaintStatusV2(UpdateRaiseComplaintRequest model)
        {
            try
            {
                Task<Tuple<bool, string>> objList = _repo.UpdateRaiseComplaintStatusV2(model);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateRaiseComplaintStatusV2"));
            }
        }


        public async Task<Tuple<bool, string>> UpdateRaiseComplaintStatusFromSO(UpdateRaiseComplaintRequest model, long soTicketId)
        {
            try
            {
                Tuple<bool, string> objList = await _repo.UpdateRaiseComplaintStatusFromSO(model, soTicketId);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateRaiseComplaintStatusFromSO"));
            }
        }


        public async Task<DataTable> GetSOUpdateStatus(HistoryPageRequestModelV2 model)
        {
            try
            {
                DataTable objList = await _repo.GetSOUpdateStatus(model);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSOUpdateStatus"));
            }
        }


        public async Task<long> UpdateSOTicketStatus(Ticket ticket)
        {
            try
            {
                long objList = await _repo.UpdateSOTicketStatus(ticket);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "UpdateSOTicketStatus"));
            }
        }


        public async Task<DataTable> GetRaiseComplaintHistory(HistoryPageRequestModelV2 model)
        {
            try
            {
                DataTable objList = await _repo.GetRaiseComplaintHistory(model);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRaiseComplaintHistory"));
            }
        }


        public async Task<DataTable> GetRSOInformation(RetailerRequestV2 retailer)
        {
            try
            {
                DataTable objList = await _repo.GetRSOInformation(retailer);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSOInformation"));
            }
        }


        public async Task<long> SaveRSORating(RSORatingReqModel model, int userId)
        {
            try
            {
                long objList = await _repo.SaveRSORating(model, userId);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveRSORating"));
            }
        }


        public async Task<DataTable> GetRSORatingHistory(HistoryPageRequestModelV2 model)
        {
            try
            {
                DataTable objList = await _repo.GetRSORatingHistory(model);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetRSORatingHistory"));
            }
        }


        public async Task<DataTable> GetNotifications(RetailerRequestV2 retailer)
        {
            try
            {
                DataTable objList = await _repo.GetNotifications(retailer);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetNotifications"));
            }
        }


        public async Task<string[]> GetRetailersTopThreeDeno(RetailerRequestV2 retailerRequest)
        {
            try
            {
                string[] objList = await _repo.GetRetailersTopThreeDeno(retailerRequest);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "retailerRequest"));
            }
        }


        public async Task<DataTable> RetailerDenoReport(SearchRequestV2 searchRequest)
        {
            try
            {
                DataTable objList = await _repo.RetailerDenoReport(searchRequest);
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "RetailerDenoReport"));
            }
        }


        public async Task<long> SaveContact(ContactSaveRequest contact)
        {
            return await _repo.SaveContact(contact);
        }


        public async Task<int> VORByRetailer(VORModel model)
        {
            return await _repo.VORByRetailer(model);
        }


        public async Task<int> SaveVorImage(int vorId, string base64Header, string base64Str)
        {
            return await _repo.SaveVorImage(vorId, base64Header, base64Str);
        }


        public async Task<DataTable> GetRSOMemo(RsoMemoRequest retailer, int userId)
        {
            return await _repo.GetRSOMemo(retailer, userId);
        }


        public async Task<DataTable> GetRSOFeedback(RsoMemoRequest retailer, int userId)
        {
            return await _repo.GetRSOFeedback(retailer, userId);
        }


        public async Task<DataTable> GetFeedbackCategoryList(int userId)
        {
            return await _repo.GetFeedbackCategoryList(userId);
        }


        public async Task<DataTable> GetOperatorList(int userId)
        {
            return await _repo.GetOperatorList(userId);
        }


        public async Task<DataTable> GetProductRatingList(ProductRatingRequest model)
        {
            return await _repo.GetProductRatingList(model);
        }


        public async Task<DataTable> GetAllIRISProductRating(IrisOfferRequestNew model, int userId)
        {
            return await _repo.GetAllIRISProductRating(model, userId);
        }


        public async Task<DataTable> GetProductTypeList()
        {
            return await _repo.GetProductTypeList();
        }


        public async Task<DataTable> GetProductRatingHistory(HistoryPageRequestModel model)
        {
            return await _repo.GetProductRatingHistory(model);
        }


        public async Task<DataTable> GetBTSLocation(string retailerCode)
        {
            return await _repo.GetBTSLocation(retailerCode);
        }


        public async Task<DataTable> GetBTSLocationDetails(int lac, int cid)
        {
            return await _repo.GetBTSLocationDetails(lac, cid);
        }


        public async Task<DataTable> GetContactList(string retailerCode)
        {
            var result = await _repo.GetContactList(retailerCode);
            return result;
        }


        public async Task<DataTable> GetCampaignKPIList(CampaignRequestV2 model)
        {
            return await _repo.GetCampaignKPIList(model);
        }


        public async Task<DataTable> GetCampaignRewardList(CampaignRequestV2 campaignRequest)
        {
            return await _repo.GetCampaignRewardList(campaignRequest);
        }


        public async Task<DataTable> GetCampFurtherDetails(CampaignRequestV2 model)
        {
            return await _repo.GetCampFurtherDetails(model);
        }


        public async Task<DataTable> GetSelfCampaignKPIList(CampaignRequestV2 campaignRequest)
        {
            return await _repo.GetSelfCampaignKPIList(campaignRequest);
        }


        public async Task<DataTable> GetSelfCampaignRewardList(CampaignRequestV2 campaignRequest)
        {
            return await _repo.GetSelfCampaignRewardList(campaignRequest);
        }


        public async Task<DataTable> GetSelfCampFurtherDetails(CampaignRequestV2 model)
        {
            return await _repo.GetSelfCampFurtherDetails(model);
        }


        public async Task<DataTable> GetCampRetailerDates(SelfCampDatesRequest model, string ids)
        {
            return await _repo.GetCampRetailerDates(model, ids);
        }


        public async Task<DataTable> GetSelfRewardList(SelfCampaignRewardRequest model)
        {
            return await _repo.GetSelfRewardList(model);
        }


        public async Task<DataTable> GetSelfCampDayList(string retailerCode, string ids)
        {
            return await _repo.GetSelfCampDayList(retailerCode, ids);
        }


        public async Task<DataTable> GetCampKPIList(SelfKPIListRequest model)
        {
            return await _repo.GetCampKPIList(model);
        }


        public async Task<int> EnrollExtCampaign(CampaignEnrollOrCancelRequest model)
        {
            return await _repo.EnrollExtCampaign(model);
        }


        public async Task<int> EnrollSelfCampaign(CampaignEnrollOrCancelRequest model)
        {
            return await _repo.EnrollSelfCampaign(model);
        }


        public async Task<int> CancelExtCampaignEnroll(CampaignEnrollOrCancelRequest model)
        {
            return await _repo.CancelExtCampaignEnroll(model);
        }


        public async Task<int> CancelSelfCampaignEnroll(CampaignEnrollOrCancelRequest model)
        {
            return await _repo.CancelSelfCampaignEnroll(model);
        }


        public async Task<DataTable> GetCampHistoryKPIList(CampaignHistoryRequest model)
        {
            return await _repo.GetCampHistoryKPIList(model);
        }


        public async Task<DataTable> GetCampHistoryList(CampaignHistoryRequest model)
        {
            return await _repo.GetCampHistoryList(model);
        }


        public async Task<string> GetCampHistoryUpdateTill(CampaignHistoryRequest model)
        {
            return await _repo.GetCampHistoryUpdateTill(model);
        }


        public async Task<DataTable> GetSelfCampHistoryKPIList(CampaignHistoryRequest model)
        {
            return await _repo.GetSelfCampHistoryKPIList(model);
        }


        public async Task<DataTable> GetSelfCampHistoryList(CampaignHistoryRequest model)
        {
            return await _repo.GetSelfCampHistoryList(model);
        }


        public async Task<string> GetSelfCampHistoryUpdateTill(CampaignHistoryRequest model)
        {
            return await _repo.GetSelfCampHistoryUpdateTill(model);
        }


        public async Task<DataTable> GetCampKPIDetails(CampaignKPIRequest model, string ids)
        {
            return await _repo.GetCampKPIDetails(model, ids);
        }


        public async Task<string> CampaignByRetailer(CreateCampaignByRetailerRequest model)
        {
            return await _repo.CampaignByRetailer(model);
        }


        public async Task<long> InsertRetailerCampTarget(CampaignTargetRequestModel model)
        {
            return await _repo.InsertRetailerCampTarget(model);
        }


        public async Task DeleteInsertedCampaign(int campId, int campEnrollId, int rewardMapId)
        {
            await _repo.DeleteInsertedCampaign(campId, campEnrollId, rewardMapId);
        }


        public async Task<DataTable> GetDigitalServiceHistory(DigitalServiceHistoryRequest model)
        {
            return await _repo.GetDigitalServiceHistory(model);

        }


        public async Task<DataTable> GetDigitalProductList()
        {
            return await _repo.GetDigitalProductList();
        }


        public async Task DigitalServiceSmsSendToUser(int productId, string receiverNumber)
        {
            await _repo.DigitalServiceSmsSendToUser(productId, receiverNumber);
        }


        public async Task<Tuple<bool, string>> UpdateRaiseComplaintStatus(UpdateRaiseComplaintRequest model)
        {
            return await _repo.UpdateRaiseComplaintStatus(model);
        }


        public async Task<DataTable> GetAppSettingsInfo()
        {
            return await _repo.GetAppSettingsInfo();
        }


        public async Task<long> SaveDeviceTokens(DeviceTokenRequest model)
        {
            return await _repo.SaveDeviceTokens(model);
        }


        public async Task<string> GetRegionWisePopupCallingTime(string iTopUpNumber)
        {
            return await _repo.GetRegionWisePopupCallingTime(iTopUpNumber);
        }


        public async Task<long> LogoutDevice(DeviceStatusRequest model)
        {
            return await _repo.LogoutDevice(model);
        }


        public async Task<DataTable> SecondaryDeviceList(RetailerRequest model)
        {
            return await _repo.SecondaryDeviceList(model);
        }


        public async Task<long> DeregisterDevice(DeviceStatusRequest model)
        {
            return await _repo.DeregisterDevice(model);
        }


        public async Task<long> ChangeDeviceType(DeviceStatusRequest model)
        {
            return await _repo.ChangeDeviceType(model);
        }


        public async Task<long> EnableDisableDevice(DeviceStatusRequest model)
        {
            return await _repo.EnableDisableDevice(model);
        }


        public async Task<int> GetNotificationCount(RetailerRequest retailer)
        {
            return await _repo.GetNotificationCount(retailer);
        }


        public async Task<int> UpdateNotoficationStatus(RetailerNotificationRequest notificationRequest)
        {
            return await _repo.UpdateNotoficationStatus(notificationRequest);
        }


        public async Task<DataTable> GetTickerMessages(RetailerRequest model)
        {
            return await _repo.GetTickerMessages(model);
        }


        public async Task<DataTable> GetC2SPostpaidTransactions(TransactionsRequest model)
        {
            return await _repo.GetC2SPostpaidTransactions(model);
        }


        public async Task<DataTable> GetCampaignList(CampaignRequestV3 campaignRequest)
        {
            return await _repo.GetCampaignList(campaignRequest);
        }


        public async Task<DataTable> GetSelfCampaignList(CampaignRequestV3 campaignRequest)
        {
            return await _repo.GetSelfCampaignList(campaignRequest);
        }


        public async Task<long> SaveDigitalService(DigitalServiceSubmitRequest model, int userId)
        {
            return await _repo.SaveDigitalService(model, userId);
        }


        public async Task<DataTable> GetCommunications(CommunicationRequestV4 retailer)
        {
            return await _repo.GetCommunications(retailer);
        }


        public async Task<DataTable> BestPracticesHistory(RetailerRequestV2 retailer)
        {
            return await _repo.BestPracticesHistory(retailer);
        }


        public async Task<DataTable> GetRaiseComplaintInfoV2(RaiseComplSubmitRequest model, long insertedCompId)
        {
            return await _repo.GetRaiseComplaintInfoV2(model, insertedCompId);
        }


        public async Task<long> SaveProductRating(SubmitProductRating model)
        {
            return await _repo.SaveProductRating(model);
        }


        public async Task<DataTable> GetDisclaimerNotices(string lan)
        {
            try
            {
                DataTable result = await _repo.GetDisclaimerNotices(lan);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDisclaimerNotices"));
            }
        }


        public async Task<int> DeleteContact(long contactId)
        {
            return await _repo.DeleteContact(contactId);
        }


        public async Task SendPushNotification(string itopupNumber)
        {
            FcmNotificationRequest reqModel = new()
            {
                sessionToken = ExternalKeys.RetailerApiToWebCred,
                iTopUpNumber = itopupNumber
            };

            string url = $"{ExternalKeys.WebPortalBaseUrl}{ExternalKeys.WebApiNotificationEndPoint}";

            HttpRequestModel httpReqModel = new()
            {
                requestBody = reqModel,
                requestMediaType = MimeTypes.Json,
                requestUrl = url,
                requestMethod = "SendPushNotification"
            };

            HttpService httpService = new();
            await httpService.SendFcmNotificationViaWebApi<dynamic>(httpReqModel);
        }


        public async Task SendPushNotificationTemp(string itopupNumber)
        {
            DataTable dt = await _repo.LoadAllDeviceTokenByRetailer(itopupNumber);

            if (dt.Rows.Count > 0)
            {
                List<string> deviceTokenList = [.. dt.AsEnumerable().Select(row => row["DEVICE_TOKEN"] as string)];

                FcmNotificationTemp reqModel = new()
                {
                    apiToken = "KzMwUnFHQkxyNEFfTkI9RkQqMFNmQmNOKmE5ST9SU3RSSzJaT2ZsZXlmdkUkRQ==",
                    deviceIds = string.Join('|', deviceTokenList),
                    notificationHeader = "EV পিন রিসেট স্ট্যাটাস আপডেট",
                    notificationBody = "আপনার EV পিন রিসেট করা হয়েছে। দয়া করে এপ এর \"মেনু > EV পিন রিসেট/পরিবর্তন > পিন পরিবর্তন\" মেনু থেকে পিনটি পরিবর্তন করে নিন।",
                    iTopUpNumber = itopupNumber
                };

                string url = "https://retapptest.banglalink.net/NotificationAPILive/NotificationAPI/GenerateNotification";

                HttpRequestModel httpReqModel = new()
                {
                    requestBody = reqModel,
                    requestMediaType = MimeTypes.Json,
                    requestUrl = url,
                    requestMethod = "SendPushNotificationTemp"
                };

                using(HttpService httpService = new())
                {
                    await httpService.SendFcmNotificationViaWebApi<dynamic>(httpReqModel);
                }
            }
        }


        public async Task<long> SaveGamificationResponse(GamificationResponseReq responseReq)
        {
            return await _repo.SubmitGamificationResponse(responseReq);
        }


        public async Task<DataTable> GetC2SOtfTransactions(TransactionsRequest model)
        {
            return await _repo.GetC2SOtfTransactions(model);
        }


        public async Task SaveNdSendFileWithResize(ComplaintImageVM imageVM, string retailerCode)
        {
            byte[] imageBytes = Convert.FromBase64String(imageVM.ImageBase64);

            if (imageBytes.Length > 250 * 1024) // 250 KB threshold
            {
                MemoryStream respStream = await ResizeImage(imageBytes, 250); //250 Kbs

                if (respStream.Length == 0)
                    return;

                respStream.Position = 0;
                byte[] resizedImageBytes = respStream.ToArray();
                string imageBased64 = Convert.ToBase64String(resizedImageBytes);

                imageVM.ImageBase64 = imageBased64;
            }

            await SaveFileToApiServer(imageVM, retailerCode);
        }


        #region Private Methods

        private static async Task<MemoryStream> ResizeImage(byte[] imageBytes, long desiredSizeKbs)
        {
            using var mstream = new MemoryStream(imageBytes);
            mstream.Position = 0;

            long targetSizeBytes = desiredSizeKbs * 1024;
            int quality = 95;

            using var image = new MagickImage(mstream);

            do
            {
                Percentage defaultPercent = new(quality);

                var size = new MagickGeometry(defaultPercent, defaultPercent)
                {
                    IgnoreAspectRatio = false // Set to true if you want to ignore aspect ratio
                };

                image.Resize(size);
                image.Quality = (uint)quality;

                var ms = new MemoryStream();
                await image.WriteAsync(ms);
                ms.Position = 0;

                if (ms.Length <= targetSizeBytes || quality <= 40)
                {
                    return ms;
                }

                quality -= 5; // Smaller decrements for finer control
            } while (true);
        }

        #endregion
    }
}