using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Repositories
{
    public interface IAuthenticationTokenRepository : IRepository<AuthenticationToken>
    {
        Task<AuthenticationToken> GetByTokenAsync(string token);
        Task<List<AuthenticationToken>> GetUserTokensAsync(int userId);
        Task<List<AuthenticationToken>> GetActiveUserTokensAsync(int userId);
        Task RevokeTokenAsync(string token, string reason = null);
        Task RevokeUserTokensAsync(int userId, string reason = null);
        Task<bool> IsTokenValidAsync(string token);
        Task<List<AuthenticationToken>> GetExpiredTokensAsync();
        Task CleanupExpiredTokensAsync();
        Task CleanupRevokedTokensOlderThanAsync(DateTime date);
    }
}
