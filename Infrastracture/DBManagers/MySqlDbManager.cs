///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	MySql DB Connection manager.
///	Creation Date :	13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.StaticClass;
using MySqlConnector;
using System.Data;
using System.Transactions;
using static Domain.Enums.EnumCollections;

namespace Infrastracture.DBManagers
{
    public class MySqlDbManager
    {
        private readonly MySqlConnection _connection;
        private readonly MySqlCommand _command;

        public MySqlDbManager()
        {
            _connection = new MySqlConnection(Connections.RetAppMySqlCS);
            _command = new MySqlCommand();
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
                _connection.Dispose();
                _command.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========


        public void AddParameter(MySqlParameter param)
        {
            _command.Parameters.Add(param);
        }


        /// <summary>
        /// This method directly call the stored procedure where procedure name is mention in argument.
        /// This method also use transaction.
        /// <para>Use this method if you need to Insert, Update, Delete data in database</para>
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        private async Task CallStoredProcedureAsync(string procedureName)
        {
            using (_connection)
            {
                try
                {
                    if (_connection.State == ConnectionState.Closed) await _connection.OpenAsync();

                    _command.Connection = _connection;
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.CommandText = procedureName;

                    MySqlTransaction transaction = await _connection.BeginTransactionAsync();
                    _command.Transaction = transaction;

                    try
                    {
                        await _command.ExecuteNonQueryAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
                finally
                {
                    await _command.DisposeAsync();
                    if (_connection.State != ConnectionState.Closed) await _connection.CloseAsync();
                    await _connection.DisposeAsync();
                }
            }
        }


        public async Task<long> InsertByStoredProcedureAsync(string procedureName)
        {
            string outParamName = FrequentlyUsedDbParams.P_PKVALUE.ToString();
            MySqlParameter parameter = new(outParamName, MySqlDbType.Int64) { Direction = ParameterDirection.Output };

            AddParameter(parameter);

            await CallStoredProcedureAsync(procedureName);

            var result = _command.Parameters.Where(w => w.ParameterName == outParamName).Select(s => s.Value).ToList()[0];
            _ = long.TryParse(result?.ToString(), out long pkValue);

            return pkValue;
        }


        public async Task<DataTable> CallStoredProcedureSelectAsync(string procedureName)
        {
            using (_connection)
            {
                try
                {
                    if (_connection.State == ConnectionState.Closed) await _connection.OpenAsync();

                    _command.Connection = _connection;
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.CommandText = procedureName;

                    DataTable dt = new(procedureName);

                    using (MySqlDataAdapter adapter = new(_command))
                    {
                        adapter.Fill(dt);
                    }

                    return dt;
                }
                finally
                {
                    await _command.DisposeAsync();
                    if (_connection.State != ConnectionState.Closed) await _connection.CloseAsync();
                    await _connection.DisposeAsync();
                }
            }
        }


        public async Task<object> CallStoredProcedureObjectAsync(string procedureName, string outParamName)
        {
            using (_connection)
            {
                try
                {
                    if (_connection.State == ConnectionState.Closed) await _connection.OpenAsync();

                    _command.Connection = _connection;
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.CommandText = procedureName;

                    await _command.ExecuteNonQueryAsync();
                    var result = _command.Parameters.Where(w => w.ParameterName == outParamName).Select(s => s.Value).ToList()[0];
                    return result;
                }
                finally
                {
                    await _command.DisposeAsync();
                    if (_connection.State != ConnectionState.Closed) await _connection.CloseAsync();
                    await _connection.DisposeAsync();
                }
            }
        }


        public async Task ExecuteStoredProcedureAsync(string procedureName)
        {
            using (_connection)
            {
                try
                {
                    if (_connection.State == ConnectionState.Closed) await _connection.OpenAsync();

                    _command.Connection = _connection;
                    _command.CommandType = CommandType.StoredProcedure;
                    _command.CommandText = procedureName;

                    try
                    {
                        await _command.ExecuteNonQueryAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
                finally
                {
                    await _command.DisposeAsync();
                    if (_connection.State != ConnectionState.Closed) await _connection.CloseAsync();
                    await _connection.DisposeAsync();
                }
            }
        }


        public void InsertUsingMySQLBulk(string tblName, DataTable dt)
        {
            using (_connection)
            {
                try
                {
                    var temp = dt.Rows.Count;
                    string strSQl = "SELECT * FROM " + tblName;

                    TransactionScope scope = new();
                    if (_connection.State == ConnectionState.Closed) _connection.Open();
                    _command.Connection = _connection;
                    MySqlDataAdapter dataAdapter = new(strSQl, _connection);
                    MySqlCommandBuilder cmdBuilder = new(dataAdapter);
                    dataAdapter.InsertCommand = cmdBuilder.GetInsertCommand();

                    Parallel.ForEach(dt.AsEnumerable(), dr =>
                    {
                        dr.SetAdded();
                    });

                    dataAdapter.Update(dt);
                    scope.Complete();
                    scope.Dispose();
                }
                finally
                {
                    _command.Dispose();
                    if (_connection.State != ConnectionState.Closed) _connection.Close();
                    _connection.Dispose();
                }
            }
        }

    }
}