using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.FWT.Core.Services.Telegram;
using FluentValidation;
using FluentValidation.Results;
using TeleSharp.TL;
using TeleSharp.TL.Auth;
using TeleSharp.TL.Help;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Auth;
using TLSharp.Core.Network;

namespace Auth.FWT.Infrastructure.Telegram
{
    public class NewTelegramClient : ITelegramClient
    {
        private ISessionStore _store;
        private IUserSessionManager _sessionManager;
        private int _apiId;
        private string _apiHash;

        public NewTelegramClient(IUserSessionManager sessionManager, ISessionStore store, int apiId, string apiHash)
        {
            _store = store;
            _apiId = apiId;
            _apiHash = apiHash;
            _sessionManager = sessionManager;
        }

        public async Task<bool> ConnectAsync(UserSession userSession, bool reconnect = false)
        {
            if (userSession.Session.AuthKey == null || reconnect)
            {
                var result = await Authenticator.DoAuthentication(userSession.TcpTransport);
                userSession.Session.AuthKey = result.AuthKey;
                userSession.Session.TimeOffset = result.TimeOffset;
            }

            userSession.Sender = new MtProtoSender(userSession.TcpTransport, userSession.Session);

            var config = new TLRequestGetConfig();
            var request = new TLRequestInitConnection()
            {
                ApiId = _apiId,
                AppVersion = "1.0.0",
                DeviceModel = "PC",
                LangCode = "en",
                Query = config,
                SystemVersion = "Win 10.0"
            };

            var invokewithLayer = new TLRequestInvokeWithLayer() { Layer = 66, Query = request };
            await userSession.Sender.Send(invokewithLayer);
            await userSession.Sender.Receive(invokewithLayer);

            TLContext.DcOptions = ((TLConfig)invokewithLayer.Response).DcOptions.ToList();

            return true;
        }

        private async Task ReconnectToDcAsync(UserSession userSession, int dcId)
        {
            if (TLContext.DcOptions == null || !TLContext.DcOptions.Any())
            {
                throw new InvalidOperationException($"Can't reconnect. Establish initial connection first.");
            }

            TLExportedAuthorization exported = null;
            if (userSession.Session.TLUser != null)
            {
                TLRequestExportAuthorization exportAuthorization = new TLRequestExportAuthorization() { DcId = dcId };
                exported = await SendRequestAsync<TLExportedAuthorization>(userSession, exportAuthorization);
            }

            var dc = TLContext.DcOptions.First(d => d.Id == dcId);

            userSession.TcpTransport = _sessionManager.GetConnection(dc.IpAddress, dc.Port);
            userSession.Session.ServerAddress = dc.IpAddress;
            userSession.Session.Port = dc.Port;

            await ConnectAsync(userSession, true);

            if (userSession.Session.TLUser != null)
            {
                TLRequestImportAuthorization importAuthorization = new TLRequestImportAuthorization() { Id = exported.Id, Bytes = exported.Bytes };
                var imported = await SendRequestAsync<TeleSharp.TL.Auth.TLAuthorization>(userSession, importAuthorization);
                OnUserAuthenticated(userSession, (TLUser)imported.User);
            }
        }

        private void OnUserAuthenticated(UserSession userSession, TLUser user)
        {
            userSession.Session.TLUser = user;
            userSession.Session.SessionExpires = int.MaxValue;
            _store.Save(userSession.Session);
        }

        private async Task RequestWithDcMigration(UserSession userSession, TLMethod request)
        {
            if (!userSession.TcpTransport.IsConnected)
            {
                userSession.TcpTransport = _sessionManager.GetConnection(userSession.Session.ServerAddress, userSession.Session.Port);
            }

            if (userSession.Sender == null)
            {
                await ConnectAsync(userSession);
            }

            var completed = false;
            while (!completed)
            {
                try
                {
                    await userSession.Sender.Send(request);
                    await userSession.Sender.Receive(request);
                    completed = true;
                }
                catch (DataCenterMigrationException e)
                {
                    await ReconnectToDcAsync(userSession, e.DC);
                    //// prepare the request for another try
                    request.ConfirmReceived = false;
                }
            }
        }

        public bool IsUserAuthorized(UserSession userSession)
        {
            return userSession.Session.TLUser != null;
        }

        public async Task<T> SendRequestAsync<T>(UserSession userSession, TLMethod methodToExecute)
        {
            await RequestWithDcMigration(userSession, methodToExecute);
            var result = methodToExecute.GetType().GetProperty("Response").GetValue(methodToExecute);

            _store.Save(userSession.Session);

            return (T)result;
        }

        public async Task<string> SendCodeRequestAsync(UserSession userSession, string phoneNumber)
        {
            try
            {
                var request = new TLRequestSendCode() { PhoneNumber = phoneNumber, ApiId = _apiId, ApiHash = _apiHash };
                await RequestWithDcMigration(userSession, request);

                return request.Response.PhoneCodeHash;
            }
            catch (FloodException ex)
            {
                ThrowValidationError(ex);
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ("PHONE_NUMBER_BANNED"):
                    case ("PHONE_NUMBER_INVALID"):
                        {
                            ThrowValidationError(ex);
                            break;
                        }

                    default:
                        {
                            throw new Exception("Unexpected error", ex);
                        }
                }
            }

            return string.Empty;
        }

        public void ThrowValidationError(Exception ex)
        {
            throw new ValidationException(new List<ValidationFailure>() { new ValidationFailure("phoneNumber", ex.Message) });
        }

        public async Task<UserSession> MakeAuthAsync(UserSession userSession, string phoneNumber, string phoneCodeHash, string code)
        {
            var request = new TLRequestSignIn() { PhoneNumber = phoneNumber, PhoneCodeHash = phoneCodeHash, PhoneCode = code };

            await RequestWithDcMigration(userSession, request);

            OnUserAuthenticated(userSession, ((TLUser)request.Response.User));

            return userSession;
        }

        public async Task<bool> IsPhoneRegisteredAsync(UserSession session, string phoneNumber)
        {
            var authCheckPhoneRequest = new TLRequestCheckPhone() { PhoneNumber = phoneNumber };
            await RequestWithDcMigration(session, authCheckPhoneRequest);

            return authCheckPhoneRequest.Response.PhoneRegistered;
        }

        public async Task<TLAbsDialogs> GetUserDialogsAsync(UserSession session)
        {
            var peer = new TLInputPeerSelf();
            return await SendRequestAsync<TLAbsDialogs>(session, new TLRequestGetDialogs() { OffsetDate = 0, OffsetPeer = peer, Limit = 10000 });
        }

        public async Task<TLAbsMessages> GetUserChatHistory(UserSession session, int userChatId, int maxId = int.MaxValue, int limit = 100)
        {
            try
            {
                var req = new TLRequestGetHistory()
                {
                    Peer = new TLInputPeerUser() { UserId = userChatId },
                    MaxId = maxId,
                    OffsetId = maxId,
                    Limit = limit
                };

                var results = await SendRequestAsync<TLAbsMessages>(session, req);
                return results;
            }
            catch (FloodException ex)
            {
                ThrowValidationError(ex);
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ("CHANNEL_INVALID	"):
                    case ("CHANNEL_PRIVATE"):
                    case ("CHAT_ID_INVALID"):
                    case ("PEER_ID_INVALID"):
                    case ("AUTH_KEY_PERM_EMPTY"):
                    case ("Timeout"):
                        {
                            ThrowValidationError(ex);
                            break;
                        }

                    default:
                        {
                            throw new Exception("Unexpected error", ex);
                        }
                }
            }

            return null;
        }
    }
}