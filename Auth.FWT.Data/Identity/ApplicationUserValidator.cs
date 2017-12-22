using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Auth.FWT.Core.DomainModels.Identity;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Data.Identity
{
    public class ApplicationUserValidator : IIdentityValidator<User>
    {
        private static readonly Regex EmailRegex = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly UserManager<User, int> _userManager;

        public ApplicationUserValidator(UserManager<User, int> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> ValidateAsync(User item)
        {
            var errors = new List<string>();
            if (!EmailRegex.IsMatch(item.Email))
                errors.Add("Enter a valid email address.");

            if (_userManager != null)
            {
                var otherAccount = await _userManager.FindByEmailAsync(item.Email);
                if (otherAccount != null && otherAccount.Id != item.Id)
                    errors.Add("An account has already been created with this email address.");
            }

            return errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }
    }
}
