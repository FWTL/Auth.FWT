using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.FWT.Core.DomainModels.Identity;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Data.Identity
{
    public class ApplicationUserValidator : IIdentityValidator<User>
    {
        private readonly UserManager<User, int> _userManager;

        public ApplicationUserValidator(UserManager<User, int> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> ValidateAsync(User item)
        {
            var errors = new List<string>();

            var otherAccount = await _userManager.FindByNameAsync(item.UserName);
            if (otherAccount != null && otherAccount.Id != item.Id)
            {
                errors.Add("An account has already been created with this email address.");
            }

            return errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }
    }
}