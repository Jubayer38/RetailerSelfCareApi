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
using Oracle.ManagedDataAccess.Client;
using System.Data;
using static Domain.Enums.EnumCollections;

namespace Infrastracture.Repositories
{
    public class RechargeRepository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public RechargeRepository()
        {
            _mySql = new();
        }

        public RechargeRepository(string ConnectionString)
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

        public DataTable GetRechargeOffers(OfferRequest request)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = request.retailerCode });
            _db.AddParameter(new OracleParameter("P_RECHARGE_TYPE", OracleDbType.Decimal, ParameterDirection.Input) { Value = request.rechargeType });
            _db.AddParameter(new OracleParameter("P_IS_ACQUISITION_OFFER", OracleDbType.Decimal, ParameterDirection.Input) { Value = request.acquisition });
            _db.AddParameter(new OracleParameter("P_ISSIMREPLACEMENT", OracleDbType.Decimal, ParameterDirection.Input) { Value = request.simReplacement });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLRECHARGEPACKV3"); // Old: RSLRECHARGEPACKV2
            return result;
        }


        public async Task<bool> SaveTransactionLog(TransactionLogVM log)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.rCode });
            _db.AddParameter(new OracleParameter("P_TRANNUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.tranNo });
            _db.AddParameter(new OracleParameter("P_TRANTYPE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.tranType });
            _db.AddParameter(new OracleParameter("P_AMOUNT", OracleDbType.Long, ParameterDirection.Input) { Value = log.amount });
            _db.AddParameter(new OracleParameter("P_TRANDATE", OracleDbType.Date, ParameterDirection.Input) { Value = log.tranDate });
            _db.AddParameter(new OracleParameter("P_MSISDN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.msisdn });
            _db.AddParameter(new OracleParameter("P_RECHARGETYPE", OracleDbType.Int32, ParameterDirection.Input) { Value = log.rechargeType });
            _db.AddParameter(new OracleParameter("P_EMAIL", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.email });
            _db.AddParameter(new OracleParameter("P_ISTRANSACTIONSUCCESS", OracleDbType.Int32, ParameterDirection.Input) { Value = log.isTranSuccess });
            _db.AddParameter(new OracleParameter("P_TRANSACTIONMESSAGE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.tranMsg });
            _db.AddParameter(new OracleParameter("P_RETMSISDN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.retMsisdn });
            _db.AddParameter(new OracleParameter("P_LOGIN_PROVIDER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.loginProvider });
            _db.AddParameter(new OracleParameter("P_RESP_TRANID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.respTranId });
            _db.AddParameter(new OracleParameter("P_LAT", OracleDbType.Decimal, ParameterDirection.Input) { Value = log.lat });
            _db.AddParameter(new OracleParameter("P_LNG", OracleDbType.Decimal, ParameterDirection.Input) { Value = log.lng });
            _db.AddParameter(new OracleParameter("P_IP_ADDRESS", OracleDbType.Varchar2, ParameterDirection.Input) { Value = log.ipAddress });

            var result = _db.CallStoredProcedure_Insert("RSLSAVETRANLOGV3");
            bool isSuccess = result > 0;

            return isSuccess;
        }


        public async Task<DataTable> SearchOffers(SearchRequestV2 model)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_SEARCH_TEXT", OracleDbType.NVarchar2, ParameterDirection.Input) { Value = model.searchText });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSLSEARCHOFFERS");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SearchOffersV2"));
            }
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
            string outPram = procReturn.ToString();

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
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = evPinReset.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = evPinReset.iTopUpNumber });
            _mySql.AddParameter(new MySqlParameter("P_PIN_RESET_REASON", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = evPinReset.pinResetReason });
            _mySql.AddParameter(new MySqlParameter("P_STATUS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = evPinReset.status });

            var result = await _mySql.InsertByStoredProcedureAsync("RSL_SAVEPINRESETREQ");
            return result;
        }


        public async Task UpdateEvPinResetSuccessDate(ChangeEvPinRequest model, DateTime changeDate)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_SUCCESS_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = changeDate });

            await _mySql.InsertByStoredProcedureAsync("RSL_UPDATEVPINRESET_SUCCESSON");
        }

    }
}