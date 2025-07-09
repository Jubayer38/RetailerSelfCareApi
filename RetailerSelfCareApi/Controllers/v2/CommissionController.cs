///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	All Commission related APIs
///	Creation Date :	14-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No.	Date:		    Author:			    Ver:	    Area of Change:
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
using Infrastracture.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Net.Mail;
using System.Net.Mime;
using WkHtmlToPdfDotNet.Contracts;
using static Domain.Enums.EnumCollections;

namespace RetailerSelfCareApi.Controllers.v2
{
    [Route("api")]
    [ApiController]
    public class CommissionController(IConverter converter) : ControllerBase
    {
        private readonly IConverter _converter = converter;


        /// <summary>
        /// Get SalesVsCommission Approximate summary report Day wise. This api is applicable from app version v6.0.0
        /// </summary>
        /// <param name="salesVsCommissionReq"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetSalesVsCommission))]
        public async Task<IActionResult> GetSalesVsCommission([FromBody] SearchRequestV2 searchRequest)
        {
            if (string.IsNullOrWhiteSpace(searchRequest.searchText))
                searchRequest.searchText = "thismonth";

            DataTable dataTable;
            using(CommissionV2Service commissionService = new())
            {
                dataTable = await commissionService.GetSalesVsCommission(searchRequest);
            }

            SalesVsCommissionModel salesVsCommData = new(dataTable, searchRequest.searchText);

            LMSPointAdjustReq pointAdjustReq = new()
            {
                requestMethod = "GetSalesVsCommission",
                retailerCode = searchRequest.retailerCode,
                appPage = LMSAppPages.Sales_VS_Commission,
                transactionID = LMSService.GetTransactionId(searchRequest.retailerCode),
                msisdn = $"88{searchRequest.iTopUpNumber}",
                points = LMSPoints.Sales_VS_Commission.ToString(),
                adjustmentType = nameof(LmsAdjustmentType.CREDIT)
            };

            using(LMSService lmsService = new())
            {
                await lmsService.AdjustRetailerLMSPoints(pointAdjustReq);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = salesVsCommData
            });
        }


        [HttpPost]
        [Route(nameof(GetDailyCommission))]
        public async Task<IActionResult> GetDailyCommission([FromBody] CommissionRequest commission)
        {
            CommissionModel model = await ProcessDailyCommission(commission);

            if (model != null)
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = model
                });
            }
            else
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound)
                });
            }
        }


        /// <summary>
        /// Retailer Monthly Statement
        /// </summary>
        /// <param name="RetailerStatement"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(RetailerStatement))]
        public async Task<IActionResult> RetailerStatement([FromBody] SearchRequest request)
        {
            StatementMasterModel masterModel = await ProcessRetailerStatement(request);

            if (masterModel != null)
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("Success", Message.Success),
                    data = masterModel
                });
            }
            else
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound)
                });
            }

        }


        /// <summary>
        /// API to report using eamil
        /// </summary>
        /// <param name="SendMail">Requesting parameter with sendMailRequest's model</param>
        /// <returns>Return success or error status</returns>
        [HttpPost]
        [Route(nameof(SendMail))]
        public async Task<IActionResult> SendMail([FromBody] SendMailRequest sendMailRequest)
        {
            DateTime today = DateTime.Now;
            DateTime fd = new(today.Year, today.Month, 1);
            DateTime td = new(today.Year, today.Month, today.Day);

            string filename;

            switch (sendMailRequest.reportType)
            {
                case "Commission":
                    if (!sendMailRequest.startDate.HasValue)
                    {
                        sendMailRequest.startDate = fd;
                        sendMailRequest.endDate = td;
                    }

                    CommissionRequest commissionReq = new()
                    {
                        retailerCode = sendMailRequest.retailerCode,
                        startDate = sendMailRequest.startDate.Value,
                        endDate = sendMailRequest.endDate.Value
                    };

                    CommissionModel model = await ProcessDailyCommission(commissionReq);

                    if (model != null)
                    {
                        filename = "DailyCommissionReport_" + DateTime.Now.ToEnUSDateString("ddMMMyyyyhhmmss") + ".pdf";

                        VMCommissionReport commModel = PrepareCommissionModel(model, "Commission", sendMailRequest.lan.ToLower());

                        byte[] comRepotBytes = PDFGenerator.ConvertHtmlTextToPdf(commModel, _converter);

                        var comReportFile = File(comRepotBytes, "application/pdf", filename);

                        MemoryStream ms = new(comReportFile.FileContents, true);

                        ContentType ct = new(MediaTypeNames.Application.Pdf);
                        Attachment attachment = new(ms, ct);
                        attachment.ContentDisposition.FileName = filename;
                        attachment.ContentDisposition.CreationDate = DateTime.Now;
                        attachment.ContentDisposition.Size = ms.Length;

                        EmailModelV2 emailModel = new()
                        {
                            ReceiverEmail = sendMailRequest.mailAddress,
                            Subject = sendMailRequest.mailSubject,
                            Body = sendMailRequest.mailBody,
                            Regards = sendMailRequest.regards,
                            IsBodyHtml = true,
                            Attachment = attachment,
                            UseDefaultCred = false,
                            EnableSsl = true
                        };

                        try
                        {
                            await HelperMethod.SendEMail(emailModel);

                            return Ok(new ResponseMessage()
                            {
                                isError = false,
                                message = SharedResource.GetLocal("MailSentSuccessful", Message.MailSentSuccessful)
                            });
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        return Ok(new ResponseMessage()
                        {
                            isError = true,
                            message = SharedResource.GetLocal("NoDataFoundForMail", Message.NoDataFoundForMail)
                        });
                    }

                case "SalesVsCommission":
                case "RStatement":
                    SearchRequestV2 searchRequestV2 = new()
                    {
                        retailerCode = sendMailRequest.retailerCode,
                        deviceId = sendMailRequest.deviceId,
                        searchText = sendMailRequest.month
                    };

                    if (sendMailRequest.reportType == "SalesVsCommission")
                    {
                        if (string.IsNullOrWhiteSpace(sendMailRequest.month))
                            sendMailRequest.month = "thismonth";

                        CommissionV2Service commissionService = new();
                        DataTable dataTable = await commissionService.GetSalesVsCommission(searchRequestV2);

                        SalesVsCommissionModel salesVsCommData = new(dataTable, searchRequestV2.searchText);

                        string yearMonth = searchRequestV2.searchText.Equals("previousmonth", StringComparison.OrdinalIgnoreCase) ? today.AddMonths(-1).ToEnUSDateString("MMMyyyy") : fd.ToEnUSDateString("MMMyyyy");
                        filename = "SalesVsCommissionReport_" + yearMonth + ".pdf";

                        VMCommissionReport salesModel = PrepareCommissionModel(salesVsCommData, "SalesVsCommission", sendMailRequest.lan.ToLower());

                        byte[] svcReportBytes = PDFGenerator.ConvertHtmlTextToPdf(salesModel, _converter, sendMailRequest.month);

                        var svcRepotFile = File(svcReportBytes, "application/pdf", filename);

                        MemoryStream ms = new(svcRepotFile.FileContents, true);

                        ContentType ct = new(MediaTypeNames.Application.Pdf);
                        Attachment attachment = new(ms, ct);
                        attachment.ContentDisposition.FileName = filename;
                        attachment.ContentDisposition.CreationDate = DateTime.Now;
                        attachment.ContentDisposition.Size = ms.Length;

                        EmailModelV2 emailModel = new()
                        {
                            ReceiverEmail = sendMailRequest.mailAddress,
                            Subject = sendMailRequest.mailSubject,
                            Body = sendMailRequest.mailBody,
                            Regards = sendMailRequest.regards,
                            IsBodyHtml = true,
                            Attachment = attachment,
                            UseDefaultCred = false,
                            EnableSsl = true
                        };

                        try
                        {
                            await HelperMethod.SendEMail(emailModel);

                            return Ok(new ResponseMessage()
                            {
                                isError = false,
                                message = SharedResource.GetLocal("MailSentSuccessful", Message.MailSentSuccessful)
                            });
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    else if (sendMailRequest.reportType == "RStatement")
                    {
                        SearchRequest searchRequest = new()
                        {
                            retailerCode = sendMailRequest.retailerCode,
                            deviceId = sendMailRequest.deviceId,
                            searchText = sendMailRequest.month
                        };
                        StatementMasterModel masterModel = await ProcessRetailerStatement(searchRequest);

                        if (masterModel != null)
                        {
                            filename = "RetailerStatementReport_" + DateTime.Now.ToEnUSDateString("ddMMMyyyyhhmmss") + ".pdf";

                            VMCommissionReport stateModel = PrepareCommissionModel(masterModel, "SalesVsCommission", sendMailRequest.lan.ToLower());

                            byte[] stateRepotBytes = PDFGenerator.ConvertHtmlTextToPdf(stateModel, _converter);

                            var stateRepotFile = File(stateRepotBytes, "application/pdf", filename);

                            MemoryStream ms = new(stateRepotFile.FileContents, true);

                            ContentType ct = new(MediaTypeNames.Application.Pdf);
                            Attachment attachment = new(ms, ct);
                            attachment.ContentDisposition.FileName = filename;
                            attachment.ContentDisposition.CreationDate = DateTime.Now;
                            attachment.ContentDisposition.Size = ms.Length;

                            EmailModelV2 emailModel = new()
                            {
                                ReceiverEmail = sendMailRequest.mailAddress,
                                Subject = sendMailRequest.mailSubject,
                                Body = sendMailRequest.mailBody,
                                Regards = sendMailRequest.regards,
                                IsBodyHtml = true,
                                Attachment = attachment,
                                UseDefaultCred = false,
                                EnableSsl = true
                            };

                            try
                            {
                                await HelperMethod.SendEMail(emailModel);

                                return Ok(new ResponseMessage()
                                {
                                    isError = false,
                                    message = SharedResource.GetLocal("MailSentSuccessful", Message.MailSentSuccessful)
                                });
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                        else
                        {
                            return Ok(new ResponseMessage()
                            {
                                isError = true,
                                message = SharedResource.GetLocal("NoDataFoundForMail", Message.NoDataFoundForMail)
                            });
                        }
                    }
                    else
                    {
                        return Ok(new ResponseMessage()
                        {
                            isError = true,
                            message = SharedResource.GetLocal("ReportNotMatched", Message.ReportNotMatched),
                        });
                    }
                default:
                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("ReportNotMatched", Message.ReportNotMatched),
                    });
            }
        }


        /// <summary>
        /// (Overall ROI statement + Account activity report + spending statement) download
        /// </summary>
        /// <param name="ReportDownload"></param>
        /// <returns>Returns pdf file as base64string with filename and extension </returns>
        [HttpPost]
        [Route(nameof(ReportDownload))]
        public async Task<IActionResult> ReportDownload([FromBody] ReportDownloadRequest downloadRequest)
        {
            string filename;
            string base64String;

            switch (downloadRequest.reportType)
            {
                case "Commission":

                    CommissionRequest commissionReq = new()
                    {
                        retailerCode = downloadRequest.retailerCode,
                        startDate = downloadRequest.startDate,
                        endDate = downloadRequest.endDate
                    };

                    CommissionModel model = await ProcessDailyCommission(commissionReq);

                    if (model != null)
                    {
                        filename = "DailyCommissionReport_" + downloadRequest.startDate.ToEnUSDateString("ddMMyyyy") + "_" + downloadRequest.endDate.ToEnUSDateString("ddMMyyyy") + ".pdf";

                        VMCommissionReport commModel = PrepareCommissionModel(model, "Commission", downloadRequest.lan.ToLower());

                        byte[] comRepotBytes = PDFGenerator.ConvertHtmlTextToPdf(commModel, _converter);
                        base64String = Convert.ToBase64String(comRepotBytes);
                        break;
                    }
                    else
                    {
                        return new OkObjectResult(new ResponseMessage()
                        {
                            isError = true,
                            message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound)
                        });
                    }

                case "SalesVsCommission":

                    if (string.IsNullOrWhiteSpace(downloadRequest.month))
                        downloadRequest.month = "thismonth";

                    SearchRequestV2 searchRequestV2 = new()
                    {
                        retailerCode = downloadRequest.retailerCode,
                        deviceId = downloadRequest.deviceId,
                        searchText = downloadRequest.month
                    };

                    DataTable dataTable = new();
                    using (CommissionV2Service commissionService = new())
                    {
                        dataTable = await commissionService.GetSalesVsCommission(searchRequestV2);
                    }

                    SalesVsCommissionModel salesVsCommData = new(dataTable, searchRequestV2.searchText);

                    string yearMonth = searchRequestV2.searchText.Equals("previousmonth", StringComparison.OrdinalIgnoreCase) ? DateTime.Now.AddMonths(-1).ToEnUSDateString("MMMyyyy") : downloadRequest.startDate.ToEnUSDateString("MMMyyyy");
                    filename = "SalesVsCommissionReport_" + yearMonth + ".pdf";

                    VMCommissionReport salesModel = PrepareCommissionModel(salesVsCommData, "SalesVsCommission", downloadRequest.lan.ToLower());

                    byte[] salesReportBytes = PDFGenerator.ConvertHtmlTextToPdf(salesModel, _converter, downloadRequest.month);

                    base64String = Convert.ToBase64String(salesReportBytes);
                    break;

                case "RStatement":
                    SearchRequest searchRequest = new()
                    {
                        retailerCode = downloadRequest.retailerCode,
                        deviceId = downloadRequest.deviceId,
                        searchText = downloadRequest.month
                    };

                    string ym;
                    if (string.IsNullOrEmpty(downloadRequest.month))
                    {
                        ym = DateTime.Now.ToEnUSDateString("yyyyMM");
                    }
                    else
                    {
                        ym = downloadRequest.month.Replace("/", "");
                        ym = ym.Replace("-", "");
                    }

                    StatementMasterModel masterModel = await ProcessRetailerStatement(searchRequest);

                    if (masterModel != null)
                    {
                        filename = "RetailerStatementReport_" + ym + ".pdf";

                        VMCommissionReport statementModel = PrepareCommissionModel(masterModel, "RStatement", downloadRequest.lan.ToLower());

                        byte[] statementBytes = PDFGenerator.ConvertHtmlTextToPdf(statementModel, _converter);
                        base64String = Convert.ToBase64String(statementBytes);
                        break;
                    }
                    else
                    {
                        return new OkObjectResult(new ResponseMessage()
                        {
                            isError = true,
                            message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound)
                        });
                    }

                default:
                    return new OkObjectResult(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("ReportNotMatched", Message.ReportNotMatched)
                    });
            }

            if (string.IsNullOrEmpty(base64String))
            {
                return new OkObjectResult(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("NoDataFound", Message.NoDataFound)
                });
            }
            else
            {
                var data = new
                {
                    FileName = filename,
                    Extension = "pdf",
                    Base64Pdf = base64String
                };

                return new OkObjectResult(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal("PDFFileAttachAsBase64String", Message.PDFFileAttachAsBase64String),
                    data = data
                });
            }
        }


        [HttpPost]
        [Route(nameof(TarVsAchvSummary))]
        public async Task<IActionResult> TarVsAchvSummary([FromBody] RetailerRequestV2 reqModel)
        {
            string traceMsg = string.Empty;
            RedisCache redis;

            List<TarVsAchvSummaryModel> tarVsAchvs = [];
            string isUpdated = string.Empty;
            try
            {
                redis = new RedisCache();
                isUpdated = await redis.GetCacheAsync(RedisCollectionNames.RetailerTarVsAchvStatus, reqModel.retailerCode);
            }
            catch (Exception ex)
            {
                traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "GetCacheAsync", ex);
            }

            if (!string.IsNullOrWhiteSpace(isUpdated))
            {
                redis = new RedisCache();
                string tarVsAchvsStr = await redis.GetCacheAsync(RedisCollectionNames.RetailerTarVsAchvSummary, reqModel.retailerCode);
                if (!string.IsNullOrWhiteSpace(tarVsAchvsStr))
                {
                    tarVsAchvs = JsonConvert.DeserializeObject<List<TarVsAchvSummaryModel>>(tarVsAchvsStr)!;
                }
            }
            else
            {
                CommissionV2Service tarVsAchvService = new();
                DataTable tarVsAchvDT = new();

                try
                {
                    tarVsAchvDT = await tarVsAchvService.TarVsAchvSummary(reqModel);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "TarVsAchvSummary"));
                }

                tarVsAchvs = tarVsAchvDT.AsEnumerable().Select(row => HelperMethod.ModelBinding<TarVsAchvSummaryModel>(row)).ToList();

                redis = new RedisCache();
                await redis.SetCacheAsync(RedisCollectionNames.RetailerTarVsAchvStatus, reqModel.retailerCode, "1");

                redis = new RedisCache();
                await redis.SetCacheAsync(RedisCollectionNames.RetailerTarVsAchvSummary, reqModel.retailerCode, tarVsAchvs.ToJsonString());
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(reqModel, "v2/TarVsAchvSummary", traceMsg);
            }

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = tarVsAchvs
            });
        }


        [HttpPost]
        [Route(nameof(TarVsAchvDetails))]
        public async Task<IActionResult> TarVsAchvDetails([FromBody] TarVsAchvRequestV2 tarVsAchvRequest)
        {
            DataTable tarVsAchv = new();

            using (CommissionV2Service tarVsAchvService = new())
            {
                try
                {
                    tarVsAchv = await tarVsAchvService.TarVsAchvDeatils(tarVsAchvRequest);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "TarVsAchvDeatils"));
                }
            }

            List<TarVsAchvDetailsModel> tarVsAchvs = tarVsAchv.AsEnumerable().Select(row => HelperMethod.ModelBinding<TarVsAchvDetailsModel>(row)).ToList();

            return Ok(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = tarVsAchvs
            });
        }


        #region==========|  Private Methods  |==========

        private static async Task<CommissionModel> ProcessDailyCommission(CommissionRequest commission)
        {
            CommissionV2Service commissionService = new();
            DataTable commSummary = new();

            try
            {
                commSummary = await commissionService.GetDailyCommSummary(commission);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDailyCommSummary"));
            }

            if (commSummary.Rows.Count > 0)
            {
                var dr = commSummary.Rows[0];

                if (!string.IsNullOrEmpty(commission.sortByAmount))
                {
                    commission.sortByAmount = commission.sortByAmount.ToUpper();
                }

                commissionService = new();
                DataTable commDetails = new();

                try
                {
                    commDetails = await commissionService.GetDailyCommissionDetails(commission);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "GetDailyCommDetails"));
                }

                List<CommissionDetails> items = commDetails.AsEnumerable().Select(row => HelperMethod.ModelBinding<CommissionDetails>(row)).ToList();

                CommissionModel model = HelperMethod.ModelBinding<CommissionModel, CommissionDetails>(dr, items);

                return model;
            }

            return null;
        }


        private static async Task<StatementMasterModel> ProcessRetailerStatement(SearchRequest request)
        {
            DateTime today = DateTime.Now;
            DateTime fd = new(today.Year, today.Month, 1);
            DateTime td = new(today.Year, today.Month, today.Day);

            if (!string.IsNullOrEmpty(request.searchText))
            {
                request.searchText = request.searchText.Replace('/', '-');
                string ym = request.searchText + "-01";
                DateTime selectDate = Convert.ToDateTime(ym);
                DateTime lastDate = selectDate.AddMonths(1).AddDays(-1);
                if (today.Month == lastDate.Month)
                {
                    lastDate = td;
                }
                fd = selectDate;
                td = lastDate;
            }

            CommissionV2Service commisionObj = new();
            DataTable masterDt = new();

            try
            {
                masterDt = await commisionObj.StatementSummary(request, fd, td);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "StatementSummary"));
            }

            if (masterDt.Rows.Count > 0)
            {
                var masterDr = masterDt.Rows[0];

                commisionObj = new();
                DataTable details = new();

                try
                {
                    details = await commisionObj.StatementDetails(request, fd, td);
                }
                catch (Exception ex)
                {
                    throw new Exception(HelperMethod.ExMsgBuild(ex, "StatementDetails"));
                }

                List<StatementDetailsModel> detailsList = details.AsEnumerable().Select(cf => HelperMethod.ModelBinding<StatementDetailsModel>(cf)).ToList();

                StatementMasterModel masterModel = HelperMethod.ModelBinding<StatementMasterModel, StatementDetailsModel>(masterDr, detailsList);

                masterModel.month = fd.ToEnUSDateString("dd MMM yyyy") + " - " + td.ToEnUSDateString("dd MMM yyyy");

                return masterModel;
            }

            return null;
        }


        private static VMCommissionReport PrepareCommissionModel(dynamic model, string reportType, string lan)
        {
            if (!string.IsNullOrWhiteSpace(lan))
            {
                lan = lan.ToLower();
            }

            switch (reportType)
            {
                case "Commission":
                    VMCommissionReport comModel = new()
                    {
                        DataModel = model,
                        ReportType = "Commission",
                        lan = lan
                    };

                    if (lan == "bn")
                    {
                        comModel.PageHeader = "কমিশন বিবৃতি";
                        comModel.ReportHeaders = ["তারিখ", "বিবরণ", "কমিশন"];
                    }
                    else
                    {
                        comModel.PageHeader = "COMMISSION STATEMENT";
                        comModel.ReportHeaders = ["Date", "Description", "Commission"];
                    }
                    return comModel;
                case "SalesVsCommission":
                    VMCommissionReport salesModel = new()
                    {
                        DataModel = model,
                        ReportType = "SalesVsCommission",
                        lan = lan
                    };

                    if (lan == "bn")
                    {
                        salesModel.PageHeader = "বিক্রি আর কমিশন বিবৃতি";
                        salesModel.ReportHeaders = ["বিবৃতি মাস", "কমিশন", "বিক্রি"];
                    }
                    else
                    {
                        salesModel.PageHeader = "Sales Vs Commission Statement";
                        salesModel.ReportHeaders = ["Statement Month", "Commission", "Sales"];
                    }

                    return salesModel;
                case "RStatement":
                    VMCommissionReport statementModel = new()
                    {
                        DataModel = model,
                        ReportType = "RStatement",
                        lan = lan
                    };

                    if (lan == "bn")
                    {
                        statementModel.PageHeader = "রিটেইলার বিবৃতি";
                        statementModel.ReportHeaders = ["তারিখ", "বিবরণ", "লিফটিং (৳)", "সেলস (৳)", "কমিশন (৳)", "অগ্রিম ট্যাক্স (৳)", "প্রাপ্য (৳)"];
                    }
                    else
                    {
                        statementModel.PageHeader = "RETAILER STATEMENT";
                        statementModel.ReportHeaders = ["Date", "Description", "Lifting Amount (BDT)", "Sales Amount (BDT)", "Commission (BDT)", "Advance Income Tax (BDT)", "Amount Received (BDT)"];
                    }
                    return statementModel;
                default:
                    return new VMCommissionReport();
            }
        }

        #endregion==========|  Private Methods  |==========

    }
}