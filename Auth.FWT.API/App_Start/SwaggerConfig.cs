using System.Web.Http;
using Swashbuckle.Application;

namespace Auth.FWT.API
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
            .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Auth.FWT.API");
                })
            .EnableSwaggerUi(c => { });
        }
    }
}