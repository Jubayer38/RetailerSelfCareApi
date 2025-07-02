using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IO;
using System.IO.Compression;

namespace RetailerSelfCareApi.Middlewares
{
    public class CompressResponseAttribute : ActionFilterAttribute
    {
        private Stream _originalBodyStream;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();
        private RecyclableMemoryStream responseBody;

        //private Stream _originStream = null;
        //private MemoryStream _ms = null;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpRequest request = context.HttpContext.Request;
            string acceptEncoding = request.Headers.AcceptEncoding;
            if (string.IsNullOrWhiteSpace(acceptEncoding))
            {
                request.Headers.Append("Accept-Encoding", "br");
            }

            HttpResponse response = context.HttpContext.Response;

            if (response.Body is not BrotliStream)
            {
                _originalBodyStream = response.Body;
                responseBody = _recyclableMemoryStreamManager.GetStream();
                response.Body = new BrotliStream(responseBody, CompressionLevel.Fastest);
                response.Headers.Append("Content-encoding", "br");
            }

            base.OnActionExecuting(context);
        }

        public override async void OnResultExecuted(ResultExecutedContext context)
        {
            if (_originalBodyStream != null && responseBody != null)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(_originalBodyStream);
                await responseBody.DisposeAsync();
            }
            base.OnResultExecuted(context);
        }

    }
}