using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Auth.FWT.Core.Services.Telegram;
using FluentValidation;
using FluentValidation.Results;
using TeleSharp.TL;
using TeleSharp.TL.Auth;
using TeleSharp.TL.Help;
using TeleSharp.TL.Messages;
using TeleSharp.TL.Upload;
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

        public bool Connect(UserSession userSession, bool reconnect = false)
        {
            if (userSession.Session.AuthKey == null || reconnect)
            {
                var result = Authenticator.DoAuthentication(userSession.TcpTransport);
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
            userSession.Sender.Send(invokewithLayer);
            userSession.Sender.Receive(invokewithLayer);

            TLContext.DcOptions = ((TLConfig)invokewithLayer.Response).DcOptions.ToList();

            return true;
        }

        private void ReconnectToDc(UserSession userSession, int dcId)
        {
            if (TLContext.DcOptions == null || !TLContext.DcOptions.Any())
            {
                throw new InvalidOperationException($"Can't reconnect. Establish initial connection first.");
            }

            TLExportedAuthorization exported = null;
            if (userSession.Session.TLUser != null)
            {
                TLRequestExportAuthorization exportAuthorization = new TLRequestExportAuthorization() { DcId = dcId };
                exported = SendRequest<TLExportedAuthorization>(userSession, exportAuthorization);
            }

            var dc = TLContext.DcOptions.First(d => d.Id == dcId);

            userSession.TcpTransport = _sessionManager.GetConnection(dc.IpAddress, dc.Port);
            userSession.Session.ServerAddress = dc.IpAddress;
            userSession.Session.Port = dc.Port;

            Connect(userSession, true);

            if (userSession.Session.TLUser != null)
            {
                TLRequestImportAuthorization importAuthorization = new TLRequestImportAuthorization() { Id = exported.Id, Bytes = exported.Bytes };
                var imported = SendRequest<TeleSharp.TL.Auth.TLAuthorization>(userSession, importAuthorization);
                OnUserAuthenticated(userSession, (TLUser)imported.User);
            }
        }

        private void OnUserAuthenticated(UserSession userSession, TLUser user)
        {
            userSession.Session.TLUser = user;
            userSession.Session.SessionExpires = int.MaxValue;
            _store.Save(userSession.Session);
        }

        private void RequestWithDcMigration(UserSession userSession, TLMethod request)
        {
            if (!userSession.TcpTransport.IsConnected)
            {
                userSession.TcpTransport = _sessionManager.GetConnection(userSession.Session.ServerAddress, userSession.Session.Port);
            }

            if (userSession.Sender == null)
            {
                Connect(userSession);
            }

            var completed = false;
            while (!completed)
            {
                try
                {
                    userSession.Sender.Send(request);
                    userSession.Sender.Receive(request);
                    completed = true;
                }
                catch (DataCenterMigrationException e)
                {
                    ReconnectToDc(userSession, e.DC);
                    //// prepare the request for another try
                    request.ConfirmReceived = false;
                }
            }
        }

        public bool IsUserAuthorized(UserSession userSession)
        {
            return userSession.Session.TLUser != null;
        }

        public T SendRequest<T>(UserSession userSession, TLMethod methodToExecute)
        {
            RequestWithDcMigration(userSession, methodToExecute);
            var result = methodToExecute.GetType().GetProperty("Response").GetValue(methodToExecute);

            _store.Save(userSession.Session);

            return (T)result;
        }

        public string SendCodeRequest(UserSession userSession, string phoneNumber)
        {
            try
            {
                var request = new TLRequestSendCode() { PhoneNumber = phoneNumber, ApiId = _apiId, ApiHash = _apiHash };
                RequestWithDcMigration(userSession, request);

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
            throw new ValidationException(new List<ValidationFailure>() { new ValidationFailure("Error", ex.Message) });
        }

        public UserSession MakeAuth(UserSession userSession, string phoneNumber, string phoneCodeHash, string code)
        {
            var request = new TLRequestSignIn() { PhoneNumber = phoneNumber, PhoneCodeHash = phoneCodeHash, PhoneCode = code };

            RequestWithDcMigration(userSession, request);
            OnUserAuthenticated(userSession, ((TLUser)request.Response.User));

            return userSession;
        }

        public bool IsPhoneRegistered(UserSession session, string phoneNumber)
        {
            var authCheckPhoneRequest = new TLRequestCheckPhone() { PhoneNumber = phoneNumber };
            RequestWithDcMigration(session, authCheckPhoneRequest);

            return authCheckPhoneRequest.Response.PhoneRegistered;
        }

        public TLAbsDialogs GetUserDialogs(UserSession session)
        {
            var peer = new TLInputPeerSelf();
            return SendRequest<TLAbsDialogs>(session, new TLRequestGetDialogs() { OffsetDate = 0, OffsetPeer = peer, Limit = 10000 });
        }

        public TLAbsMessages GetUserChatHistory(UserSession session, int userChatId, int maxId = int.MaxValue, int limit = 100)
        {
            var userPeer = new TLInputPeerUser() { UserId = userChatId };
            return GetHistory(session, userPeer, maxId, limit);
        }

        public TLAbsMessages GetChannalHistory(UserSession session, int channalId, int maxId, int limit = 100)
        {
            var channalPeer = new TLInputPeerChannel() { ChannelId = channalId };
            return GetHistory(session, channalPeer, maxId, limit);
        }

        public TLAbsMessages GetChatHistory(UserSession session, int chatId, int maxId, int limit = 100)
        {
            var chatPeer = new TLInputPeerChat() { ChatId = chatId };
            return GetHistory(session, chatPeer, maxId, limit);
        }

        private TLAbsMessages GetHistory(UserSession session, TLAbsInputPeer peer, int maxId, int limit = 100)
        {
            try
            {
                var req = new TLRequestGetHistory()
                {
                    Peer = peer,
                    MaxId = maxId,
                    OffsetId = maxId,
                    Limit = limit
                };

                var results = SendRequest<TLAbsMessages>(session, req);
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

        public byte[] GetFile(UserSession userSession, TLInputDocumentFileLocation location, int size)
        {
            int filePart = 512 * 1024;
            int offset = 0;

            TLFile result = null;
            using (MemoryStream ms = new MemoryStream())
            {
                while (offset < size)
                {
                    result = SendRequest<TLFile>(userSession, new TLRequestGetFile()
                    {
                        Location = location,
                        Limit = (int)Math.Pow(2, Math.Ceiling(Math.Log(size, 2))),
                        Offset = offset
                    });
                    ms.Write(result.Bytes, 0, result.Bytes.Length);
                    offset += filePart;
                }

                return ms.ToArray();
            }
        }
    }
}