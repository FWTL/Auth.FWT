using System.Web.Http.Filters;
using Auth.FWT.API.Filters;
using Auth.FWT.Infrastructure.Logging;

namespace Auth.FWT.API.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(HttpFilterCollection filters)
        {
            filters.Add(new ApiExceptionAttribute(NLogLogger.Instance));
        }
    }
}
