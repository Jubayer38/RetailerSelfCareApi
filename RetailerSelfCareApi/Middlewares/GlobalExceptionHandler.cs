///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Global exception handle
///	Creation Date :	13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Domain.ViewModel.LogModels;
using Microsoft.IO;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace RetailerSelfCareApi.Middlewares
{
    public class GlobalExceptionHandler(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            string _method = context.Request.Path.Value;

            LogModel _log = await InitLogModel(context, _method);

            context.Items["token"] = _log.sessionToken;
            context.Items["retailerCode"] = _log.retailerCode;
            context.Items["deviceId"] = _log.deviceId;

            var originalBodyStream = context.Response.Body;

            RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                _log.responseBody = await ParseResponseBody(context);
                await responseBody.CopyToAsync(originalBodyStream);
                _log.isSuccess = 1;

                SaveLog(_log);
            }
            catch (Exception ex)
            {
                _log.isSuccess = 0;

                string errMsg = HelperMethod.FormattedExceptionMsg(ex);
                string[] errDet = ex.Message.Split(["_split_"], StringSplitOptions.None);
                if (errDet.Length > 1)
                {
                    errMsg = errDet[0];
                    string msg = errDet[1] + " || " + errDet[0];
                    _log.errorMessage = HelperMethod.SubStrString(msg);
                }
                else
                {
                    _log.errorMessage = HelperMethod.ExMsgSubString(ex, _method);
                }

                string errorResponse = GetErrorResponse(_method, errMsg, ex);

                context.Response.StatusCode = (int)HttpStatusCode.OK;

                _log.responseBody = errorResponse;

                await new MemoryStream(Encoding.UTF8.GetBytes(_log.responseBody)).CopyToAsync(originalBodyStream);

                SaveLog(_log);
            }
        }

        #region==========| Start of Private Methods |==========

        private static async Task<LogModel> InitLogModel(HttpContext context, string _methodname)
        {
            LogModel logModel = new()
            {
                apiStartTime = DateTime.Now,
                methodName = _methodname
            };

            if (context.Request is not null)
            {
                logModel.requestBody = await ParseRequestBody(context, _methodname);
            }

            string hostIp = HelperMethod.GetIPAddress();
            string requestHost = context.Request.Host.ToString();
            logModel.hostAndOtherInfo = hostIp + " || Request Url-" + requestHost;
            logModel.userAgentNdIP = context.Request.Headers.UserAgent.ToString();

            if (!string.IsNullOrWhiteSpace(_methodname))
            {
                RequestValidityModel validity = IsRequestValid(logModel.requestBody, _methodname);

                logModel.deviceId = validity.isValid ? validity.retailerRequest.deviceId : string.Empty;

                logModel.hostAndOtherInfo += " || DeviceId: " + logModel.deviceId;

                logModel.sessionToken = validity.isValid ? validity.retailerRequest.sessionToken : string.Empty;
                logModel.iTopUpNumber = validity.isValid ? validity.retailerRequest.iTopUpNumber : string.Empty;
                logModel.lan = validity.isValid ? validity.retailerRequest.lan : string.Empty;

                string _retailerCode;
                if (_methodname.Contains("loginsmartpos", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        _retailerCode = "R" + JsonConvert.DeserializeObject<LoginRequest>(logModel.requestBody).UserName;
                    }
                    catch (Exception)
                    {
                        _retailerCode = "Couldn't parse";
                    }
                }
                else
                {
                    _retailerCode = validity.isValid ? validity.retailerRequest.retailerCode : string.Empty;
                }

                logModel.retailerCode = _retailerCode;
            }

            return logModel;
        }


        private static async Task<string> ParseRequestBody(HttpContext context, string methodName)
        {
            context.Request.EnableBuffering();
            var _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            context.Request.Body.Position = 0;
            var reqBody = ReadStreamInChunks(requestStream);
            string requestBody = MaskSensitiveData(reqBody, methodName);

            return requestBody;
        }


        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }


        private static async Task<string> ParseResponseBody(HttpContext context)
        {
            try
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                string responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                return responseBody;
            }
            catch (Exception ex) { return HelperMethod.FormattedExceptionMsg(ex); }
        }


        private static string MaskSensitiveData(string reqBody, string methodName)
        {
            //string requestBody = string.Empty;
            //Need to implement below section after completion the Login API
            //if (methodName.Contains("Login"))
            //{
            //    var reqBody = JsonConvert.DeserializeObject<Login>(reqestBody);
            //    reqBody.Password = "******";
            //}

            //requestBody = reqBody.ToJsonString();
            return reqBody;
        }


        private static RequestValidityModel IsRequestValid(string requestString, string methodName)
        {
            try
            {
                ContextHelperModel reqModel = !string.IsNullOrWhiteSpace(requestString) ?
                    JsonConvert.DeserializeObject<ContextHelperModel>(requestString) :
                    new ContextHelperModel();

                if (AuthorizedRouteList.Routes.Contains(methodName))
                {
                    RequestValidityModel request = new() { retailerRequest = reqModel, isValid = true };
                    return request;
                }
                else if (!string.IsNullOrWhiteSpace(reqModel.sessionToken) &&
                    !string.IsNullOrWhiteSpace(reqModel.retailerCode) &&
                    !string.IsNullOrWhiteSpace(reqModel.deviceId))
                {
                    RequestValidityModel request = new() { retailerRequest = reqModel, isValid = true };
                    return request;
                }
                else
                {
                    RequestValidityModel request = new() { isValid = false };
                    return request;
                }
            }
            catch (Exception)
            {
                RequestValidityModel request = new() { isValid = false };
                return request;
            }
        }


        private static string GetErrorResponse(string methodName, string errorMsg, Exception exception)
        {
            return methodName switch
            {
                "/api/Security/DeviceValidation" => new DeviceValidationResponse()
                {
                    isSuccess = false,
                    responseMessage = SharedResource.GetLocal("DeviceValidFailed", Message.DeviceValidFailed),
                    ErrorDetails = exception.StackTrace
                }.ToJsonString(),
                "/api/Security/RegisterWithChangePWD" => new LogInResponse()
                {
                    isAuthenticate = false,
                    authenticationMessage = errorMsg,
                    hasUpdate = false,
                    ErrorDetails = exception.StackTrace
                }.ToJsonString(),
                "/api/Security/Login" => new LogInResponse()
                {
                    isAuthenticate = false,
                    isThisVersionBlocked = false,
                    authenticationMessage = errorMsg,
                    hasUpdate = false,
                    ErrorDetails = exception.StackTrace
                }.ToJsonString(),
                "/api/Security/RegisterWithOTPValidation" => new LogInResponse()
                {
                    isAuthenticate = false,
                    authenticationMessage = errorMsg,
                    hasUpdate = false,
                    ErrorDetails = exception.StackTrace
                }.ToJsonString(),
                "api/Security/InternalLogin" => new LogInResponse()
                {
                    isAuthenticate = false,
                    authenticationMessage = Message.SomethingWentWrong,
                    hasUpdate = false,
                    ErrorDetails = exception.StackTrace
                }.ToJsonString(),
                "/api/Security/DevicePWDValidation" or
                "/api/EvRecharge" or "/api/v2/EvRecharge" or
                "/api/MultiEvRecharge" or "/api/v2/MultiEvRecharge" or
                "/api/IrisRecharge" or "/api/v2/IrisRecharge" or
                "/api/IRISOffers" or "/api/v2/IRISOffers" or
                "/api/GetItopUpStockDetails" or "/api/v2/GetItopUpStockDetails" or
                "/api/GetAppSettings" or
                "/api/v2/ShowAllIRISOffers" or "/api/v2/ShowAllIRISOffersV2" or "/api/ShowAllIRISOffers" or
                "/api/SendIrisOfferToCustomer" or "/api/v2/SendIrisOfferToCustomer" or
                "/api/ResetEVPinReq" or "/api/v2/ResetEVPinReq" or
                "/api/ChangeEVPin" or "/api/v2/ChangeEVPin" or
                "/api/GetRetailerStock" or "/api/v2/GetRetailerStock" or
                "/api/ValidateIRISDenoOffer" or "/api/v2/ValidateIRISDenoOffer" => new ResponseMessage()
                {
                    isError = true,
                    message = errorMsg,
                    ErrorDetails = exception.StackTrace
                }.ToJsonString(),
                "/api/UpdateDigitalServiceStatus" or
                "/api/UpdateEVPinResetStatus" => new ExternalSubmitResponse()
                {
                    success = false,
                    message = errorMsg
                }.ToJsonString(),
                "/api/RaiseComplaintHistory" => new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong),
                    data = Array.Empty<RaiseComplaintHistoryModel>(),
                    ErrorDetails = exception.StackTrace
                }.ToJsonString(),
                "/api/SyncRetailerInfo" => new RACommonResponse()
                {
                    result = false,
                    message = errorMsg
                }.ToJsonString(),
                _ => new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong),
                    ErrorDetails = exception.StackTrace
                }.ToJsonString(),
            };
        }


        private static void SaveLog(LogModel _log)
        {
            _log.apiEndTime = DateTime.Now;
            _log.totalApiTimeInS = (_log.apiEndTime - _log.apiStartTime).TotalSeconds;

            switch (_log.methodName)
            {
                case "/":
                case "/home/index":
                case "/wwwroot/bootstrap_5_0_2/css/bootstrap.min.css":
                case "/wwwroot/bootstrap_5_0_2/js/bootstrap.bundle.min.js":
                    break;
                case "/api/SyncRetailerInfo":
                    ExternalAPICallVM extLog = ExternalAPICallVM.LogModelToExternalModel(_log);
                    LoggerService.WriteDmsLogInText(extLog);
                    break;
                default:
                    if (TextLogging.IsEnableApiTextLog)
                    {
                        LoggerService.WriteApiLogInText(_log);
                    }
                    break;
            }
        }

        #endregion==========| End of Private Methods |==========

    }
}