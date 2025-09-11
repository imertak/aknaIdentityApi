using aknaIdentities_api.Domain.Entities;
using aknaIdentity_api.Domain.Interfaces.Contracts;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using aknaIdentity_api.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Application.Services
{
    public class DeviceInfoService : IDeviceInfoService
    {
        private readonly IDeviceInfoRepository _deviceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeviceInfoService(IDeviceInfoRepository deviceRepository, IUnitOfWork unitOfWork)
        {
            _deviceRepository = deviceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeviceInfo> RegisterDeviceAsync(int userId, string deviceIdentifier, string pushToken = null, string osType = null, string deviceModel = null)
        {
            // Check if device already exists for this user
            var existingDevice = await _deviceRepository.GetUserDeviceByIdentifierAsync(userId, deviceIdentifier);

            if (existingDevice != null)
            {
                // Update existing device
                existingDevice.PushNotificationToken = pushToken;
                existingDevice.OsType = osType;
                existingDevice.DeviceModel = deviceModel;
                existingDevice.IsActive = true;

                await _deviceRepository.UpdateAsync(existingDevice);
                await _unitOfWork.SaveChangesAsync();
                return existingDevice;
            }

            var device = new DeviceInfo
            {
                UserId = userId,
                DeviceIdentifier = deviceIdentifier,
                PushNotificationToken = pushToken,
                OsType = osType,
                DeviceModel = deviceModel,
                IsActive = true
            };

            await _deviceRepository.AddAsync(device);
            await _unitOfWork.SaveChangesAsync();

            return device;
        }

        public async Task<DeviceInfo> UpdateDeviceInfoAsync(int deviceId, string pushToken = null, string osType = null, string deviceModel = null)
        {
            var device = await _deviceRepository.GetByIdAsync(deviceId);
            if (device == null)
                return null;

            if (pushToken != null)
                device.PushNotificationToken = pushToken;

            if (!string.IsNullOrWhiteSpace(osType))
                device.OsType = osType;

            if (!string.IsNullOrWhiteSpace(deviceModel))
                device.DeviceModel = deviceModel;

            await _deviceRepository.UpdateAsync(device);
            await _unitOfWork.SaveChangesAsync();

            return device;
        }

        public async Task<List<DeviceInfo>> GetUserDevicesAsync(int userId)
        {
            return await _deviceRepository.GetUserDevicesAsync(userId);
        }

        public async Task<List<DeviceInfo>> GetActiveUserDevicesAsync(int userId)
        {
            return await _deviceRepository.GetActiveUserDevicesAsync(userId);
        }

        public async Task<DeviceInfo> GetByDeviceIdentifierAsync(string deviceIdentifier)
        {
            return await _deviceRepository.GetByDeviceIdentifierAsync(deviceIdentifier);
        }

        public async Task<DeviceInfo> GetUserDeviceByIdentifierAsync(int userId, string deviceIdentifier)
        {
            return await _deviceRepository.GetUserDeviceByIdentifierAsync(userId, deviceIdentifier);
        }

        public async Task DeactivateDeviceAsync(int deviceId)
        {
            await _deviceRepository.DeactivateDeviceAsync(deviceId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateUserDevicesAsync(int userId)
        {
            await _deviceRepository.DeactivateUserDevicesAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DeviceInfo>> GetDevicesForNotificationAsync(int userId)
        {
            return await _deviceRepository.GetDevicesForNotificationAsync(userId);
        }

        public async Task UpdatePushTokenAsync(string deviceIdentifier, string pushToken)
        {
            var device = await _deviceRepository.GetByDeviceIdentifierAsync(deviceIdentifier);
            if (device != null)
            {
                device.PushNotificationToken = pushToken;
                await _deviceRepository.UpdateAsync(device);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<List<DeviceInfo>> GetDevicesByPushTokenAsync(string pushToken)
        {
            return await _deviceRepository.GetDevicesByPushTokenAsync(pushToken);
        }

        public Task<DeviceInfo> RegisterDeviceAsync(Guid userId, string deviceIdentifier, string pushToken = null, string osType = null, string deviceModel = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<DeviceInfo>> GetUserDevicesAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<DeviceInfo>> GetActiveUserDevicesAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceInfo> GetUserDeviceByIdentifierAsync(Guid userId, string deviceIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task DeactivateUserDevicesAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<DeviceInfo>> GetDevicesForNotificationAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}