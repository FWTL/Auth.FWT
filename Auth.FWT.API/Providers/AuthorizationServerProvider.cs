using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Auth.FWT.Core;
using Auth.FWT.Core.Entities.API;
using Auth.FWT.Core.Entities.Identity;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Core.Helpers;
using Auth.FWT.Data;
using Auth.FWT.Infrastructure.Telegram;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using TLSharp.Core;
using static Auth.FWT.Core.Enums.DomainEnums;

namespace Auth.FWT.API.Providers
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public AuthorizationServerProvider()
        {
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var unitOfWork = new UnitOfWork(new AppContext());

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            if (allowedOrigin.IsNull())
            {
                allowedOrigin = "*";
            }

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            IFormCollection form = await context.Request.ReadFormAsync();
            string phoneNumber = HashHelper.GetHash($"+{Regex.Match(form["phoneNumber"], @"\d+").Value}");
            string code = form["code"];

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                context.SetError("invalid_grant", "The phoneNumber is empty");
                return;
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                context.SetError("invalid_grant", "The code is empty");
                return;
            }

            var sqlStore = new SQLSessionStore(unitOfWork);
            var telegramClient = new TelegramClient(ConfigKeys.TelegramApiId, ConfigKeys.TelegramApiHash, sqlStore, null);
            await telegramClient.ConnectAsync();

            string hash = await unitOfWork.TelegramCodeRepository.Query().Where(tc => tc.Id == phoneNumber).Select(tc => tc.CodeHash).FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(hash))
            {
                context.SetError("invalid_code", "Request for new code");
                return;
            }

            await telegramClient.MakeAuthAsync(phoneNumber, hash, code);

            User user = await unitOfWork.UserRepository.Query().Where(x => x.LockoutEnabled).FirstOrDefaultAsync();

            if (user.IsNull())
            {
                context.SetError("invalid_grant", "The phone number or code is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

            foreach (RoleClaim claim in user.Roles.SelectMany(r => r.Claims))
            {
                identity.AddClaim(new Claim(claim.ClaimType, claim.ClaimValue));
            }

            foreach (UserClaim claim in user.Claims)
            {
                identity.AddClaim(new Claim(claim.ClaimType, claim.ClaimValue));
            }

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", (context.ClientId.IsNull()) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", context.UserName
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            var unitOfWork = new UnitOfWork(new AppContext());
            ClientAPI client = unitOfWork.ClientAPIRepository.GetSingle(context.ClientId);

            if (client.IsNull())
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            if (client.ApplicationType == ApplicationType.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    if (client.Secret != HashHelper.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.IsActive)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult(0);
        }
    }
}
