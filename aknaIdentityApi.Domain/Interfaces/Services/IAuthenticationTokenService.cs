using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface IAuthenticationTokenService
    {
        Task<AuthenticationToken> CreateTokenAsync(int userId, string token, TimeSpan? expiration = null, string ipAddress = null);
        Task<AuthenticationToken> GetByTokenAsync(string token);
        Task<List<AuthenticationToken>> GetUserTokensAsync(int userId);
        Task<List<AuthenticationToken>> GetActiveUserTokensAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
        Task RevokeTokenAsync(string token, string reason = null);
        Task RevokeUserTokensAsync(int userId, string reason = null);
        Task CleanupExpiredTokensAsync();
        Task CleanupOldRevokedTokensAsync(int daysOld = 30);
        Task<string> GenerateSecureTokenAsync();
    }
}