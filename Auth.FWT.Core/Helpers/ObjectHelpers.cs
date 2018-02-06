using System;
using System.Linq.Expressions;
using Auth.FWT.Core.Extensions;

namespace Auth.FWT.Core.Helpers
{
    public static class ObjectHelpers
    {
        public static string GetName(Expression<Func<object>> exp)
        {
            MemberExpression body = exp.Body as MemberExpression;

            if (body.IsNull())
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }
    }
}
