// IVerificationService.cs
using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface IVerificationService
    {
        Task<Verification> CreateVerificationAsync(Guid userId, string type, TimeSpan? expiration = null);
        Task<Verification> GetByCodeAsync(string code);
        Task<List<Verification>> GetUserVerificationsAsync(Guid userId, string type = null);
        Task<Verification> GetLatestUserVerificationAsync(Guid userId, string type);
        Task<bool> ValidateVerificationCodeAsync(string code);
        Task<bool> UseVerificationCodeAsync(string code);
        Task<bool> ResendVerificationAsync(Guid userId, string type);
        Task ExpireUserVerificationsAsync(Guid userId, string type);
        Task CleanupExpiredVerificationsAsync();
        Task<bool> CanRequestNewVerificationAsync(Guid userId, string type, int maxRequestsPerHour = 5);
        Task<string> GenerateVerificationCodeAsync(int length = 6);
        Task<int> GetRecentVerificationCountAsync(Guid userId, string type, TimeSpan timeSpan);
    }
}