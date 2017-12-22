using System;

namespace Auth.FWT.Core.DomainModels
{
    public abstract class BaseEntity<TKey>
    {
        public DateTime? DeleteDateUTC { get; set; }

        public TKey Id { get; set; }

        public bool IsDeleted { get; set; }
    }
}
