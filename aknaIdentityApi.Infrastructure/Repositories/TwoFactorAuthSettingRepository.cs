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
    public class TwoFactorAuthSettingRepository : Repository<TwoFactorAuthSetting>, ITwoFactorAuthSettingRepository
    {
        public TwoFactorAuthSettingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TwoFactorAuthSetting> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<bool> IsUserTwoFactorEnabledAsync(int userId)
        {
            return await _dbSet
                .Where(t => !t.IsDeleted)
                .AnyAsync(t => t.UserId == userId && t.IsEnabled);
        }

        public async Task<List<TwoFactorAuthSetting>> GetEnabledTwoFactorSettingsAsync()
        {
            return await _dbSet
                .Include(t => t.User)
                .Where(t => !t.IsDeleted && t.IsEnabled)
                .ToListAsync();
        }

        public async Task<TwoFactorAuthSetting> GetUserTwoFactorByMethodAsync(int userId, string method)
        {
            if (string.IsNullOrWhiteSpace(method))
                return null;

            return await _dbSet
                .Include(t => t.User)
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Method == method);
        }
    }
}
