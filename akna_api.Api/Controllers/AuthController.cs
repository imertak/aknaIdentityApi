using aknaIdentity_api.Domain.Dtos;
using aknaIdentity_api.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace aknaIdentity_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var result = await _authService.LoginAsync(request.Email, request.Password, ipAddress, userAgent);

                if (result.User == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                return Ok(new LoginResponse
                {
                    User = new UserDto
                    {
                        Id = result.User.Id,
                        Email = result.User.Email,
                        FirstName = result.User.FirstName,
                        LastName = result.User.LastName,
                        PhoneNumber = result.User.PhoneNumber,
                        UserType = result.User.UserType,
                        AccountStatus = result.User.AccountStatus,
                        IsEmailConfirmed = result.User.IsEmailConfirmed,
                        IsPhoneNumberConfirmed = result.User.IsPhoneNumberConfirmed
                    },
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("login-2fa")]
        public async Task<IActionResult> LoginWithTwoFactor([FromBody] LoginTwoFactorRequest request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var result = await _authService.LoginWithTwoFactorAsync(request.Email, request.Password, request.TwoFactorCode, ipAddress, userAgent);

                if (result.User == null)
                {
                    return Unauthorized(new { message = "Invalid credentials or 2FA code" });
                }

                return Ok(new LoginResponse
                {
                    User = new UserDto
                    {
                        Id = result.User.Id,
                        Email = result.User.Email,
                        FirstName = result.User.FirstName,
                        LastName = result.User.LastName,
                        PhoneNumber = result.User.PhoneNumber,
                        UserType = result.User.UserType,
                        AccountStatus = result.User.AccountStatus,
                        IsEmailConfirmed = result.User.IsEmailConfirmed,
                        IsPhoneNumberConfirmed = result.User.IsPhoneNumberConfirmed
                    },
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during 2FA login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName, request.PhoneNumber, request.UserType);

                if (user == null)
                {
                    return BadRequest(new { message = "Registration failed" });
                }

                return Ok(new { message = "Registration successful. Please check your email for verification." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var result = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress);

                if (result.AccessToken == null)
                {
                    return Unauthorized(new { message = "Invalid refresh token" });
                }

                return Ok(new
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new { message = "An error occurred during token refresh" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                var result = await _authService.LogoutAsync(request.RefreshToken);
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogoutAllDevices()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var result = await _authService.LogoutAllDevicesAsync(userId);
                return Ok(new { message = "Logged out from all devices" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout all devices");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request.Email);
                return Ok(new { message = "If the email exists, a reset code has been sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(request.Email, request.ResetCode, request.NewPassword);

                if (!result)
                {
                    return BadRequest(new { message = "Invalid reset code or email" });
                }

                return Ok(new { message = "Password reset successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return StatusCode(500, new { message = "An error occurred during password reset" });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

                if (!result)
                {
                    return BadRequest(new { message = "Current password is incorrect" });
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return StatusCode(500, new { message = "An error occurred during password change" });
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            try
            {
                var result = await _authService.ConfirmEmailAsync(request.Email, request.ConfirmationCode);

                if (!result)
                {
                    return BadRequest(new { message = "Invalid confirmation code" });
                }

                return Ok(new { message = "Email confirmed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email confirmation");
                return StatusCode(500, new { message = "An error occurred during email confirmation" });
            }
        }

        [HttpPost("confirm-phone")]
        public async Task<IActionResult> ConfirmPhone([FromBody] ConfirmPhoneRequest request)
        {
            try
            {
                var result = await _authService.ConfirmPhoneAsync(request.PhoneNumber, request.ConfirmationCode);

                if (!result)
                {
                    return BadRequest(new { message = "Invalid confirmation code" });
                }

                return Ok(new { message = "Phone number confirmed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during phone confirmation");
                return StatusCode(500, new { message = "An error occurred during phone confirmation" });
            }
        }

        [HttpPost("resend-email-confirmation")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationRequest request)
        {
            try
            {
                var result = await _authService.SendEmailConfirmationAsync(request.Email);
                return Ok(new { message = "Confirmation email sent if the email exists" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resend email confirmation");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost("resend-phone-confirmation")]
        public async Task<IActionResult> ResendPhoneConfirmation([FromBody] ResendPhoneConfirmationRequest request)
        {
            try
            {
                var result = await _authService.SendPhoneConfirmationAsync(request.PhoneNumber);
                return Ok(new { message = "Confirmation SMS sent if the phone number exists" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resend phone confirmation");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }
    }