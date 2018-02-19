using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Auth;
using TeleSharp.TL.Help;
using TLSharp.Core;
using TLSharp.Core.Auth;
using TLSharp.Core.Network;

namespace TLSharp.Custom
{
    public class NewTelegramClient : ITelegramClient
    {
        private string _apiHash;
        private int _apiId;
        private List<TLDcOption> _dcOptions;

        public NewTelegramClient(int apiId, string apiHash)
        {
            if (apiId == default(int))
                throw new MissingApiConfigurationException("API_ID");
            if (string.IsNullOrEmpty(apiHash))
                throw new MissingApiConfigurationException("API_HASH");

            ///Init in IoC
            TLContext.Init();
            _apiHash = apiHash;
            _apiId = apiId;
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

            _dcOptions = ((TLConfig)invokewithLayer.Response).DcOptions.ToList();

            return true;
        }

        private async Task ReconnectToDcAsync(UserSession userSession, int dcId)
        {
            if (_dcOptions == null || !_dcOptions.Any())
            {
                throw new InvalidOperationException($"Can't reconnect. Establish initial connection first.");
            }

            TLExportedAuthorization exported = null;
            if (userSession.Session.TLUser != null)
            {
                TLRequestExportAuthorization exportAuthorization = new TLRequestExportAuthorization() { DcId = dcId };
                exported = await SendRequestAsync<TLExportedAuthorization>(userSession, exportAuthorization);
            }

            var dc = _dcOptions.First(d => d.Id == dcId);

            userSession.TcpTransport = new TcpTransport(dc.IpAddress, dc.Port, null);
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

        private void OnUserAuthenticated(UserSession userSession, TLUser TLUser)
        {
            userSession.Session.TLUser = TLUser;
            userSession.Session.SessionExpires = int.MaxValue;

            userSession.Session.Save();
        }

        private async Task RequestWithDcMigration(UserSession userSession, TLMethod request)
        {
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
                    // prepare the request for another try
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

            return (T)result;
        }

        public async Task<string> SendCodeRequestAsync(UserSession userSession, string phoneNumber)
        {
            var request = new TLRequestSendCode() { PhoneNumber = phoneNumber, ApiId = _apiId, ApiHash = _apiHash };
            await RequestWithDcMigration(userSession, request);

            return request.Response.PhoneCodeHash;
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
    }
}