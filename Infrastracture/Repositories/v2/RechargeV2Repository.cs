///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Infrastracture.DBManagers;
using MySqlConnector;
using System.Data;
using static Domain.Enums.EnumCollections;

namespace Infrastracture.Repositories.v2
{
    public class RechargeV2Repository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public RechargeV2Repository()
        {
            _mySql = new();
        }

        public RechargeV2Repository(string ConnectionString)
        {
            _db = new(ConnectionString);
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
                _db.Dispose();
                _mySql.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========

        public async Task<DataTable> GetRechargeOffers(OfferRequest request)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = request.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_RECHARGE_TYPE", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = request.rechargeType });
            _mySql.AddParameter(new MySqlParameter("P_IS_ACQUISITION_OFFER", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = request.acquisition });
            _mySql.AddParameter(new MySqlParameter("P_ISSIMREPLACEMENT", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = request.simReplacement });

            var result = await _mySql.CallStoredProcedureSelectAsync("RSLRECHARGEPACK");
            return result;
        }


        public async Task<bool> SaveTransactionLog(TransactionLogVM log)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.rCode });
            _mySql.AddParameter(new MySqlParameter("P_TRANNUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.tranNo });
            _mySql.AddParameter(new MySqlParameter("P_TRANTYPE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.tranType });
            _mySql.AddParameter(new MySqlParameter("P_AMOUNT", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = log.amount });
            _mySql.AddParameter(new MySqlParameter("P_TRANDATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = log.tranDate });
            _mySql.AddParameter(new MySqlParameter("P_MSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.msisdn });
            _mySql.AddParameter(new MySqlParameter("P_RECHARGETYPE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.rechargeType });
            _mySql.AddParameter(new MySqlParameter("P_EMAIL", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.email });
            _mySql.AddParameter(new MySqlParameter("P_ISTRANSACTIONSUCCESS", MySqlDbType.Int32) { Direction = ParameterDirection.Input, Value = log.isTranSuccess });
            _mySql.AddParameter(new MySqlParameter("P_TRANSACTIONMESSAGE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.tranMsg });
            _mySql.AddParameter(new MySqlParameter("P_RETMSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.retMsisdn });
            _mySql.AddParameter(new MySqlParameter("P_LOGIN_PROVIDER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.loginProvider });
            _mySql.AddParameter(new MySqlParameter("P_RESP_TRANID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.respTranId });
            _mySql.AddParameter(new MySqlParameter("P_LAN", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = log.lat });
            _mySql.AddParameter(new MySqlParameter("P_LNG", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = log.lng });
            _mySql.AddParameter(new MySqlParameter("P_IP_ADDRESS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = log.ipAddress });

            var result = await _mySql.InsertByStoredProcedureAsync("SAVETRANLOG");
            bool isSuccess = result > 0;

            return isSuccess;
        }


        public async Task<(bool, string)> UpdateEVPinStatus(EVPinResetStatusRequest model)
        {
            bool isNull;
            _mySql.AddParameter(new MySqlParameter("P_UPDATED_BY", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.userId });
            _mySql.AddParameter(new MySqlParameter("P_RESET_BY", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.userName, IsNullable = true });
            _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });
            _mySql.AddParameter(new MySqlParameter("P_REQ_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.resetReqId });
            _mySql.AddParameter(new MySqlParameter("P_STATUS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.status });

            MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.VarChar) { Direction = ParameterDirection.Output };
            _mySql.AddParameter(param);
            object procReturn = await _mySql.CallStoredProcedureObjectAsync("RSLSYNC_EVPIN_RESET_STATUS", FrequentlyUsedDbParams.P_RETURN.ToString());
            string outPram = procReturn.DBNullToString();

            if (outPram is null)
                isNull = true;
            else
                isNull = false;

            if (isNull) return (false, "Unable to update");
            else
            {
                string result = outPram;
                bool isIntVal = int.TryParse(result, out int res);

                if (isIntVal && res > 0)
                    return (true, Message.Success);
                else if (isIntVal && res == 0)
                    return (false, "No EV Pin Request Found.");
                else
                    return (false, result);
            }
        }


        public async Task<DataTable> GetRetailerEvPinResetHistory(HistoryPageRequestModel reqModel)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = reqModel.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_START_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = reqModel.startDate });
            _mySql.AddParameter(new MySqlParameter("P_END_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = reqModel.endDate });

            DataTable result = await _mySql.CallStoredProcedureSelectAsync("RSLGET_RETAILER_EVPINRESETHIS");
            return result;
        }


        public async Task<long> SaveResetEVPinReqLog(EvPinResetRequest evPinReset)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = evPinReset.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = evPinReset.iTopUpNumber });
                _mySql.AddParameter(new MySqlParameter("P_PIN_RESET_REASON", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = evPinReset.pinResetReason });
                _mySql.AddParameter(new MySqlParameter("P_STATUS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = evPinReset.status });

                var result = await _mySql.InsertByStoredProcedureAsync("RSL_SAVEPINRESETREQ");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveResetEVPinReqLog"));
            }
        }


        public async Task UpdateEvPinResetSuccessDate(ChangeEvPinRequest model, DateTime changeDate)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_SUCCESS_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = changeDate });

            await _mySql.InsertByStoredProcedureAsync("RSL_UPDATEVPINRESET_SUCCESSON");
        }

    }
}