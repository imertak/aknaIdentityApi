using aknaIdentities_api.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface ITwoFactorAuthService
    {
        Task<TwoFactorAuthSetting> EnableTwoFactorAsync(int userId, string method, string secretKey = null);
        Task<bool> DisableTwoFactorAsync(int userId);
        Task<TwoFactorAuthSetting> GetUserTwoFactorSettingAsync(int userId);
        Task<bool> IsUserTwoFactorEnabledAsync(int userId);
        Task<bool> VerifyTwoFactorCodeAsync(int userId, string code);
        Task<string> GenerateSecretKeyAsync();
        Task<bool> ValidateAuthenticatorCodeAsync(string secretKey, string code);
        Task UpdateTwoFactorMethodAsync(int userId, string method);
        Task<TwoFactorAuthSetting> GetUserTwoFactorByMethodAsync(int userId, string method);
        Task<List<TwoFactorAuthSetting>> GetEnabledTwoFactorSettingsAsync();
    }
}
