using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Auth.FWT.API.SwaggerExtensions
{
    public class DefaultValueOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                return;
            }

            var actionParams = apiDescription.ActionDescriptor.GetParameters();
            foreach (var param in operation.parameters)
            {
                var actionParam = actionParams.FirstOrDefault(p => p.ParameterName == param.name);
                if (actionParam != null)
                {
                    if (actionParam.ParameterType == typeof(bool))
                    {
                        param.@default = false;
                    }
                }
            }
        }
    }
}