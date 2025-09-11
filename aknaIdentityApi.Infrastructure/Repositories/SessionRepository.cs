using akna_api.Infrastructure.Context;
using aknaIdentities_api.Domain.Entities;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Infrastructure.Repositories
{
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Session>> GetUserSessionsAsync(int userId)
        {
            return await _dbSet
                .Where(s => !s.IsDeleted && s.UserId == userId)
                .OrderByDescending(s => s.LoginTime)
                .ToListAsync();
        }

        public async Task<List<Session>> GetActiveSessionsAsync(int userId)
        {
            return await _dbSet
                .Where(s => !s.IsDeleted && s.UserId == userId && s.LogoutTime == null)
                .OrderByDescending(s => s.LoginTime)
                .ToListAsync();
        }

        public async Task<Session> GetActiveSessionByDeviceAsync(int userId, string deviceInfo)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => !s.IsDeleted &&
                                         s.UserId == userId &&
                                         s.DeviceInfo == deviceInfo &&
                                         s.LogoutTime == null);
        }

        public async Task TerminateSessionAsync(int sessionId)
        {
            var session = await GetByIdAsync(sessionId);
            if (session != null && session.LogoutTime == null)
            {
                session.LogoutTime = DateTime.UtcNow;
                session.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(session);
            }
        }

        public async Task TerminateUserSessionsAsync(int userId)
        {
            var activeSessions = await GetActiveSessionsAsync(userId);

            foreach (var session in activeSessions)
            {
                session.LogoutTime = DateTime.UtcNow;
                session.UpdatedAt = DateTime.UtcNow;
            }

            if (activeSessions.Any())
            {
                _dbSet.UpdateRange(activeSessions);
            }
        }

        public async Task TerminateUserSessionsExceptCurrentAsync(int userId, int currentSessionId)
        {
            var activeSessions = await _dbSet
                .Where(s => !s.IsDeleted &&
                           s.UserId == userId &&
                           s.Id != currentSessionId &&
                           s.LogoutTime == null)
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.LogoutTime = DateTime.UtcNow;
                session.UpdatedAt = DateTime.UtcNow;
            }

            if (activeSessions.Any())
            {
                _dbSet.UpdateRange(activeSessions);
            }
        }

        public async Task<int> GetActiveSessionCountAsync(int userId)
        {
            return await _dbSet
                .CountAsync(s => !s.IsDeleted && s.UserId == userId && s.LogoutTime == null);
        }

        public async Task<List<Session>> GetExpiredSessionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(s => !s.IsDeleted &&
                           s.ExpirationTime.HasValue &&
                           s.ExpirationTime < now &&
                           s.LogoutTime == null)
                .ToListAsync();
        }

        public async Task CleanupExpiredSessionsAsync()
        {
            var expiredSessions = await GetExpiredSessionsAsync();

            foreach (var session in expiredSessions)
            {
                session.LogoutTime = DateTime.UtcNow;
                session.UpdatedAt = DateTime.UtcNow;
            }

            if (expiredSessions.Any())
            {
                _dbSet.UpdateRange(expiredSessions);
            }
        }
    }
}
