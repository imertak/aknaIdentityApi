using aknaIdentities_api.Domain.Entities;
using aknaIdentity_api.Domain.Interfaces.Contracts;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using aknaIdentity_api.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Application.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly ITwoFactorAuthSettingRepository _twoFactorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TwoFactorAuthService(ITwoFactorAuthSettingRepository twoFactorRepository, IUnitOfWork unitOfWork)
        {
            _twoFactorRepository = twoFactorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TwoFactorAuthSetting> EnableTwoFactorAsync(int userId, string method, string secretKey = null)
        {
            var existingSetting = await _twoFactorRepository.GetByUserIdAsync(userId);

            if (existingSetting != null)
            {
                existingSetting.IsEnabled = true;
                existingSetting.Method = method;
                if (!string.IsNullOrWhiteSpace(secretKey))
                    existingSetting.AuthenticatorSecretKey = secretKey;

                await _twoFactorRepository.UpdateAsync(existingSetting);
                await _unitOfWork.SaveChangesAsync();
                return existingSetting;
            }

            var setting = new TwoFactorAuthSetting
            {
                UserId = userId,
                IsEnabled = true,
                Method = method,
                AuthenticatorSecretKey = secretKey ?? (method == "Authenticator" ? await GenerateSecretKeyAsync() : null)
            };

            await _twoFactorRepository.AddAsync(setting);
            await _unitOfWork.SaveChangesAsync();

            return setting;
        }

        public async Task<bool> DisableTwoFactorAsync(int userId)
        {
            var setting = await _twoFactorRepository.GetByUserIdAsync(userId);
            if (setting != null && setting.IsEnabled)
            {
                setting.IsEnabled = false;
                await _twoFactorRepository.UpdateAsync(setting);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<TwoFactorAuthSetting> GetUserTwoFactorSettingAsync(int userId)
        {
            return await _twoFactorRepository.GetByUserIdAsync(userId);
        }

        public async Task<bool> IsUserTwoFactorEnabledAsync(int userId)
        {
            return await _twoFactorRepository.IsUserTwoFactorEnabledAsync(userId);
        }

        public async Task<bool> VerifyTwoFactorCodeAsync(int userId, string code)
        {
            var setting = await _twoFactorRepository.GetByUserIdAsync(userId);
            if (setting == null || !setting.IsEnabled)
                return false;

            switch (setting.Method?.ToLower())
            {
                case "authenticator":
                    return ValidateAuthenticatorCodeAsync(setting.AuthenticatorSecretKey, code).Result;
                default:
                    return false;
            }
        }

        public async Task<string> GenerateSecretKeyAsync()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[20];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes).Replace("=", "").Replace("+", "").Replace("/", "");
            }
        }

        public async Task<bool> ValidateAuthenticatorCodeAsync(string secretKey, string code)
        {
            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(code))
                return false;

            try
            {
                // Bu basit bir TOTP implementasyonudur. Üretim ortamında daha güvenli bir kütüphane kullanılmalıdır.
                var unixTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                var timeStep = unixTimestamp / 30;

                var secretBytes = Convert.FromBase64String(secretKey + new string('=', (4 - secretKey.Length % 4) % 4));
                var timeBytes = BitConverter.GetBytes(timeStep);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(timeBytes);

                using (var hmac = new HMACSHA1(secretBytes))
                {
                    var hash = hmac.ComputeHash(timeBytes);
                    var offset = hash[hash.Length - 1] & 0x0F;
                    var binaryCode = (hash[offset] & 0x7F) << 24
                                   | (hash[offset + 1] & 0xFF) << 16
                                   | (hash[offset + 2] & 0xFF) << 8
                                   | (hash[offset + 3] & 0xFF);

                    var generatedCode = (binaryCode % 1000000).ToString("D6");
                    return generatedCode == code;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task UpdateTwoFactorMethodAsync(int userId, string method)
        {
            var setting = await _twoFactorRepository.GetByUserIdAsync(userId);
            if (setting != null)
            {
                setting.Method = method;
                await _twoFactorRepository.UpdateAsync(setting);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<TwoFactorAuthSetting> GetUserTwoFactorByMethodAsync(int userId, string method)
        {
            return await _twoFactorRepository.GetUserTwoFactorByMethodAsync(userId, method);
        }

        public async Task<List<TwoFactorAuthSetting>> GetEnabledTwoFactorSettingsAsync()
        {
            return await _twoFactorRepository.GetEnabledTwoFactorSettingsAsync();
        }
    }
}
