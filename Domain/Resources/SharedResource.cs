using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Domain.Resources
{
    public class SharedResource
    {
        public static string GetLocal(string key, string message = null)
        {
            HttpContext _httpContext = new HttpContextAccessor().HttpContext;

            Type localizerType = typeof(IStringLocalizer<SharedResource>);

            IStringLocalizer _local = (IStringLocalizer)_httpContext.RequestServices.GetService(localizerType);

            string localMessage = _local.GetString(key).Value;

            if ((string.IsNullOrWhiteSpace(localMessage) || localMessage == key) && !string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            if (localMessage == key)
            {
                localMessage = $"No Message Found. Missing Locale key '{key}'";
            }

            return localMessage;
        }
    }
}