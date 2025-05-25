///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	Lifting Controller
///	Creation Date :	11-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Application.Services;
using Domain.Helpers;
using Domain.RequestModel;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static Domain.Enums.EnumCollections;

namespace RetailerSelfCareApi.Controllers
{
    [Route("api/v2")]
    [ApiController]
    public class LiftingController : ControllerBase
    {
        [HttpPost]
        [Route("ProductCategory")]
        public async Task<IActionResult> ProductCategory([FromBody] ProductCategoryRequest categoryRequest)
        {
            LiftingService liftingService = new(Connections.DMSCS);
            DataTable lifting = await liftingService.ProductCategory(categoryRequest);

            List<ProductCategoryModel> liftings = lifting.AsEnumerable().Select(row => new ProductCategoryModel(row)).ToList();

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = liftings
            });
        }


        [HttpPost]
        [Route("CheckSimStatus")]
        public async Task<IActionResult> CheckSimStatus([FromBody] SimStatusRequestModel simStatus)
        {
            LiftingService ewHomeService = new(Connections.DMSCS);
            string url = ExternalKeys.CheckSimUrl;
            SimStatusModel simAvailablity = await ewHomeService.CheckSimStatus(simStatus, url);

            return new OkObjectResult(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = simAvailablity
            });
        }


        [HttpPost]
        [Route("Lifting")]
        public async Task<IActionResult> Lifting([FromBody] LiftingRequest liftingRequest)
        {
            string traceMsg = string.Empty;
            LiftingService liftingService;
            liftingRequest.type = liftingRequest.type.ToUpper();

            int typeIndex = (int)Enum.Parse(typeof(OrderItemsRso), liftingRequest.type);
            liftingRequest.appVisibleType = ExtensionMethods.GetDescription((OrderItemsApp)typeIndex);

            long res = 0;
            try
            {
                liftingService = new(Connections.RetAppDbCS);
                res = await liftingService.GetExistRequest(liftingRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetExistRequest"));
            }
            if (res > 0)
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = "Already Exist the request for same product.",
                    data = Array.Empty<string>()
                });
            }

            if (liftingRequest.type == "EV")
            {
                liftingRequest.quantity = liftingRequest.amount;
                liftingRequest.category = "I-TOPUP";
            }

            long lifting = 0;
            try
            {
                liftingService = new(Connections.RetAppDbCS);
                lifting = await liftingService.SaveLiftingV3(liftingRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveLiftingV3"));
            }

            ExternalSubmitResponse rcExtSubmit = new();

            if (lifting > 0)
            {
                SubmitStockOrderToRSOModel toRSOModel = new()
                {
                    RequestId = lifting,
                    RetailerCode = liftingRequest.retailerCode,
                    ProductType = liftingRequest.type,
                    ProductCode = liftingRequest.category,
                    RequestProductCount = Convert.ToInt32(liftingRequest.quantity),
                    RetailerMsisdn = liftingRequest.iTopUpNumber,
                    PaymentType = liftingRequest.paymentType,
                    UserName = ExternalKeys.RsoUser,
                    Password = ExternalKeys.RsoCred,
                    SubmitUrl = ExternalKeys.StockSubmissionUrl
                };

                try
                {
                    HttpRequestModel httpModel = new()
                    {
                        requestBody = toRSOModel,
                        requestUrl = ExternalKeys.StockSubmissionUrl,
                        requestMediaType = MimeTypes.Json,
                        requestMethod = "Lifting"
                    };

                    HttpService httpService = new();
                    rcExtSubmit = await httpService.SubmitExternalRequest(httpModel);
                }
                catch (Exception ex)
                {
                    rcExtSubmit.success = false;
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "Lifting || SubmitExternalRequest", ex);
                }
            }

            if (!rcExtSubmit.success && string.IsNullOrEmpty(rcExtSubmit.message))
            {
                rcExtSubmit.message = "Unable to send request to RSO.";
            }

            if (!rcExtSubmit.success)
            {
                UpdateLifting ul = new()
                {
                    RetailerCode = liftingRequest.retailerCode,
                    RequestId = lifting,
                    Status = (int)OrderStatus.Failed
                };

                try
                {
                    liftingService = new(Connections.RetAppDbCS);
                    long resp = await liftingService.UpdateStockRequestStatus(ul);
                }
                catch (Exception ex)
                {
                    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "UpdateStockRequestStatus", ex);
                }
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(liftingRequest, "Lifting", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = rcExtSubmit.statusCode != 200,
                message = rcExtSubmit.statusCode == 200 ? SharedResource.GetLocal("Success", Message.Success) : rcExtSubmit.message
            });

        }


        [HttpPost]
        [Route("LiftingHistory")]
        public async Task<IActionResult> LiftingHistory([FromBody] HistoryPageRequestModel liftingRequest)
        {
            DataTable lifting = new();

            try
            {
                LiftingService liftingService = new(Connections.RetAppDbCS);
                lifting = await liftingService.LiftingHistoryV3(liftingRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "LiftingHistoryV3"));
            }

            List<LiftingModel> liftings = lifting.AsEnumerable().Select(row => new LiftingModel(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = liftings
            });

        }

    }
}