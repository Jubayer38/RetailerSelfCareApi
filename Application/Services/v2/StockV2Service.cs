///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	
///	Creation Date :	08-Jan-2024
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
using Infrastracture.Repositories.v2;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application.Services.v2
{
    public class StockV2Service : IDisposable
    {
        private readonly StockV2Repository _repo;

        public StockV2Service()
        {
            _repo = new();
        }

        public StockV2Service(string connectionString)
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


        public async Task<RetailerSessionCheck> CheckRetailerByCode(string retailerCode, string loginProvider)
        {
            try
            {
                var result = await _repo.CheckRetailerByCode(retailerCode, loginProvider);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "v2/CheckRetailerByCode"));
            }
        }


        public async Task<DataTable> GetScStockDetails(StockDetialRequest request)
        {
            try
            {
                var result = await _repo.GetScStockDetails(request);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "v2/GetScStockDetails"));
            }
        }


        public DataTable GetSimStockDetails(StockDetialRequest request)
        {
            try
            {
                var result = _repo.GetSimStockDetails(request);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "v2/GetSimStockDetails"));
            }
        }


        public EvXmlResponse GetITOPUPStockSummary(ItopUpXmlRequest xmlRequest, StockDetialRequest stockDetialRequest)
        {
            try
            {
                var result = GetITOPUPStockSummaryData(xmlRequest, stockDetialRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "v2/GetITOPUPStockSummaryData"));
            }
        }


        public StockDetialsModel StockDetialModelMaker(EvXmlResponse xmlResponse)
        {
            DataRow dr = new DataTable().NewRow();
            StockDetialsModel stockDetialsModel = new(dr);
            if (xmlResponse?.Record == null) return stockDetialsModel;

            stockDetialsModel.amount = xmlResponse.Record.balance;
            stockDetialsModel.message = xmlResponse.message;
            stockDetialsModel.quantity = "0";
            stockDetialsModel.categoryTitle = xmlResponse.Record.productShortName;

            //string[] dateTimeSplit = xmlResponse.date.Split(' ');
            //string[] dateSplit = dateTimeSplit[0].Split('/');

            stockDetialsModel.dateTime = xmlResponse.date;

            return stockDetialsModel;
        }


        public async Task<int> UpdateItopUpBalance(VMItopUpStock model)
        {
            var result = await _repo.UpdateItopUpBalance(model);
            return result;
        }


        public DataTable ScExpire(string retailerCode)
        {
            try
            {
                var result = _repo.ScExpire(retailerCode);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "v2/ScExpire"));
            }
        }


        public async Task<DataTable> GetRetailerByCode(string retailerCode)
        {
            try
            {
                var result = await _repo.GetRetailerByCode(retailerCode);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "v2/GetRetailerByCode"));
            }
        }


        public string GetRetailerMSISDN(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                string retMsisdn = dt.Rows[0]["MSISDN"] as string;
                retMsisdn = retMsisdn.Substring(1);

                return retMsisdn;
            }
            else
            {
                string errMsg = "Retailer MSISDN not found in RetAppDB_split_GetRetailerByCode";
                throw new Exception(errMsg);
            }
        }


        public DataTable GetSCStocksSummaryV2(RetailerRequest retailer)
        {
            try
            {
                var result = _repo.GetSCStocksSummaryV2(retailer);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "v2/GetSCStocksSummaryV2"));
            }
        }


        public string GetITOPUPBalance(ItopUpXmlRequestV2 xmlRequest, RetailerRequestV2 stockDetialRequest, string userAgent)
        {
            LoggerService loggerService = new();
            EvLogViewModel evLog = new();
            evLog = HelperMethod.LogModelBind(stockDetialRequest, evLog, "v2/GetITOPUPBalance", userAgent);
            evLog.amount = string.Empty;
            evLog.subMSISDN = string.Empty;

            string reqBody = string.Empty;
            string response = string.Empty;

            try
            {
                reqBody = BuildEvPinLessBlncReqBody(xmlRequest);
                response = XMLService.PostTextData(xmlRequest.Url, reqBody);

                evLog.tranMsg = response;
                return response;
            }
            catch (Exception ex)
            {
                evLog.isSuccess = 0;
                evLog.errorMessage = HelperMethod.FormattedExceptionMsg(ex);
                throw;
            }
            finally
            {
                #region==========================|| LOG ||=============================
                evLog.reqBodyStr = reqBody;
                if (!string.IsNullOrEmpty(response))
                    evLog.resBodyStr = response;

                if (TextLogging.IsEnableEVTextLog)
                {
                    evLog.endTime = DateTime.Now;
                    loggerService.WriteEVLogInText(evLog);
                }
                #endregion==========|| Log ||==========
            }
        }


        public void FormatEvBalanceResponse(ref StockSummaryModel stockSummary, string resp)
        {
            if (resp != null)
            {
                EVPinLessBalanceResponse balanceResponse = new(resp);

                if (balanceResponse.TxnStatus.Equals("200"))
                {
                    DateTime _dateTime = DateTime.ParseExact(balanceResponse.DateTime, "dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture);

                    Regex regex = new(@"(?<=eTopUP:\s*)\d+(\.\d+)?");
                    Match match = regex.Match(balanceResponse.Message);
                    string amount = match.Success ? match.Value : "0";

                    stockSummary.itemCode = "3";
                    stockSummary.itemTitle = "iTopUp";
                    stockSummary.quantity = "0";
                    stockSummary.amount = amount;
                    stockSummary.updateTime = _dateTime.ToEnUSDateString("hh:mm:ss tt, dd MMM yyyy");
                }
                else
                {
                    string message = EvIrisMessageParsing.ParseMessage(balanceResponse.Message);
                    throw new Exception(message);
                }
            }
            else
            {
                throw new Exception("No Balance Data Found.");
            }
        }


        public async Task<DataTable> GetFilteredScList(SCListRequestModel reqModel)
        {
            return await _repo.GetFilteredScList(reqModel);
        }


        public async Task<long> SubmitScratchCardData(SCSalesRequest reqModel)
        {
            return await _repo.SubmitScratchCardData(reqModel);
        }


        public async Task<DataTable> GetSCSalesHistory(HistoryPageRequestModel reqModel)
        {
            return await _repo.GetSCSalesHistory(reqModel);
        }


        public async Task<DataTable> GetSIMSCStocksSummary(RetailerRequest retailer)
        {
            return await _repo.GetSIMSCStocksSummary(retailer);
        }


        public async Task<DataTable> GetItopUpSummary(RetailerRequest retailer)
        {
            return await _repo.GetItopUpSummary(retailer); ;
        }


        /// <summary>
        /// Get I-TopUp current balance without PIN from EV BY RSO
        /// </summary>
        /// <param name="xmlRequest"></param>
        /// <param name="stockDetialRequest"></param>
        /// <returns></returns>
        public string GetITopUpBalanceNew(RSOEligibility rsoEligibility, ItopUpXmlRequestV2 xmlRequest, string userAgent)
        {
            LoggerService loggerService = new();
            EvLogViewModel evLog = new();
            evLog = HelperMethod.LogModelBind(xmlRequest, evLog, "v2/GetITopUpBalanceNew", userAgent);
            evLog.amount = string.Empty;
            evLog.subMSISDN = string.Empty;

            string reqBody = "";
            string response = "";

            try
            {
                reqBody = BuildEvPinLessBlncReqBody(xmlRequest);
                response = XMLService.PostTextData(xmlRequest.Url, reqBody);

                evLog.tranMsg = response;
                return response;
            }
            catch (Exception ex)
            {
                evLog.isSuccess = 0;
                evLog.errorMessage = HelperMethod.FormattedExceptionMsg(ex);
                return null;
            }
            finally
            {
                #region==========|| LOG ||==========
                evLog.retailerCode = rsoEligibility.retailerCode;
                evLog.reqBodyStr = reqBody;
                if (!string.IsNullOrEmpty(response))
                    evLog.resBodyStr = response;

                if (TextLogging.IsEnableEVTextLog)
                {
                    evLog.endTime = DateTime.Now;
                    loggerService.WriteEVLogInText(evLog);
                }
                #endregion==========|| LOG ||==========
            }
        }


        #region==========|| Private Methods ||==========

        private static EvXmlResponse GetITOPUPStockSummaryData(ItopUpXmlRequest xmlRequest, StockDetialRequest stockDetialRequest)
        {
            EvLogViewModel evLog = new()
            {
                startTime = DateTime.Now,
                isSuccess = 1,
                errorMessage = "",
                amount = ""
            };

            string xmlReq = GetItopUpBalReqXML(xmlRequest);
            string responseXML = XMLService.PostXMLData(xmlRequest.Url, xmlReq);
            EvXmlResponse itopUpStock = (EvXmlResponse)XMLService.ParseXML(responseXML, typeof(EvXmlResponse));

            if (itopUpStock.Record != null)
            {
                evLog.amount = itopUpStock.Record.balance;
            }

            return itopUpStock;
        }


        private static string GetItopUpBalReqXML(ItopUpXmlRequest xml)
        {
            string reqXML = @"<?xml version=""1.0""?>
                    <!DOCTYPE COMMAND PUBLIC ""-//Ocam//DTD XML Command 1.0//EN"" ""xml/command.dtd"">
                    <COMMAND>
                        <TYPE>" + xml.Type + @"</TYPE>
                        <DATE>" + xml.Date + @"</DATE>
                        <EXTNWCODE>" + xml.Extnwcode + @"</EXTNWCODE>
                        <MSISDN>" + xml.Msisdn + @"</MSISDN>
                        <PIN>" + xml.Pin + @"</PIN>
                        <LOGINID>" + xml.Loginid + @"</LOGINID>
                        <PASSWORD>" + xml.Pass + @"</PASSWORD>
                        <EXTCODE>" + xml.Extcode + @"</EXTCODE>
                        <EXTREFNUM>" + xml.Extrefnum + @"</EXTREFNUM>
                        <LANGUAGE1>" + xml.Language1 + @"</LANGUAGE1>
                    </COMMAND>";
            return reqXML;
        }


        private static string BuildEvPinLessBlncReqBody(ItopUpXmlRequestV2 xmlRequest)
        {
            string reqBody = "TYPE=" + xmlRequest.Type + "&MSISDN=" + xmlRequest.Msisdn + "&PIN=" + xmlRequest.Pin + "&LOGINID=" + xmlRequest.Loginid + "&PASSWORD=" + xmlRequest.Password + "&DATETIME=" + xmlRequest.DateTime + "&IMEI=" + xmlRequest.Imei + "&MSISDN2=" + xmlRequest.Msisdn2 + "&LANGUAGE1=" + xmlRequest.Language1 + "&EXTREFNUM=" + xmlRequest.Extrefnum + "";
            // example
            //string reqBody = "TYPE=" + xmlRequest.Type + "&MSISDN=1984207861&PIN=" + xmlRequest.Pin + "&LOGINID=" + xmlRequest.Loginid + "&PASSWORD=" + xmlRequest.Password + "&DATETIME=" + xmlRequest.DateTime + "&IMEI=" + xmlRequest.Imei + "&MSISDN2=1984207863&LANGUAGE1=" + xmlRequest.Language1 + "&EXTREFNUM=" + xmlRequest.Extrefnum + "";

            return reqBody;
        }

        #endregion==========|| Private Methods ||==========

    }
}