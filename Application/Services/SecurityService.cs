///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	A service class for user validation
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Application.Utils;
using Domain.Helpers;
using Domain.RequestModel;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Infrastracture.Repositories;
using System.Data;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class SecurityService : IDisposable
    {

        private readonly SecurityRepository _repo;

        public SecurityService()
        {
            _repo = new();
        }

        public SecurityService(string connectionString)
        {
            _repo = new(connectionString);
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
                _repo.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========


        /// <summary>
        /// Validate user by iTopUpNumber and cred
        /// </summary>
        /// <param name="VMUserInfo"></param>
        /// <returns></returns>
        public async Task<LoginUserInfoResponseV2> ValidateUser(VMUserInfo model)
        {
            LoginUserInfoResponseV2 userInfo = new();

            try
            {
                DataTable dt = await _repo.ValidateUser(model);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    userInfo.user_id = Convert.ToString(dr["USER_ID"] ?? null);
                    userInfo.user_name = Convert.ToString(dr["USER_NAME"] ?? null);
                    userInfo.role_id = Convert.ToString(dr["ROLE_ID"] ?? null);
                    userInfo.role_name = Convert.ToString(dr["ROLE_NAME"] ?? null);
                    userInfo.is_role_active = Convert.ToInt32(dr["IS_ROLE_ACTIVE"] ?? 0);
                    userInfo.channel_id = Convert.ToInt32(dr["CHANNEL_ID"] ?? 0);
                    userInfo.channel_name = Convert.ToString(dr["CHANNEL_NAME"] ?? null);
                    userInfo.is_activedirectory_user = Convert.ToInt32(dr["IS_ACTIVEDIRECTORY_USER"] ?? 0);
                    userInfo.role_access = Convert.ToString(dr["ROLEACCESS"] ?? null);
                    userInfo.distributor_code = Convert.ToString(dr["DIST_CODE"] ?? null);
                    userInfo.center_code = Convert.ToString(dr["CENTER_CODE"] ?? null);
                    userInfo.regionCode = dr["REGION_CODE"] as string;
                    userInfo.regionName = dr["REGION_NAME"] as string;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ValidateUserV4"));
            }

            return userInfo;
        }


        /// <summary>
        /// Save LogIn Attempt Info with new requirement
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        public async Task<long> SaveLoginAtmInfo(UserLogInAttempt loginInfo)
        {
            long res = 0;
            try
            {
                res = await _repo.SaveLoginAtmInfo(loginInfo);
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveLoginAtmInfoV3"));
            }
        }


        public async Task UpsertLoginProviderIntoRedis(string retailerCode, string deviceId, string loginProvider)
        {
            string dataKey = retailerCode + "_" + deviceId;
            RedisCache redis;
            string hasLoginProvider;
            using (redis = new RedisCache())
            {
                hasLoginProvider = await redis.GetCacheAsync(RedisCollectionNames.RetailerChkInGuids, dataKey);
            }
            if (!string.IsNullOrWhiteSpace(hasLoginProvider))
            {
                using(redis = new RedisCache())
                {
                    await redis.UpdateCacheAsync(RedisCollectionNames.RetailerChkInGuids, dataKey, loginProvider.ToJsonString());
                }
            }
            else
            {
                using(redis = new RedisCache())
                {
                    await redis.SetCacheAsync(RedisCollectionNames.RetailerChkInGuids, dataKey, loginProvider.ToJsonString());
                }
            }
        }


        //public bool IsSecurityTokenValid(string loginProvider, string deviceId)
        //{
        //    long res = 0;

        //    try
        //    {
        //        res = _repo.IsSecurityTokenValid2(loginProvider, deviceId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(HelperMethod.ExMsgBuild(ex, "IsSecurityTokenValid"));
        //    }

        //    return res == 1 ? true : false;
        //}


        //public async Task<LoginUserInfoResponse> ValidateUser(string username, string cred)
        //{
        //    LoginUserInfoResponse userInfo = new LoginUserInfoResponse();
        //    try
        //    {
        //        vmUserInfo logInInfo = new vmUserInfo()
        //        {
        //            user_name = username,
        //            cred = cred
        //        };
        //        DataTable dataRow = await _repo.ValidateUser(logInInfo);
        //        if (dataRow.Rows.Count > 0)
        //        {
        //            userInfo.user_id = Convert.ToString(dataRow.Rows[0]["USER_ID"] ?? null);
        //            userInfo.user_name = Convert.ToString(dataRow.Rows[0]["USER_NAME"] ?? null);
        //            userInfo.role_id = Convert.ToString(dataRow.Rows[0]["ROLE_ID"] ?? null);
        //            userInfo.role_name = Convert.ToString(dataRow.Rows[0]["ROLE_NAME"] ?? null);
        //            userInfo.is_role_active = Convert.ToInt32(dataRow.Rows[0]["IS_ROLE_ACTIVE"] ?? 0);
        //            userInfo.channel_id = Convert.ToInt32(dataRow.Rows[0]["CHANNEL_ID"] ?? 0);
        //            userInfo.channel_name = Convert.ToString(dataRow.Rows[0]["CHANNEL_NAME"] ?? null);
        //            userInfo.is_activedirectory_user = Convert.ToInt32(dataRow.Rows[0]["IS_ACTIVEDIRECTORY_USER"] ?? 0);
        //            userInfo.role_access = Convert.ToString(dataRow.Rows[0]["ROLEACCESS"] ?? null);
        //            userInfo.distributor_code = Convert.ToString(dataRow.Rows[0]["DIST_CODE"] ?? null);
        //            //vuf.inventory_id = Convert.ToInt32(dataRow.Rows[0]["INVENTORY_ID"] ?? 0);
        //            userInfo.center_code = Convert.ToString(dataRow.Rows[0]["CENTER_CODE"] ?? null);


        //            //return vuf;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(HelperMethod.ExMsgBuild(ex, "ValidateUser"));
        //    }
        //    return userInfo;
        //}

        //public string IsUserCurrentlyLoggedIn(string userId)
        //{
        //    string loginProvider = null;
        //    try
        //    {
        //        var dataRow = _repo.IsUserCurrentlyLoggedIn(Convert.ToDecimal(userId));

        //        if (dataRow.Rows.Count > 0)
        //        {
        //            loginProvider = dataRow.Rows[0]["LOGINPROVIDER"] == DBNull.Value ? (string)dataRow.Rows[0]["LOGINPROVIDER"] : null;
        //        }
        //        else
        //        {
        //            //respObj.result = false;
        //            //respObj.message = MessageCollection.InvalidUserName;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(HelperMethod.ExMsgBuild(ex, "IsUserCurrentlyLoggedIn"));
        //    }
        //    return loginProvider;
        //}


        //public long SaveLoginOTPLessAtmInfo(UserLogInOTPLessAttempt loginInfo)
        //{
        //    long res = 0;
        //    try
        //    {
        //        res = _repo.SaveLoginOTPLessAtmInfo(loginInfo);
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}


        /// <summary>
        /// Generate OTP
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<RAOTPResponse> GenerateOTPUA(OTPGenerateRequest model)
        {
            RAOTPResponse response = new();

            int res = 0;

            try
            {
                long? result = await _repo.GenerateOTPDAL(model);

                if (result > 0)
                {
                    SMSInformationModel smsData = new()
                    {
                        SMSApiUrl = ExternalKeys.SMSApiUrl,
                        SMSBody = $"Your OTP is {result} .",
                        ReceiverAddress = model.iTopUpNumber,
                        SMSCoding = 0
                    };

                    //HttpService httpService = new();
                    //SMSInformationModel sms = await httpService.SendSMS(smsData);
                    SMSInformationModel sms = new() { ReceiverAddress = model.iTopUpNumber, SMSBody = smsData.SMSBody, status = 2, remarks = "Sent via API", isSuccess = true, deliveredOn = DateTime.Now };

                    if (!sms.isSuccess)
                    {
                        //SecurityRepository secRepo = new();
                        //secRepo.SMSLogSaveInDB(sms);

                        response.result = false;
                        response.message = "Failed to send OTP.";
                        response.key = "OTPSendFailed";
                        return response;
                    }

                    response.result = true;
                    response.message = Message.OTPSentToMobile;
                    response.key = "OTPHasBeenSentToYourMobileNumber";
                    return response;
                }
                else if (res == -888)
                {
                    response.result = false;
                    response.message = "Failed to send OTP.";
                    response.key = "OTPSendFailed";
                    return response;
                }
                else
                {
                    response.result = false;
                    response.message = Message.OTPFailed;
                    response.key = "OTPGenerationFailed";
                    return response;
                }
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "GenerateOTPUA");
                throw new Exception(errMsg);
            }
        }


        /// <summary>
        /// Generate new cred for primary user
        /// Generate new random PWD in code level.
        /// </summary>
        /// <returns>new random PWD, is success, message, message</returns>
        public async Task<Tuple<string, bool, string, string>> GenerateNewPWD()
        {
            try
            {
                var globalSettingsData = await getChangePasswordGlobalSettingsData();
                int randomPWD = 0;

                for (int i = 0; i < 10; i++)
                {
                    randomPWD = HelperMethod.GenerateSecureRandomPassword();
                    var result = IsSameNumberOrCharacterOrTrendScquenceExists(randomPWD.ToString(), globalSettingsData.Item1, globalSettingsData.Item2, globalSettingsData.Item3);

                    if (result.Item1)
                        continue;
                    else
                        return Tuple.Create(randomPWD.ToString(), true, "Success", Message.Success);
                }

                return Tuple.Create(randomPWD.ToString(), false, "TryNewPassword", "Please, try to create new cred again.");
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "GenerateNewPWD");
                throw new Exception(errMsg);
            }
        }


        public async Task<Tuple<bool, string>> IsPasswordFormatValid(string inputStr, string lan)
        {
            if (string.IsNullOrEmpty(inputStr))
                return Tuple.Create(false, SharedResource.GetLocal("PWDNotNull", "Password cann't be null or empty."));

            //Item1: passLen
            //Item2: maxAcceptableSameNumLen
            //Item3: maxAcceptableTrendSeqLen
            var globalSettingsData = await getChangePasswordGlobalSettingsData();

            if (inputStr.Length < globalSettingsData.Item1)
            {
                string localeMsg = SharedResource.GetLocal("PWDMinLength", "Please enter a minimum {0} digit length.");
                string msg = string.Format(localeMsg, globalSettingsData.Item1);
                return Tuple.Create(false, msg);
            }

            var isSpecialCharecterExistsResult = IsSpecialCharecterExists(inputStr);

            if (isSpecialCharecterExistsResult == true)
            {
                string localeMsg = SharedResource.GetLocal("PWDSpecCharNotAllow", "Special character not allowed on cred.");
                return Tuple.Create(false, localeMsg);
            }

            var isSameNumberOrTrendScquenceExistsResult = IsSameNumberOrCharacterOrTrendScquenceExists(inputStr, globalSettingsData.Item1, globalSettingsData.Item2, globalSettingsData.Item3);

            if (isSameNumberOrTrendScquenceExistsResult.Item1 == true)
            {
                string localKey = isSameNumberOrTrendScquenceExistsResult.Item2 == -123 ? "PWDRepeatedNumberNotAllow" : "PWDSpecCharNotAllow";
                string defaultMsg = isSameNumberOrTrendScquenceExistsResult.Item2 == -123 ? "Special character not allowed on cred." : "Repeated number or character can’t allowed on cred.";
                string localeMsg = SharedResource.GetLocal(localKey, defaultMsg);
                return Tuple.Create(false, localeMsg);
            }

            return Tuple.Create(true, SharedResource.GetLocal("Success", Message.Success));
        }

        /// <summary>
        /// Save New Device Info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<RACommonResponse> SaveDeviceInfo(DeviceInfo model)
        {
            var res = await _repo.SaveDeviceInfo(model);
            return new RACommonResponse()
            {
                result = res > 0,
                message = res > 0 ? Message.Success : Message.Failed
            };
        }

        /// <summary>
        /// Change user Password before loggedIn
        /// </summary>
        /// <param name="retailerCode"></param>
        /// <param name="oldPass"></param>
        /// <param name="newPass"></param>
        /// <returns>Return Common response</returns>
        public async Task<RACommonResponse> ChangePassword(string retailerCode, string oldPass, string newPass, string invalidUserMsg)
        {
            RACommonResponse response = new();
            long result = 0;

            try
            {
                VMChangePassword vmCpObj = new()
                {
                    username = retailerCode,
                    old_password = CryptographyFile.Encrypt(oldPass, true),
                    new_password = CryptographyFile.Encrypt(newPass, true)
                };

                result = await _repo.ChangePassword(vmCpObj);

                if (result > 0)
                {
                    response.result = true;
                    response.message = "ChangePWD";
                    return response;
                }
                else if (result == -888/*(int)ChangePasswordEnum.invalidUser*/)
                {
                    response.result = false;
                    response.message = invalidUserMsg;
                    return response;
                }
                else if (result == -777/*(int)ChangePasswordEnum.passwordNotMatched*/)
                {
                    response.result = false;
                    response.message = "PasswordNotMatched";
                    return response;
                }
                else if (result == -999/*(int)ChangePasswordEnum.unableToUpdate*/)
                {
                    response.result = false;
                    response.message = "UnableUpdatePWD";
                    return response;
                }
                else
                {
                    response.result = false;
                    response.message = "SomethingWentWrong";
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ChangePasswordV3"));
            }
        }


        /// <summary>
        /// Validate Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> ValidatePWD(DevicePWDValidationRequest model)
        {
            int res = 0;
            try
            {
                var passwordHashed = CryptographyFile.Encrypt(model.password, true);
                model.password = passwordHashed;

                res = await _repo.ISPWDValid(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ValidatePWD"));
            }

            return res == 1;
        }


        /// <summary>
        /// Validate user OTP and Get UserInfo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<LoginUserInfoResponseV2> ValidateOTPGetUser(VMValidateOtp model)
        {
            try
            {
                LoginUserInfoResponseV2 vuf = new();

                DataTable dt = await _repo.ValidateOTPGetUser(model);

                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];

                    vuf.user_id = Convert.ToString(dr["USER_ID"] ?? null);
                    vuf.user_name = Convert.ToString(dr["USER_NAME"] ?? null);
                    vuf.role_id = Convert.ToString(dr["ROLE_ID"] ?? null);
                    vuf.role_name = Convert.ToString(dr["ROLE_NAME"] ?? null);
                    vuf.is_role_active = Convert.ToInt32(dr["IS_ROLE_ACTIVE"] ?? 0);
                    vuf.channel_id = Convert.ToInt32(dr["CHANNEL_ID"] ?? 0);
                    vuf.channel_name = Convert.ToString(dr["CHANNEL_NAME"] ?? null);
                    vuf.is_activedirectory_user = Convert.ToInt32(dr["IS_ACTIVEDIRECTORY_USER"] ?? 0);
                    vuf.role_access = Convert.ToString(dr["ROLEACCESS"] ?? null);
                    vuf.distributor_code = Convert.ToString(dr["DIST_CODE"] ?? null);
                    vuf.center_code = Convert.ToString(dr["CENTER_CODE"] ?? null);
                }

                return vuf;
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "ValidateOTPGetUser");
                throw new Exception(errMsg);
            }
        }


        /// <summary>
        /// Validate reseller device
        /// </summary>
        /// <param name="ValidateDeviceV4"></param>
        /// <returns>Returns DeviceValidationResponse</returns>
        public async Task<DeviceValidationResponse> ValidateDevice(DeviceValidationRequest model)
        {
            DeviceValidationResponse respModel = new();

            try
            {
                DataTable dataRow = await _repo.ValidateDevice(model);
                if (dataRow.Rows.Count > 0)
                {
                    var row = dataRow.Rows[0];

                    respModel.primaryDeviceModel = row["PRIMARY_DEVICE_MODEL"].ToString();

                    if (Convert.ToInt32(row["IS_SUCCESS"]) == 1)
                    {
                        respModel.isSuccess = true;
                    }
                    else
                    {
                        respModel.isSuccess = false;
                    }

                    if (Convert.ToInt32(row["IS_PRIMARY"]) == 1)
                    {
                        respModel.isPrimary = true;
                    }
                    else
                    {
                        respModel.isPrimary = false;
                    }

                    if (Convert.ToInt32(row["IS_REGISTERED"]) == 1)
                    {
                        respModel.isRegistered = true;
                    }
                    else
                    {
                        respModel.isRegistered = false;
                    }

                    if (Convert.ToInt32(row["IS_DEVICELIMITEXCEED"]) == 1)
                    {
                        respModel.isDeviceLimitExceed = true;
                    }
                    else
                    {
                        respModel.isDeviceLimitExceed = false;
                    }

                    if (row["SIM_SELLER"].ToString() == "Y")
                    {
                        respModel.isSimSeller = true;
                    }
                    else
                    {
                        respModel.isSimSeller = false;
                    }

                    respModel.responseMessage = row["RESPONSE_MESSAGE"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ValidateDevice"));
            }

            return respModel;
        }


        /// <summary>
        /// Save Password during Device registration
        /// </summary>
        /// <param name="iTopUpNumber"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        public async Task<RACommonResponse> SavePassword(string iTopUpNumber, string newPass)
        {
            RACommonResponse response = new();
            long result;

            try
            {
                VMSavePassword vmSpObj = new()
                {
                    iTopUpNumber = iTopUpNumber,
                    newPassword = newPass,
                    newPassHash = CryptographyFile.Encrypt(newPass, true)
                };

                SMSInformationModel smsData = new()
                {
                    SMSApiUrl = ExternalKeys.SMSApiUrl,
                    SMSBody = $"Your temporary password is {newPass} .",
                    ReceiverAddress = iTopUpNumber,
                    SMSCoding = 0
                };

                result = await _repo.SavePassword(vmSpObj);

                if (result > 0)
                {
                    //HttpService httpService = new();
                    //SMSInformationModel sms = await httpService.SendSMS(smsData);
                    SMSInformationModel sms = new() { ReceiverAddress = iTopUpNumber, SMSBody = smsData.SMSBody, status = 2, remarks = "Sent via API", isSuccess = true, deliveredOn = DateTime.Now };

                    if (!sms.isSuccess)
                    {
                        //SecurityRepository _authRepo = new();
                        //_authRepo.SMSLogSaveInDB(sms);

                        response.result = false;
                        response.message = "SMSSendFailed";
                        return response;
                    }

                    response.result = true;
                    response.message = "ChangePWD";
                    return response;
                }
                else if (result == -888)
                {
                    response.result = false;
                    response.message = "InvalidUsername";
                    return response;
                }
                else if (result == -999/*(int)ChangePasswordEnum.unableToUpdate*/)
                {
                    response.result = false;
                    response.message = "UnableUpdatePWD";
                    return response;
                }
                else
                {
                    response.result = false;
                    response.message = "SomethingWentWrong";
                    return response;
                }
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgBuild(ex, "SavePassword");
                throw new Exception(errMsg);
            }
        }

        public async Task<long> SubmitNewDeviceRequest(NewDeviceRequest model)
        {
            try
            {
                return await _repo.SubmitNewDeviceRequest(model);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SubmitNewDeviceRequest"));
            }
        }


        #region==========|  Old Security Controller Methods  |==========

        public async Task<LoginUserInfoResponse> ValidateInternalUser(VMUserInfo model)
        {
            try
            {
                LoginUserInfoResponse userInfo = new();

                DataTable dataRow = await _repo.IsInternalUserValid(model);

                if (dataRow.Rows.Count > 0)
                {
                    userInfo.user_id = Convert.ToString(dataRow.Rows[0]["USERID"] ?? null);
                    userInfo.user_name = Convert.ToString(dataRow.Rows[0]["USERNAME"] ?? null);
                }
                return userInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ValidateInternalUser"));
            }

        }


        /// <summary>
        /// Save LogIn Attempt Info with new requirement
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        public async Task<long> SaveInternalLoginAtmInfo(UserLogInAttempt loginInfo)
        {
            long res = 0;
            try
            {
                res = await _repo.SaveInternalLoginAtmInfo(loginInfo);
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SaveInternalLoginAtmInfo"));
            }
        }


        #endregion==========|  Old Security Controller Methods   |==========


        #region==========|  Private Methods  |==========
        /// <summary>
        /// Get data from global config table for PWD change.
        /// </summary>
        /// <returns> item1: passLen </returns>
        /// <returns> item2: maxAcceptableSameNumLen </returns>
        /// <returns> item3: maxAcceptableTrendSeqLen </returns>
        private async Task<Tuple<int, int, int>> getChangePasswordGlobalSettingsData()
        {
            RAPassLenResponse response = new();
            int passLen = 0, maxAcceptableTrendSeqLen = 0, maxAcceptableSameNumLen = 0;

            try
            {
                var dataRow = await _repo.GetChangePasswordGlobalSettingsData();

                if (dataRow.Rows.Count > 0)
                {
                    passLen = Convert.ToInt16(dataRow.Rows[0]["RA_PASSWORD_LENGTH"] == DBNull.Value ? null : dataRow.Rows[0]["RA_PASSWORD_LENGTH"]);
                    maxAcceptableSameNumLen = Convert.ToInt16(dataRow.Rows[0]["MAX_ACCEPTABLE_SAME_NUM_LEN"] == DBNull.Value ? null : dataRow.Rows[0]["MAX_ACCEPTABLE_SAME_NUM_LEN"]);
                    maxAcceptableTrendSeqLen = Convert.ToInt16(dataRow.Rows[0]["MAX_ACCEPTABLE_TREND_SEQ_LEN"] == DBNull.Value ? null : dataRow.Rows[0]["MAX_ACCEPTABLE_TREND_SEQ_LEN"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "getChangePasswordGlobalSettingsData"));
            }

            return Tuple.Create(passLen, maxAcceptableSameNumLen, maxAcceptableTrendSeqLen);
        }


        /// <summary>
        /// cred string lenght must be more then 1 digit. 
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="maxAcceptableSameNumLength"></param>
        /// <param name="maxAcceptableTrendSeqLength"></param>
        /// <param name="minPassLength"></param>
        /// <returns></returns>
        /// return_code = -111 for same char match, 
        /// return_code = -123 for trend char match, 
        /// return_code = 1 for not match 
        private static Tuple<bool, int> IsSameNumberOrCharacterOrTrendScquenceExists(string inputStr, int minPassLength, int maxAcceptableSameNumLength, int maxAcceptableTrendSeqLength)
        {
            try
            {
                int trendSequenceLengthCount = 0;
                int sameCharacterLengthCount = 0;
                bool trendSecquencLengthCheckFlag = false;
                bool sameCharacterCheckFlag = false;

                char[] charArray = inputStr.ToCharArray();

                for (int i = 1; i < charArray.Length; i++)
                {
                    //Same Number Check
                    if (charArray[i] - charArray[i - 1] == 0)
                    {
                        // Ex: 123321
                        if (trendSecquencLengthCheckFlag == true && trendSequenceLengthCount == maxAcceptableTrendSeqLength)
                            return Tuple.Create(trendSecquencLengthCheckFlag, -123);

                        ++sameCharacterLengthCount;
                        trendSequenceLengthCount = 0;

                        if (sameCharacterCheckFlag == true && sameCharacterLengthCount > maxAcceptableSameNumLength)
                            return Tuple.Create(sameCharacterCheckFlag, -111);

                        sameCharacterCheckFlag = true;
                        trendSecquencLengthCheckFlag = false;
                        continue;
                    }
                    //Trend Secquence check 
                    else if (charArray[i] - charArray[i - 1] == 1 &&
                             trendSequenceLengthCount <= maxAcceptableTrendSeqLength)
                    {
                        //Ex: 1112223, 112223, 32111321
                        if (sameCharacterCheckFlag == true && sameCharacterLengthCount == maxAcceptableSameNumLength)
                            return Tuple.Create(sameCharacterCheckFlag, -111);

                        sameCharacterCheckFlag = false;
                        sameCharacterLengthCount = 0;
                        trendSecquencLengthCheckFlag = true;
                        ++trendSequenceLengthCount;

                        //Ex: 123321, 321abc123
                        if (trendSequenceLengthCount > maxAcceptableTrendSeqLength)
                            return Tuple.Create(trendSecquencLengthCheckFlag, -123);
                    }
                    else
                    {
                        //Ex: 3211235
                        if ((trendSequenceLengthCount + 1) > maxAcceptableTrendSeqLength &&
                            trendSecquencLengthCheckFlag == true)
                            return Tuple.Create(trendSecquencLengthCheckFlag, -123);

                        //Ex: 1112223
                        if ((sameCharacterLengthCount + 1) > maxAcceptableSameNumLength &&
                            sameCharacterCheckFlag == true)
                            return Tuple.Create(sameCharacterCheckFlag, -111);

                        sameCharacterCheckFlag = false;
                        sameCharacterLengthCount = 0;
                        trendSecquencLengthCheckFlag = false;
                        trendSequenceLengthCount = 0;
                    }
                }

                //Ex: 321456
                if (trendSequenceLengthCount == maxAcceptableTrendSeqLength &&
                        trendSecquencLengthCheckFlag == true)
                    return Tuple.Create(trendSecquencLengthCheckFlag, -123);

                //Ex: 3214111
                if (sameCharacterLengthCount == maxAcceptableSameNumLength &&
                        sameCharacterCheckFlag == true)
                    return Tuple.Create(sameCharacterCheckFlag, -111);

                return Tuple.Create(false, 1);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "IsSameNumberOrCharacterOrTrendScquenceExists"));
            }
        }

        private static bool IsSpecialCharecterExists(string inputStr)
        {
            try
            {
                var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
                return !regexItem.IsMatch(inputStr.Trim());
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion==========|  Private Methods  |==========


    }
}