///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	All security rleated api collection releated to Retailer App
///	Creation Date :	02-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Application.Services;
using Application.Utils;
using Domain.Helpers;
using Domain.RequestModel;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RetailerSelfCareApi.Controllers
{
    [Route("api/security")]
    [ApiController]
    public class SecurityController : Controller
    {
        /// <summary>
        /// <para>This API method using from v6.0.0.
        /// Changes imei parameter removed.</para>
        /// Generate-OTP
        /// </summary>
        /// <param name="otprequest"></param>
        /// <returns>Return existing OTP if less then 5minute otherwise new OTP</returns>
        [HttpPost]
        [Route(nameof(GetRepeatOTP))]
        public async Task<IActionResult> GetRepeatOTP([FromBody] OTPGenerateRequest otprequest)
        {

            OTPGenerateRequest _otprequest = new()
            {
                iTopUpNumber = otprequest.iTopUpNumber,
                deviceId = otprequest.deviceId,
                isNewOTP = otprequest.isNewOTP,
                moduleName = "Device Registration",
                lan = otprequest.lan
            };

            SecurityService _auth = new();

            RAOTPResponse otpObj = await _auth.GenerateOTPUA(_otprequest);

            if (otpObj.result)
            {
                return Ok(new ResponseMessage()
                {
                    isError = false,
                    message = SharedResource.GetLocal(otpObj.key, otpObj.message),
                });
            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal(otpObj.key, otpObj.message)
                });
            }

        }


        /// <summary>
        /// <para>This API method using from v6.0.0
        /// Changes: implement max allow device for user</para>
        /// </summary>
        /// <param name="DeviceValidationV3"></param>
        /// <returns>Returns DeviceValidationResponseV2 response</returns>
        [HttpPost]
        [Route(nameof(DeviceValidation))]
        public async Task<IActionResult> DeviceValidation([FromBody] DeviceValidationRequest deviceValidation)
        {
            SecurityService _auth = new();

            DeviceValidationResponse result = await _auth.ValidateDevice(deviceValidation);

            if (result.isSuccess)
            {
                if (result.isPrimary && !result.isRegistered)
                {
                    _auth = new();
                    var generatePWDRes = await _auth.GenerateNewPWD();

                    if (generatePWDRes.Item2)
                    {
                        try
                        {
                            _auth = new();
                            string generatePWD = generatePWDRes.Item1;
                            var res = await _auth.SavePassword(deviceValidation.iTopUpNumber, generatePWD);

                            if (res.result)
                            {
                                return Ok(new DeviceValidationResponse()
                                {
                                    isSuccess = true,
                                    isPrimary = result.isPrimary,
                                    isRegistered = result.isRegistered,
                                    isSimSeller = result.isSimSeller,
                                    responseMessage = SharedResource.GetLocal("PWDSentToMobile", ResponseMessages.CredPSentToMobile)
                                });
                            }

                            return Ok(new DeviceValidationResponse()
                            {
                                isSuccess = false,
                                responseMessage = SharedResource.GetLocal(res.message, res.message)
                            });
                        }
                        catch (Exception ex)
                        {
                            string errMsg = HelperMethod.ExMsgBuild(ex, "SavePassword");
                            throw new Exception(errMsg);
                        }
                    }

                    return Ok(new DeviceValidationResponse()
                    {
                        isSuccess = false,
                        responseMessage = SharedResource.GetLocal(generatePWDRes.Item3, generatePWDRes.Item4)
                    });
                }

                return Ok(new DeviceValidationResponse()
                {
                    isSuccess = result.isSuccess,
                    isRegistered = result.isRegistered,
                    isPrimary = result.isPrimary,
                    primaryDeviceModel = result.primaryDeviceModel,
                    isSimSeller = result.isSimSeller,
                    responseMessage = SharedResource.GetLocal(result.responseMessage, result.responseMessage)
                });
            }
            else
            {
                return Ok(new DeviceValidationResponse()
                {
                    isSuccess = false,
                    isRegistered = result.isRegistered,
                    isPrimary = result.isPrimary,
                    isDeviceLimitExceed = result.isDeviceLimitExceed,
                    primaryDeviceModel = result.primaryDeviceModel,
                    isSimSeller = result.isSimSeller,
                    responseMessage = SharedResource.GetLocal(result.responseMessage, result.responseMessage)
                });
            }
        }


        /// <summary>
        /// <para>This API method using from v6.0.0.
        /// Changes add jwt session validation</para>
        /// Verify user and generates security token.
        /// 1. If the user logged in for first time new security token will generate.  
        /// 2. One user can login from diferrent device. 
        /// 3. No OTP will generate while consuming this api.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(RegisterWithChangePWD))]
        public async Task<IActionResult> RegisterWithChangePWD([FromBody] PWDLoginRequests login)
        {
            _ = int.TryParse(login.versionName.Replace(".", string.Empty), out int _versionName);
            if (ApiManager.IsApkVersionBlockAsync(login.versionCode, _versionName, login.appToken))
            {
                string lowerVersionBlockMessage = AppAllowedVersion.lower_version_block_message;
                string lowerVersionBlockMessageBN = AppAllowedVersion.lower_version_block_message_bn;

                return Ok(new LogInResponse()
                {
                    isAuthenticate = false,
                    authenticationMessage = login.lan.ToLower() == "en" ? lowerVersionBlockMessage : lowerVersionBlockMessageBN,
                    hasUpdate = true,
                });
            }
            else
            {
                SecurityService _auth = new();

                var validationResult = await _auth.IsPasswordFormatValid(login.newPassword, login.lan);
                if (validationResult.Item1 == false)
                {
                    return Ok(new LogInResponse
                    {
                        isAuthenticate = validationResult.Item1,
                        authenticationMessage = validationResult.Item2,
                        hasUpdate = false,
                    });
                }

                string encriptedOldPwd = CryptographyFile.Encrypt(login.oldPassword, true);
                string encriptedNewPwd = CryptographyFile.Encrypt(login.newPassword, true);

                VMUserInfo vmModel = new()
                {
                    iTopUpNumber = login.iTopUpNumber,
                    password = encriptedOldPwd,
                    deviceId = null
                };

                _auth = new();
                LoginUserInfoResponseV2 user = new();
                try
                {
                    user = await _auth.ValidateUser(vmModel);

                    if (user.user_name == null)
                    {
                        return Ok(new LogInResponse()
                        {
                            isAuthenticate = false,
                            authenticationMessage = ResponseMessages.InvalidCredP,
                            hasUpdate = false,
                        });
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = HelperMethod.ExMsgBuild(ex, "ValidateUserV4");
                    throw new Exception(errMsg);
                }

                // save device info - start
                string ipaddress = HttpContext.Request?.Host.Value ?? string.Empty;
                string machinename = string.Empty;
                try
                {
                    machinename = Dns.GetHostName();
                }
                catch (Exception)
                {
                    machinename = string.Empty;
                }

                var deviceInfo = new DeviceInfo()
                {
                    retailerCode = user.user_name,
                    deviceId = login.deviceId,
                    isPrimary = login.isPrimary,
                    isEnable = true,
                    ipAddress = ipaddress,
                    machineName = machinename,
                    versionCode = login.versionCode.ToString(),
                    versionName = login.versionName,
                    osVersion = login.osVersion,
                    kernelVersion = login.kernelVersion,
                    fermwareVersion = login.fermwareVersion,
                    platformId = login.platformId,
                    imeiNumber = "No IMEI found",
                    deviceModel = login.deviceModel,
                    lat = login.lat,
                    lng = login.lng,
                    createdBy = Convert.ToInt32(user.user_id)
                };

                RACommonResponse saveDeviceResp = new();

                DeviceValidationRequest deviceValidReq = new()
                {
                    iTopUpNumber = login.iTopUpNumber,
                    deviceId = login.deviceId,
                    lan = login.lan
                };
                _auth = new SecurityService();
                DeviceValidationResponse result = await _auth.ValidateDevice(deviceValidReq);

                if (result.isSuccess)
                {
                    try
                    {
                        saveDeviceResp = await _auth.SaveDeviceInfo(deviceInfo);
                    }
                    catch (Exception ex)
                    {
                        string errMsg = HelperMethod.ExMsgBuild(ex, "SaveDeviceInfo");
                        throw new Exception(errMsg);
                    }
                }
                else
                {
                    return Ok(new LogInResponse()
                    {
                        isAuthenticate = false,
                        isPrimary = result.isPrimary,
                        authenticationMessage = SharedResource.GetLocal(result.responseMessage, result.responseMessage),
                        hasUpdate = false,
                    });
                }
                // end

                // Change cred - start
                if (saveDeviceResp.result)
                {
                    try
                    {
                        var respChangePwd = await _auth.ChangePassword(user.user_name, login.oldPassword, login.newPassword, ResponseMessages.InvalidUserName);

                        if (!respChangePwd.result)
                        {
                            return Ok(new LogInResponse()
                            {
                                isAuthenticate = false,
                                authenticationMessage = SharedResource.GetLocal(respChangePwd.message, respChangePwd.message),
                                hasUpdate = false,
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        string errMsg = HelperMethod.ExMsgBuild(ex, "ChangePasswordV2");
                        throw new Exception(errMsg);
                    }
                }
                //  Change cred - end

                UserLogInAttempt loginAtmInfo;
                string loginProvider = Guid.NewGuid().ToString();

                loginAtmInfo = new UserLogInAttempt()
                {
                    userid = user.user_id,
                    is_success = 1,
                    ip_address = ipaddress,
                    machine_name = machinename,
                    loginprovider = loginProvider,
                    deviceid = login.deviceId,
                    lan = login.lan,
                    versioncode = login.versionCode,
                    versionname = login.versionName,
                    osversion = login.osVersion,
                    kernelversion = login.kernelVersion,
                    fermwarevirsion = login.fermwareVersion,
                    imei = "No IMEI found",
                    devicemodel = login.deviceModel,
                    lat = login.lat,
                    lng = login.lng
                };


                await Task.Factory.StartNew(async () =>
                {
                    _auth = new();
                    await _auth.SaveLoginAtmInfo(loginAtmInfo);
                });

                await Task.Factory.StartNew(async () =>
                {
                    _auth = new();
                    await _auth.UpsertLoginProviderIntoRedis(user.user_name, login.deviceId, loginProvider);
                });

                JweTokenModel tokenModel = new()
                {
                    ITopUpNumber = login.iTopUpNumber,
                    RetailerCode = user.user_name,
                    DeviceId = login.deviceId,
                    LoginProvider = loginProvider,
                    UserId = user.user_id
                };

                string _sessionToken = JweTokenUtils.GenerateJWEToken(tokenModel);
                string validationMessage = SharedResource.GetLocal("UserSuccessfullyValidated", ResponseMessages.UserSuccessfullyValidated ?? Message.Success);

                bool isEnableScSales = FeatureStatus.IsEnableSCSales;
                IEnumerable<string> roleList = user.role_access.Split(',').Distinct();

                return Ok(new LogInResponse()
                {
                    sessionToken = _sessionToken,
                    isAuthenticate = true,
                    isThisVersionBlocked = false,
                    authenticationMessage = validationMessage,
                    iTopUpNumber = login.iTopUpNumber,
                    deviceId = login.deviceId,
                    hasUpdate = false,
                    minimumScore = "65",
                    optionalMinimumScore = "30",
                    maximumRetry = "2",
                    roleAccess = string.Join(',', roleList),
                    channelId = user.channel_id,
                    channelName = user.channel_name,
                    inventoryId = user.inventory_id,
                    centerCode = user.center_code,
                    version = string.Empty,
                    retailerCode = user.user_name,
                    isPrimary = true,
                    isRegistered = true,
                    isDeviceEnable = true,
                    regionCode = user.regionCode,
                    regionName = user.regionName,
                    isEnableSCSales = isEnableScSales
                });
            }
        }


        /// <summary>
        /// This API method using from v6.0.0.
        /// Device Password Validation
        /// 1. Check user provided cred is valid or not.
        /// </summary>
        /// <param name="DevicePWDValidation"></param>
        /// <returns>Returns common response</returns>
        [HttpPost]
        [Route(nameof(DevicePWDValidation))]
        public async Task<IActionResult> DevicePWDValidation([FromBody] DevicePWDValidationRequest pwdValidation)
        {
            bool result = false;
            using (SecurityService _auth = new())
            {
                result = await _auth.ValidatePWD(pwdValidation);
            }

            if (result)
            {
                OTPGenerateRequest otpModel = new()
                {
                    iTopUpNumber = pwdValidation.iTopUpNumber,
                    deviceId = pwdValidation.deviceId,
                    moduleName = "Device Registration",
                    isNewOTP = true
                };

                using (SecurityService _auth = new())
                {
                    RAOTPResponse otpObj = await _auth.GenerateOTPUA(otpModel);

                    if (otpObj.result == true)
                    {
                        return Ok(new ResponseMessage()
                        {
                            isError = false,
                            message = SharedResource.GetLocal("OTPHasBeenSentToYourMobileNumber", Message.OTPSentToMobile)
                        });

                    }
                    else
                    {
                        return Ok(new ResponseMessage()
                        {
                            isError = true,
                            message = SharedResource.GetLocal("OTPGenerationFailed", Message.OTPFailed)
                        });
                    }
                }

            }
            else
            {
                return Ok(new ResponseMessage()
                {
                    isError = true,
                    message = SharedResource.GetLocal("InvalidPassword", ResponseMessages.InvalidCredP)
                });
            }
        }


        /// <summary>
        /// <para>This API method using from v6.0.0
        /// Changes imei parameter removed.</para>
        /// Secondery Device Validation With OTP
        /// 1. Check is request device secondery and send otp.
        /// </summary>
        /// <param name="RegisterWithOTPValidation">Register the device on OTP validate successfull</param>
        /// <returns>Returns registration succesful response</returns>
        [HttpPost]
        [Route(nameof(RegisterWithOTPValidation))]
        public async Task<IActionResult> RegisterWithOTPValidation([FromBody] OTPLoginRequests login)
        {
            SecurityService _auth = new();

            int.TryParse(login.versionName.Replace(".", string.Empty), out int _versionName);
            if (ApiManager.IsApkVersionBlockAsync(login.versionCode, _versionName, login.appToken))
            {
                string lowerVersionBlockMessage = AppAllowedVersion.lower_version_block_message;
                string lowerVersionBlockMessageBN = AppAllowedVersion.lower_version_block_message_bn;

                return Ok(new LogInResponse()
                {
                    isAuthenticate = false,
                    authenticationMessage = login.lan.ToLower() == "en" ? lowerVersionBlockMessage : lowerVersionBlockMessageBN,
                    hasUpdate = true,
                });
            }
            else
            {
                string encriptedPwd = CryptographyFile.Encrypt(login.password, true);

                VMValidateOtp vmModel = new()
                {
                    iTopUpNumber = login.iTopUpNumber,
                    otp = login.otp,
                    deviceId = login.deviceId,
                    moduleName = "Device Registration"
                };

                LoginUserInfoResponseV2 user = await _auth.ValidateOTPGetUser(vmModel);
                if (user.user_name == null)
                {
                    return Ok(new LogInResponse()
                    {
                        isAuthenticate = false,
                        authenticationMessage = SharedResource.GetLocal("InvalidOTP", Message.InvalidOTP),
                        hasUpdate = false,
                    });
                }

                // save device info - start
                string ipaddress = HttpContext.Request?.Host.Value ?? string.Empty;
                string machinename = string.Empty;
                try
                {
                    machinename = Dns.GetHostName();
                }
                catch (Exception)
                {
                    machinename = string.Empty;
                }

                var deviceInfo = new DeviceInfo()
                {
                    retailerCode = user.user_name,
                    deviceId = login.deviceId,
                    isPrimary = false,
                    isEnable = true,
                    ipAddress = ipaddress,
                    machineName = machinename,
                    versionCode = login.versionCode.ToString(),
                    versionName = login.versionName,
                    osVersion = login.osVersion,
                    kernelVersion = login.kernelVersion,
                    fermwareVersion = login.fermwareVersion,
                    platformId = login.platformId,
                    imeiNumber = "No IMEI found",
                    deviceModel = login.deviceModel,
                    lat = login.lat,
                    lng = login.lng,
                    createdBy = Convert.ToInt32(user.user_id)
                };

                RACommonResponse saveDeviceResp = new();

                DeviceValidationRequest deviceValidReq = new()
                {
                    iTopUpNumber = login.iTopUpNumber,
                    deviceId = login.deviceId,
                    lan = login.lan
                };
                _auth = new SecurityService ();
                DeviceValidationResponse result = await _auth.ValidateDevice(deviceValidReq);
                if (result.isSuccess)
                {
                    try
                    {
                        _auth = new();
                        saveDeviceResp = await _auth.SaveDeviceInfo(deviceInfo);
                    }
                    catch (Exception ex)
                    {
                        string errMsg = HelperMethod.ExMsgBuild(ex, "SaveDeviceInfoV2");
                        throw new Exception(errMsg);
                    }

                }
                else
                {
                    return Ok(new LogInResponse()
                    {
                        isAuthenticate = false,
                        isPrimary=result.isPrimary,
                        authenticationMessage = SharedResource.GetLocal(result.responseMessage, result.responseMessage),
                        hasUpdate = false,
                    });
                }
              
                // end

                UserLogInAttempt loginAtmInfo;
                string loginProvider = Guid.NewGuid().ToString();

                loginAtmInfo = new UserLogInAttempt()
                {
                    userid = user.user_id,
                    is_success = 1,
                    ip_address = ipaddress,
                    machine_name = machinename,
                    loginprovider = loginProvider,
                    deviceid = login.deviceId,
                    lan = login.lan,
                    versioncode = login.versionCode,
                    versionname = login.versionName,
                    osversion = login.osVersion,
                    kernelversion = login.kernelVersion,
                    fermwarevirsion = login.fermwareVersion,
                    imei = "No IMEI found",
                    devicemodel = login.deviceModel,
                    lat = login.lat,
                    lng = login.lng
                };

                await Task.Factory.StartNew(async () =>
                {
                    _auth = new();
                    await _auth.SaveLoginAtmInfo(loginAtmInfo);
                });

                await Task.Factory.StartNew(async () =>
                {
                    _auth = new();
                    await _auth.UpsertLoginProviderIntoRedis(user.user_name, login.deviceId, loginProvider);
                });


                JweTokenModel tokenModel = new()
                {
                    ITopUpNumber = login.iTopUpNumber,
                    RetailerCode = user.user_name,
                    DeviceId = login.deviceId,
                    LoginProvider = loginProvider,
                    UserId = user.user_id
                };

                string _sessionToken = JweTokenUtils.GenerateJWEToken(tokenModel);
                string validationMessage = SharedResource.GetLocal("UserSuccessfullyValidated", ResponseMessages.UserSuccessfullyValidated ?? Message.Success);

                bool isEnableScSales = FeatureStatus.IsEnableSCSales;
                IEnumerable<string> roleList = user.role_access.Split(',').Distinct();

                return Ok(new LogInResponse()
                {
                    sessionToken = _sessionToken,
                    isAuthenticate = true,
                    isThisVersionBlocked = false,
                    authenticationMessage = validationMessage,
                    iTopUpNumber = login.iTopUpNumber,
                    deviceId = login.deviceId,
                    hasUpdate = false,
                    minimumScore = "65",
                    optionalMinimumScore = "30",
                    maximumRetry = "2",
                    roleAccess = string.Join(',', roleList),
                    channelId = user.channel_id,
                    channelName = user.channel_name,
                    inventoryId = user.inventory_id,
                    centerCode = user.center_code,
                    version = string.Empty,
                    retailerCode = user.user_name,
                    isPrimary = false,
                    isRegistered = true,
                    isDeviceEnable = true,
                    regionCode = user.regionCode,
                    regionName = user.regionName,
                    isEnableSCSales = isEnableScSales
                });
            }

        }


        /// <summary>
        /// <para>This API method using from v6.0.0.
        /// new changes: add jwt authentication
        /// </para>
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] LoginRequestsV4 login)
        {
            // apk version validation check
            int.TryParse(login.versionName.Replace(".", string.Empty), out int _versionName);
            if (ApiManager.IsApkVersionBlockAsync(login.versionCode, _versionName, login.appToken))
            {
                string lowerVersionBlockMessage = AppAllowedVersion.lower_version_block_message;
                string lowerVersionBlockMessageBN = AppAllowedVersion.lower_version_block_message_bn;

                return Ok(new LogInResponse()
                {
                    isAuthenticate = false,
                    isThisVersionBlocked = true,
                    authenticationMessage = login.lan.ToLower() == "en" ? lowerVersionBlockMessage : lowerVersionBlockMessageBN,
                    hasUpdate = true
                });
            }
            else
            {
                SecurityService _auth = new();

                DeviceValidationRequest deviceValidReq = new()
                {
                    iTopUpNumber = login.iTopUpNumber,
                    deviceId = login.deviceId,
                    lan = login.lan
                };

                DeviceValidationResponse result = new();

                // Device Validation
                result = await _auth.ValidateDevice(deviceValidReq);

                if (result.isSuccess)
                {
                    if (!result.isRegistered)
                    {
                        return Ok(new LogInResponse()
                        {
                            isAuthenticate = false,
                            isThisVersionBlocked = false,
                            authenticationMessage = SharedResource.GetLocal("DeviceDeregistered", Message.DeviceDeregistered),
                            hasUpdate = false,
                            isPrimary = result.isPrimary,
                            isRegistered = false
                        });
                    }
                }
                else
                {
                    LogInResponse loginResp = new()
                    {
                        isAuthenticate = false,
                        isThisVersionBlocked = false,
                        authenticationMessage = SharedResource.GetLocal(result.responseMessage, result.responseMessage),
                        hasUpdate = false,
                        isPrimary = result.isPrimary,
                        isRegistered = result.isRegistered
                    };

                    if (result.responseMessage == "ThisDeviceIsNotEnable")
                    {
                        loginResp.isDeviceEnable = false;
                    }
                    return Ok(loginResp);
                }


                _auth = new SecurityService();
                string encriptedPwd = CryptographyFile.Encrypt(login.password, true);

                VMUserInfo vmModel = new()
                {
                    iTopUpNumber = login.iTopUpNumber,
                    password = encriptedPwd,
                    deviceId = login.deviceId
                };

                // User validation
                LoginUserInfoResponseV2 user = new();

                user = await _auth.ValidateUser(vmModel);


                if (user.user_name == null)
                {
                    return Ok(new LogInResponse()
                    {
                        isAuthenticate = false,
                        isThisVersionBlocked = false,
                        authenticationMessage = SharedResource.GetLocal("InvalidPassword", ResponseMessages.InvalidCredP),
                        hasUpdate = false,
                        isPrimary = result.isPrimary,
                        isRegistered = result.isRegistered,
                        isDeviceEnable = true
                    });
                }

                string ipaddress = HttpContext.Request?.Host.Value ?? string.Empty;
                string machinename = string.Empty;
                try
                {
                    machinename = Dns.GetHostName();
                }
                catch (Exception)
                {
                    machinename = string.Empty;
                }

                UserLogInAttempt loginAtmInfo;
                string loginProvider = Guid.NewGuid().ToString();

                loginAtmInfo = new UserLogInAttempt()
                {
                    userid = user.user_id,
                    is_success = 1,
                    ip_address = ipaddress,
                    machine_name = machinename,
                    loginprovider = loginProvider,
                    deviceid = login.deviceId,
                    lan = login.lan,
                    versioncode = login.versionCode,
                    versionname = login.versionName,
                    osversion = login.osVersion,
                    kernelversion = login.kernelVersion,
                    fermwarevirsion = login.fermwareVersion,
                    imei = "No IMEI found",
                    devicemodel = login.deviceModel,
                    lat = login.lat,
                    lng = login.lng
                };

                //Save Login Attempts Data
                await Task.Factory.StartNew(async () =>
                {
                    _auth = new();
                    await _auth.SaveLoginAtmInfo(loginAtmInfo);
                });

                //await Task.Factory.StartNew(async () =>
                //{
                //    _auth = new();
                //    await _auth.UpsertLoginProviderIntoRedis(user.user_name, login.deviceId, loginProvider);
                //});

                JweTokenModel tokenModel = new()
                {
                    ITopUpNumber = login.iTopUpNumber,
                    RetailerCode = user.user_name,
                    DeviceId = login.deviceId,
                    LoginProvider = loginProvider,
                    UserId = user.user_id
                };

                string _sessionToken = JweTokenUtils.GenerateJWEToken(tokenModel);
                string validationMessage = SharedResource.GetLocal("UserSuccessfullyValidated", ResponseMessages.UserSuccessfullyValidated ?? Message.Success);

                bool isEnableScSales = FeatureStatus.IsEnableSCSales;
                IEnumerable<string> roleList = user.role_access.Split(',').Distinct();

                return Ok(new LogInResponse()
                {
                    sessionToken = _sessionToken,
                    isAuthenticate = true,
                    isThisVersionBlocked = false,
                    authenticationMessage = validationMessage,
                    iTopUpNumber = login.iTopUpNumber,
                    deviceId = login.deviceId,
                    hasUpdate = false,
                    minimumScore = "65",
                    optionalMinimumScore = "30",
                    maximumRetry = "2",
                    roleAccess = string.Join(',', roleList),
                    channelId = user.channel_id,
                    channelName = user.channel_name,
                    inventoryId = user.inventory_id,
                    centerCode = user.center_code,
                    version = string.Empty,
                    retailerCode = user.user_name,
                    isPrimary = result.isPrimary,
                    isRegistered = true,
                    isDeviceEnable = true,
                    regionCode = user.regionCode,
                    regionName = user.regionName,
                    isEnableSCSales = isEnableScSales
                });
            }
        }


        [HttpPost]
        [Route(nameof(RequestNewDevice))]
        public async Task<IActionResult> RequestNewDevice([FromBody] NewDeviceRequest newDeviceRequest)
        {
            using (SecurityService _auth = new())
            {
                long res = await _auth.SubmitNewDeviceRequest(newDeviceRequest);

                if (res == -111)
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = true,
                        message = SharedResource.GetLocal("NewDeviceRequestPending", Message.NewDeviceRequestPending)
                    });
                }

                if (res > 0)
                {
                    return Ok(new ResponseMessage()
                    {
                        isError = false,
                        message = SharedResource.GetLocal("NewDeviceRequestSuccess", Message.NewDeviceRequestSuccess)
                    });
                }
            }

            return Ok(new ResponseMessage()
            {
                isError = true,
                message = SharedResource.GetLocal("NewDeviceRequestFailed", Message.NewDeviceRequestFailed)
            });
        }


        /// <summary>
        /// Recover cred by Reseller's iTopUpNumber
        /// </summary>
        /// <param name="VMUserInfoForForgetPWD"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(ForgetPwd))]
        public async Task<IActionResult> ForgetPwd([FromBody] VMUserInfoForForgetPWD model)
        {
            using (SecurityService _auth = new())
            {
                var generatePWDRes = await _auth.GenerateNewPWD();

                if (generatePWDRes.Item2)
                {
                    string generatePWD = generatePWDRes.Item1;
                    var res = await _auth.SavePassword(model.iTopUpNumber, generatePWD);

                    if (res.result)
                    {
                        return Ok(new RACommonResponse()
                        {
                            result = true,
                            message = SharedResource.GetLocal("PWDSentToMobile", ResponseMessages.CredPSentToMobile)
                        });
                    }

                    return Ok(res);
                }

                return Ok(new RACommonResponse()
                {
                    result = false,
                    message = SharedResource.GetLocal(generatePWDRes.Item3, generatePWDRes.Item4)
                });
            }

        }


        /// <summary>
        /// API for change cred. Requesting parameter with old cred and new cred
        /// </summary>
        /// <param name="changePasswordReq"></param>
        /// <returns>Return RACommonResponse response</returns>
        [HttpPost]
        [Route(nameof(ChangePassword))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestsV2 changePasswordReq)
        {
            using (SecurityService _auth = new())
            {
                var validationResult = await _auth.IsPasswordFormatValid(changePasswordReq.newPassword, changePasswordReq.lan);

                if (validationResult.Item1 == false)
                {
                    return Ok(new RACommonResponse()
                    {
                        result = validationResult.Item1,
                        message = validationResult.Item2
                    });
                }
                else
                {
                    try
                    {
                        RACommonResponse resp = await _auth.ChangePassword(changePasswordReq.retailerCode, changePasswordReq.oldPassword, changePasswordReq.newPassword, ResponseMessages.InvalidUserName);

                        resp.message = SharedResource.GetLocal(resp.message, resp.message);
                        return Ok(resp);
                    }
                    catch (Exception ex)
                    {
                        string errMsg = HelperMethod.ExMsgBuild(ex, "ChangePassword");
                        throw new Exception(errMsg);
                    }
                }
            }
        }

    }
}