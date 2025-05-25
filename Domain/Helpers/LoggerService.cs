///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Initiate text log write
///	Creation Date :	18-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.StaticClass;
using Domain.ViewModel;
using Domain.ViewModel.LogModels;
using Mapster;
using System.Text;

namespace Domain.Helpers
{
    public class LoggerService
    {
        public LoggerService() { }

        public void WriteTraceMessageInText<T, M>(T request, M methodName, string message)
        {
            LogModel _log = request.Adapt<LogModel>();
            _log.requestBody = request.ToJsonString();
            _log.errorMessage = message;
            _log.methodName = methodName.ToString();
            _log.apiEndTime = DateTime.Now;
            WriteTraceMsg(_log);
        }


        public static void WriteApiLogInText(LogModel log)
        {
            Task.Factory.StartNew(() =>
            {
                SaveApiLogInTxt(log);
            });
        }


        public static void WriteTraceMsg(LogModel log)
        {
            Task.Factory.StartNew(() =>
            {
                SaveTraceMsgInTxt(log);
            });
        }


        public void WriteEVLogInText(EvLogViewModel log)
        {
            Task.Factory.StartNew(() =>
            {
                SaveEVLogInTxt(log);
            });
        }


        public static void WriteIRISLogInText(IRISLogViewModel log)
        {
            Task.Factory.StartNew(() =>
            {
                SaveIRISLogInTxt(log);
            });
        }


        public static void WriteRechargeApiExternalLogInText(ExternalAPICallVM log, string folderName)
        {
            Task.Factory.StartNew(() =>
            {
                SaveRechargeApiExternalCallLogWrite(log, folderName);
            });
        }


        public static void WriteExternalLogInText(ExternalAPICallVM log)
        {
            Task.Factory.StartNew(() =>
            {
                SaveExternalApiLogTxt(log);
            });
        }


        public static void WriteDmsLogInText(ExternalAPICallVM log)
        {
            Task.Factory.StartNew(() =>
            {
                SaveDmsApiLogTxt(log);
            });
        }


        public static void ExceptionFromTextLogWrite(string mainLogStr, string retailerCode, Exception ex, string methodName)
        {
            try
            {
                string sb = string.Empty;

                sb = new
                {
                    retailerCode,
                    logMethodName = methodName,
                    errorMessage = HelperMethod.ExMsgSubString(ex, "", 256),
                    mainLogStr,
                    errorDetails = ex.StackTrace,
                    createdDate = DateTime.Now,
                    logId = DateTime.Now.Ticks
                }.ToJsonString() + ",";

                TextLogWriter.WriteLogFromLogWriteError(sb);
            }
            catch (Exception)
            {
            }
        }


        #region Private Methods

        private static void SaveTraceMsgInTxt(LogModel log)
        {
            StringBuilder sb = new();

            try
            {
                ApiSummaryLog summary = new(log);
                sb.Append(summary.ToJsonString() + ",");

                TextLogWriter.WriteApiTraceLogToFile(sb, "TraceMessages");
            }
            catch (Exception ex)
            {
                string retailer = string.IsNullOrWhiteSpace(log.retailerCode) ? log.iTopUpNumber : log.retailerCode;
                ExceptionFromTextLogWrite(log.ToJsonString(), retailer, ex, "SaveApiLogInTxt");
            }
        }


        private static void SaveApiLogInTxt(LogModel log)
        {
            StringBuilder summarySb = new();
            StringBuilder detailsSb = new();

            try
            {
                ApiSummaryLog summary = new(log);
                summarySb.Append(summary.ToJsonString() + ",");

                TextLogWriter.WriteApiLogToFile(summarySb, "SummaryLogs");
                if (TextLogging.IsEnableApiDetailsLog)
                {
                    ApiDetailsLog details = new(log);
                    detailsSb.Append(details.ToJsonString() + ",");
                    TextLogWriter.WriteApiLogToFile(detailsSb, "DetailLogs");
                }
            }
            catch (Exception ex)
            {
                string retailer = string.IsNullOrWhiteSpace(log.retailerCode) ? log.iTopUpNumber : log.retailerCode;
                ExceptionFromTextLogWrite(log.ToJsonString(), retailer, ex, "SaveApiLogInTxt");
            }
        }


        private static void SaveEVLogInTxt(EvLogViewModel log)
        {
            StringBuilder summarySb = new();
            StringBuilder detailsSb = new();

            try
            {
                EVSummaryLog summary = new(log);
                summarySb.Append(summary.ToJsonString() + ",");

                TextLogWriter.WriteEVLogToFile(summarySb, "SummaryLogs");
                if (TextLogging.IsEnableEVDetailsLog)
                {
                    EVDetailsLog details = new(log);
                    detailsSb.Append(details.ToJsonString() + ",");
                    TextLogWriter.WriteEVLogToFile(detailsSb, "DetailLogs");
                }
            }
            catch (Exception ex)
            {
                ExceptionFromTextLogWrite(log.ToJsonString(), log.retailerCode, ex, "SaveEVLogInTxt");
            }
        }


        private static void SaveIRISLogInTxt(IRISLogViewModel log)
        {
            StringBuilder summarySb = new();
            StringBuilder detailsSb = new();

            try
            {
                IRISSummaryLog summary = new(log);
                summarySb.Append(summary.ToJsonString() + ",");

                TextLogWriter.WriteIRISLogToFile(summarySb, "SummaryLogs");
                if (TextLogging.IsEnableEVDetailsLog)
                {
                    IRISDetailsLog details = new(log);
                    detailsSb.Append(details.ToJsonString() + ",");
                    TextLogWriter.WriteIRISLogToFile(detailsSb, "DetailLogs");
                }
            }
            catch (Exception ex)
            {
                ExceptionFromTextLogWrite(log.ToJsonString(), log.retailerCode, ex, "SaveIRISLogInTxt");
            }
        }


        private static void SaveRechargeApiExternalCallLogWrite(ExternalAPICallVM log, string folderName)
        {
            StringBuilder summarySb = new();
            StringBuilder detailsSb = new();

            try
            {
                ExternalApiSummaryLog summary = new(log);
                summarySb.Append(summary.ToJsonString() + ",");

                TextLogWriter.WriteRechargeExternalApiLogToFile(summarySb, folderName, "SummaryLogs");
                if (TextLogging.IsEnableRechargeApiExternalDetailLog)
                {
                    ExternalApiDetailsLog details = new(log);
                    detailsSb.Append(details.ToJsonString() + ",");
                    TextLogWriter.WriteRechargeExternalApiLogToFile(detailsSb, folderName, "DetailLogs");
                }
            }
            catch (Exception ex)
            {
                ExceptionFromTextLogWrite(log.ToJsonString(), log.retailerCode, ex, "SaveExternalApiLogTxt");
            }
        }


        private static void SaveExternalApiLogTxt(ExternalAPICallVM log)
        {
            StringBuilder summarySb = new();
            StringBuilder detailsSb = new();

            try
            {
                ExternalApiSummaryLog summary = new(log);
                summarySb.Append(summary.ToJsonString() + ",");

                TextLogWriter.WriteExternalApiLogToFile(summarySb, "SummaryLogs");
                if (TextLogging.IsEnableExternalDetailsLog)
                {
                    ExternalApiDetailsLog details = new(log);
                    detailsSb.Append(details.ToJsonString() + ",");
                    TextLogWriter.WriteExternalApiLogToFile(detailsSb, "DetailLogs");
                }
            }
            catch (Exception ex)
            {
                ExceptionFromTextLogWrite(log.ToJsonString(), log.retailerCode, ex, "SaveExternalApiLogTxt");
            }
        }


        private static void SaveDmsApiLogTxt(ExternalAPICallVM log)
        {
            StringBuilder summarySb = new();
            StringBuilder detailsSb = new();

            try
            {
                ExternalApiSummaryLog summary = new(log);
                summarySb.Append(summary.ToJsonString() + ",");

                TextLogWriter.WriteDmsApiLogToFile(summarySb, "SummaryLogs");
                if (TextLogging.IsEnableDetailsTextLogToDms)
                {
                    ExternalApiDetailsLog details = new(log);
                    detailsSb.Append(details.ToJsonString() + ",");
                    TextLogWriter.WriteDmsApiLogToFile(detailsSb, "DetailLogs");
                }
            }
            catch (Exception ex)
            {
                ExceptionFromTextLogWrite(log.ToJsonString(), log.retailerCode, ex, "SaveDmsApiLogTxt");
            }
        }

        #endregion

    }
}