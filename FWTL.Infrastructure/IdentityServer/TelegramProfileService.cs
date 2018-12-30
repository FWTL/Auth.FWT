using System.Security.Claims;
using System.Threading.Tasks;
using FWTL.Core;
using FWTL.Core.Extensions;
using IdentityServer4.Validation;

namespace FWTL.Infrastructure.IdentityServer
{
    public class TokenRequestValidator : ICustomTokenRequestValidator
    {
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            string userId = context.Result.ValidatedRequest.Raw[Const.USER_ID];
            if (userId.IsNotNull())
            {
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim(Const.USER_ID, userId));
            }

            return Task.CompletedTask;
        }
    }
}
