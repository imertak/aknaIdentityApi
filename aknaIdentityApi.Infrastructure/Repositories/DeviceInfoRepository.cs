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
    public class DeviceInfoRepository : Repository<DeviceInfo>, IDeviceInfoRepository
    {
        public DeviceInfoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<DeviceInfo>> GetUserDevicesAsync(int userId)
        {
            return await _dbSet
                .Where(d => !d.IsDeleted && d.UserId == userId)
                .OrderByDescending(d => d.UpdatedAt)
                .ToListAsync();
        }

        public async Task<List<DeviceInfo>> GetActiveUserDevicesAsync(int userId)
        {
            return await _dbSet
                .Where(d => !d.IsDeleted && d.UserId == userId && d.IsActive)
                .OrderByDescending(d => d.UpdatedAt)
                .ToListAsync();
        }

        public async Task<DeviceInfo> GetByDeviceIdentifierAsync(string deviceIdentifier)
        {
            if (string.IsNullOrWhiteSpace(deviceIdentifier))
                return null;

            return await _dbSet
                .Where(d => !d.IsDeleted)
                .FirstOrDefaultAsync(d => d.DeviceIdentifier == deviceIdentifier);
        }

        public async Task<DeviceInfo> GetUserDeviceByIdentifierAsync(int userId, string deviceIdentifier)
        {
            if (string.IsNullOrWhiteSpace(deviceIdentifier))
                return null;

            return await _dbSet
                .Where(d => !d.IsDeleted)
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceIdentifier == deviceIdentifier);
        }

        public async Task<List<DeviceInfo>> GetDevicesByPushTokenAsync(string pushToken)
        {
            if (string.IsNullOrWhiteSpace(pushToken))
                return new List<DeviceInfo>();

            return await _dbSet
                .Where(d => !d.IsDeleted && d.PushNotificationToken == pushToken)
                .ToListAsync();
        }

        public async Task DeactivateDeviceAsync(int deviceId)
        {
            var device = await GetByIdAsync(deviceId);
            if (device != null && device.IsActive)
            {
                device.IsActive = false;
                device.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(device);
            }
        }

        public async Task DeactivateUserDevicesAsync(int userId)
        {
            var activeDevices = await GetActiveUserDevicesAsync(userId);

            foreach (var device in activeDevices)
            {
                device.IsActive = false;
                device.UpdatedAt = DateTime.UtcNow;
            }

            if (activeDevices.Any())
            {
                _dbSet.UpdateRange(activeDevices);
            }
        }

        public async Task<List<DeviceInfo>> GetDevicesForNotificationAsync(int userId)
        {
            return await _dbSet
                .Where(d => !d.IsDeleted &&
                           d.UserId == userId &&
                           d.IsActive &&
                           !string.IsNullOrEmpty(d.PushNotificationToken))
                .ToListAsync();
        }
    }
}
