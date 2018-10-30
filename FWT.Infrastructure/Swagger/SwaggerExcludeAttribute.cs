using System;
using System.Collections.Generic;
using System.Text;

namespace FWT.Infrastructure.Swagger
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SwaggerExcludeAttribute : Attribute
    {
    }
}
