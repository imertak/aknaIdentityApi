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
    public class VerificationRepository : Repository<Verification>, IVerificationRepository
    {
        public VerificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Verification> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return await _dbSet
                .Include(v => v.User)
                .Where(v => !v.IsDeleted)
                .FirstOrDefaultAsync(v => v.Code == code);
        }

        public async Task<List<Verification>> GetUserVerificationsAsync(int userId, string type)
        {
            var query = _dbSet.Where(v => !v.IsDeleted && v.UserId == userId);

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(v => v.Type == type);
            }

            return await query
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<Verification> GetLatestUserVerificationAsync(int userId, string type)
        {
            return await _dbSet
                .Where(v => !v.IsDeleted && v.UserId == userId && v.Type == type)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task ExpireVerificationsAsync(int userId, string type)
        {
            var activeVerifications = await _dbSet
                .Where(v => !v.IsDeleted &&
                           v.UserId == userId &&
                           v.Type == type &&
                           !v.IsUsed &&
                           v.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var verification in activeVerifications)
            {
                verification.IsUsed = true;
                verification.UpdatedAt = DateTime.UtcNow;
            }

            if (activeVerifications.Any())
            {
                _dbSet.UpdateRange(activeVerifications);
            }
        }

        public async Task<int> GetRecentVerificationCountAsync(int userId, string type, TimeSpan timeSpan)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(timeSpan);

            return await _dbSet
                .CountAsync(v => !v.IsDeleted &&
                                v.UserId == userId &&
                                v.Type == type &&
                                v.CreatedAt >= cutoffTime);
        }

       

        public async Task<bool> IsCodeValidAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(v => !v.IsDeleted)
                .AnyAsync(v => v.Code == code &&
                              !v.IsUsed &&
                              v.ExpiresAt > now);
        }

        public async Task<List<Verification>> GetExpiredVerificationsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(v => !v.IsDeleted && v.ExpiresAt <= now && !v.IsUsed)
                .ToListAsync();
        }

        public async Task CleanupExpiredVerificationsAsync()
        {
            var expiredVerifications = await GetExpiredVerificationsAsync();

            foreach (var verification in expiredVerifications)
            {
                verification.IsUsed = true;
                verification.UpdatedAt = DateTime.UtcNow;
            }

            if (expiredVerifications.Any())
            {
                _dbSet.UpdateRange(expiredVerifications);
            }
        }
    }
}
