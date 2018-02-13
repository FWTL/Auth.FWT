using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Help;
using TLSharp.Core;
using TLSharp.Core.Auth;

namespace TLSharp.Custome
{
    public class NewTelegramClient
    {
        private string _apiHash;
        private int _apiId;
        private List<TLDcOption> _dcOptions;

        public NewTelegramClient(int apiId, string apiHash, ISessionStore store = null)
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
            await userSession.Sender.Send(invokewithLayer);

            //_dcOptions = ((TLConfig)invokewithLayer.Response).DcOptions.ToList(); ??

            return true;
        }
    }
}