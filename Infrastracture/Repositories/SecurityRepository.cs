///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Repository for user validation
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Domain.ViewModel.LogModels;
using Infrastracture.DBManagers;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using static Domain.Enums.EnumCollections;

namespace Infrastracture.Repositories
{
    public class SecurityRepository
    {
        private readonly OracleDbManager _db;
        private readonly MySqlDbManager _mySql;

        public SecurityRepository()
        {
            _mySql = new();
        }

        public SecurityRepository(string connectionString)
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


        /// <summary>
        /// Validate user by iTopUpNumber and cred
        /// </summary>
        /// <param name="VMUserInfov2"></param>
        /// <returns></returns>
        public async Task<DataTable> ValidateUser(VMUserInfo model)
        {
            DataTable result = new();
            try
            {
                MySqlDbManager _mySql = new();
                _mySql.AddParameter(new MySqlParameter("P_MOBILE_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });
                _mySql.AddParameter(new MySqlParameter("P_PASSWORD", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.password });

                result = await _mySql.CallStoredProcedureSelectAsync("RSLVALIDATE_USER");
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "ValidateUser",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "RSLVALIDATE_USER",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "ValidateUser"));
            }

            return result;
        }


        /// <summary>
        /// Save LogIn Attempt Info with new requirement
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<long> SaveLoginAtmInfo(UserLogInAttempt model)
        {
            long result;

            try
            {
                _mySql.AddParameter(new MySqlParameter("P_USERID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.userid });
                _mySql.AddParameter(new MySqlParameter("P_IS_SUCCESS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.is_success });
                _mySql.AddParameter(new MySqlParameter("P_IP_ADDRESS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.ip_address });
                _mySql.AddParameter(new MySqlParameter("P_MACHINE_NAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.machine_name });
                _mySql.AddParameter(new MySqlParameter("P_LOGINPROVIDER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.loginprovider });
                _mySql.AddParameter(new MySqlParameter("P_DEVICEID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceid });
                _mySql.AddParameter(new MySqlParameter("P_LAN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.lan });
                _mySql.AddParameter(new MySqlParameter("P_VERSIONCODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.versioncode });
                _mySql.AddParameter(new MySqlParameter("P_VERSIONNAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.versionname });
                _mySql.AddParameter(new MySqlParameter("P_OSVERSION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.osversion });
                _mySql.AddParameter(new MySqlParameter("P_KERNELVERSION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.kernelversion });
                _mySql.AddParameter(new MySqlParameter("P_FERMWAREVIRSION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.fermwarevirsion });
                _mySql.AddParameter(new MySqlParameter("P_IMEI", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.imei });
                _mySql.AddParameter(new MySqlParameter("P_DEVICEMODEL", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.devicemodel });
                _mySql.AddParameter(new MySqlParameter("P_LAT", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.lat });
                _mySql.AddParameter(new MySqlParameter("P_LNG", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.lng });

                result = await _mySql.InsertByStoredProcedureAsync("RSLLOGINATTEMPTINSERT");
                return result;
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "SaveLoginAtmInfoV3",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "RSLLOGINATTEMPTINSERT",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveLoginAtmInfoV3"));
            }
        }


        //public int IsSecurityTokenValid2(string loginProvider, string deviceId)
        //{
        //    int status = 0;

        //    try
        //    {
        //        _mySql.AddParameter(new MySqlParameter("P_DEVICEID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = deviceId });
        //        _mySql.AddParameter(new MySqlParameter("P_LOGIN_PROVIDER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = loginProvider });
        //        MySqlParameter param = new MySqlParameter(nameof(FrequentlyUsedDbParams.P_RETURN), MySqlDbType.Int64) { Direction = ParameterDirection.Output };

        //        _mySql.AddParameter(param);

        //        _mySql.CallStoredProcedureSelectAsync("ENROLL_EXT_CAMPAIGN");

        //        if (param.Value != DBNull.Value)
        //        {
        //            status = Convert.ToInt32(param.Value);
        //        }
        //        return status;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (TextLogging.IsEnableErrorTextLog)
        //        {
        //            ErrorLogModel log = new ErrorLogModel()
        //            {
        //                methodName = "IsSecurityTokenValid2",
        //                logSaveTime = DateTime.Now,
        //                requestModel = new { loginProvider, deviceId }.ToJsonString(),
        //                procedureName = "ISLOGINPROVIDERVALID",
        //                errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
        //                errorSource = ex.Source,
        //                errorCode = ex.HResult,
        //                errorDetails = ex.StackTrace
        //            };

        //            TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
        //        }

        //        throw new Exception(HelperMethod.ExMsgBuild(ex, "IsSecurityTokenValid2"));
        //    }
        //}

        //public DataTable IsUserCurrentlyLoggedIn(decimal userId)
        //{
        //    DataTable dataTable;
        //    try
        //    {
        //        _db.AddParameter(new OracleParameter("P_USER_ID", OracleDbType.Decimal, ParameterDirection.Input) { Value = userId });
        //        _db.AddParameter(new OracleParameter("PO_LOGIN_PROVIDER", OracleDbType.RefCursor, ParameterDirection.Output));

        //        dataTable = _db.CallStoredProcedure_Select("RSLISUSERCURRENTLYLOGGEDIN");
        //    }
        //    catch (Exception ex)
        //    {
        //        if (TextLogging.IsEnableErrorTextLog)
        //        {
        //            ErrorLogModel log = new ErrorLogModel()
        //            {
        //                methodName = "IsUserCurrentlyLoggedIn",
        //                logSaveTime = DateTime.Now,
        //                requestModel = new { userId }.ToJsonString(),
        //                procedureName = "RSLISUSERCURRENTLYLOGGEDIN",
        //                errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
        //                errorSource = ex.Source,
        //                errorCode = ex.HResult,
        //                errorDetails = ex.StackTrace
        //            };

        //            TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
        //        }

        //        throw new Exception(HelperMethod.ExMsgBuild(ex, "IsUserCurrentlyLoggedIn"));
        //    }
        //    return dataTable;
        //}

        //public long SaveLoginOTPLessAtmInfo(UserLogInOTPLessAttempt model)
        //{
        //    long result;
        //    try
        //    {
        //        _db.AddParameter(new OracleParameter("P_USERID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.userid });
        //        _db.AddParameter(new OracleParameter("P_IS_SUCCESS", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.is_success });
        //        _db.AddParameter(new OracleParameter("P_IP_ADDRESS", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.ip_address });
        //        _db.AddParameter(new OracleParameter("P_MACHINE_NAME", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.machine_name });
        //        _db.AddParameter(new OracleParameter("P_LOGINPROVIDER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.loginprovider });
        //        _db.AddParameter(new OracleParameter("P_DEVICEID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.deviceid });
        //        _db.AddParameter(new OracleParameter("P_LAN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.lan });
        //        _db.AddParameter(new OracleParameter("P_VERSIONCODE", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.versioncode });
        //        _db.AddParameter(new OracleParameter("P_VERSIONNAME", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.versionname });
        //        _db.AddParameter(new OracleParameter("P_OSVERSION", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.osversion });
        //        _db.AddParameter(new OracleParameter("P_KERNELVERSION", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.kernelversion });
        //        _db.AddParameter(new OracleParameter("P_FERMWAREVIRSION", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.fermwarevirsion });

        //        result = (long)_db.CallStoredProcedure_Insert("RSLLOGINOTPLESSATTEMPTINSERT");
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (TextLogging.IsEnableErrorTextLog)
        //        {
        //            ErrorLogModel log = new ErrorLogModel()
        //            {
        //                methodName = "SaveLoginOTPLessAtmInfo",
        //                logSaveTime = DateTime.Now,
        //                requestModel = model.ToJsonString(),
        //                procedureName = "RSLLOGINOTPLESSATTEMPTINSERT",
        //                errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
        //                errorSource = ex.Source,
        //                errorCode = ex.HResult,
        //                errorDetails = ex.StackTrace
        //            };

        //            TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
        //        }

        //        throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveLoginOTPLessAtmInfo"));
        //    }
        //}


        /// <summary>
        /// Generate OTP and save into MySQL DB
        /// </summary>
        /// <param name="ValidatePWD"></param>
        /// <returns>Returns PWDValidationResponse</returns>
        public async Task<long> GenerateOTPDAL(OTPGenerateRequest model)
        {
            long result = 0;

            try
            {
                int isnewotp = model.isNewOTP ? 1 : 0;

                _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });

                _mySql.AddParameter(new MySqlParameter("P_DEVICEID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceId });

                _mySql.AddParameter(new MySqlParameter("P_MODULE_NAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.moduleName });

                _mySql.AddParameter(new MySqlParameter("P_IS_NEW", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = isnewotp });

                result = await _mySql.InsertByStoredProcedureAsync("INSERTANDGETOTP");
                return result;
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "GenerateOTPDAL",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "INSERTANDGETOTP",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "GenerateOTPDAL"));
            }
        }


        public void SMSLogSaveInDB(SMSInformationModel sms)
        {
            Task.Factory.StartNew(async () =>
            {
                await SaveSMSIntoTABLE(sms);
            });
        }


        public async Task<DataTable> GetChangePasswordGlobalSettingsData()
        {
            DataTable dataRows;
            try
            {
                MySqlDbManager _mySql = new();
                return dataRows = await _mySql.CallStoredProcedureSelectAsync("RSLGETDATAFORPWDCHANGE");
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "GetChangePasswordGlobalSettingsData",
                        logSaveTime = DateTime.Now,
                        procedureName = "RSLGETDATAFORPWDCHANGE",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetChangePasswordGlobalSettingsData"));
            }
        }


        /// <summary>
        /// Save New Device Info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<long> SaveDeviceInfo(DeviceInfo model)
        {
            long result;

            try
            {
                MySqlDbManager _mySql = new();
                int isPrimary = model.isPrimary ? 1 : 0;
                int isEnable = model.isEnable ? 1 : 0;

                _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
                _mySql.AddParameter(new MySqlParameter("P_DEVICEID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceId });
                _mySql.AddParameter(new MySqlParameter("P_IS_PRIMARY", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = isPrimary });
                _mySql.AddParameter(new MySqlParameter("P_IS_ENABLE", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = isEnable });
                _mySql.AddParameter(new MySqlParameter("P_IP_ADDRESS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.ipAddress });
                _mySql.AddParameter(new MySqlParameter("P_MACHINE_NAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.machineName });
                _mySql.AddParameter(new MySqlParameter("P_VERSIONCODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.versionCode });
                _mySql.AddParameter(new MySqlParameter("P_VERSIONNAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.versionName });
                _mySql.AddParameter(new MySqlParameter("P_OSVERSION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.osVersion });
                _mySql.AddParameter(new MySqlParameter("P_KERNELVERSION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.kernelVersion });
                _mySql.AddParameter(new MySqlParameter("P_FERMWAREVIRSION", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.fermwareVersion });
                _mySql.AddParameter(new MySqlParameter("P_PLATFORMID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.platformId });
                _mySql.AddParameter(new MySqlParameter("P_IMEI", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.imeiNumber });
                _mySql.AddParameter(new MySqlParameter("P_DEVICEMODEL", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceModel });
                _mySql.AddParameter(new MySqlParameter("P_LAT", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.lat });
                _mySql.AddParameter(new MySqlParameter("P_LNG", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.lng });
                _mySql.AddParameter(new MySqlParameter("P_CREATEDBY", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.createdBy });

                result = await _mySql.InsertByStoredProcedureAsync("RSLSAVEDEVICEINFO");
                return result;
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "SaveDeviceInfo",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "RSLSAVEDEVICEINFO",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveDeviceInfo"));
            }
        }


        public async Task<long> ChangePassword(VMChangePassword model)
        {
            try
            {
                MySqlDbManager _mySql = new();
                _mySql.AddParameter(new MySqlParameter("P_OLD_PASS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.old_password });
                _mySql.AddParameter(new MySqlParameter("P_NEW_PASS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.new_password });
                _mySql.AddParameter(new MySqlParameter("P_USERNAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.username });

                long data = await _mySql.InsertByStoredProcedureAsync("RSLCHANGEPASSWORD");

                return Convert.ToInt64(data.ToString());
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "ChangePassword",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "RSLCHANGEPASSWORD",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "ChangePassword"));
            }
        }

        /// <summary>
        /// Validate reseller Password
        /// </summary>
        /// <param name="ValidatePWD"></param>
        /// <returns>Returns PWDValidationResponse</returns>
        public async Task<int> ISPWDValid(DevicePWDValidationRequest model)
        {
            int result = 0;

            try
            {
                _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });
                _mySql.AddParameter(new MySqlParameter("P_GIVEN_PASSWORD_HASH", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.password });

                MySqlParameter param = new(FrequentlyUsedDbParams.P_RETURN.ToString(), MySqlDbType.Int32) { Direction = ParameterDirection.Output };

                _mySql.AddParameter(param);

                object procReturn = await _mySql.CallStoredProcedureObjectAsync("RSLISPWDVALID", FrequentlyUsedDbParams.P_RETURN.ToString());

                result = Convert.ToInt32(procReturn.DBNullToString());

                return result;
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "ISPWDValid",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "RSLISPWDVALID",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "ISPWDValid"));
            }
        }

        /// <summary>
        /// Validate OTP
        /// </summary>
        /// <param name="ValidatePWD"></param>
        /// <returns>Returns DataTable</returns>
        public async Task<DataTable> ValidateOTPGetUser(VMValidateOtp model)
        {
            DataTable result = null;
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_MOBILE_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });
                _mySql.AddParameter(new MySqlParameter("P_OTP", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.otp });
                _mySql.AddParameter(new MySqlParameter("P_DEVICE_ID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceId });
                _mySql.AddParameter(new MySqlParameter("P_MODULE_NAME", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.moduleName });

                result = await _mySql.CallStoredProcedureSelectAsync("VALIDATE_OTP_GETUSER");
                return result;
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "ValidateOTPGetUserV3",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "VALIDATE_OTP_GETUSER",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 400),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "ValidateOTPGetUserV3"));
            }
        }

        /// <summary>
        /// Validate retailer device with limit. Also check sim-seller status.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns DataTable</returns>
        public async Task<DataTable> ValidateDevice(DeviceValidationRequest model)
        {
            DataTable result = new();
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });
                _mySql.AddParameter(new MySqlParameter("P_DEVICEID", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceId });

                result = await _mySql.CallStoredProcedureSelectAsync("VALIDATEDEVICE_V2");
                return result;
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "ValidateDevice",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "VALIDATEDEVICE_V2",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "ValidateDevice"));
            }
        }

        /// <summary>
        /// Save Password during Device registration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<long> SavePassword(VMSavePassword model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_NEW_PASS_HASH", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.newPassHash });
                _mySql.AddParameter(new MySqlParameter("P_NEW_PASS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.newPassword });
                _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });

                long data = await _mySql.InsertByStoredProcedureAsync("RSLSAVEPASSWORD");

                return data;
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "SavePassword",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "RSLSAVEPASSWORD",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "SavePassword"));
            }
        }


        public async Task<long> SubmitNewDeviceRequest(NewDeviceRequest model)
        {
            _mySql.AddParameter(new MySqlParameter("P_ITOPUP_NUMBER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.iTopUpNumber });
            _mySql.AddParameter(new MySqlParameter("P_DEVICE_COUNT", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.deviceCount });

            long result = await _mySql.InsertByStoredProcedureAsync("RSL_REQUEST_FOR_NEW_DEVICE");
            return result;
        }


        #region==========|  Old Security Controller Methods  |==========

        /// <summary>
        /// Validate rso user
        /// </summary>
        /// <param name="IsRSOUserValid"></param>
        /// <returns>Returns RSO User Validation response</returns>
        public async Task<DataTable> IsInternalUserValid(VMUserInfo model)
        {
            DataTable result = null;

            try
            {
                _db.AddParameter(new OracleParameter("P_USER_NAME", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.userName });

                _db.AddParameter(new OracleParameter("P_PASSWORD_HASH", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.password });

                _db.AddParameter(new OracleParameter("PO_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output));

                result = _db.CallStoredProcedure_Select("RSLVALIDATE_INTERNALUSER");
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "IsInternalUserValid",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "RSLVALIDATE_INTERNALUSER",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }
                throw new Exception(HelperMethod.ExMsgBuild(ex, "IsInternalUserValid"));
            }
            return result;
        }


        /// <summary>
        /// Save LogIn Attempt Info with new requirement
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<long> SaveInternalLoginAtmInfo(UserLogInAttempt model)
        {
            long result;

            try
            {
                _db.AddParameter(new OracleParameter("P_USERID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.userid });
                _db.AddParameter(new OracleParameter("P_IS_SUCCESS", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.is_success });
                _db.AddParameter(new OracleParameter("P_IP_ADDRESS", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.ip_address });
                _db.AddParameter(new OracleParameter("P_MACHINE_NAME", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.machine_name });
                _db.AddParameter(new OracleParameter("P_LOGINPROVIDER", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.loginprovider });
                _db.AddParameter(new OracleParameter("P_DEVICEID", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.deviceid });
                _db.AddParameter(new OracleParameter("P_LAN", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.lan });
                _db.AddParameter(new OracleParameter("P_VERSIONCODE", OracleDbType.Decimal, ParameterDirection.Input) { Value = model.versioncode });
                _db.AddParameter(new OracleParameter("P_VERSIONNAME", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.versionname });
                _db.AddParameter(new OracleParameter("P_OSVERSION", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.osversion });
                _db.AddParameter(new OracleParameter("P_KERNELVERSION", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.kernelversion });
                _db.AddParameter(new OracleParameter("P_FERMWAREVIRSION", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.fermwarevirsion });
                _db.AddParameter(new OracleParameter("P_IMEI", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.imei });
                _db.AddParameter(new OracleParameter("P_DEVICEMODEL", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.devicemodel });
                _db.AddParameter(new OracleParameter("P_LAT", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.lat });
                _db.AddParameter(new OracleParameter("P_LNG", OracleDbType.Varchar2, ParameterDirection.Input) { Value = model.lng });

                result = (long)_db.CallStoredProcedure_Insert("RSLLOGINATTEMPTINSERTV2");
                return result;
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "SaveInternalLoginAtmInfo",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "RSLLOGINATTEMPTINSERTV2",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveInternalLoginAtmInfo"));
            }
        }



        #endregion==========|  Old Security Controller Methods   |==========

        #region==========|  Private Methods  |==========

        /// <summary>
        /// Save SMS data log into table
        /// </summary>
        /// <param name="SMSInformationModel"></param>
        /// <returns>Returns void</returns>
        private async Task SaveSMSIntoTABLE(SMSInformationModel model)
        {
            try
            {
                _mySql.AddParameter(new MySqlParameter("P_MSISDN", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.ReceiverAddress });
                _mySql.AddParameter(new MySqlParameter("P_MESSAGE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.SMSBody });
                _mySql.AddParameter(new MySqlParameter("P_REPLY_ADDR", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.SenderAddress });
                _mySql.AddParameter(new MySqlParameter("P_CODE", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.SMSCoding });
                _mySql.AddParameter(new MySqlParameter("P_REMARKS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.remarks });
                _mySql.AddParameter(new MySqlParameter("P_DELIVERY_TIME", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.deliveredOn });
                _mySql.AddParameter(new MySqlParameter("P_SMS_STATUS", MySqlDbType.Int16) { Direction = ParameterDirection.Input, Value = model.status });

                await _mySql.InsertByStoredProcedureAsync("SAVE_INTO_SMS_TBL");
            }
            catch (Exception ex)
            {
                if (TextLogging.IsEnableErrorTextLog)
                {
                    ErrorLogModel log = new()
                    {
                        methodName = "SaveSMSIntoTABLE",
                        logSaveTime = DateTime.Now,
                        requestModel = model.ToJsonString(),
                        procedureName = "SAVE_INTO_SMS_TBL",
                        errorMessage = HelperMethod.ExMsgSubString(ex, "", 500),
                        errorSource = ex.Source,
                        errorCode = ex.HResult,
                        errorDetails = ex.StackTrace
                    };

                    TextLogWriter.WriteErrorLog(log.ToJsonString() + ",");
                }

                throw;
            }
        }

        #endregion==========|  Private Methods  |==========

    }
}