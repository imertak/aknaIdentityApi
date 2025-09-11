using aknaIdentities_api.Domain.Entities;
using aknaIdentity_api.Domain.Interfaces.Contracts;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using aknaIdentity_api.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace aknaIdentity_api.Application.Services
{
    public class AuthenticationTokenService : IAuthenticationTokenService
    {
        private readonly IAuthenticationTokenRepository _tokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationTokenService(IAuthenticationTokenRepository tokenRepository, IUnitOfWork unitOfWork)
        {
            _tokenRepository = tokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticationToken> CreateTokenAsync(int userId, string token, TimeSpan? expiration = null, string ipAddress = null)
        {
            var authToken = new AuthenticationToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromDays(30)),
                IsRevoked = false,
                IpAddress = ipAddress
            };

            await _tokenRepository.AddAsync(authToken);
            await _unitOfWork.SaveChangesAsync();

            return authToken;
        }

        public async Task<AuthenticationToken> GetByTokenAsync(string token)
        {
            return await _tokenRepository.GetByTokenAsync(token);
        }

        public async Task<List<AuthenticationToken>> GetUserTokensAsync(int userId)
        {
            return await _tokenRepository.GetUserTokensAsync(userId);
        }

        public async Task<List<AuthenticationToken>> GetActiveUserTokensAsync(int userId)
        {
            return await _tokenRepository.GetActiveUserTokensAsync(userId);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            return await _tokenRepository.IsTokenValidAsync(token);
        }

        public async Task RevokeTokenAsync(string token, string reason = null)
        {
            await _tokenRepository.RevokeTokenAsync(token, reason);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RevokeUserTokensAsync(int userId, string reason = null)
        {
            await _tokenRepository.RevokeUserTokensAsync(userId, reason);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CleanupExpiredTokensAsync()
        {
            await _tokenRepository.CleanupExpiredTokensAsync();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CleanupOldRevokedTokensAsync(int daysOld = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
            await _tokenRepository.CleanupRevokedTokensOlderThanAsync(cutoffDate);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> GenerateSecureTokenAsync()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
            }
        }
    }
}