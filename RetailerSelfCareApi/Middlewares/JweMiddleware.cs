///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	JWT token Generation and validation Middleware
///	Creation Date :	20-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Application.Utils;
using Domain.ResponseModel;
using Domain.StaticClass;

namespace RetailerSelfCareApi.Middlewares
{
    public class JweMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            string token = (string)context.Items["token"];
            string retailerCode = (string)context.Items["retailerCode"];
            string deviceId = (string)context.Items["deviceId"];

            if (string.IsNullOrWhiteSpace(token))
            {
                if (IsEnableUnauthorizedRoute(context))
                {
                    await _next(context);
                }
                else
                {
                    string errMessage = string.Empty;

                    if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(retailerCode) || string.IsNullOrWhiteSpace(deviceId))
                    {
                        errMessage = "Required parameter's value is missing.";
                    }

                    ResponseMessage message = new()
                    {
                        isError = true,
                        message = "Bad Request!",
                        ErrorDetails = errMessage
                    };
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = MimeTypes.Json;

                    await context.Response.WriteAsJsonAsync(message);
                }
            }
            else
            {
                ResponseMessage tokenValidation = JweTokenUtils.ValidateJweToken(token, retailerCode, deviceId);

                if (tokenValidation.isError)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = MimeTypes.Json;

                    await context.Response.WriteAsJsonAsync(tokenValidation);
                }
                else if (!tokenValidation.isError)
                {
                    context.Items["loginProviderId"] = tokenValidation.data as string;
                    await _next(context);
                }
            }
        }

        private static bool IsEnableUnauthorizedRoute(HttpContext context)
        {
            if (context.Request.Path.Value is not null)
            {
                return AuthorizedRouteList.Routes.Contains(context.Request.Path.Value.ToString());
            }

            return false;
        }

    }
}