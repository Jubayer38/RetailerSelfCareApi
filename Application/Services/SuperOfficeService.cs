///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	SuperOffice service
///	Creation Date :	16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Newtonsoft.Json;
using System.Text;

namespace Application.Services
{
    public class SuperOfficeService
    {
        public async Task<long> SubmitTicketToSuperOffice(ComplaintSuperOfficeModel model)
        {
            ExternalAPICallVM externalApiVM = new();
            externalApiVM.reqStartTime = DateTime.Now;
            externalApiVM.isSuccess = 1;
            string logPath = TextLogging.TextLogPath;
            string reqXmlString = string.Empty;
            string respXmlString = string.Empty;

            long insertId = -1;

            try
            {
                reqXmlString = GenerateSubmitRequestXmlBody(model, "{sessionToken}");
                string _url = ExternalKeys.SuperOfficeInternalAPI;

                SOExternalCallReqModel extarnalReqBody = new()
                {
                    username = ExternalKeys.SuperOfficeInternalUser,
                    cred = ExternalKeys.SuperOfficeInternalCred,
                    retailerCode = model.retailerCode,
                    soBody = reqXmlString
                };

                HttpRequestModel httpReq = new()
                {
                    requestUrl = _url,
                    requestBody = extarnalReqBody,
                    requestMediaType = MimeTypes.Json,
                    requestMethod = "SubmitTicketToSuperOffice"
                };

                HttpService httpService = new();
                var soReturnObj = await httpService.CallExternalApi<SOResponse>(httpReq);

                string soReturnStr = JsonConvert.SerializeObject(soReturnObj.Object, Newtonsoft.Json.Formatting.None);

                SOResponse soResponse = JsonConvert.DeserializeObject<SOResponse>(soReturnStr)!;

                if (soResponse.isSuccess)
                {
                    insertId = Convert.ToInt64(soResponse.data);
                }

                externalApiVM.reqBodyStr = reqXmlString;
                externalApiVM.resBodyStr = respXmlString;
                return insertId;
            }
            catch (Exception ex)
            {
                externalApiVM.isSuccess = 0;
                externalApiVM.errorMessage = HelperMethod.ExMsgSubString(ex, "", 400);
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitTicketToSuperOffice"));
            }
            finally
            {
                #region====================|SAVE EXTERNAL API LOG|==========================
                externalApiVM.retailerCode = model.retailerCode;
                externalApiVM.methodName = "SubmitTicketToSuperOffice";

                if (TextLogging.IsEnableExternalTextLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteExternalLogInText(externalApiVM);
                }
                #endregion
            }
        }


        public async Task<SOTicket> GetSOTicketByID(string retailerCode, long soId)
        {
            ExternalAPICallVM externalApiVM = new()
            {
                reqStartTime = DateTime.Now
            };
            string reqXmlString = string.Empty;
            string respXmlString = string.Empty;
            SOTicket respModel = new();

            try
            {
                string _url = ExternalKeys.SuperOfficeInternalAPI;

                var today = DateTime.Now;
                var toDate = today.ToEnUSDateString("dd/M/yyyy");

                var startDate = today.AddDays(-14);
                var fromDate = startDate.ToEnUSDateString("dd/M/yyyy");

                string requestBody = "<soapenv:Body><mes:executeScript><p-sessionKey>" + "{sessionToken}" + "</p-sessionKey><p-includeName>getTickets</p-includeName><p-parameters><item><field>TicketId</field><value>" + soId + "</value><field>DateFrom</field><value>" + fromDate + "</value><field>DateTo</field><value>" + toDate + "</value></item></p-parameters></mes:executeScript></soapenv:Body>";

                SOExternalCallReqModel extarnalReqBody = new()
                {
                    username = ExternalKeys.SuperOfficeInternalUser,
                    cred = ExternalKeys.SuperOfficeInternalCred,
                    retailerCode = retailerCode,
                    soBody = requestBody
                };

                HttpRequestModel httpReq = new()
                {
                    requestUrl = _url,
                    requestBody = extarnalReqBody,
                    requestMediaType = MimeTypes.Json,
                    requestMethod = "SubmitTicketToSuperOffice"
                };

                string soReturnStr;

                using (HttpService httpService = new())
                {
                    var soReturnObj = await httpService.CallExternalApi<SOResponse>(httpReq);

                    soReturnStr = JsonConvert.SerializeObject(soReturnObj.Object, Newtonsoft.Json.Formatting.None);
                }

                SOResponse soResponse = JsonConvert.DeserializeObject<SOResponse>(soReturnStr)!;

                if (soResponse.isSuccess)
                {
                    respModel = JsonConvert.DeserializeObject<SOTicket>(soResponse.data)!;
                }
                else
                {
                    respModel.tickets[0].ticketId = 0;
                }

                externalApiVM.retailerCode = retailerCode;
                externalApiVM.methodName = "GetSOTicketByID";
                externalApiVM.reqBodyStr = requestBody;
                externalApiVM.resBodyStr = soResponse.ToJsonString();
                externalApiVM.isSuccess = soResponse.isSuccess ? 1 : 0;
                return respModel;
            }
            catch (Exception ex)
            {
                externalApiVM.errorMessage = ex.InnerException.Message;
                throw new Exception(ex.InnerException.Message);
            }
            finally
            {
                if (TextLogging.IsEnableExternalTextLog)
                {
                    externalApiVM.reqEndTime = DateTime.Now;
                    LoggerService.WriteExternalLogInText(externalApiVM);
                }
            }
        }


        #region==========|| Private Methods ||==========

        private static string GenerateSubmitRequestXmlBody(ComplaintSuperOfficeModel model, string sessionToken)
        {
            //        "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:mes=\"http://tempuri.org/message/\">" +
            //"<soapenv:Header/>" +

            string requestXmlFormat = "<soapenv:Body>" + "<mes:executeScript>" + "<p-sessionKey>{0}</p-sessionKey>" + "<p-includeName>createTicket</p-includeName>" + "<p-parameters>" + "<item><field>MSISDN</field><value>88{1}</value></item>" + "<item><field>CategoryId</field><value>{2}</value></item>" + "<item><field>ComplainDetails</field><value>{3}</value></item>" + "<item><field>ChannelName</field><value>Retailer App</value></item>" + "AddParameters" + "</p-parameters>" + "</mes:executeScript>" + "</soapenv:Body>";

            //"</soapenv:Envelope>"

            string fileParams = "<item><field>AttachmentFilename</field><value>{4}</value></item>" +
                "<item><field>AttachmentBase64</field><value>{5}</value></item>";

            StringBuilder sb = new();

            if (string.IsNullOrEmpty(model.image))
                sb.AppendFormat(requestXmlFormat, sessionToken, model.iTopUpNumber, model.soSubCategoryId, model.description);
            else
            {
                requestXmlFormat = requestXmlFormat.Replace("AddParameters", fileParams);
                sb.AppendFormat(requestXmlFormat, sessionToken, model.iTopUpNumber, model.soSubCategoryId, model.description, model.fileName, model.image);
            }

            return sb.ToString();

        }

    }

    #endregion==========|| Private Methods ||==========

}

