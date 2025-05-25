///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	SalesController
///	Creation Date :	09-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Infrastracture.DBManagers;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Infrastracture.Repositories.v2
{
    public class SalesV2Repository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public SalesV2Repository()
        {
            _mySql = new();
        }

        public SalesV2Repository(string ConnectionString)
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


        public async Task<DataTable> GetSalesUpdate(RetailerRequest retailerRequest)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerRequest.retailerCode });
            var result = await _mySql.CallStoredProcedureSelectAsync("GETSALESUPDATE");
            return result;
        }


        public async Task<DataTable> GetTodaySalesMemo(RetailerRequest retailer)
        {
            _db.AddParameter(new OracleParameter("vRETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
            _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var result = _db.CallStoredProcedure_Select("GETSALESMEMO");
            return result;
        }


        public async Task<DataTable> GetSalesWeeklyTrend(RetailerRequest retailerRequest)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerRequest.retailerCode });
            var result = await _mySql.CallStoredProcedureSelectAsync("GETSALESWTEND");
            return result;
        }


        public async Task<DataTable> GetSalesRoutePerformance(RetailerRequest retailerRequest)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerRequest.retailerCode });
            var result = await _mySql.CallStoredProcedureSelectAsync("GET_SALES_ROUTE_PERF");
            return result;
        }


        public async Task<DataTable> GetThreeDaysSalesMemo(RetailerRequest retailer)
        {
            _db.AddParameter(new OracleParameter("vRETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
            _db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("GETDAYSSALESMEMO");
            return result;
        }


        public DataTable GetSalesSummaryData(RetailerRequest retailer)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("GET_SALES_SUMMERY");// RSLGETSALESSUMMERYV2  BIODB SP: RSLGETSALESSUMMARY
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesSummaryData"));
            }
        }


        public async Task<DataTable> GetSalesDetails(SalesDetailRequest salesDetails)
        {
            try
            {
                salesDetails = DateFormater(salesDetails);

                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = salesDetails.retailerCode });
                _db.AddParameter(new OracleParameter("P_ITEM_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = salesDetails.itemCode });
                _db.AddParameter(new OracleParameter("P_START_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = Convert.ToDateTime(salesDetails.startDate) });
                _db.AddParameter(new OracleParameter("P_END_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = Convert.ToDateTime(salesDetails.endDate) });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("GET_SALES_DETAIL");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesDetails"));
            }
        }


        private static SalesDetailRequest DateFormater(SalesDetailRequest salesDetails)
        {
            string monthFirstDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString();
            string monthEndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).ToString();
            salesDetails.startDate = string.IsNullOrWhiteSpace(salesDetails.startDate) ? monthFirstDate : salesDetails.startDate;
            salesDetails.endDate = string.IsNullOrWhiteSpace(salesDetails.endDate) ? monthEndDate : salesDetails.endDate;

            return salesDetails;
        }

    }
}