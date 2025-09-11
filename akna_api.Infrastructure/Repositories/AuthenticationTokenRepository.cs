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
    public class AuthenticationTokenRepository : Repository<AuthenticationToken>, IAuthenticationTokenRepository
    {
        public AuthenticationTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<AuthenticationToken> GetByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return await _dbSet
                .Include(t => t.User)
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task<List<AuthenticationToken>> GetUserTokensAsync(int userId)
        {
            return await _dbSet
                .Where(t => !t.IsDeleted && t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<AuthenticationToken>> GetActiveUserTokensAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(t => !t.IsDeleted &&
                           t.UserId == userId &&
                           !t.IsRevoked &&
                           t.ExpiresAt > now)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task RevokeTokenAsync(string token, string reason = null)
        {
            var tokenEntity = await GetByTokenAsync(token);
            if (tokenEntity != null && !tokenEntity.IsRevoked)
            {
                tokenEntity.IsRevoked = true;
                tokenEntity.RevokedReason = reason ?? "Manual revocation";
                tokenEntity.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(tokenEntity);
            }
        }

        public async Task RevokeUserTokensAsync(int userId, string reason = null)
        {
            var activeTokens = await GetActiveUserTokensAsync(userId);

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedReason = reason ?? "User tokens revoked";
                token.UpdatedAt = DateTime.UtcNow;
            }

            if (activeTokens.Any())
            {
                _dbSet.UpdateRange(activeTokens);
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(t => !t.IsDeleted)
                .AnyAsync(t => t.Token == token &&
                              !t.IsRevoked &&
                              t.ExpiresAt > now);
        }

        public async Task<List<AuthenticationToken>> GetExpiredTokensAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(t => !t.IsDeleted && t.ExpiresAt <= now && !t.IsRevoked)
                .ToListAsync();
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await GetExpiredTokensAsync();

            foreach (var token in expiredTokens)
            {
                token.IsRevoked = true;
                token.RevokedReason = "Expired";
                token.UpdatedAt = DateTime.UtcNow;
            }

            if (expiredTokens.Any())
            {
                _dbSet.UpdateRange(expiredTokens);
            }
        }

        public async Task CleanupRevokedTokensOlderThanAsync(DateTime date)
        {
            var oldRevokedTokens = await _dbSet
                .Where(t => !t.IsDeleted && t.IsRevoked && t.UpdatedAt < date)
                .ToListAsync();

            foreach (var token in oldRevokedTokens)
            {
                await SoftDeleteAsync(token.Id, 0); // System cleanup
            }
        }
    }
}
