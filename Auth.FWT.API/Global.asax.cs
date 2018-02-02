using System.Web.Http;
using Auth.FWT.API.App_Start;

namespace Auth.FWT.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            
        }
    }
}
