using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Auth.FWT.Core.Helpers
{
    public static class HttpHelper
    {
        public static string GetJsonStringFromResponse(Func<WebResponse> function)
        {
            var response = function.Invoke();
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        public static HttpWebResponse SendGetRequest(string url)
        {
            var httpRequest = HttpWebRequest.Create(url);
            httpRequest.Method = "GET";

            return (HttpWebResponse)httpRequest.GetResponse();
        }

        public static HttpWebResponse SendPostRequest(string url, string bearerToken = null)
        {
            try
            {
                var httpRequest = HttpWebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                if (!string.IsNullOrEmpty(bearerToken))
                {
                    httpRequest.Headers["Authorization"] = "Bearer " + bearerToken;
                }

                var streamWriter = new StreamWriter(httpRequest.GetRequestStream());
                streamWriter.Close();

                return (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                return (HttpWebResponse)ex.Response;
            }
        }

        public static HttpWebResponse SendPostRequest<T>(T data, string url)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);

                var httpRequest = HttpWebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                return (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                return (HttpWebResponse)ex.Response;
            }
        }
    }
}
