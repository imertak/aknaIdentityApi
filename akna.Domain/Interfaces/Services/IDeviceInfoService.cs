// IDeviceInfoService.cs
using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface IDeviceInfoService
    {
        Task<DeviceInfo> RegisterDeviceAsync(Guid userId, string deviceIdentifier, string pushToken = null, string osType = null, string deviceModel = null);
        Task<DeviceInfo> UpdateDeviceInfoAsync(int deviceId, string pushToken = null, string osType = null, string deviceModel = null);
        Task<List<DeviceInfo>> GetUserDevicesAsync(Guid userId);
        Task<List<DeviceInfo>> GetActiveUserDevicesAsync(Guid userId);
        Task<DeviceInfo> GetByDeviceIdentifierAsync(string deviceIdentifier);
        Task<DeviceInfo> GetUserDeviceByIdentifierAsync(Guid userId, string deviceIdentifier);
        Task DeactivateDeviceAsync(int deviceId);
        Task DeactivateUserDevicesAsync(Guid userId);
        Task<List<DeviceInfo>> GetDevicesForNotificationAsync(Guid userId);
        Task UpdatePushTokenAsync(string deviceIdentifier, string pushToken);
        Task<List<DeviceInfo>> GetDevicesByPushTokenAsync(string pushToken);
    }
}