///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	08-Jan-2024
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
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Domain.ViewModel.LogModels;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;

namespace Application.Services
{
    public partial class HttpService : IDisposable
    {
        private readonly HttpClientHandler clientHandler;
        private readonly HttpClient _client;

        public HttpService()
        {
            clientHandler = new HttpClientHandler()
            {
                UseDefaultCredentials = true,
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                SslProtocols = SslProtocols.Tls12
            };
            _client = new HttpClient(clientHandler);
            _client.DefaultRequestHeaders.Accept.Clear();
        }

        public HttpService(int timeOutDuration)
        {
            clientHandler = new HttpClientHandler()
            {
                UseDefaultCredentials = true,
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                SslProtocols = SslProtocols.Tls12
            };
            _client = new HttpClient(clientHandler);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.Timeout = TimeSpan.FromSeconds(timeOutDuration);
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
                clientHandler.Dispose();
                _client.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========


        public async Task<Response<T>> CallExternalApi<T>(HttpRequestModel httpModel)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                reqStartTime = DateTime.Now,
                isSuccess = 1
            };
            var resp = new Response<T>().Object;
            string traceMsg = String.Empty;

            try
            {
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(httpModel.requestMediaType));

                var jsonRequest = ((object)httpModel.requestBody).ToJsonString();

                HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, httpModel.requestMediaType);
                traceMsg += "RequestBody: " + jsonRequest + " || ";
                HttpResponseMessage result = await _client.PostAsync(httpModel.requestUrl, content);
                string responseStr = await result.Content.ReadAsStringAsync();
                traceMsg += "Response: " + responseStr + " || ";
                resp = JsonConvert.DeserializeObject<T>(responseStr)!;

                return new Response<T>()
                {
                    Object = resp,
                    Objects = [resp]
                };
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 400);

                LogModel _log = new();
                _log.errorMessage = traceMsg + "error: " + HelperMethod.FormattedExceptionMsg(ex);
                _log.methodName = "CallExternalApi";
                _log.apiEndTime = DateTime.Now;
                LoggerService.WriteTraceMsg(_log);

                throw;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.retailerCode = httpModel.requestBody.retailerCode;
                externalApiVM.methodName = httpModel.requestMethod + " || CallExternalApi";
                externalApiVM.reqBodyStr = httpModel.ToJsonString();
                externalApiVM.resBodyStr = resp.ToJsonString();

                if (TextLogging.IsEnableExternalTextLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteExternalLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task<Response<T>> CallDmsExternalApi<T>(HttpRequestModel httpModel)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                reqStartTime = DateTime.Now,
                isSuccess = 1
            };
            var resp = new Response<T>().Object;

            try
            {
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(httpModel.requestMediaType));

                var jsonRequest = ((object)httpModel.requestBody).ToJsonString();

                HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, httpModel.requestMediaType);
                HttpResponseMessage result = await _client.PostAsync(httpModel.requestUrl, content);
                string responseStr = await result.Content.ReadAsStringAsync();

                resp = JsonConvert.DeserializeObject<T>(responseStr)!;

                return new Response<T>()
                {
                    Object = resp,
                    Objects = [resp]
                };
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 400);
                throw;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.retailerCode = httpModel.requestBody.retailerCode;
                externalApiVM.methodName = httpModel.requestMethod + " || CallDmsExternalApi";
                externalApiVM.reqBodyStr = httpModel.ToJsonString();
                externalApiVM.resBodyStr = resp.ToJsonString();

                if (TextLogging.IsEnableTextLogToDms)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteDmsLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task<string> CallSMSSendAPI(SendRechargeOfferRequest model, StringBuilder urlWithBody)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                reqStartTime = DateTime.Now,
                isSuccess = 1
            };

            string responseStr = string.Empty;

            try
            {
                _client.Timeout = TimeSpan.FromSeconds(2);
                HttpResponseMessage result = await _client.GetAsync(urlWithBody.ToString());
                if (result.IsSuccessStatusCode)
                {
                    responseStr = result.StatusCode.ToString();
                    return responseStr;
                }
                else
                {
                    responseStr = await result.Content.ReadAsStringAsync();
                    throw new Exception(responseStr);
                }
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 400);
                throw;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.retailerCode = model.retailerCode;
                externalApiVM.methodName = "SendIrisOfferToCustomer || CallSMSSendAPI";
                externalApiVM.reqBodyStr = urlWithBody.ToString();
                externalApiVM.resBodyStr = responseStr;

                if (TextLogging.IsEnableExternalTextLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteExternalLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task<BiometricAppInfo> GetBiometricAppLatestUrl<T>(HttpRequestModel httpModel)
        {
            ExternalAPICallVM externalApiVM = new();
            externalApiVM.reqStartTime = DateTime.Now;
            externalApiVM.isSuccess = 1;
            string responseStr = string.Empty;

            try
            {
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(httpModel.requestMediaType));

                var jsonRequest = ((object)httpModel.requestBody).ToJsonString();

                HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, httpModel.requestMediaType);
                HttpResponseMessage result = await _client.PostAsync(httpModel.requestUrl, content);
                responseStr = await result.Content.ReadAsStringAsync();

                BiometricAppInfo resp = JsonConvert.DeserializeObject<BiometricAppInfo>(responseStr)!;

                return resp;
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 400);
                throw;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.retailerCode = httpModel.requestBody.username;
                externalApiVM.methodName = httpModel.requestMethod + " || GetBiometricAppLatestUrl";
                externalApiVM.reqBodyStr = ((object)httpModel.requestBody).ToJsonString();
                externalApiVM.resBodyStr = responseStr;

                if (TextLogging.IsEnableExternalTextLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteExternalLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task<SMSInformationModel> SendSMS(SMSInformationModel model)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                reqStartTime = DateTime.Now,
                isSuccess = 1
            };
            string responseStr = string.Empty;

            try
            {
                StringBuilder urlWithBody = new();
                urlWithBody.AppendFormat(model.SMSApiUrl, model.SenderAddress, model.ReceiverAddress, model.SMSBody, model.SMSCoding);
                HttpResponseMessage result;
                using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
                {
                    result = await _client.GetAsync(urlWithBody.ToString());
                }

                if (result != null && result.IsSuccessStatusCode)
                {
                    responseStr = await result.Content.ReadAsStringAsync();
                    model.status = 2;
                    model.remarks = "Sent via API";
                    model.isSuccess = true;
                    model.deliveredOn = DateTime.Now;

                    return model;
                }
                else
                {
                    model.status = 9;
                    model.remarks = "Couldn't Sent SMS via API";
                    model.isSuccess = false;

                    return model;
                }
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 400);

                model.status = 9;
                model.remarks = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                return model;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.methodName = "SendSMS";
                externalApiVM.reqBodyStr = model.ToJsonString();
                externalApiVM.resBodyStr = responseStr;

                if (TextLogging.IsEnableExternalTextLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteExternalLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task UpdateBioRetailerStatus(RetailerInfoRequest model, string url)
        {
            ExternalAPICallVM externalApiVM = new();
            externalApiVM.reqStartTime = DateTime.Now;
            externalApiVM.isSuccess = 1;
            string response = string.Empty;

            try
            {
                var jsonRequest = model.ToJsonString();
                HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, MimeTypes.Json);
                HttpResponseMessage result;
                using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
                {
                    result = await _client.PostAsync(url, content);
                }
                response = await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 400);
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.retailerCode = model.retailerCode;
                externalApiVM.methodName = "UpdateBioRetailerStatus";
                externalApiVM.reqBodyStr = model.ToJsonString();
                externalApiVM.resBodyStr = response;

                if (TextLogging.IsEnableExternalTextLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteExternalLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task<Response<T>> SendFcmNotificationViaWebApi<T>(HttpRequestModel httpModel)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                reqStartTime = DateTime.Now,
                isSuccess = 1
            };
            string responseStr = string.Empty;

            try
            {
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(httpModel.requestMediaType));

                var jsonRequest = ((object)httpModel.requestBody).ToJsonString();

                HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, httpModel.requestMediaType);
                HttpResponseMessage result = await _client.PostAsync(httpModel.requestUrl, content);
                responseStr = await result.Content.ReadAsStringAsync();

                var resp = new Response<T>().Object;
                resp = JsonConvert.DeserializeObject<T>(responseStr);

                return new Response<T>()
                {
                    Object = resp,
                    Objects = [resp]
                };
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.FormattedExceptionMsg(ex);
                throw;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.retailerCode = httpModel.requestBody.iTopUpNumber;
                externalApiVM.methodName = httpModel.requestMethod + " || SendFcmNotificationViaWebApi";
                externalApiVM.reqBodyStr = httpModel.ToJsonString();
                externalApiVM.resBodyStr = responseStr;

                if (TextLogging.IsEnableExternalTextLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteExternalLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task<SimStatusModel> CallCheckSimStatusApi(SimStatusRequestModel model, string url)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                reqStartTime = DateTime.Now,
                isSuccess = 1
            };

            string responseStr = string.Empty;
            try
            {
                _client.BaseAddress = new Uri(url);
                _client.Timeout = TimeSpan.FromSeconds(2);
                string status = "not available";
                HttpResponseMessage response = await _client.GetAsync(model.msisdn);
                responseStr = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject<dynamic>(responseStr)!;

                status = response.IsSuccessStatusCode ? json.data.attributes.status : json.errors.title;
                SimStatusModel simStatusModel = new(new DataTable().NewRow()) { isAvailable = false, productName = status };
                simStatusModel.isAvailable = status.Contains("available") || simStatusModel.isAvailable;
                return simStatusModel;
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 400);
                throw;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.retailerCode = model.retailerCode;
                externalApiVM.methodName = "CheckSimStatus || CallCheckSimStatusApi";
                externalApiVM.reqBodyStr = model.msisdn;
                externalApiVM.resBodyStr = responseStr;

                if (TextLogging.IsEnableTextLogToDms)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteDmsLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task<IRISResponseModel> GetIRISOffers(RechargeRequestVM rechargeXmlVM)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                isSuccess = 1
            };
            if (string.IsNullOrWhiteSpace(rechargeXmlVM.OriginMethodName))
                rechargeXmlVM.OriginMethodName = "HttpService || ";

            try
            {
                var uri = new Uri(rechargeXmlVM.RequestUrl);
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeTypes.Json));

                HttpContent content = new StringContent(rechargeXmlVM.RequestBody, Encoding.UTF8, MimeTypes.Json);
                externalApiVM.reqStartTime = DateTime.Now;
                HttpResponseMessage result = await _client.PostAsync(uri, content);
                string responseStr = await result.Content.ReadAsStringAsync();
                externalApiVM.reqEndTime = DateTime.Now;
                externalApiVM.resBodyStr = responseStr;

                var irisResponse = string.IsNullOrWhiteSpace(responseStr) ? null : JsonConvert.DeserializeObject<IRISResponseModel>(responseStr)!;

                return irisResponse;
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 500);
                externalApiVM.errorDetails = ex;
                throw;
            }
            finally
            {
                #region====================|SAVE EXTERNAL API LOG|==========================
                externalApiVM.retailerCode = rechargeXmlVM.RetailerCode;
                externalApiVM.methodName = $"{rechargeXmlVM.OriginMethodName} || GetIRISOffers";
                externalApiVM.reqBodyStr = rechargeXmlVM.RequestBody;

                if (TextLogging.IsEnableRechargeApiExternalLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteRechargeApiExternalLogInText(externalApiVM, "IrisExternalApi");
                }
                #endregion
            }
        }


        public async Task<IrisRechargeResponse> IRISRechargeRequest(RechargeRequestVM rechargeXmlVM)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                isSuccess = 1
            };
            if (string.IsNullOrWhiteSpace(rechargeXmlVM.OriginMethodName))
                rechargeXmlVM.OriginMethodName = "HttpService || ";

            try
            {
                HttpContent content = new StringContent(rechargeXmlVM.RequestBody, Encoding.UTF8, MimeTypes.Json);
                var uri = new Uri(rechargeXmlVM.RequestUrl);

                externalApiVM.reqStartTime = DateTime.Now;
                HttpResponseMessage result = _client.PostAsync(uri, content).Result;
                string responseStr = await result.Content.ReadAsStringAsync();
                externalApiVM.reqEndTime = DateTime.Now;
                externalApiVM.resBodyStr = responseStr;

                if (!string.IsNullOrWhiteSpace(responseStr))
                    return JsonConvert.DeserializeObject<IrisRechargeResponse>(responseStr);
                else
                {
                    externalApiVM.errorMessage = responseStr;
                    return null;
                }
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 500);
                throw;
            }
            finally
            {
                #region====================|SAVE EXTERNAL API LOG|==========================
                externalApiVM.retailerCode = rechargeXmlVM.RetailerCode;
                externalApiVM.methodName = $"{rechargeXmlVM.OriginMethodName} || IRISRechargeRequest";
                externalApiVM.reqBodyStr = rechargeXmlVM.RequestBody;

                if (TextLogging.IsEnableRechargeApiExternalLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteRechargeApiExternalLogInText(externalApiVM, "IrisExternalApi");
                }
                #endregion
            }
        }


        public async Task<StarTrekResponse> StarTrekStatusCheckingRequest(HttpRequestModel httpRequestModel)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                isSuccess = 1
            };
            string responseStr = string.Empty;

            try
            {
                _client.Timeout = TimeSpan.FromSeconds(1);
                externalApiVM.reqStartTime = DateTime.Now;
                HttpResponseMessage result = await _client.GetAsync(httpRequestModel.requestUrl);
                externalApiVM.reqEndTime = DateTime.Now;
                responseStr = await result.Content.ReadAsStringAsync();

                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception(responseStr);
                }
                else
                {
                    StarTrekResponse resp = JsonConvert.DeserializeObject<StarTrekResponse>(responseStr)!;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.reqEndTime = DateTime.Now;
                externalApiVM.errorMessage = HelperMethod.FormattedExceptionMsg(ex);
                return new StarTrekResponse();
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.retailerCode = httpRequestModel.requestBody.retailerCode;
                externalApiVM.methodName = httpRequestModel.requestMethod + " || StarTrekStatusCheckingRequest";
                externalApiVM.reqBodyStr = httpRequestModel.requestUrl;
                externalApiVM.resBodyStr = responseStr;

                if (TextLogging.IsEnableExternalTextLog)
                {
                    LoggerService.WriteExternalLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task<ExternalSubmitResponse> SubmitExternalRequest(HttpRequestModel httpModel)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                reqStartTime = DateTime.Now,
                isSuccess = 1
            };
            string requestStr = string.Empty;
            string responseStr = string.Empty;

            try
            {
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(httpModel.requestMediaType));

                requestStr = ((object)httpModel.requestBody).ToJsonString();

                HttpContent content = new StringContent(requestStr, Encoding.UTF8, httpModel.requestMediaType);
                HttpResponseMessage result = await _client.PostAsync(httpModel.requestUrl, content);
                responseStr = await result.Content.ReadAsStringAsync();

                var resp = JsonConvert.DeserializeObject<ExternalSubmitResponse>(responseStr)!;
                externalApiVM.isSuccess = resp.statusCode == 200 ? 1 : 0;
                if (resp.statusCode != 200)
                {
                    externalApiVM.errorMessage = resp.message;
                }

                return resp;
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 400);
                throw;
            }
            finally
            {
                #region====================|SAVE LOG|==========================
                externalApiVM.retailerCode = httpModel.requestBody.retailerCode;
                externalApiVM.methodName = httpModel.requestMethod + " || HttpService || SubmitExternalRequest";
                externalApiVM.reqBodyStr = requestStr;
                externalApiVM.resBodyStr = responseStr;

                if (TextLogging.IsEnableTextLogToDms)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteDmsLogInText(externalApiVM);
                }
                #endregion
            }
        }

    }
}