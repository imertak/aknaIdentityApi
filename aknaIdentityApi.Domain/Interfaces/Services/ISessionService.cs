using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface ISessionService
    {
        Task<Session> CreateSessionAsync(int userId, string deviceInfo, string ipAddress, TimeSpan? expiration = null);
        Task<List<Session>> GetUserSessionsAsync(int userId);
        Task<List<Session>> GetActiveSessionsAsync(int userId);
        Task<Session> GetActiveSessionByDeviceAsync(int userId, string deviceInfo);
        Task<int> GetActiveSessionCountAsync(int userId);
        Task TerminateSessionAsync(int sessionId);
        Task TerminateUserSessionsAsync(int userId);
        Task TerminateUserSessionsExceptCurrentAsync(int userId, int currentSessionId);
        Task<bool> IsSessionValidAsync(int sessionId);
        Task UpdateSessionActivityAsync(int sessionId);
        Task CleanupExpiredSessionsAsync();
    }
}