
///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	All Commission related repositories
///	Creation Date :	14-Jan-2024
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
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Infrastracture.Repositories
{
    public class CommissionRepository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public CommissionRepository()
        {
            _mySql = new();
        }

        public CommissionRepository(string ConnectionString)
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


        public async Task<DataTable> GetDailyCommSummary(CommissionRequest model)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
            _db.AddParameter(new OracleParameter("P_FROM_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.startDate });
            _db.AddParameter(new OracleParameter("P_TO_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.endDate });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETDAILYCOMMSUMMARY");
            return result;
        }


        public async Task<DataTable> GetDailyCommDetails(CommissionRequest model)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
            _db.AddParameter(new OracleParameter("P_FROM_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.startDate });
            _db.AddParameter(new OracleParameter("P_TO_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = model.endDate });
            _db.AddParameter(new OracleParameter("P_SORT_BY_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.sortByAmount });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETDAILYCOMMDETAILS");
            return result;
        }


        public async Task<DataTable> GetSalesVsCommission(SearchRequestV2 model)
        {
            try
            {
                _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
                _db.AddParameter(new OracleParameter("P_QUERY_MONTH", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.searchText.ToUpper() });
                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                var result = _db.CallStoredProcedure_Select("GET_SALES_VS_COMMISSION");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSalesVsCommission"));
            }
        }


        public async Task<DataTable> StatementSummary(SearchRequest model, DateTime fd, DateTime td)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
            _db.AddParameter(new OracleParameter("P_FROM_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = fd });
            _db.AddParameter(new OracleParameter("P_TO_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = td });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETSTATEMENTSUMMARY");
            return result;
        }


        public async Task<DataTable> StatementDetails(SearchRequest model, DateTime fd, DateTime td)
        {
            _db.AddParameter(new OracleParameter("P_RETAILER_CODE", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.retailerCode });
            _db.AddParameter(new OracleParameter("P_FROM_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = fd });
            _db.AddParameter(new OracleParameter("P_TO_DATE", OracleDbType.Date, ParameterDirection.Input) { Value = td });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLGETSTATEMENTDETAILS");
            return result;
        }


        public async Task<DataTable> TarVsAchvSummary(RetailerRequestV2 retailerRequest)
        {
            _ = int.TryParse(retailerRequest.retailerCode.Substring(1), out int retailerId);
            _db.AddParameter(new OracleParameter("P_RETAILER_ID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = retailerId });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLTARVSACHVSUMMARY");
            return result;
        }


        public async Task<DataTable> TarVsAchvDeatils(TarVsAchvRequestV2 tarVsAchvRequest)
        {
            _ = int.TryParse(tarVsAchvRequest.retailerCode.Substring(1), out int retailerId);
            _db.AddParameter(new OracleParameter("P_RETAILER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = retailerId });
            _db.AddParameter(new OracleParameter("P_KPIID", OracleDbType.Decimal, ParameterDirection.Input) { Value = tarVsAchvRequest.kpiInt });
            _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

            var result = _db.CallStoredProcedure_Select("RSLTARVSACHVDETAILS");
            return result;
        }

    }
}
