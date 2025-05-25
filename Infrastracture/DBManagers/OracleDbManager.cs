///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Oracle DB Connection manager.
///	Creation Date :	13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.StaticClass;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Infrastracture.DBManagers
{
    public class OracleDbManager : IDisposable
    {
        public OracleConnection connection;
        public OracleCommand command;

        public OracleDbManager()
        {
            connection = new OracleConnection(Connections.DefaultCS);
            command = new OracleCommand();
        }


        public OracleDbManager(string ConnectionString)
        {
            connection = new OracleConnection(ConnectionString);
            command = new OracleCommand();
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
                connection.Dispose();
                command.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========

        /// <summary>
        /// This method directly call the stored procedure where procedure name is mention in argument. If need any parameter passing, just do it before call this method by AddParameter()
        /// </summary>
        /// <param name="storedProcedureName">The store procedure name</param>
        public void CallStoredProcedure(string storedProcedureName)
        {
            using (connection)
            {
                //long rowAffected = 0;
                try
                {
                    if (connection.State != ConnectionState.Open) connection.Open();

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storedProcedureName;
                    OracleTransaction transaction = connection.BeginTransaction();
                    command.Transaction = transaction;
                    try
                    {
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }

                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }
            }
        }


        /// <summary>
        /// This function helps to get outoput paremeter from procedure as an object.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Output Parameter</returns>
        public object GetValueOfOutputParameter(string outputParam)
        {
            return command.Parameters[outputParam].Value;
        }


        /// <summary>
        /// This function takes a produre name as string and 
        /// returns an object which determines weather the procedure exceute successsfully or not.  
        /// </summary>
        /// <returns>int</returns>
        public object CallStoredProcedure_1(string storedProcedureName, string outputParam)
        {
            using (connection)
            {
                try
                {
                    if (connection.State != ConnectionState.Open) connection.Open();

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storedProcedureName;
                    OracleTransaction transaction = connection.BeginTransaction();
                    command.Transaction = transaction;
                    try
                    {
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        return command.Parameters[outputParam].Value;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }
            }
        }


        public async Task<object> CallStoredProcedure(string storedProcedureName, string outputParam)
        {
            using (connection)
            {
                try
                {
                    if (connection.State != ConnectionState.Open) await connection.OpenAsync();

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storedProcedureName;
                    OracleTransaction transaction = connection.BeginTransaction();
                    command.Transaction = transaction;
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        transaction.Commit();
                        return command.Parameters[outputParam].Value;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }
            }
        }


        public DateTime CallStoredProceduredbDate()
        {
            DateTime pkValue = DateTime.Now;

            OracleParameter param = new("po_PKValue", OracleDbType.Date)
            {
                Direction = ParameterDirection.Output
            };
            AddParameter(param);

            try
            {
                CallStoredProcedure("SSP_GetDBDate");

                if (param.Value != DBNull.Value)
                    pkValue = ((OracleDate)param.Value).Value;
            }
            catch (Exception)
            {

                throw;
            }


            return pkValue;
        }


        public void AddParameter(OracleParameter param)
        {
            command.Parameters.Add(param);
        }


        public DataSet CallStoredProcedure_SelectDS(string storedProcedureName)
        {

            using (connection)
            {
                try
                {
                    if (connection.State != ConnectionState.Open) connection.Open();

                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;

                    DataSet dt = new(storedProcedureName);

                    using (OracleDataAdapter adapter = new(command))
                    {
                        //adapter.SelectCommand = command;
                        adapter.Fill(dt);
                    }
                    return dt;
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }
            }
        }


        public DataTable CallStoredProcedure_Select(string storedProcedureName)
        {
            using (connection)
            {
                try
                {
                    //if (connection.State != ConnectionState.Open) connection.Open();

                    command.Connection = connection;
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;

                    DataTable dt = new(storedProcedureName);

                    using (OracleDataAdapter adapter = new(command))
                    {
                        //adapter.SelectCommand = command;
                        adapter.Fill(dt);
                    }

                    return dt;
                }
                catch (OracleException)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }
            }
        }


        /// <summary>
        /// This method directly call the stored procedure where procedure name is mention in argument.It must return inserted primary key value. If need any parameter passing, just do it before call this method by AddParameter()
        /// Primary key value[Developmer must implement code in SP for return]
        /// </summary>
        /// <param name="storedProcedureName">Enter Stored Procedure Name</param>
        /// <returns>Primary key value[Developmer must implement code in SP for return]</returns>
        public long? CallStoredProcedure_Insert(string storedProcedureName)
        {
            long? pkValue = null;
            OracleParameter param = new("po_PKValue", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };

            AddParameter(param);
            try
            {
                CallStoredProcedure(storedProcedureName);

                if (param.Value != DBNull.Value)
                    pkValue = (long)((OracleDecimal)param.Value).Value;
                return pkValue;
            }
            catch (Exception)
            {
                throw;
            }
            //return pkValue;
        }


        /// <summary>
        /// Use this method when want to save multiple data in one transaction,
        /// Remember when you use this method, you need to call DBTrunsaction method at finally
        /// </summary>
        /// <param name="storedProcedureName">The store procedure name</param>
        internal protected long? CallSPForMultipleTrunsection(string storedProcedureName)
        {
            long? pkValue = null;
            OracleParameter param = new("PO_PKVALUE", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };

            AddParameter(param);

            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = storedProcedureName;

                try
                {
                    command.ExecuteNonQuery();

                    if (param.Value != DBNull.Value)
                        pkValue = (long)((OracleDecimal)param.Value).Value;
                    return pkValue;
                }
                catch (Exception)
                {
                    throw;
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                command.Parameters.Clear();
            }
        }


        /// <summary>
        /// This method can be used for multiple trunsaction. 
        /// </summary>
        /// <returns></returns>
        internal protected bool DBTrunsaction()
        {
            bool result;
            using (connection)
            {
                OracleTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                try
                {
                    transaction.Commit();
                    result = true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    result = false;
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }
            }
            return result;
        }


        /// <summary>
        /// Add this method for proper way DBNull handle. 21-Apr-2022 By Basher
        /// This method directly call the stored procedure where procedure name is mention in argument.
        /// It can return inserted/updated primary key value or any number.
        /// If need any parameter passing, just do it before call this method by AddParameter()
        /// Primary key value[Developmer must implement code in SP for return]
        /// </summary>
        /// <param name="storedProcedureName">Enter Stored Procedure Name</param>
        /// <returns>Primary key value[Developmer must implement code in SP for return]</returns>
        public long? CallStoredProcedureInsertV2(string storedProcedureName)
        {
            long? pkValue = null;
            OracleParameter param = new("PO_PKVALUE", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };

            AddParameter(param);
            try
            {
                CallStoredProcedure(storedProcedureName);

                bool isNull = ((OracleDecimal)param.Value).IsNull;
                if (!isNull)
                {
                    pkValue = (long)((OracleDecimal)param.Value).Value;
                }

                return pkValue;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void CallStoredProcedureDelete(string storedProcedureName)
        {
            try
            {
                CallStoredProcedure(storedProcedureName);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region Retailer APP CR 2022

        /// <summary>
        /// This method call only getting data param wise by procedure.
        /// <para>Here no transaction is used.</para>
        /// </summary>
        /// <param name="storedProcedureName">The store procedure name</param>
        public void CallStoredProcedureV2(string storedProcedureName)
        {
            using (connection)
            {
                try
                {
                    if (connection.State != ConnectionState.Open) connection.Open();

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storedProcedureName;

                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }
            }
        }


        /// <summary>
        /// Begin Transaction for Multiple _db call under sigle Transaction
        /// </summary>
        /// <returns></returns>
        internal protected Tuple<OracleConnection, OracleTransaction, OracleCommand> BeginTransaction()
        {
            if (connection.State != ConnectionState.Open) connection.Open();

            OracleTransaction myTrans;
            myTrans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            command.Transaction = myTrans;

            var tuple = Tuple.Create(connection, myTrans, command);
            return tuple;
        }


        /// <summary>
        /// Use this function when need to Multiple _db call under sigle Transaction
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="storedProcedureName"></param>
        internal protected void CallStoredProcedureMultiTran(OracleConnection connection, string storedProcedureName)
        {
            if (connection.State != ConnectionState.Open)
            {
                throw new Exception("Oracle Connection Closed");
            }

            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;

            command.ExecuteNonQuery();
        }


        /// <summary>
        /// Call this insert method when need to multiple insert in single transaction
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="storedProcedureName"></param>
        /// <returns></returns>
        internal protected long? StoredProcedureInsertMultiTran(OracleConnection connection, string storedProcedureName)
        {
            long? pkValue = null;
            OracleParameter param = new("PO_PKVALUE", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(param);

            CallStoredProcedureMultiTran(connection, storedProcedureName);

            bool isNull = ((OracleDecimal)param.Value).IsNull;
            if (!isNull)
            {
                pkValue = (long)((OracleDecimal)param.Value).Value;
            }

            return pkValue;
        }


        /// <summary>
        /// Call a procedure asyncronously without transaction and out param
        /// </summary>
        /// <param name="storedProcedureName"></param>
        public async Task CallStoredProcedureOnlyAsync(string storedProcedureName)
        {
            using (connection)
            {
                try
                {
                    if (connection.State != ConnectionState.Open) await connection.OpenAsync();

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = storedProcedureName;

                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed) connection.Close();
                }
            }
        }


        public async Task<long> SaveUsingOracleBulkCopy(DataTable dt)
        {
            long isInserted = 0;
            try
            {
                using (connection)
                {
                    await connection.OpenAsync();
                    int[] reqID = new int[dt.Rows.Count];
                    string[] retailerCode = new string[dt.Rows.Count];
                    string[] productType = new string[dt.Rows.Count];
                    string[] productCode = new string[dt.Rows.Count];
                    int[] productCount = new int[dt.Rows.Count];
                    int[] statusCount = new int[dt.Rows.Count];

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        reqID[j] = Convert.ToInt32(dt.Rows[j]["REQUESTID"]);
                        retailerCode[j] = Convert.ToString(dt.Rows[j]["RETAILERCODE"]);
                        productType[j] = Convert.ToString(dt.Rows[j]["PRODUCTTYPE"]);
                        productCode[j] = Convert.ToString(dt.Rows[j]["PRODUCTCODE"]);
                        productCount[j] = Convert.ToInt32(dt.Rows[j]["PRODUCTCOUNT"]);
                        statusCount[j] = Convert.ToInt32(dt.Rows[j]["STATUS"]);
                    }

                    OracleParameter reqIds = new();
                    reqIds.OracleDbType = OracleDbType.Int32;
                    reqIds.Value = reqID;

                    OracleParameter retailerCodes = new();
                    retailerCodes.OracleDbType = OracleDbType.Varchar2;
                    retailerCodes.Value = retailerCode;

                    OracleParameter productTypes = new();
                    productTypes.OracleDbType = OracleDbType.Varchar2;
                    productTypes.Value = productType;

                    OracleParameter productCodes = new();
                    productCodes.OracleDbType = OracleDbType.Varchar2;
                    productCodes.Value = productCode;

                    OracleParameter productCounts = new();
                    productCounts.OracleDbType = OracleDbType.Varchar2;
                    productCounts.Value = productCount;

                    OracleParameter statusCounts = new();
                    statusCounts.OracleDbType = OracleDbType.Varchar2;
                    statusCounts.Value = statusCount;

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO RSLBULKUPDATEDDATA (REQUESTID, RETAILERCODE, PRODUCTTYPE,PRODUCTCODE,PRODUCTCOUNT,STATUS) VALUES (:1, :2, :3, :4, :5, :6)";
                    cmd.ArrayBindCount = reqID.Length;
                    cmd.Parameters.Add(reqIds);
                    cmd.Parameters.Add(retailerCodes);
                    cmd.Parameters.Add(productTypes);
                    cmd.Parameters.Add(productCodes);
                    cmd.Parameters.Add(productCounts);
                    cmd.Parameters.Add(statusCounts);

                    await cmd.ExecuteNonQueryAsync();
                    isInserted = 1;
                }

                return isInserted;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed) connection.Close();
            }
        }

        #endregion
    }
}
