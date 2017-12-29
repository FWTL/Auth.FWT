using System;
using Auth.FWT.Core;
using Auth.FWT.Core.Services.Captcha;
using Auth.FWT.Core.Services.Logging;
using RestSharp;

namespace Auth.FWT.Infrastructure.Captcha
{
    public class GoogleCaptcha : ICaptchaService
    {
        private readonly string _secretKey = ConfigKeys.Captcha;

        private ILogger _logger;

        public GoogleCaptcha(ILogger logger)
        {
            _logger = logger;
        }

        public bool Validate(string response)
        {
            try
            {
                var client = new RestClient("https://www.google.com");
                var request = new RestRequest("/recaptcha/api/siteverify?secret={secret}&response={response}", Method.GET);
                request.AddUrlSegment("secret", _secretKey);
                request.AddUrlSegment("response", response);

                var captchaResponse = client.Execute<CaptchaResponse>(request);
                return captchaResponse.Data.Success;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                return false;
            }
        }
    }
}