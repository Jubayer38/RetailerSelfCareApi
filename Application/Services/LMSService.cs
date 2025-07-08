///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	10-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.LMS;
using Domain.LMS.Request;
using Domain.LMS.Response;
using Domain.RequestModel;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel.LogModels;
using Infrastracture.Repositories;
using Newtonsoft.Json;

namespace Application.Services
{
    public class LMSService : IDisposable
    {
        private readonly LMSRepository _repo;

        public LMSService()
        {
            _repo = new();
        }

        public LMSService(string connectionString)
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

        public static string GetTransactionId(string retailerCode)
        {
            string _retailerCode = retailerCode.Substring(1);
            string _tranId = $"{DateTime.Now:yyyyMMddHHmmssFFF}{_retailerCode}";
            return _tranId;
        }

        public async Task AdjustRetailerLMSPoints(LMSPointAdjustReq model)
        {
            if (FeatureStatus.IsLMSFeatureEnable)
            {
                await Task.Factory.StartNew(async () =>
                {
                    await AdjustLMSPoints(model);
                });
            }
        }

        public async Task<long> SaveRedeemTransaction(LMSRedeemReward model, LMSRewardResp redeem)
        {
            return await _repo.SaveRedeemTransaction(model, redeem);
        }


        public async Task<List<LmsTermsFaqs>> GetLmsTermsConditionsAndFaqs(int featureType, string lan)
        {
            return await _repo.GetLmsTermsConditionsAndFaqs(featureType, lan);
        }


        public async Task<List<LMSPartner>> GetLmsPartners(RetailerRequestV2 model)
        {
            CommonLMSRequest reqBody = new()
            {
                retailerCode = model.retailerCode,
                msisdn = "88" + model.iTopUpNumber,
                transactionID = GetTransactionId(model.retailerCode)
            };

            HttpRequestModel httpReq = new()
            {
                requestUrl = LMSKyes.RequestBaseUrl + LmsApiEndPoints.getPartners,
                requestBody = reqBody,
                requestMediaType = MimeTypes.VndBLApiJson,
                requestMethod = "GetLmsPartners"
            };

            using(HttpService httpService = new())
            {
                var partnersObj = await httpService.CallExternalApi<LMSPartnerResp>(httpReq);

                List<LMSPartner> partners = partnersObj.Object.partnerArray;

                return partners;
            }

        }


        public async Task<LMSPointHistory> PullLMSPointHistoryByMonth(LMSPointHistReq reqBody)
        {
            HttpRequestModel httpReq = new()
            {
                requestUrl = LMSKyes.RequestBaseUrl + LmsApiEndPoints.getCombinedPointHistory,
                requestBody = reqBody,
                requestMediaType = MimeTypes.VndBLApiJson,
                requestMethod = "GetPointHistory"
            };

            using (HttpService httpService = new())
            {
                Response<LMSPointHistory> pointHistoryObj = await httpService.CallExternalApi<LMSPointHistory>(httpReq);

                return pointHistoryObj.Object;
            }
        }


        #region==========| Start of Private Methods |==========

        private async Task AdjustLMSPoints(LMSPointAdjustReq model)
        {
            try
            {
                RedisCache redis = new();
                var pointsTrackStr = await redis.GetCacheAsync(RedisCollectionNames.LMSPointsTrack, model.retailerCode);

                if (!string.IsNullOrWhiteSpace(pointsTrackStr))
                {
                    var pointsAchTrack = JsonConvert.DeserializeObject<dynamic>(pointsTrackStr)!;
                    DateTime pageLastUpdateDT = new(1970, 01, 01);
                    try
                    {
                        pageLastUpdateDT = new DateTime((long)pointsAchTrack[model.appPage]);
                    }
                    catch (Exception)
                    {
                        //redis = new RedisCache();
                        //string dataKey = "$." + model.retailerCode + "." + model.appPage;
                        //await redis.UpdateCacheAsync(RedisCollectionNames.LMSPointsTrack, dataKey, DateTime.Now.Ticks.ToJsonString());
                    }

                    //check if the date is yestarday
                    if (DateTime.Now.Date > pageLastUpdateDT.Date)
                    {
                        await ExecuteTransaction(model);
                    }
                }
                else
                {
                    var jsonObj = new Dictionary<string, object>
                    {
                        [model.retailerCode] = new Dictionary<string, object>
                        {
                            [model.appPage] = DateTime.Now.Ticks
                        }
                    };

                    redis = new RedisCache();
                    await redis.SetCacheAsync(RedisCollectionNames.LMSPointsTrack, jsonObj.ToJsonString());

                    await ExecuteTransaction(model);
                }
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "AdjustLMSPoints",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "SAVE_LMS_TRANSACTIONS",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }
            }
        }


        private async Task ExecuteTransaction(LMSPointAdjustReq model)
        {
            HttpRequestModel httpReq = new()
            {
                requestUrl = LMSKyes.RequestBaseUrl + LmsApiEndPoints.getAdjustPoints,
                requestBody = model,
                requestMediaType = MimeTypes.VndBLApiJson,
                requestMethod = model.requestMethod
            };

            using (HttpService httpService = new())
            {
                var pointAdjustObj = await httpService.CallExternalApi<LMSPointAdjustResp>(httpReq);
                LMSPointAdjustResp pointAdjust = pointAdjustObj.Object;

                pointAdjust.points = model.points;
                pointAdjust.retailerCode = model.retailerCode;
                pointAdjust.appPage = model.appPage;
                pointAdjust.adjustmentType = model.adjustmentType;
                pointAdjust.description = model.appPage.Replace("_", " ");

                // Save LMS transaction into Database
                await _repo.SaveTransaction(pointAdjust);

                if (pointAdjust.statusCode == "0")
                {
                    using (RedisCache redis = new())
                    {
                        string dataKey = "$." + model.retailerCode + "." + model.appPage;
                        await redis.UpdateCacheAsync(RedisCollectionNames.LMSPointsTrack, dataKey, DateTime.Now.Ticks.ToJsonString());
                    }
                }
            }
        }


        //private async Task<int> UpdateTransactionData(long transactionID, LMSPointAdjustResp model)
        //{
        //    MySqlDbManager _mySql = new MySqlDbManager();

        //    _mySql.AddParameter(new MySqlParameter("P_MSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.msisdn });
        //    _mySql.AddParameter(new MySqlParameter("P_TRANSACTION_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = transactionID });
        //    _mySql.AddParameter(new MySqlParameter("P_SRC_TRANSACTION_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.srcTransactionID });
        //    _mySql.AddParameter(new MySqlParameter("P_STATUS_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.statusCode });
        //    _mySql.AddParameter(new MySqlParameter("P_STATUS_MSG", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.statusMsg });
        //    _mySql.AddParameter(new MySqlParameter("P_MEMBERSHIP_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.membershipID });
        //    _mySql.AddParameter(new MySqlParameter("P_RESPONSE_DATETIME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.responseDateTime });
        //    _mySql.AddParameter(new MySqlParameter("P_TOTAL_POINTS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.totalPoints });

        //    MySqlParameter param = new MySqlParameter(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.VarChar) { Direction = ParameterDirection.Output };
        //    _mySql.AddParameter(param);

        //    await _mySql.CallStoredProcedureObjectAsync("UPDATE_LMS_TRANSACTIONS", FrequentlyUsedDbParams.P_RETURN.ToString());

        //    return param.Value != DBNull.Value ? Convert.ToInt32(param.Value) : 0;
        //}


        //private async Task<int> UpdateMemberTotalPoints(string retailerCode, string totalPoints)
        //{
        //    MySqlDbManager _mySql = new MySqlDbManager();

        //    _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerCode });
        //    _mySql.AddParameter(new MySqlParameter("P_TOTAL_POINTS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = totalPoints });

        //    MySqlParameter param = new MySqlParameter(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.VarChar) { Direction = ParameterDirection.Output };
        //    _mySql.AddParameter(param);

        //    await _mySql.CallStoredProcedureObjectAsync("UPDATE_MEMBER_TOTAL_POINTS", FrequentlyUsedDbParams.P_RETURN.ToString());

        //    return param.Value != DBNull.Value ? Convert.ToInt32(param.Value) : 0;
        //}
        #endregion==========| End of Private Methods |==========
    }
}
