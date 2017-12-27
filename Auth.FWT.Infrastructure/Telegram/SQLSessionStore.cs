using System;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.DomainModels;
using Auth.FWT.Core.DomainModels.Identity;
using TLSharp.Core;

namespace Auth.FWT.Infrastructure.Telegram
{
    public class SQLSessionStore : ISessionStore
    {
        private User _currentUser;
        private IUnitOfWork _unitOfWork;

        public SQLSessionStore(IUnitOfWork unitOfWork, User currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public Session Load(string sessionUserId)
        {
            if (!_currentUser.TelegramSessionId.HasValue)
            {
                return null;
            }

            TelegramSession telegramSession = _unitOfWork.TelegramSessionRepository.GetSingle(_currentUser.TelegramSessionId.Value);
            return Session.FromBytes(telegramSession.Session, this, sessionUserId);
        }

        public void Save(Session session)
        {
            if (_currentUser.TelegramSessionId.HasValue)
            {
                Update(session);
            }

            Create(session);
        }

        private void Create(Session session)
        {
            _unitOfWork.BeginTransaction();

            TelegramSession telegramSession = new TelegramSession();
            telegramSession.Session = session.ToBytes();
            telegramSession.ExpireDateUtc = DateTimeOffset.FromUnixTimeSeconds(session.SessionExpires).UtcDateTime;
            telegramSession.UserId = _currentUser.Id;
            _unitOfWork.TelegramSessionRepository.Insert(telegramSession);

            _currentUser.TelegramSessionId = telegramSession.Id;
            _unitOfWork.UserRepository.UpdateColumns(_currentUser, () => _currentUser.TelegramSessionId);

            _unitOfWork.Commit();
        }

        private void Update(Session session)
        {
            TelegramSession telegramSession = _unitOfWork.TelegramSessionRepository.GetSingle(_currentUser.TelegramSessionId.Value);
            telegramSession.Session = session.ToBytes();
            telegramSession.ExpireDateUtc = DateTimeOffset.FromUnixTimeSeconds(session.SessionExpires).UtcDateTime;

            _unitOfWork.TelegramSessionRepository.Update(telegramSession);
            _unitOfWork.SaveChanges();
        }
    }
}