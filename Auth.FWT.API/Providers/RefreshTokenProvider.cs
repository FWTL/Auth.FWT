using System;
using System.Threading.Tasks;
using Auth.FWT.Core.Entities.API;
using Auth.FWT.Core.Helpers;
using Auth.FWT.Data;
using Microsoft.Owin.Security.Infrastructure;

namespace Auth.FWT.API.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var unitOfWork = new UnitOfWork(new Data.AppContext());

            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");
            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshToken()
            {
                Id = HashHelper.GetHash(refreshTokenId),
                ClientAPIId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            unitOfWork.BeginTransaction();
                unitOfWork.RefreshTokenRepository.BatchDelete(rt => rt.ClientAPIId == token.ClientAPIId && rt.Subject == token.Subject);
                unitOfWork.RefreshTokenRepository.Insert(token);
            await unitOfWork.CommitAsync();

            context.SetToken(refreshTokenId);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = HashHelper.GetHash(context.Token);

            var unitOfWork = new UnitOfWork(new Data.AppContext());
            RefreshToken refreshToken = await unitOfWork.RefreshTokenRepository.GetSingleAsync(hashedTokenId);

            if (refreshToken != null)
            {
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                unitOfWork.RefreshTokenRepository.Delete(refreshToken);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
