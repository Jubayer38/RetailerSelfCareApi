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

using Domain.RequestModel;
using Infrastracture.DBManagers;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Infrastracture.Repositories
{
    public class SalesRepository : IDisposable
    {
        private readonly OracleDbManager db;
        private readonly MySqlDbManager _mySql;

        public SalesRepository()
        {
            _mySql = new();
        }

        public SalesRepository(string ConnectionString)
        {
            db = new(ConnectionString);
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
                db.Dispose();
                _mySql.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========


        public async Task<DataTable> GetSalesUpdate(RetailerRequest retailerRequest)
        {
            db.AddParameter(new OracleParameter("RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailerRequest.retailerCode });
            db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var result = db.CallStoredProcedure_Select("RSLGETSALESUPDATE");
            return result;
        }


        public async Task<DataTable> GetTodaySalesMemo(RetailerRequest retailer)
        {
            db.AddParameter(new OracleParameter("vRETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
            db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var result = db.CallStoredProcedure_Select("RSLGETSALESMEMO");
            return result;
        }


        public async Task<DataTable> GetSalesWeeklyTrend(RetailerRequest retailerRequest)
        {
            db.AddParameter(new OracleParameter("RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailerRequest.retailerCode });
            db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var result = db.CallStoredProcedure_Select("RSLGETSALESWTEND");
            return result;
        }


        public async Task<DataTable> GetSalesRoutePerformance(RetailerRequest retailerRequest)
        {
            db.AddParameter(new OracleParameter("RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailerRequest.retailerCode });
            db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var result = db.CallStoredProcedure_Select("RSLGETSALESROUTEPERF");
            return result;
        }


        public async Task<DataTable> GetThreeDaysSalesMemo(RetailerRequest retailer)
        {
            db.AddParameter(new OracleParameter("vRETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
            db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var result = db.CallStoredProcedure_Select("RSLGETDAYSSALESMEMO");
            return result;
        }


        public async Task<DataTable> GetSalesSummaryV2(RetailerRequest retailer)
        {
            db.AddParameter(new OracleParameter("VRETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailer.retailerCode });
            db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = db.CallStoredProcedure_Select("RSLGETSALESSUMMERYV2");// BIODB SP: RSLGETSALESSUMMARY
            return result;
        }


        public async Task<DataTable> GetSalesDetails(SalesDetailRequest salesDetails)
        {
            salesDetails = DateFormater(salesDetails);

            db.AddParameter(new OracleParameter("RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = salesDetails.retailerCode });
            db.AddParameter(new OracleParameter("ITEM_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = salesDetails.itemCode });
            db.AddParameter(new OracleParameter("START_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = Convert.ToDateTime(salesDetails.startDate) });
            db.AddParameter(new OracleParameter("END_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = Convert.ToDateTime(salesDetails.endDate) });
            db.AddParameter(new OracleParameter("po_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var result = db.CallStoredProcedure_Select("RSLGETSALESDETAIL");
            return result;
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