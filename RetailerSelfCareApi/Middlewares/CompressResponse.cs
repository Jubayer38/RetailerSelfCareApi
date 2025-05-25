using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IO;
using System.IO.Compression;

namespace RetailerSelfCareApi.Middlewares
{
    public class CompressResponseAttribute : ActionFilterAttribute
    {

        private readonly Stream _originalBodyStream = null;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();
        private readonly RecyclableMemoryStream responseBody = null;

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

            if (response.Body is not BrotliStream)// avoid twice compression.
            {
                _recyclableMemoryStreamManager.GetStream();
                response.Body = responseBody;

                //_originStream = response.Body;
                //_ms = new MemoryStream();
                response.Headers.Append("Content-encoding", "br");
                response.Body = new BrotliStream(responseBody, CompressionLevel.Fastest);
            }

            base.OnActionExecuting(context);
        }

        public override async void OnResultExecuted(ResultExecutedContext context)
        {
            if ((_originalBodyStream != null) && (_recyclableMemoryStreamManager != null))
            {
                await responseBody.CopyToAsync(_originalBodyStream);

                //HttpResponse response = context.HttpContext.Response;
                //await response.Body.FlushAsync();
                //_ms.Seek(0, SeekOrigin.Begin);
                //response.Headers.ContentLength = _ms.Length;
                //await _ms.CopyToAsync(_originStream);
                //response.Body.Dispose();
                //_ms.Dispose();
                //response.Body = _originStream;
            }
            base.OnResultExecuted(context);
        }

    }
}