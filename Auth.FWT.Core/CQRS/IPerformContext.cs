using Hangfire.Server;

namespace Auth.FWT.Core.CQRS
{
    public interface IPerformContext
    {
        PerformContext PerformContext { get; set; }
    }
}