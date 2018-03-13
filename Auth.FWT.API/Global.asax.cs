using System.Web.Http;
using Newtonsoft.Json;

namespace Auth.FWT.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            HttpConfiguration config = GlobalConfiguration.Configuration;

            var formatting = Formatting.None;
#if DEBUG
            formatting = Formatting.Indented;
#endif
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = formatting,
            };

            config.Formatters.JsonFormatter.SerializerSettings = settings;
        }
    }
}