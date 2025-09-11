using aknaIdentities_api.Domain.Entities;
using aknaIdentity_api.Domain.Interfaces.Contracts;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using aknaIdentity_api.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(ISessionRepository sessionRepository, IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Session> CreateSessionAsync(int userId, string deviceInfo, string ipAddress, TimeSpan? expiration = null)
        {
            var session = new Session
            {
                UserId = userId,
                DeviceInfo = deviceInfo,
                IpAddress = ipAddress,
                LoginTime = DateTime.UtcNow,
                ExpirationTime = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : (DateTime?)null
            };

            await _sessionRepository.AddAsync(session);
            await _unitOfWork.SaveChangesAsync();

            return session;
        }

        public async Task<List<Session>> GetUserSessionsAsync(int userId)
        {
            return await _sessionRepository.GetUserSessionsAsync(userId);
        }

        public async Task<List<Session>> GetActiveSessionsAsync(int userId)
        {
            return await _sessionRepository.GetActiveSessionsAsync(userId);
        }

        public async Task<Session> GetActiveSessionByDeviceAsync(int userId, string deviceInfo)
        {
            return await _sessionRepository.GetActiveSessionByDeviceAsync(userId, deviceInfo);
        }

        public async Task<int> GetActiveSessionCountAsync(int userId)
        {
            return await _sessionRepository.GetActiveSessionCountAsync(userId);
        }

        public async Task TerminateSessionAsync(int sessionId)
        {
            await _sessionRepository.TerminateSessionAsync(sessionId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task TerminateUserSessionsAsync(int userId)
        {
            await _sessionRepository.TerminateUserSessionsAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task TerminateUserSessionsExceptCurrentAsync(int userId, int currentSessionId)
        {
            await _sessionRepository.TerminateUserSessionsExceptCurrentAsync(userId, currentSessionId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsSessionValidAsync(int sessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.LogoutTime.HasValue)
                return false;

            if (session.ExpirationTime.HasValue && session.ExpirationTime < DateTime.UtcNow)
                return false;

            return true;
        }

        public async Task UpdateSessionActivityAsync(int sessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session != null && !session.LogoutTime.HasValue)
            {
                await _sessionRepository.UpdateAsync(session);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task CleanupExpiredSessionsAsync()
        {
            await _sessionRepository.CleanupExpiredSessionsAsync();
            await _unitOfWork.SaveChangesAsync();
        }
    }
}