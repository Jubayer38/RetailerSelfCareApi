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

using Infrastracture.DBManagers;
using MySqlConnector;
using System.Data;


namespace Infrastracture.Repositories
{
    public class UserRepository : IDisposable
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public UserRepository()
        {
            _mySql = new();
        }

        public UserRepository(string connectionString)
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


        public async Task<long> ValidateExternalUsers(string userName, string pass)
        {
            _mySql.AddParameter(new MySqlParameter("P_USERNAME", MySqlDbType.Int64) { Direction = ParameterDirection.Input, Value = userName });
            _mySql.AddParameter(new MySqlParameter("P_PASSWORD", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = pass });

            var result = await _mySql.InsertByStoredProcedureAsync("RSLVALIDATE_EXTERNAL_USER");
            return result;
        }

    }
}