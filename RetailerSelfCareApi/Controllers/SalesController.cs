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

namespace RetailerSelfCareApi.Controllers
{
    [Route("api/v2")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        #region=======================|    MTD Sales Section   |======================

        [HttpPost]
        [Route("GetSalesUpdate")]
        public async Task<IActionResult> GetSalesUpdate([FromBody] RetailerRequest retailerRequest)
        {
            SalesService salesService = new(Connections.DMSCS);
            DataTable salesUpdate = await salesService.GetSalesUpdate(retailerRequest);
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
            SalesService salesService = new(Connections.DMSCS);
            DataTable salesTend = await salesService.GetSalesWeeklyTrend(retailerRequest);
            List<SalesTendModel> salesTends = salesTend.AsEnumerable().Select(row => new SalesTendModel(row)).ToList();

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
            SalesService salesService = new(Connections.DMSCS);
            DataTable salesRoutePerform = await salesService.GetSalesRoutePerformance(retailerRequest);
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
        public async Task<IActionResult> ThreeDaysMemo([FromBody] RetailerRequest retailerRequest)
        {
            SalesService salesService = new(Connections.DMSCS);
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
            DataTable todaysSales;
            using(SalesService salesService = new(Connections.DMSCS))
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
            RedisCache redis;

            List<SalesSummaryModel> summary = [];

            try
            {
                redis = new RedisCache();
                var redPkgDetailsStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerSalesSummary, retailer.retailerCode);

                if (!string.IsNullOrEmpty(redPkgDetailsStr))
                {
                    summary = JsonConvert.DeserializeObject<List<SalesSummaryModel>>(redPkgDetailsStr)!;
                }
                else
                {
                    SalesService salesService = new(Connections.DMSCS);
                    DataTable sales = new();

                    try
                    {
                        sales = await salesService.GetSalesSummaryV2(retailer);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesSummaryV2"));
                    }

                    summary = sales.AsEnumerable().Select(row => HelperMethod.ModelBinding<SalesSummaryModel>(row)).ToList();

                    redis = new RedisCache();
                    await redis.SetCacheAsync(RedisCollectionNames.RetailerSalesSummary, retailer.retailerCode, summary.ToJsonString());
                }
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "", ex);
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(retailer, "GetSalesSummery", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = summary
            });
        }


        [HttpPost]
        [Route(nameof(GetSalesDetails))]
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

            using(LMSService lmsService = new())
            {
                await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);
            }

            DataTable sales;
            using (SalesService salesService = new(Connections.DMSCS))
            {
                sales = await salesService.GetSalesDetails(salesDetails);
            }
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