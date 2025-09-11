using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Repositories
{
    public interface ISessionRepository : IRepository<Session>
    {
        Task<List<Session>> GetUserSessionsAsync(int userId);
        Task<List<Session>> GetActiveSessionsAsync(int userId);
        Task<Session> GetActiveSessionByDeviceAsync(int userId, string deviceInfo);
        Task TerminateSessionAsync(int sessionId);
        Task TerminateUserSessionsAsync(int userId);
        Task TerminateUserSessionsExceptCurrentAsync(int userId, int currentSessionId);
        Task<int> GetActiveSessionCountAsync(int userId);
        Task<List<Session>> GetExpiredSessionsAsync();
        Task CleanupExpiredSessionsAsync();
    }
}
