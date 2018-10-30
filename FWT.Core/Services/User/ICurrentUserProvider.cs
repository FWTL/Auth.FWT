namespace FWT.Core.Services.User
{
    public interface ICurrentUserProvider
    {
        string Email { get; }

        string FullName { get; }

        string Id { get; }
    }
}
