///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	Sales Controller
///	Creation Date :	09-Jan-2024
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
using Infrastracture.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using static Domain.Enums.EnumCollections;

namespace RetailerSelfCareApi.Controllers.v2
{
    [Route("api")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        #region=======================|    MTD Sales Section   |======================

        [HttpPost]
        [Route("GetSalesUpdate")]
        public async Task<IActionResult> GetSalesUpdate([FromBody] RetailerRequest retailerRequest)
        {
            DataTable salesUpdate;
            using (SalesV2Service salesService = new())
            {
                salesUpdate = await salesService.GetSalesUpdate(retailerRequest);
            }
            List<SalesUpdateModel> salesUpdates = salesUpdate.AsEnumerable().Select(row => new SalesUpdateModel(row)).ToList();

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = salesUpdates
            });
        }


        [HttpPost]
        [Route("GetSalesWeeklyTrend")]
        public async Task<IActionResult> GetSalesWeeklyTrend([FromBody] RetailerRequest retailerRequest)
        {
            List<SalesTendModel> salesTends = [];
            using (SalesV2Service salesService = new())
            {
                DataTable salesTend = await salesService.GetSalesWeeklyTrend(retailerRequest);
                salesTends = salesTend.AsEnumerable().Select(row => new SalesTendModel(row)).ToList();
            }


            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = salesTends
            });

        }


        [HttpPost]
        [Route("GetSalesRoutePerformance")]
        public async Task<IActionResult> GetSalesRoutePerformance([FromBody] RetailerRequest retailerRequest)
        {
            DataTable salesRoutePerform;
            using (SalesV2Service salesService = new())
            {
                salesRoutePerform = await salesService.GetSalesRoutePerformance(retailerRequest);
            }

            List<SalesPerformModel> salesRoutePerforms = salesRoutePerform.AsEnumerable().Select(row => new SalesPerformModel(row)).ToList();

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = salesRoutePerforms
            });
        }


        [HttpPost]
        [Route("ThreeDaysMemo")]
        public async Task<IActionResult> ThreeDaysSalesMemo([FromBody] RetailerRequest retailerRequest)
        {
            SalesV2Service salesService = new(Connections.RetAppDbCS);
            DataTable SalesMemo = await salesService.GetThreeDaysSalesMemo(retailerRequest);
            List<SalesMemoModel> SalesMemos = SalesMemo.AsEnumerable().Select(row => new SalesMemoModel(row)).ToList();

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = SalesMemos
            });
        }


        [HttpPost]
        [Route("TodaySalesMemo")]
        public async Task<IActionResult> TodaySalesMemo([FromBody] RetailerRequest retailerRequest)
        {
            DataTable todaysSales = new();

            using (SalesV2Service salesService = new(Connections.RetAppDbCS))
            {
                todaysSales = await salesService.GetTodaySalesMemo(retailerRequest);
            }
            List<TodaysSalesMemoModel> todaysSalesMemo = todaysSales.AsEnumerable().Select(row => new TodaysSalesMemoModel(row)).ToList();

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = todaysSalesMemo
            });
        }

        #endregion=======================|    MTD Sales Section   |======================

        /// <summary>
        /// Get Deno offers with recharge type. Applicable from APK v6.0.0 
        /// </summary>
        /// <param name="RetailerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetSalesSummery")]
        public async Task<IActionResult> GetMTDSalesSummary([FromBody] RetailerRequest retailer)
        {
            string traceMsg = string.Empty;
            string _dataKey = $"{retailer.retailerCode}_{DateTime.Now.Ticks}";
            string resultKey = string.Empty;
            bool isEligibleDBCall = false;
            RedisCache redis;

            List<SalesSummaryModel> summary = [];

            try
            {
                using (redis = new RedisCache())
                {
                    var redPkgDetailsStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerSalesSummaryMySql, retailer.retailerCode);

                    if (!string.IsNullOrEmpty(redPkgDetailsStr))
                    {
                        var resultDict = JsonConvert.DeserializeObject<Dictionary<string, List<SalesSummaryModel>>>(redPkgDetailsStr);
                        if (resultDict.Count > 0)
                        {
                            resultKey = resultDict.Keys.FirstOrDefault();
                            summary = resultDict.Values.FirstOrDefault();
                        }

                        isEligibleDBCall = HelperMethod.CheckSalesSummaryDbCallEligibility(resultKey, _dataKey);
                    }
                }

                if (summary.Count == 0 || isEligibleDBCall)
                {
                    DataTable sales = new();

                    using (SalesV2Service salesService = new(Connections.RetAppDbCS))
                    {
                        sales = salesService.GetSalesSummaryData(retailer);
                    }

                    summary = sales.AsEnumerable().Select(row => HelperMethod.ModelBinding<SalesSummaryModel>(row)).ToList();
                    if (summary.Count == 0)
                        summary = SalesSummaryModel.InitSalesModel();

                    Dictionary<string, List<SalesSummaryModel>> summaryDict = [];
                    summaryDict.Add(_dataKey, summary);
                    string dataStr = summaryDict.ToJsonString();

                    using (redis = new RedisCache())
                    {
                        await redis.DeleteAsync(RedisCollectionNames.RetailerSalesSummaryMySql, retailer.retailerCode);
                    }

                    using (redis = new RedisCache())
                    {
                        await redis.SetCacheAsync(RedisCollectionNames.RetailerSalesSummaryMySql, retailer.retailerCode, dataStr);
                    }
                }
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "", ex);
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(retailer, "v2/GetSalesSummary", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = summary
            });
        }


        [HttpPost]
        [Route("GetSalesDetails")]
        public async Task<IActionResult> GetSalesDetails([FromBody] SalesDetailRequest salesDetails)
        {
            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "GetSalesDetailsV2",
                retailerCode = salesDetails.retailerCode,
                transactionID = LMSService.GetTransactionId(salesDetails.retailerCode),
                msisdn = $"88{salesDetails.iTopUpNumber}",
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            switch (salesDetails.itemCode)
            {
                case 1:
                    pointAdjustReq.appPage = LMSAppPages.SC_Sales;
                    pointAdjustReq.points = LMSPoints.SC_Sales.ToString();
                    break;
                case 2:
                    pointAdjustReq.appPage = LMSAppPages.SIM_Sales;
                    pointAdjustReq.points = LMSPoints.SIM_Sales.ToString();
                    break;
                case 3:
                    pointAdjustReq.appPage = LMSAppPages.Itopup_Sales;
                    pointAdjustReq.points = LMSPoints.Itopup_Sales.ToString();
                    break;
            }

            LMSService lmsService = new();
            await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);

            SalesV2Service salesService = new(Connections.RetAppDbCS);
            DataTable sales = await salesService.GetSalesDetails(salesDetails);
            List<SalesDetailModel> salesDetailModels = sales.AsEnumerable().Select(row => new SalesDetailModel(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = salesDetailModels
            });

        }

    }
}