///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	Stock Controller
///	Creation Date :	08-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Application.Services;
using Application.Services.v2;
using Domain.Helpers;
using Domain.LMS;
using Domain.LMS.Request;
using Domain.RequestModel;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using static Domain.Enums.EnumCollections;

namespace RetailerSelfCareApi.Controllers.v2
{
    [Route("api")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpPost]
        [Route("GetSCSummaryDetails")]
        public async Task<IActionResult> GetSCSummaryDetails([FromBody] RetailerRequest retailer)
        {
            StockV2Service stockService;
            //SC Summary
            DataTable scStock = new();
            try
            {
                stockService = new(Connections.RetAppDbCS);
                scStock = stockService.GetSCStocksSummaryV2(retailer);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSCStocksSummaryV2"));
            }

            StockSummaryModel scSummary = HelperMethod.ModelBinding<StockSummaryModel>(scStock.Rows[0], "", "SC");

            //SC Details
            StockDetialRequest scDetailsReq = new()
            {
                retailerCode = retailer.retailerCode
            };

            DataTable scDetails = new();
            try
            {
                stockService = new();
                scDetails = await stockService.GetScStockDetails(scDetailsReq);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetScStockDetails"));
            }

            List<StockDetialsModel> stockDetials = scDetails.AsEnumerable().Select(row => HelperMethod.ModelBinding<StockDetialsModel>(row)).ToList();

            var data = new
            {
                scSummary.itemCode,
                scSummary.itemTitle,
                scSummary.quantity,
                scSummary.amount,
                scSummary.updateTime,
                stockDetials
            };

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = data
            });
        }


        [HttpPost]
        [Route("GetStockSummary")]
        public async Task<IActionResult> GetStockSummary([FromBody] RetailerRequestV2 retailer)
        {
            List<StockSummaryModel> stockSummaries = [];
            StockV2Service stockService;

            //SIM and SC
            DataTable simscStock = new();

            try
            {
                stockService = new(Connections.RetAppDbCS);
                simscStock = await stockService.GetSIMSCStocksSummary(retailer);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSIMSCStocksSummary"));
            }

            if (simscStock.Rows.Count > 0)
            {
                var stockList = simscStock.AsEnumerable().Select(row => HelperMethod.ModelBinding<StockSummaryModel>(row, row["ITEM"].ToString(), row["ITEM"].ToString())).ToList();
                stockSummaries.AddRange(stockList);
            }

            //ITopUP - Not Current Balance 
            stockService = new();
            DataTable itopUpStock = new();

            try
            {
                itopUpStock = await stockService.GetItopUpSummary(retailer);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetItopUpSummary"));
            }

            if (itopUpStock.Rows.Count > 0)
            {
                stockSummaries.Add(HelperMethod.ModelBinding<StockSummaryModel>(itopUpStock.Rows[0], "ITopUp", "iTopUp"));
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = stockSummaries
            });
        }


        [HttpPost]
        [Route("GetStockDetails")]
        public async Task<IActionResult> GetStockDetails([FromBody] StockDetialRequest detailRequest)
        {
            string traceMsg = string.Empty;

            StockV2Service stockService;
            List<StockDetialsModel> stockDetials = [];
            bool status = true;
            EvXmlResponse itopupSock = new();
            RetailerSessionCheck retailer = new();

            stockService = new();
            string loginProvider = Request.HttpContext.Items["loginProviderId"] as string;

            retailer = await stockService.CheckRetailerByCode(detailRequest.retailerCode, loginProvider);

            if (string.IsNullOrEmpty(loginProvider) || !retailer.isSessionValid)
            {
                return Unauthorized(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("InvalidSession", Message.InvalidSession)
                });
            }

            if (detailRequest.itemCode == 1)//SC
            {
                stockService = new();
                DataTable scDetails = await stockService.GetScStockDetails(detailRequest);
                stockDetials = scDetails.AsEnumerable().Select(row => new StockDetialsModel(row)).ToList();
            }
            else if (detailRequest.itemCode == 2)//SIM
            {
                stockService = new(Connections.RetAppDbCS);
                DataTable simDetails = stockService.GetSimStockDetails(detailRequest);
                stockDetials = simDetails.AsEnumerable().Select(row => new StockDetialsModel(row)).ToList();
            }
            else if (detailRequest.itemCode == 3)//ITopUp Current Balance
            {
                string evUrl = ExternalKeys.EvURL;

                ItopUpXmlRequest xmlRequest = new()
                {
                    Url = evUrl,
                    Type = "EXUSRBALREQ",
                    Date = "",
                    Extnwcode = "BD",
                    Msisdn = retailer.msisdn,
                    Pin = detailRequest.userPin,
                    Loginid = "",
                    Pass = "",
                    Extcode = "",
                    Extrefnum = "",
                    Language1 = "0"
                };

                stockService = new();
                itopupSock = stockService.GetITOPUPStockSummary(xmlRequest, detailRequest);
                traceMsg = itopupSock.message;

                stockService = new();
                StockDetialsModel stockDetail = stockService.StockDetialModelMaker(itopupSock);

                stockDetials.Add(stockDetail);
                status = itopupSock.txnStatus.Equals("200");

                if (status)
                {
                    DateTime _dateTime = DateTime.ParseExact(stockDetail.dateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string dateTime = _dateTime.ToEnUSDateString("hh:mm:ss tt, dd MMM yyyy");
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, $"{stockDetail.dateTime};{dateTime}", null);

                    try
                    {
                        VMItopUpStock model = new()
                        {
                            RetailerCode = detailRequest.retailerCode,
                            NewBalance = Convert.ToDouble(stockDetail.amount),
                            UpdateTime = dateTime
                        };

                        stockService = new();
                        int res = await stockService.UpdateItopUpBalance(model);
                        if (res == 0)
                        {
                            traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "Unable to update Retailer Balance;", null);
                        }
                    }
                    catch (Exception ex)
                    {
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, dateTime, null);
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "UpdateItopUpBalance", ex);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(detailRequest, "v2/GetStockDetails", traceMsg);
            }

            return new OkObjectResult(new ResponseMessage()
            {
                isError = !status,
                message = detailRequest.itemCode == 3 & !status ? itopupSock.message : SharedResource.GetLocal("Success", Message.Success),
                data = status ? stockDetials : (new string[] { })
            });
        }


        [HttpPost]
        [Route(nameof(GetItopUpStockDetails))]
        public async Task<IActionResult> GetItopUpStockDetails([FromBody] RetailerRequestV2 retailerRequest)
        {
            string traceMsg = string.Empty;
            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "GetItopUpStockDetails",
                retailerCode = retailerRequest.retailerCode,
                appPage = LMSAppPages.Stock_Update,
                transactionID = LMSService.GetTransactionId(retailerRequest.retailerCode),
                msisdn = $"88{retailerRequest.iTopUpNumber}",
                points = LMSPoints.Stock_Update.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            int isEligible;
            string rsoNumber;
            double currBlnc;
            string updateTime;

            try
            {
                RetailerV2Service retailerService = new();
                DataTable dt = await retailerService.GetRSOAndLastTime(retailerRequest);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];
                    rsoNumber = dr["SRNUMBER"] as string;
                    isEligible = Convert.ToInt32(dr["IS_ELIGIBLE"]);
                    currBlnc = Convert.ToDouble(dr["CURR_BALANCE"]);
                    updateTime = dr["UPDATETIME"] as string;
                }
                else
                {
                    string msg = EvIrisMessageParsing.ParseMessage(Message.NoRsoDataFound);
                    throw new Exception(msg);
                }

                if (string.IsNullOrWhiteSpace(rsoNumber))
                {
                    string msg = EvIrisMessageParsing.ParseMessage(Message.NoRsoDataFound);
                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = true,
                        message = msg
                    });
                }
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "GetRSOAndLastTime", ex);
                throw;
            }

            DataRow emptyDr = new DataTable().NewRow();
            StockSummaryModel stockSummary = new(emptyDr, "iTopUp");
            if (isEligible == 1)
            {
                string evPinLessBalanceUrl = ExternalKeys.EvPinLessBlncURL;

                ItopUpXmlRequestV2 xmlRequest = new()
                {
                    Url = evPinLessBalanceUrl,
                    Type = "OTHERBALAN",
                    Msisdn = rsoNumber.Substring(0),
                    Pin = "",
                    Loginid = "",
                    Password = "",
                    DateTime = "",
                    Imei = "",
                    Msisdn2 = retailerRequest.iTopUpNumber.Substring(0),
                    Language1 = "0",
                    Extrefnum = "123456"
                };

                StockV2Service stockService = new(Connections.RetAppDbCS);
                var userAgent = HttpContext.Request?.Headers.UserAgent.ToString();
                string resp = stockService.GetITOPUPBalance(xmlRequest, retailerRequest, userAgent);

                try
                {
                    stockService = new(Connections.RetAppDbCS);
                    stockService.FormatEvBalanceResponse(ref stockSummary, resp);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "FormatEvBalanceResponse"));
                }

                try
                {
                    VMItopUpStock model = new()
                    {
                        ItopUpNumber = retailerRequest.iTopUpNumber.Substring(1),
                        RetailerCode = retailerRequest.retailerCode,
                        NewBalance = Convert.ToDouble(stockSummary.amount),
                        UpdateTime = stockSummary.updateTime
                    };

                    stockService = new();
                    int res = await stockService.UpdateItopUpBalance(model);
                    if (res == 0)
                    {
                        traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "Unable to update Retailer Balance;", null);
                    }
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, stockSummary.updateTime, null);
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "UpdateItopUpBalance", ex);
                }

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(retailerRequest, "v2/GetItopUpStockDetails", traceMsg);
                }

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = stockSummary
                });
            }
            else if (isEligible == 0)
            {
                stockSummary.amount = currBlnc.ToString();
                stockSummary.updateTime = updateTime;

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = stockSummary
                });
            }
            else
            {
                throw new Exception("Unable to check eligibility.");
            }
        }


        #region Scratch Card Sale from Retailer App. This feature not using right now

        [HttpPost]
        [Route("GetSCList")]
        public async Task<IActionResult> GetSCList([FromBody] SCListRequestModel reqModel)
        {
            DataTable dt = new();

            try
            {
                StockV2Service stockService = new(Connections.RetAppDbCS);
                dt = await stockService.GetFilteredScList(reqModel);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetFilteredScList"));
            }

            List<dynamic> scList = dt.AsEnumerable().Select(row => (dynamic)new { scNumber = row["SC_SERIAL_NO"] as string, amount = Convert.ToInt32(row["AMOUNT"]), productCode = row["PRODUCTCODE"] as string }).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = scList
            });
        }


        [HttpPost]
        [Route("GetSCExpiry")]
        public IActionResult SCExpires([FromBody] RetailerRequest request)
        {
            StockV2Service rechargeService = new(Connections.RetAppDbCS);
            DataTable scDetails = rechargeService.ScExpire(request.retailerCode);
            List<ScExpireModel> scExpireModels = scDetails.AsEnumerable().Select(row => new ScExpireModel(row)).ToList();

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = scExpireModels
            });
        }


        [HttpPost]
        [Route("SubmitSCSalesData")]
        public async Task<IActionResult> SubmitSCSalesData([FromBody] SCSalesRequest reqModel)
        {
            string traceMsg = string.Empty;
            long res = 0;

            using (StockV2Service stockService = new(Connections.RetAppDbCS))
            {
                res = await stockService.SubmitScratchCardData(reqModel);
            }

            if (res > 0)
            {
                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("SCSalesSuccess", Message.SCSalesSuccess)
                });
            }
            else
            {
                string msg = "Unable to sold SC Number: " + reqModel.scNumber + " || Customer: " + reqModel.customerMsisdn;
                msg += " || Primary Reason: No Data Exist or Already Sold.";
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, msg, null);

                if (!string.IsNullOrWhiteSpace(traceMsg))
                {
                    LoggerService _logger = new();
                    _logger.WriteTraceMessageInText(reqModel, "v2/SubmitSCSalesData", traceMsg);
                }

                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("SCSalesFailed", Message.SCSalesFailed)
                });
            }
        }


        [HttpPost]
        [Route("SCSalesHistory")]
        public async Task<IActionResult> SCSalesHistory([FromBody] HistoryPageRequestModel reqModel)
        {
            List<SCSellsHistoryModel> scHitoryList = new();
            DataTable dt = new();

            try
            {
                StockV2Service stockService = new(Connections.RetAppDbCS);
                dt = await stockService.GetSCSalesHistory(reqModel);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSCSalesHistory"));
            }

            scHitoryList = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<SCSellsHistoryModel>(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = scHitoryList
            });
        }

        #endregion

    }
}