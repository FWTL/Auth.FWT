using FWT.Core;
using FWT.Core.Extensions;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FWT.Infrastructure.IdentityServer
{
    public class TokenRequestValidator : ICustomTokenRequestValidator
    {
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            string phoneHashId = context.Result.ValidatedRequest.Raw["PhoneHasshId"];
            if (phoneHashId.IsNotNull())
            {
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim(Const.PHONE_HASH_ID, phoneHashId));
            }

            return Task.CompletedTask;
        }
    }
}