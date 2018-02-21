using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Entities.Identity;
using Auth.FWT.Core.Extensions;
using NodaTime;
using TLSharp.Core;

namespace Auth.FWT.Infrastructure.Telegram
{
    public class SQLSessionStore : ISessionStore
    {
        private IClock _clock;
        private IUnitOfWork _unitOfWork;

        public SQLSessionStore(IUnitOfWork unitOfWork, IClock clock)
        {
            _unitOfWork = unitOfWork;
            _clock = clock;
        }

        public Session Load(string sessionUserId)
        {
            if (string.IsNullOrWhiteSpace(sessionUserId))
            {
                return null;
            }

            TelegramSession telegramSession = _unitOfWork.TelegramSessionRepository.GetSingle(sessionUserId.To<int>());
            if (telegramSession != null && telegramSession.ExpireDateUtc < _clock.UtcNow())
            {
                return Session.FromBytes(telegramSession.Session, this, sessionUserId);
            }

            return null;
        }

        public void Save(Session session)
        {
            if (string.IsNullOrWhiteSpace(session.SessionUserId))
            {
                return;
            }

            User currentUser = null;
            currentUser = _unitOfWork.UserRepository.GetSingle(session.SessionUserId.To<int>());

            UpdateOrCreate(session, currentUser);
        }

        private void UpdateOrCreate(Session session, User currentUser)
        {
            TelegramSession telegramSession = _unitOfWork.TelegramSessionRepository.GetSingle(currentUser.Id);
            if (telegramSession == null)
            {
                telegramSession = new TelegramSession(session, currentUser);
                _unitOfWork.TelegramSessionRepository.Insert(telegramSession);
            }
            else
            {
                telegramSession = new TelegramSession(session, currentUser);
                _unitOfWork.TelegramSessionRepository.Update(telegramSession);
            }

            _unitOfWork.SaveChanges();
        }
    }
}