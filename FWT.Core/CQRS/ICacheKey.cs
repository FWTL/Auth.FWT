using System;

namespace FWT.Core.CQRS
{
    public interface ICacheKey<TQuery>
    {
        Func<TQuery, string> KeyFn { get; set; }
    }
}
