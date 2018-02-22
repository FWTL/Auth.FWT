using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Auth.FWT.API.Filters;
using Auth.FWT.API.SwaggerExtensions;
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
                    c.BasicAuth("basic").Description("Bearer Token Authentication");
                })
            .EnableSwaggerUi(c =>
            {
                c.CustomAsset("index", thisAssembly, "Auth.FWT.API.SwaggerExtensions.Index.html");
            });
        }
    }
}
