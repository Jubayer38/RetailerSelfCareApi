///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	JWT token Generation and validation
///	Creation Date :	20-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Utils
{
    public static class JweTokenUtils
    {
        #region Json Web Token (JWT) using JWE mechanism

        /// <summary>
        /// Generate Json Web Token using JWE mechanism. In Jwe mechanism encryption key must be 128bits(string Length 16), 256bits(string Length 32)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GenerateJWEToken(JweTokenModel model)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JweKeysModel.TokenSignKey));
            string algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredentials = new SigningCredentials(signingKey, algorithm);

            var encryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JweKeysModel.TokenKey));
            var encryptCredentials = new EncryptingCredentials(encryptionKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512);

            DateTime nowDate = DateTime.UtcNow;
            DateTime expireDate = nowDate.AddDays(3);

            var claims = new[]
            {
                new Claim("retailerCode", model.RetailerCode),
                new Claim("dvicId", model.DeviceId),
                new Claim("userId", model.UserId),
                new Claim(JwtRegisteredClaimNames.Jti, model.LoginProvider)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = expireDate,
                Issuer = JweKeysModel.Issuer,
                Audience = JweKeysModel.Audience,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }


        public static ResponseMessage ValidateJweToken(string _sessionToken, string _retailerCode, string _deviceId)
        {
            string sToken = _sessionToken;

            ClaimsPrincipal simplePrinciple;
            string failedMsg = SharedResource.GetLocal("InvalidSession", Message.InvalidSession);

            try
            {
                simplePrinciple = GetJwePrincipal(sToken, out _);
            }
            catch (Exception ex)
            {
                string errMsg = HelperMethod.ExMsgSubString(ex, "", 500);
                if (errMsg.StartsWith("IDX10223: Lifetime validation failed"))
                {
                    string sessionExpiredMsg = SharedResource.GetLocal("InvalidSession", Message.InvalidSession);
                    string errorDetails = HelperMethod.BuildErrorDetails(ex, "");
                    return new ResponseMessage(true, sessionExpiredMsg, null, errorDetails, null);
                }

                string errDetails = HelperMethod.BuildErrorDetails(ex, "");
                return new ResponseMessage(true, failedMsg, null, errDetails, null);
            }

            if (!(simplePrinciple?.Identity is ClaimsIdentity identity))
                return new ResponseMessage(true, failedMsg, null, "Claims Identity is Null.", null);

            if (!identity.IsAuthenticated)
                return new ResponseMessage(true, failedMsg, null, "Claims Identity authorization failed.", null);

            string uniqueName = identity.FindFirst("retailerCode")?.Value;

            UserSession.InitSessionNew(identity);
            string deviceId = identity.FindFirst("dvicId")?.Value;
            string loginProviderId = identity.FindFirst("jti")?.Value;

            if (string.IsNullOrWhiteSpace(uniqueName))
                return new ResponseMessage(true, failedMsg, null, "Retailer Code is null in session token.", null);

            if (_retailerCode != uniqueName)
                return new ResponseMessage(true, failedMsg, null, "Request user and Session user not matched", null);

            if (string.IsNullOrWhiteSpace(deviceId))
                return new ResponseMessage(true, failedMsg, null, "Device Id is null in session token.", null);

            if (_deviceId != deviceId)
                return new ResponseMessage(true, failedMsg, null, "Request device and Session device not matched", null);

            string successMsg = SharedResource.GetLocal("Success", Message.Success);

            return new ResponseMessage(false, successMsg, loginProviderId, "Validation Success.", null);
        }


        public static TokenValidationParameters GetJweTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = JweKeysModel.Issuer,
                ValidateAudience = true,
                ValidAudience = JweKeysModel.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JweKeysModel.TokenSignKey)),
                TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JweKeysModel.TokenKey))
            };
        }


        private static ClaimsPrincipal GetJwePrincipal(string sToken, out SecurityToken securityToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetJweTokenValidationParameters();

            ClaimsPrincipal principal = tokenHandler.ValidateToken(sToken, validationParameters, out securityToken);

            return principal;
        }

        #endregion
    }
}
