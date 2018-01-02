using System;

namespace Auth.FWT.Core.DomainModels
{
    public abstract class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
