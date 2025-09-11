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
    public class VerificationService : IVerificationService
    {
        private readonly IVerificationRepository _verificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VerificationService(IVerificationRepository verificationRepository, IUnitOfWork unitOfWork)
        {
            _verificationRepository = verificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Verification> CreateVerificationAsync(int userId, string type, TimeSpan? expiration = null)
        {
            // Expire existing verifications of the same type
            await ExpireUserVerificationsAsync(userId, type);

            var verification = new Verification
            {
                UserId = userId,
                Code = await GenerateVerificationCodeAsync(),
                Type = type,
                ExpiresAt = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromMinutes(15)),
                IsUsed = false
            };

            await _verificationRepository.AddAsync(verification);
            await _unitOfWork.SaveChangesAsync();

            return verification;
        }

        public async Task<Verification> GetByCodeAsync(string code)
        {
            return await _verificationRepository.GetByCodeAsync(code);
        }

        public async Task<List<Verification>> GetUserVerificationsAsync(int userId, string type = null)
        {
            return await _verificationRepository.GetUserVerificationsAsync(userId, type);
        }

        public async Task<Verification> GetLatestUserVerificationAsync(int userId, string type)
        {
            return await _verificationRepository.GetLatestUserVerificationAsync(userId, type);
        }

        public async Task<bool> ValidateVerificationCodeAsync(string code)
        {
            return await _verificationRepository.IsCodeValidAsync(code);
        }

        public async Task<bool> UseVerificationCodeAsync(string code)
        {
            var verification = await _verificationRepository.GetByCodeAsync(code);
            if (verification == null || verification.IsUsed || verification.ExpiresAt <= DateTime.UtcNow)
                return false;

            verification.IsUsed = true;
            await _verificationRepository.UpdateAsync(verification);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResendVerificationAsync(int userId, string type)
        {
            if (!await CanRequestNewVerificationAsync(userId, type))
                return false;

            await CreateVerificationAsync(userId, type);
            return true;
        }

        public async Task ExpireUserVerificationsAsync(int userId, string type)
        {
            await _verificationRepository.ExpireVerificationsAsync(userId, type);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CleanupExpiredVerificationsAsync()
        {
            await _verificationRepository.CleanupExpiredVerificationsAsync();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CanRequestNewVerificationAsync(int userId, string type, int maxRequestsPerHour = 5)
        {
            var recentCount = await _verificationRepository.GetRecentVerificationCountAsync(userId, type, TimeSpan.FromHours(1));
            return recentCount < maxRequestsPerHour;
        }

        public async Task<string> GenerateVerificationCodeAsync(int length = 6)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                var code = "";

                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(bytes, i, 1);
                    code += (bytes[i] % 10).ToString();
                }

                return code;
            }
        }

        public async Task<int> GetRecentVerificationCountAsync(int userId, string type, TimeSpan timeSpan)
        {
            return await _verificationRepository.GetRecentVerificationCountAsync(userId, type, timeSpan);
        }

        public Task<Verification> CreateVerificationAsync(Guid userId, string type, TimeSpan? expiration = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<Verification>> GetUserVerificationsAsync(Guid userId, string type = null)
        {
            throw new NotImplementedException();
        }

        public Task<Verification> GetLatestUserVerificationAsync(Guid userId, string type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResendVerificationAsync(Guid userId, string type)
        {
            throw new NotImplementedException();
        }

        public Task ExpireUserVerificationsAsync(Guid userId, string type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanRequestNewVerificationAsync(Guid userId, string type, int maxRequestsPerHour = 5)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetRecentVerificationCountAsync(Guid userId, string type, TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
    }
}