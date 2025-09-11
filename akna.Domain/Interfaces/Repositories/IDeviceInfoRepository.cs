using aknaIdentities_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Repositories
{
    public interface IDeviceInfoRepository : IRepository<DeviceInfo>
    {
        Task<List<DeviceInfo>> GetUserDevicesAsync(int userId);
        Task<List<DeviceInfo>> GetActiveUserDevicesAsync(int userId);
        Task<DeviceInfo> GetByDeviceIdentifierAsync(string deviceIdentifier);
        Task<DeviceInfo> GetUserDeviceByIdentifierAsync(int userId, string deviceIdentifier);
        Task<List<DeviceInfo>> GetDevicesByPushTokenAsync(string pushToken);
        Task DeactivateDeviceAsync(int deviceId);
        Task DeactivateUserDevicesAsync(int userId);
        Task<List<DeviceInfo>> GetDevicesForNotificationAsync(int userId);
    }
}
