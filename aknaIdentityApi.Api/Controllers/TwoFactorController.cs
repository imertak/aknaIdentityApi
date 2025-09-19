// TwoFactorController.cs
using aknaIdentity_api.Domain.Dtos;
using aknaIdentity_api.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aknaIdentity_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TwoFactorController : ControllerBase
    {
        private readonly ITwoFactorAuthService _twoFactorService;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;
        private readonly ILogger<TwoFactorController> _logger;

        public TwoFactorController(
            ITwoFactorAuthService twoFactorService,
            INotificationService notificationService,
            IAuditService auditService,
            ILogger<TwoFactorController> logger)
        {
            _twoFactorService = twoFactorService;
            _notificationService = notificationService;
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetTwoFactorStatus()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var setting = await _twoFactorService.GetUserTwoFactorSettingAsync(userId);

                return Ok(new TwoFactorStatusResponse
                {
                    IsEnabled = setting?.IsEnabled ?? false,
                    Method = setting?.Method,
                    IsConfigured = setting != null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting 2FA status");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost("enable")]
        public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var setting = await _twoFactorService.EnableTwoFactorAsync(userId, request.Method);

                await _auditService.LogTwoFactorEventAsync(userId, "Enable", true, ipAddress);

                var response = new EnableTwoFactorResponse
                {
                    IsEnabled = true,
                    Method = setting.Method,
                    SecretKey = setting.Method == "Authenticator" ? setting.AuthenticatorSecretKey : null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling 2FA");
                return StatusCode(500, new { message = "An error occurred while enabling 2FA" });
            }
        }

        [HttpPost("disable")]
        public async Task<IActionResult> DisableTwoFactor([FromBody] DisableTwoFactorRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                // Verify the code first for security
                if (!await _twoFactorService.VerifyTwoFactorCodeAsync(userId, request.VerificationCode))
                {
                    await _auditService.LogTwoFactorEventAsync(userId, "DisableAttempt", false, ipAddress);
                    return BadRequest(new { message = "Invalid verification code" });
                }

                var result = await _twoFactorService.DisableTwoFactorAsync(userId);

                if (result)
                {
                    await _auditService.LogTwoFactorEventAsync(userId, "Disable", true, ipAddress);
                    return Ok(new { message = "Two-factor authentication disabled successfully" });
                }

                return BadRequest(new { message = "Failed to disable two-factor authentication" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling 2FA");
                return StatusCode(500, new { message = "An error occurred while disabling 2FA" });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyTwoFactorCode([FromBody] VerifyTwoFactorRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var isValid = await _twoFactorService.VerifyTwoFactorCodeAsync(userId, request.Code);

                await _auditService.LogTwoFactorEventAsync(userId, "Verify", isValid, ipAddress);

                if (isValid)
                    return Ok(new { isValid = true, message = "Code verified successfully" });

                return BadRequest(new { isValid = false, message = "Invalid code" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying 2FA code");
                return StatusCode(500, new { message = "An error occurred while verifying code" });
            }
        }

        [HttpPost("send-code")]
        public async Task<IActionResult> SendTwoFactorCode()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var setting = await _twoFactorService.GetUserTwoFactorSettingAsync(userId);
                if (setting == null || !setting.IsEnabled)
                    return BadRequest(new { message = "Two-factor authentication is not enabled" });

                var code = await _twoFactorService.GenerateSecretKeyAsync();
                // Store the code temporarily (you might want to use a cache here)

                // Send code based on method
                bool sent = false;
                if (setting.Method == "SMS")
                {
                    // Get user's phone number
                    sent = await _notificationService.SendTwoFactorCodeAsync(setting.User.PhoneNumber, code, "SMS");
                }
                else if (setting.Method == "Email")
                {
                    sent = await _notificationService.SendTwoFactorCodeAsync(setting.User.Email, code, "Email");
                }

                if (sent)
                    return Ok(new { message = "Verification code sent successfully" });

                return BadRequest(new { message = "Failed to send verification code" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending 2FA code");
                return StatusCode(500, new { message = "An error occurred while sending code" });
            }
        }
    }
}
