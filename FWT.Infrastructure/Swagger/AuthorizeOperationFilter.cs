using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace FWT.Infrastructure.Swagger
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var hasAuthAttribute = context.ApiDescription.ControllerAttributes().Union(context.ApiDescription.ActionAttributes()).OfType<AuthorizeAttribute>().Any();
            if (hasAuthAttribute)
            {
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
                operation.Security.Add(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                });
            }
        }
    }
}