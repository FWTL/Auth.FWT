using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Auth.FWT.Core.Helpers
{
    public static class ResponseHelper
    {
        public static HttpResponseMessage FileResult(string fileName, byte[] data)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new MemoryStream(data));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            return response;
        }
    }
}