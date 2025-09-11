using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Repositories
{
    public interface ITwoFactorAuthSettingRepository : IRepository<TwoFactorAuthSetting>
    {
        Task<TwoFactorAuthSetting> GetByUserIdAsync(int userId);
        Task<bool> IsUserTwoFactorEnabledAsync(int userId);
        Task<List<TwoFactorAuthSetting>> GetEnabledTwoFactorSettingsAsync();
        Task<TwoFactorAuthSetting> GetUserTwoFactorByMethodAsync(int userId, string method);
    }
}
