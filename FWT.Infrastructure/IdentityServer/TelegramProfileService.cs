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
            string phoneHashId = context.Result.ValidatedRequest.Raw[Const.PHONE_HASH_ID];
            if (phoneHashId.IsNotNull())
            {
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim(Const.PHONE_HASH_ID, phoneHashId));
            }

            return Task.CompletedTask;
        }
    }
}
