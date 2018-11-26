using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FWT.Infrastructure.IdentityServer
{
    public class TokenRequestValidator : ICustomTokenRequestValidator
    {
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            string phoneHashId = context.Result.ValidatedRequest.Raw["PhoneHashId"];
            context.Result.ValidatedRequest.ClientClaims.Add(new Claim("PhoneHashId", phoneHashId));
            return Task.CompletedTask;
        }
    }
}