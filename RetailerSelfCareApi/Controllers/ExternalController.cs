///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	
///	Purpose	      :	
///	Creation Date :	11-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No.	Date:		    Author:			    Ver:	    Area of Change:
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
using Newtonsoft.Json;
using System.Data;

namespace RetailerSelfCareApi.Controllers
{
    [Route("api/v2")]
    [ApiController]
    public class ExternalController : ControllerBase
    {
        [HttpPost]
        [Route(nameof(SyncRetailerInfo))]
        public async Task<IActionResult> SyncRetailerInfo([FromBody] RetailerInfoRequest retailerModel)
        {
            if (string.IsNullOrEmpty(retailerModel.userName) && string.IsNullOrEmpty(retailerModel.password))
            {
                return Ok(new RACommonResponse()
                {
                    result = false,
                    message = "Credential not found."
                });
            }

            if (string.IsNullOrEmpty(retailerModel.retailerCode))
            {
                return Ok(new RACommonResponse()
                {
                    result = false,
                    message = "Invalid Parameter: retailerCode"
                });
            }

            if (string.IsNullOrEmpty(retailerModel.iTopUpNumber))
            {
                return Ok(new RACommonResponse()
                {
                    result = false,
                    message = "Invalid Parameter: iTopUpNumber"
                });
            }

            if (string.IsNullOrEmpty(retailerModel.typeName))
            {
                return Ok(new RACommonResponse()
                {
                    result = false,
                    message = "Invalid Parameter: typeName"
                });
            }

            long userValidRes;

            using(UserService userService = new())
            {
                try
                {
                    userValidRes = await userService.ValidateExternalUsers(retailerModel.userName, retailerModel.password);
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "ValidateExternalUsers");
                    throw new Exception(errMsg);
                }
            }

            if (userValidRes > 0)
            {
                try
                {
                    using (RetailerService service = new())
                    {
                        var result = await service.UpdateRetailerInfoMySQL(retailerModel);

                        #region===============| Call Biometric API for Status Update  |===============

                        string bioStatusURL = BiometricKeys.BioRetailerStatusURL;

                        RetailerInfoRequest bioStatusModel = new()
                        {
                            userName = BiometricKeys.BioRetailerStatusUserName,
                            password = BiometricKeys.BioRetailerStatusCred,
                            retailerCode = retailerModel.retailerCode,
                            iTopUpNumber = retailerModel.iTopUpNumber,
                            isActive = retailerModel.isActive,
                            typeName = retailerModel.typeName
                        };

                        using(HttpService httpService = new())
                        {
                            await httpService.UpdateBioRetailerStatus(bioStatusModel, bioStatusURL);
                        }

                        #endregion===============|  Call Biometric API for Status Update |===============

                        return Ok(new RACommonResponse()
                        {
                            result = result.Item1,
                            message = result.Item2
                        });
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "UpdateRetailerInfo");
                    throw new Exception(errMsg);
                }
            }
            else
            {
                throw new Exception("Invalid user.");
            }
        }


        [HttpPost]
        [Route(nameof(UpdStockReqDeliveredOrder))]
        public async Task<IActionResult> UpdStockReqDeliveredOrder([FromBody] StockReqPendingOrderStatusRequest reqModel)
        {
            ExternalAPICallVM exLog = new()
            {
                reqStartTime = DateTime.Now,
                isSuccess = 1,
                reqBodyStr = reqModel.updateDataList.ToJsonString()
            };

            LiftingService liftingService;
            long userValidRes;

            try
            {
                using (UserService userService = new())
                {
                    userValidRes = await userService.ValidateExternalUsers(reqModel.userName, reqModel.password);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ValidateExternalUsers"));
            }

            if (userValidRes > 0)
            {
                if (reqModel.updateDataList != null)
                {
                    string reqModelStr = reqModel.updateDataList.ToJsonString();
                    List<UpdateLifting> responseList = JsonConvert.DeserializeObject<List<UpdateLifting>>(reqModelStr)!;

                    DataTable dt = new();
                    dt.Columns.Add("REQUESTID");
                    dt.Columns.Add("RETAILERCODE");
                    dt.Columns.Add("PRODUCTTYPE");
                    dt.Columns.Add("PRODUCTCODE");
                    dt.Columns.Add("PRODUCTCOUNT");
                    dt.Columns.Add("STATUS");

                    foreach (UpdateLifting item in responseList)
                    {
                        DataRow dr = dt.NewRow();
                        dr["REQUESTID"] = item.RequestId;
                        dr["RETAILERCODE"] = item.RetailerCode;
                        dr["PRODUCTTYPE"] = item.ProductType;
                        dr["PRODUCTCODE"] = item.ProductCode;
                        dr["PRODUCTCOUNT"] = item.ProductCount;
                        dr["STATUS"] = item.Status;

                        dt.Rows.Add(dr);
                        exLog.retailerCode = item.RetailerCode;

                        if (item.Status <= 0)
                        {
                            string statusErrMsg = "Status 0 not acceptable during update. It must be between 1 and 3";
                            throw new Exception(statusErrMsg);
                        }
                    }

                    long isBulkDataInsert;
                    try
                    {
                        using (liftingService = new(Connections.RetAppDbCS))
                        {
                            isBulkDataInsert = await liftingService.SaveUsingOracleBulkCopy(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveUsingOracleBulkCopy"));
                    }

                    if (isBulkDataInsert > 0)
                    {
                        using (liftingService = new(Connections.RetAppDbCS))
                        {
                            _ = liftingService.UpdateStockRequisitionDeliveredOrder();
                        }

                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("Success", Message.Success)
                        });
                    }
                    else
                    {
                        return Ok(new ResponseMessage()
                        {
                            isError = true,
                            message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong)
                        });
                    }
                }
                else
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("SomethingWentWrong", Message.SomethingWentWrong)
                    });
                }
            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("InvalidUserCredentials", ResponseMessages.InvalidUserCred)
                });
            }
        }


        [HttpPost]
        [Route("UpdateEVPinResetStatus")]
        public async Task<IActionResult> UpdateEVPinResetStatus([FromBody] EVPinResetStatusRequest model)
        {
            model.iTopUpNumber = model.iTopUpNumber.PadLeft(11, '0');
            if (model.iTopUpNumber.Length != 11)
            {
                return Ok(new ExternalSubmitResponse()
                {
                    success = false,
                    message = "Invalid iTopUpNumber",
                });
            }

            long userValidRes;
            try
            {
                using (UserService userService = new())
                {
                    userValidRes = await userService.ValidateExternalUsers(model.userName, model.password);
                }
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "ValidateExternalUsers");
                throw new Exception(errMsg);
            }

            if (userValidRes > 0)
            {
                try
                {
                    model.userId = userValidRes;
                    using (RechargeService rechargeService = new())
                    {
                        var result = await rechargeService.UpdateEVPinStatus(model);

                        if (result.Item1)
                        {
                            using (RetailerService retailerService = new())
                            {
                                await retailerService.SendPushNotification(model.iTopUpNumber);
                            }
                        }

                        return Ok(new ExternalSubmitResponse()
                        {
                            success = result.Item1,
                            message = result.Item2,
                        });
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "UpdateEVPinStatus");
                    throw new Exception(errMsg);
                }
            }
            else
            {
                throw new Exception("Invalid user.");
            }
        }


        [HttpPost]
        [Route("UpdateRaiseComplaintStatus")]
        public async Task<IActionResult> UpdateRaiseComplaintStatus([FromBody] UpdateRaiseComplaintRequest model)
        {
            long userValidRes;

            using(UserService userService = new())
            {
                try
                {
                    userValidRes = await userService.ValidateExternalUsers(model.userName, model.password);
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "ValidateExternalUsers");
                    throw new Exception(errMsg);
                }
            }

            if (userValidRes > 0)
            {
                try
                {
                    using (RetailerService reService = new())
                    {
                        var result = await reService.UpdateRaiseComplaintStatus(model);

                        return Ok(new RACommonResponse()
                        {
                            result = result.Item1,
                            message = result.Item2
                        });
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "UpdateRaiseComplaintStatus");
                    throw new Exception(errMsg);
                }
            }
            else
            {
                throw new Exception("Username or Password is not valid");
            }
        }


        [HttpPost]
        [Route(nameof(UpdateDigitalServiceStatus))]
        public async Task<IActionResult> UpdateDigitalServiceStatus([FromBody] UpdateDigitalServiceStatus model)
        {
            long userValidRes;
            try
            {
                using(UserService userService = new())
                {
                    userValidRes = await userService.ValidateExternalUsers(model.userName, model.password);
                }
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "ValidateExternalUsers");
                throw new Exception(errMsg);
            }

            if (userValidRes > 0)
            {
                try
                {
                    using (RetailerService retailerService = new())
                    {
                        var result = await retailerService.UpdateDigitalServiceStatus(model);

                        return Ok(new ExternalSubmitResponse()
                        {
                            success = result.Item1,
                            message = result.Item2,
                        });
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "UpdateDigitalServiceStatus");
                    throw new Exception(errMsg);
                }
            }
            else
            {
                throw new Exception("Invalid user.");
            }
        }
    }
}