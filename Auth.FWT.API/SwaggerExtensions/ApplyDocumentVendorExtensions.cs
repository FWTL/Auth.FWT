using System;
using System.Collections.Generic;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace GitGud.API.SwaggerExtensions
{
    public class ApplyDocumentVendorExtensions : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            foreach (var schemaDictionaryItem in swaggerDoc.definitions)
            {
                var schema = schemaDictionaryItem.Value;
                foreach (var propertyDictionaryItem in schema.properties)
                {
                    var property = propertyDictionaryItem.Value;
                    var propertyEnums = property.@enum;
                    if (propertyEnums != null && propertyEnums.Count > 0)
                    {
                        var enumDescriptions = new List<string>();
                        for (int i = 0; i < propertyEnums.Count; i++)
                        {
                            var enumOption = propertyEnums[i];
                            enumDescriptions.Add(string.Format("{0} = {1} ", (int)enumOption, Enum.GetName(enumOption.GetType(), enumOption)));
                        }

                        property.description += string.Format(" ({0})", string.Join(", ", enumDescriptions.ToArray()));
                    }
                }
            }
        }
    }
}
