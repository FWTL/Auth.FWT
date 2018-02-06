using System;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Extensions;
using Auth.FWT.Domain.Entities;
using Auth.FWT.Domain.Entities.Identity;
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
            User currentUser = null;
            if (!string.IsNullOrWhiteSpace(sessionUserId))
            {
                currentUser = _unitOfWork.UserRepository.GetSingle(sessionUserId.To<int>());
            }

            if (currentUser.IsNull())
            {
                return null;
            }

            if (!currentUser.TelegramSessionId.HasValue)
            {
                return null;
            }

            TelegramSession telegramSession = _unitOfWork.TelegramSessionRepository.GetSingle(currentUser.TelegramSessionId.Value);
            return Session.FromBytes(telegramSession.Session, this, sessionUserId);
        }

        public void Save(Session session)
        {
            if (string.IsNullOrWhiteSpace(session.SessionUserId))
            {
                return;
            }

            User currentUser = null;
            currentUser = _unitOfWork.UserRepository.GetSingle(session.SessionUserId.To<int>());

            if (currentUser.TelegramSessionId.HasValue)
            {
                Update(session, currentUser);
            }

            Create(session, currentUser);
        }

        private void Create(Session session, User currentUser)
        {
            _unitOfWork.BeginTransaction();

            TelegramSession telegramSession = new TelegramSession();
            telegramSession.Session = session.ToBytes();
            telegramSession.ExpireDateUtc = DateTimeOffset.FromUnixTimeSeconds(session.SessionExpires).UtcDateTime;
            telegramSession.UserId = currentUser.Id;
            _unitOfWork.TelegramSessionRepository.Insert(telegramSession);

            currentUser.TelegramSessionId = telegramSession.Id;
            _unitOfWork.UserRepository.UpdateColumns(currentUser, () => currentUser.TelegramSessionId);

            _unitOfWork.Commit();
        }

        private void Update(Session session, User currentUser)
        {
            TelegramSession telegramSession = _unitOfWork.TelegramSessionRepository.GetSingle(currentUser.TelegramSessionId.Value);
            telegramSession.Session = session.ToBytes();
            telegramSession.ExpireDateUtc = DateTimeOffset.FromUnixTimeSeconds(session.SessionExpires).UtcDateTime;

            _unitOfWork.TelegramSessionRepository.Update(telegramSession);
            _unitOfWork.SaveChanges();
        }
    }
}