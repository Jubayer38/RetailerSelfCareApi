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

using Domain.RequestModel;
using Infrastracture.DBManagers;
using MySqlConnector;
using System.Data;

namespace Infrastracture.Repositories.v2
{
    public class CommissionV2Repository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public CommissionV2Repository()
        {
            _mySql = new();
        }

        public CommissionV2Repository(string ConnectionString)
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
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_FROM_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
            _mySql.AddParameter(new MySqlParameter("P_TO_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });

            DataTable result = await _mySql.CallStoredProcedureSelectAsync("RSLGETDAILYCOMMSUMMARY");
            return result;
        }


        public async Task<DataTable> GetDailyCommissionDetails(CommissionRequest model)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_FROM_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.startDate });
            _mySql.AddParameter(new MySqlParameter("P_TO_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = model.endDate });
            _mySql.AddParameter(new MySqlParameter("P_SORT_BY_AMOUNT", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.sortByAmount });

            DataTable result = await _mySql.CallStoredProcedureSelectAsync("RSLGETDAILYCOMMDETAILS");
            return result;
        }


        public async Task<DataTable> GetSalesVsCommission(SearchRequestV2 model)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_QUERY_MONTH", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.searchText.ToUpper() });

            DataTable result = await _mySql.CallStoredProcedureSelectAsync("GET_SALES_VS_COMMISSION");
            return result;
        }


        public async Task<DataTable> StatementSummary(SearchRequest model, DateTime fd, DateTime td)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_FROM_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = fd });
            _mySql.AddParameter(new MySqlParameter("P_TO_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = td });

            var result = await _mySql.CallStoredProcedureSelectAsync("GETSTATEMENTSUMMARY");
            return result;
        }


        public async Task<DataTable> StatementDetails(SearchRequest model, DateTime fd, DateTime td)
        {
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_FROM_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = fd });
            _mySql.AddParameter(new MySqlParameter("P_TO_DATE", MySqlDbType.Date) { Direction = ParameterDirection.Input, Value = td });

            var result = await _mySql.CallStoredProcedureSelectAsync("RSLGETSTATEMENTDETAILS");
            return result;
        }


        public async Task<DataTable> TarVsAchvSummary(RetailerRequestV2 retailerRequest)
        {
            _ = int.TryParse(retailerRequest.retailerCode.Substring(1), out int retailerId);
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerId });
            var result = await _mySql.CallStoredProcedureSelectAsync("TARVSACHVSUMMARY");
            return result;
        }


        public async Task<DataTable> TarVsAchvDeatils(TarVsAchvRequestV2 tarVsAchvRequest)
        {
            _ = int.TryParse(tarVsAchvRequest.retailerCode.Substring(1), out int retailerId);
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_ID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = retailerId });
            _mySql.AddParameter(new MySqlParameter("P_KPIID", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = tarVsAchvRequest.kpiInt });
            var result = await _mySql.CallStoredProcedureSelectAsync("TARVSACHVDETAILS");
            return result;
        }

    }
}
