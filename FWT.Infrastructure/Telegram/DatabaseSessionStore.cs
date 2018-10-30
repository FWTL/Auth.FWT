using FWT.Core.Services.Dapper;
using OpenTl.ClientApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FWT.Infrastructure.Telegram
{
    public class DatabaseSessionStore : ISessionStore
    {
        public DatabaseSessionStore(IDatabaseConnector database)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public byte[] Load()
        {
            throw new NotImplementedException();
        }

        public Task Save(byte[] session)
        {
            throw new NotImplementedException();
        }

        public void SetSessionTag(string sessionTag)
        {
            throw new NotImplementedException();
        }
    }
}
