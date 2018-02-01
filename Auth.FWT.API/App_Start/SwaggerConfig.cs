using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Auth.FWT.API.Filters;
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
                    c.OAuth2("oauth2").Flow("password").TokenUrl(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "/token"));
                    c.OperationFilter<AssignOAuth2SecurityRequirements>();
                })
            .EnableSwaggerUi(c =>
            {
            });
        }
    }
}