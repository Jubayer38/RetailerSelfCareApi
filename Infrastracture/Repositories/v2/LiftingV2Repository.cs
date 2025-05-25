
///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	LoginSmartPos Controller
///	Creation Date :	11-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************
///	
using Domain.Helpers;
using Domain.RequestModel;
using Domain.ResponseModel;
using Domain.ViewModel;
using Infrastracture.DBManagers;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Infrastracture.Repositories.v2
{
    public class LiftingV2Repository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public LiftingV2Repository()
        {
            _mySql = new();
        }

        public LiftingV2Repository(string ConnectionString)
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

        public async Task<DataTable> ProductCategory(ProductCategoryRequest categoryRequest)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = categoryRequest.retailerCode });
            _mySql.AddParameter(new MySqlParameter("vType", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = categoryRequest.type });

            var result = await _mySql.CallStoredProcedureSelectAsync("RSLGETPRODUCTCATEGORY");
            return result;
        }


        public async Task<SimStatusModel> CheckSimStatusByserialNo(SimStatusRequestModel simStatus)
        {
            try
            {
                _db.AddParameter(new OracleParameter("RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = simStatus.retailerCode });
                _db.AddParameter(new OracleParameter("SERIALNO", OracleDbType.Varchar2, ParameterDirection.Input) { Value = simStatus.serialNo });
                _db.AddParameter(new OracleParameter("MSISDN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = simStatus.msisdn });
                _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("RSLGETSIMSTATUS");
                SimStatusModel statusModel = result.Rows.Count > 0 ? new SimStatusModel(result.Rows[0]) : new SimStatusModel(new DataTable().NewRow()) { isAvailable = false, productName = "not available" };
                return statusModel;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "CheckSimStatusByserialNo"));
            }
        }


        public async Task<long> SaveUsingOracleBulkCopy(DataTable dt)
        {
            return await _db.SaveUsingOracleBulkCopy(dt);
        }


        public async Task<long> UpdateStockRequisitionDeliveredOrder()
        {
            var result = _db.CallStoredProcedureInsertV2("RSL_LIFTINGUPDATEREQSTATUS");
            return result.GetValueOrDefault();
        }


        public async Task<long> GetExistRequest(LiftingRequest model)
        {
            long result = 0;
            _mySql.AddParameter(new MySqlParameter("P_PRODUCT_TYPE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.appVisibleType });
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });

            MySqlParameter param = new("PO_RETURN", MySqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };

            _mySql.AddParameter(param);

            await _mySql.CallStoredProcedureSelectAsync("RSL_GETINSERTEDREQUEST");

            if (param.Value != DBNull.Value)
            {
                result = Convert.ToInt64(param.Value);
            }

            return result;
        }


        public async Task<long> SaveLiftingV3(LiftingRequest liftingRequest)
        {
            long result = 0;

            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = liftingRequest.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_MSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = liftingRequest.iTopUpNumber });
            _mySql.AddParameter(new MySqlParameter("P_PRODUCT_TYPE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = liftingRequest.appVisibleType });
            _mySql.AddParameter(new MySqlParameter("P_PRODUCT_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = liftingRequest.category });
            _mySql.AddParameter(new MySqlParameter("P_REQUEST_PRODCOUNT", MySqlDbType.Int32) { Direction = ParameterDirection.Input, Value = Convert.ToInt32(liftingRequest.quantity) });
            _mySql.AddParameter(new MySqlParameter("P_PAYMENT_TYPE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = liftingRequest.paymentType });

            MySqlParameter param = new("PO_PKVALUE", MySqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };

            _mySql.AddParameter(param);

            await _mySql.CallStoredProcedureSelectAsync("RSLSAVELIFTINGV3");

            if (param.Value != DBNull.Value)
            {
                result = Convert.ToInt64(param.Value);
            }

            return result;
        }


        public async Task<long> UpdateStockRequestStatus(UpdateLifting model)
        {
            long result = 0;
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.RetailerCode });
            _mySql.AddParameter(new MySqlParameter("P_REQUEST_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.RequestId });
            _mySql.AddParameter(new MySqlParameter("P_STATUS", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = model.Status });

            MySqlParameter param = new("PO_PKVALUE", MySqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };

            _mySql.AddParameter(param);

            await _mySql.CallStoredProcedureSelectAsync("RSL_UPDATE_LIFTING_STATUS");

            if (param.Value != DBNull.Value)
            {
                result = Convert.ToInt64(param.Value);
            }

            return result;
        }


        public async Task<DataTable> LiftingHistoryV3(HistoryPageRequestModel liftingRequest)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = liftingRequest.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_FORM_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = liftingRequest.startDate });
            _mySql.AddParameter(new MySqlParameter("P_TO_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = liftingRequest.endDate });

            var result = await _mySql.CallStoredProcedureSelectAsync("RSLGETLIFTINGHISTORY");
            return result;
        }

    }
}