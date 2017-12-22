using Auth.FWT.Core.Helpers;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Data.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return PasswordHelper.CreateHash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return PasswordHelper.ValidatePassword(providedPassword, hashedPassword) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
}
