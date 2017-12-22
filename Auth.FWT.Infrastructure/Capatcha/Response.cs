using System.Collections.Generic;
using Newtonsoft.Json;

namespace Auth.FWT.Infrastructure.Captcha
{
    public class CaptchaResponse
    {
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
