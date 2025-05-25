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

using Domain.Helpers;
using Domain.RequestModel;
using Domain.ResponseModel;
using Domain.ViewModel;
using Infrastracture.DBManagers;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Infrastracture.Repositories
{
    public class LiftingRepository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public LiftingRepository()
        {
            _mySql = new();
        }

        public LiftingRepository(string ConnectionString)
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
            _db.AddParameter(new OracleParameter("vRETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = categoryRequest.retailerCode });
            _db.AddParameter(new OracleParameter("vType", OracleDbType.Varchar2, ParameterDirection.Input) { Value = categoryRequest.type });
            _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETPRODUCTCATEGORY");
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
            _db.AddParameter(new OracleParameter("P_PRODUCT_TYPE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.appVisibleType });
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });

            OracleParameter param = new("PO_RETURN", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };

            _db.AddParameter(param);
            _db.CallStoredProcedure_Select("RSL_GETINSERTEDREQUEST");

            long? result = (long?)((OracleDecimal)param.Value).Value;
            return result.GetValueOrDefault();
        }


        public async Task<long> SaveLiftingV3(LiftingRequest liftingRequest)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = liftingRequest.retailerCode });
            _db.AddParameter(new OracleParameter("P_RETAILER_MSISDN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = liftingRequest.iTopUpNumber });
            _db.AddParameter(new OracleParameter("P_PRODUCT_TYPE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = liftingRequest.appVisibleType });
            _db.AddParameter(new OracleParameter("P_PRODUCT_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = liftingRequest.category });
            _db.AddParameter(new OracleParameter("P_REQUEST_PRODCOUNT", OracleDbType.Int32, ParameterDirection.Input) { Value = Convert.ToInt32(liftingRequest.quantity) });
            _db.AddParameter(new OracleParameter("P_PAYMENT_TYPE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = liftingRequest.paymentType });

            var result = _db.CallStoredProcedureInsertV2("RSLSAVELIFTINGV3");
            return result.GetValueOrDefault();
        }


        public async Task<long> UpdateStockRequestStatus(UpdateLifting model)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.RetailerCode });
            _db.AddParameter(new OracleParameter("P_REQUEST_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.RequestId });
            _db.AddParameter(new OracleParameter("P_STATUS", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.Status });

            var result = _db.CallStoredProcedureInsertV2("RSL_UPDATE_LIFTING_STATUS");
            return result.GetValueOrDefault();
        }


        public async Task<DataTable> LiftingHistoryV3(HistoryPageRequestModel liftingRequest)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = liftingRequest.retailerCode });
            _db.AddParameter(new OracleParameter("P_FORM_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = liftingRequest.startDate });
            _db.AddParameter(new OracleParameter("P_TO_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = liftingRequest.endDate });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETLIFTINGHISTORY");
            return result;
        }

    }
}