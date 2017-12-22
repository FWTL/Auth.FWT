namespace Auth.FWT.Core.Identity
{
    public enum SignInStatus
    {
        Success,
        LockedOut,
        RequiresTwoFactorAuthentication,
        Failure
    }
}
