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
using Domain.LMS.Response;
using Domain.ResponseModel;
using Infrastracture.DBManagers;
using MySqlConnector;
using System.Data;
using static Domain.Enums.EnumCollections;


namespace Infrastracture.Repositories
{
    public class LMSRepository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public LMSRepository()
        {
            _mySql = new();
        }

        public LMSRepository(string connectionString)
        {
            _db = new(connectionString);
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
                _mySql.Dispose();
                _db.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========


        public async Task<long> SaveRedeemTransaction(LMSRedeemReward model, LMSRewardResp redeem)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_MSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.msisdn });
            _mySql.AddParameter(new MySqlParameter("P_TRANSACTION_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.transactionID });
            _mySql.AddParameter(new MySqlParameter("P_POINTS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = redeem.pointsRedeemed });
            _mySql.AddParameter(new MySqlParameter("P_ADJUSTMENT_TYPE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = nameof(LmsAdjustmentType.DEBIT) });
            _mySql.AddParameter(new MySqlParameter("P_DESCRIPTION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = "Reward Id: " + model.rewardID });
            _mySql.AddParameter(new MySqlParameter("P_APP_PAGE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = "Reward Redeem" });
            _mySql.AddParameter(new MySqlParameter("P_STATUS_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = redeem.statusCode });
            _mySql.AddParameter(new MySqlParameter("P_STATUS_MSG", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = redeem.statusMsg });
            _mySql.AddParameter(new MySqlParameter("P_RESPONSE_DATETIME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = redeem.responseDateTime });
            _mySql.AddParameter(new MySqlParameter("P_TOTAL_POINTS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = redeem.pointsAvailable });

            long? result = await _mySql.InsertByStoredProcedureAsync("SAVE_LMS_REDEEM_TRANSACTIONS");
            return result.GetValueOrDefault();

        }


        public async Task<List<LmsTermsFaqs>> GetLmsTermsConditionsAndFaqs(int featureType, string lan)
        {
            _mySql.AddParameter(new MySqlParameter("P_FEATURE_TYPE", MySqlDbType.Int32) { Direction = ParameterDirection.Input, Value = featureType });
            _mySql.AddParameter(new MySqlParameter("P_LAN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = lan.ToLower() });

            DataTable dt = await _mySql.CallStoredProcedureSelectAsync("GET_LMS_TERMS_OR_FAQS");

            List<LmsTermsFaqs> listData = dt.AsEnumerable().Select(row => HelperMethod.ModelBinding<LmsTermsFaqs>(row)).ToList();
            return listData;
        }

        public async Task<long> SaveTransaction(LMSPointAdjustResp model)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_APP_PAGE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.appPage });
            _mySql.AddParameter(new MySqlParameter("P_ADJUSTMENT_TYPE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.adjustmentType });
            _mySql.AddParameter(new MySqlParameter("P_DESCRIPTION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.description });
            _mySql.AddParameter(new MySqlParameter("P_MSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.msisdn });
            _mySql.AddParameter(new MySqlParameter("P_TRANSACTION_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.transactionID });
            _mySql.AddParameter(new MySqlParameter("P_SRC_TRANSACTION_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.srcTransactionID });
            _mySql.AddParameter(new MySqlParameter("P_STATUS_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.statusCode });
            _mySql.AddParameter(new MySqlParameter("P_STATUS_MSG", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.statusMsg });
            _mySql.AddParameter(new MySqlParameter("P_MEMBERSHIP_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.membershipID });
            _mySql.AddParameter(new MySqlParameter("P_POINTS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.points });
            _mySql.AddParameter(new MySqlParameter("P_RESPONSE_DATETIME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.responseDateTime });
            _mySql.AddParameter(new MySqlParameter("P_TOTAL_POINTS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.totalPoints });

            long? result = await _mySql.InsertByStoredProcedureAsync("SAVE_LMS_TRANSACTIONS");
            return result.GetValueOrDefault();
        }


    }
}
