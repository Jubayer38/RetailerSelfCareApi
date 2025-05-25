///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	LoginSmartPos Controller
///	Creation Date :	08-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Domain.ViewModel;
using Infrastracture.DBManagers;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Infrastracture.Repositories
{
    public class StockRepository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public StockRepository()
        {
            _mySql = new();
        }

        public StockRepository(string ConnectionString)
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

        public async Task<RetailerSessionCheck> CheckRetailerByCode(string retailerCode, string loginProvider)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_LOGIN_PROVIDER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = loginProvider });

            DataTable dt = await _mySql.CallStoredProcedureSelectAsync("CHECK_RETAILER_BY_CODE");

            RetailerSessionCheck retailer = new();

            if (dt.Rows.Count > 0)
            {
                retailer.msisdn = dt.Rows[0]["MSISDN"].ToString().Substring(1);
                retailer.isSessionValid = Convert.ToBoolean(dt.Rows[0]["IS_VALID"]);

                return retailer;
            }
            else
            {
                return retailer;
            }
        }


        public DataTable GetScStockDetails(StockDetialRequest request)
        {
            _db.AddParameter(new OracleParameter("RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = request.retailerCode });
            _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

            var scDetails = _db.CallStoredProcedure_Select("RSLGETSCDETAILS"); // BIODB: RSLGETSCSTOCKDETAILS
            return scDetails;
        }


        public DataTable GetSimStockDetails(StockDetialRequest request)
        {
            _db.AddParameter(new OracleParameter("RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = request.retailerCode });
            _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

            var scDetails = _db.CallStoredProcedure_Select("RSLGETSIMDETAILS");// BIODB: RSLGETSIMSTOCKDETAILS
            return scDetails;
        }


        public int UpdateItopUpBalance(VMItopUpStock model)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.RetailerCode });
            _db.AddParameter(new OracleParameter("P_ITOPUP_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.ItopUpNumber, IsNullable = true });
            _db.AddParameter(new OracleParameter("P_UPDATE_BALANCE", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.NewBalance });
            _db.AddParameter(new OracleParameter("P_UPDATE_TIME", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.UpdateTime });

            long? result = _db.CallStoredProcedureInsertV2("RSLUPDATEITOPUP_BALANCE");
            return (int)result.GetValueOrDefault();
        }


        public DataTable ScExpire(string retailerCode)
        {
            _db.AddParameter(new OracleParameter("p_retailer_code", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailerCode });
            _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGSPSCEXPIRY");
            return result;
        }


        public DataTable GetRetailerByCode(string retailerCode)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailerCode });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETRETAILERBYCODE");
            return result;
        }


        public DataTable GetSCStocksSummaryV2(RetailerRequest retailer)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETSCSTOCKSUMMARY");
            return result;
        }


        public async Task<DataTable> GetFilteredScList(SCListRequestModel reqModel)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.retailerCode });
            _db.AddParameter(new OracleParameter("P_SC_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.scNumber });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGET_SC_CHANNELFILL_LIST");
            return result;
        }


        public async Task<long> SubmitScratchCardData(SCSalesRequest reqModel)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.retailerCode });
                _db.AddParameter(new OracleParameter("P_SC_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.scNumber });
                _db.AddParameter(new OracleParameter("P_CUSTOMER_MSISDN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.customerMsisdn });

                var result = _db.CallStoredProcedureInsertV2("RSL_SUBMIT_SC_DATA");
                return result.GetValueOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitScratchCardData"));
            }
        }


        public async Task<DataTable> GetSCSalesHistory(HistoryPageRequestModel reqModel)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.retailerCode });
            _db.AddParameter(new OracleParameter("P_START_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = reqModel.startDate });
            _db.AddParameter(new OracleParameter("P_END_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = reqModel.endDate });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGET_SCSALES_HISTORY");
            return result;
        }


        public async Task<DataTable> GetSIMSCStocksSummary(RetailerRequest retailer)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGET_SIM_SC_STOCK_SUMMARY");
            return result;
        }


        public async Task<DataTable> GetItopUpSummary(RetailerRequest retailer)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETITOPUPTOCKSUMMARY");
            return result;
        }


        public async Task<DataTable> GetRSOAndLastTime(RetailerRequestV2 reqModel)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.retailerCode });
            _db.AddParameter(new OracleParameter("P_ITOPUP_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = reqModel.iTopUpNumber });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETRSOANDLASTBLNCTIMEV2");
            return result;
        }

    }
}
