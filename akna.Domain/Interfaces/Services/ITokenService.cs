using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(int userId, List<string> roles, List<string> permissions);
        Task<string> GenerateRefreshTokenAsync();
        Task<ClaimsPrincipal> ValidateTokenAsync(string token);
        Task<int> GetUserIdFromTokenAsync(string token);
        Task<bool> IsTokenValidAsync(string token);
        Task<DateTime> GetTokenExpirationAsync(string token);
    }
}