using FWT.Core.Services.Unique;
using System;

namespace FWT.Infrastructure.Unique
{
    public class GuidService : IGuidService
    {
        public Guid New()
        {
            return Guid.NewGuid();
        }
    }
}