using System;

namespace Auth.FWT.API.SwaggerExtensions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class SwaggerDefaultValueAttribute : Attribute
    {
        public SwaggerDefaultValueAttribute(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public string ParameterName { get; private set; }

        public object Value { get; set; }
    }
}