
// DeviceController.cs
using aknaIdentity_api.Domain.Dtos;
using aknaIdentity_api.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aknaIdentity_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceInfoService _deviceService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(
            IDeviceInfoService deviceService,
            IAuditService auditService,
            ILogger<DeviceController> logger)
        {
            _deviceService = deviceService;
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet("my-devices")]
        public async Task<IActionResult> GetMyDevices()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var userGuid = Guid.Parse(userId.ToString().PadLeft(32, '0'));
                var devices = await _deviceService.GetUserDevicesAsync(userGuid);

                var response = devices.Select(d => new DeviceDto
                {
                    Id = d.Id,
                    DeviceIdentifier = d.DeviceIdentifier,
                    DeviceModel = d.DeviceModel,
                    OsType = d.OsType,
                    IsActive = d.IsActive,
                    RegisteredAt = d.CreatedAt,
                    LastActiveAt = d.UpdatedAt
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user devices");
                return StatusCode(500, new { message = "An error occurred while fetching devices" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var userGuid = Guid.Parse(userId.ToString().PadLeft(32, '0'));
                var device = await _deviceService.RegisterDeviceAsync(
                    userGuid,
                    request.DeviceIdentifier,
                    request.PushToken,
                    request.OsType,
                    request.DeviceModel
                );

                await _auditService.LogUserActionAsync(userId, "DeviceRegister", $"Registered device: {request.DeviceIdentifier}");

                return Ok(new { message = "Device registered successfully", deviceId = device.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering device");
                return StatusCode(500, new { message = "An error occurred while registering device" });
            }
        }

        [HttpPut("{deviceId}")]
        public async Task<IActionResult> UpdateDevice(int deviceId, [FromBody] UpdateDeviceRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var userGuid = Guid.Parse(userId.ToString().PadLeft(32, '0'));

                // Verify device belongs to user
                var devices = await _deviceService.GetUserDevicesAsync(userGuid);
                if (!devices.Any(d => d.Id == deviceId))
                    return Forbid();

                var device = await _deviceService.UpdateDeviceInfoAsync(
                    deviceId,
                    request.PushToken,
                    request.OsType,
                    request.DeviceModel
                );

                if (device == null)
                    return NotFound(new { message = "Device not found" });

                return Ok(new { message = "Device updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "An error occurred while updating device" });
            }
        }

        [HttpPost("{deviceId}/deactivate")]
        public async Task<IActionResult> DeactivateDevice(int deviceId)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var userGuid = Guid.Parse(userId.ToString().PadLeft(32, '0'));

                // Verify device belongs to user
                var devices = await _deviceService.GetUserDevicesAsync(userGuid);
                if (!devices.Any(d => d.Id == deviceId))
                    return Forbid();

                await _deviceService.DeactivateDeviceAsync(deviceId);
                await _auditService.LogUserActionAsync(userId, "DeviceDeactivate", $"Deactivated device {deviceId}");

                return Ok(new { message = "Device deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating device: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "An error occurred while deactivating device" });
            }
        }

        [HttpPost("deactivate-all")]
        public async Task<IActionResult> DeactivateAllDevices()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var userGuid = Guid.Parse(userId.ToString().PadLeft(32, '0'));
                await _deviceService.DeactivateUserDevicesAsync(userGuid);
                await _auditService.LogUserActionAsync(userId, "DeviceDeactivateAll", "Deactivated all devices");

                return Ok(new { message = "All devices deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating all devices");
                return StatusCode(500, new { message = "An error occurred while deactivating devices" });
            }
        }

        [HttpPut("push-token")]
        public async Task<IActionResult> UpdatePushToken([FromBody] UpdatePushTokenRequest request)
        {
            try
            {
                await _deviceService.UpdatePushTokenAsync(request.DeviceIdentifier, request.PushToken);
                return Ok(new { message = "Push token updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating push token");
                return StatusCode(500, new { message = "An error occurred while updating push token" });
            }
        }
    }
}