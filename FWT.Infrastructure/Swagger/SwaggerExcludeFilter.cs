using FWT.Infrastructure.Grid;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FWT.Infrastructure.Swagger
{
    public class SwaggerExcludeFilter : ISchemaFilter
    {
        public void Apply(Swashbuckle.AspNetCore.Swagger.Schema schema, SchemaFilterContext context)
        {
            var excludedProperties = context.SystemType.GetProperties().Where(t => t.GetCustomAttribute<SwaggerExcludeAttribute>() != null);
           
            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name))
                {
                    schema.Properties.Remove(excludedProperty.Name);
                }
            }
        }
    }
}