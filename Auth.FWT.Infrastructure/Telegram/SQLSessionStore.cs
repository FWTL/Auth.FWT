using System;
using Auth.FWT.Core.Data;
using TLSharp.Core;

namespace Auth.FWT.Infrastructure.Telegram
{
    public class SQLSessionStore : ISessionStore
    {
        private IUnitOfWork _unitOfWork;

        public SQLSessionStore(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Session Load(string sessionUserId)
        {
            throw new NotImplementedException();
        }

        public void Save(Session session)
        {
            throw new NotImplementedException();
        }
    }
}