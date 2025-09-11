using aknaIdentities_api.Domain.Entities;
using aknaIdentity_api.Domain.Interfaces.Services;
using aknaIdentity_api.Domain.Constants;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace aknaIdentity_api.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IAuthenticationTokenService _authTokenService;
        private readonly ISessionService _sessionService;
        private readonly ITwoFactorAuthService _twoFactorService;
        private readonly IVerificationService _verificationService;
        private readonly IPermissionService _permissionService;
        private readonly IRoleService _roleService;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserService userService,
            ITokenService tokenService,
            IAuthenticationTokenService authTokenService,
            ISessionService sessionService,
            ITwoFactorAuthService twoFactorService,
            IVerificationService verificationService,
            IPermissionService permissionService,
            IRoleService roleService,
            INotificationService notificationService,
            IAuditService auditService,
            ILogger<AuthService> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _authTokenService = authTokenService;
            _sessionService = sessionService;
            _twoFactorService = twoFactorService;
            _verificationService = verificationService;
            _permissionService = permissionService;
            _roleService = roleService;
            _notificationService = notificationService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<(User User, string AccessToken, string RefreshToken)> LoginAsync(string email, string password, string ipAddress = null, string deviceInfo = null)
        {
            try
            {
                await _auditService.LogLoginAttemptAsync(email, false, ipAddress, "Login attempt started");

                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                {
                    await _auditService.LogLoginAttemptAsync(email, false, ipAddress, "User not found");
                    return (null, null, null);
                }

                if (user.AccountStatus != UserStatus.Active)
                {
                    await _auditService.LogLoginAttemptAsync(email, false, ipAddress, $"Account status: {user.AccountStatus}");
                    return (null, null, null);
                }

                if (!await _userService.ValidatePasswordAsync(password, user.PasswordHash))
                {
                    await _auditService.LogLoginAttemptAsync(email, false, ipAddress, "Invalid password");
                    return (null, null, null);
                }

                // Check if 2FA is enabled
                if (await _twoFactorService.IsUserTwoFactorEnabledAsync(user.Id))
                {
                    await _auditService.LogLoginAttemptAsync(email, false, ipAddress, "2FA required");
                    return (null, null, null); // Return special status indicating 2FA is required
                }

                return await CompleteLoginAsync(user, ipAddress, deviceInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", email);
                await _auditService.LogLoginAttemptAsync(email, false, ipAddress, $"Login error: {ex.Message}");
                return (null, null, null);
            }
        }

        public async Task<(User User, string AccessToken, string RefreshToken)> LoginWithTwoFactorAsync(string email, string password, string twoFactorCode, string ipAddress = null, string deviceInfo = null)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null || user.AccountStatus != UserStatus.Active)
                {
                    await _auditService.LogTwoFactorEventAsync(user?.Id ?? 0, "TwoFactorLogin", false, ipAddress);
                    return (null, null, null);
                }

                if (!await _userService.ValidatePasswordAsync(password, user.PasswordHash))
                {
                    await _auditService.LogTwoFactorEventAsync(user.Id, "TwoFactorLogin", false, ipAddress);
                    return (null, null, null);
                }

                if (!await _twoFactorService.VerifyTwoFactorCodeAsync(user.Id, twoFactorCode))
                {
                    await _auditService.LogTwoFactorEventAsync(user.Id, "TwoFactorLogin", false, ipAddress);
                    return (null, null, null);
                }

                await _auditService.LogTwoFactorEventAsync(user.Id, "TwoFactorLogin", true, ipAddress);
                return await CompleteLoginAsync(user, ipAddress, deviceInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during 2FA login for email: {Email}", email);
                return (null, null, null);
            }
        }

        private async Task<(User User, string AccessToken, string RefreshToken)> CompleteLoginAsync(User user, string ipAddress, string deviceInfo)
        {
            var userWithRoles = await _userService.GetUserWithRolesAndPermissionsAsync(user.Id);
            var roles = new List<string>();
            var permissions = new List<string>();

            foreach (var role in userWithRoles.Roles)
            {
                roles.Add(role.Name);
                foreach (var permission in role.Permissions)
                {
                    if (!permissions.Contains(permission.Name))
                        permissions.Add(permission.Name);
                }
            }

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user.Id, roles, permissions);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            // Store refresh token
            await _authTokenService.CreateTokenAsync(user.Id, refreshToken, TimeSpan.FromDays(30), ipAddress);

            // Create session
            await _sessionService.CreateSessionAsync(user.Id, deviceInfo, ipAddress, TimeSpan.FromHours(24));

            await _auditService.LogLoginAttemptAsync(user.Email, true, ipAddress, "Login successful");
            await _auditService.LogUserActionAsync(user.Id, "Login", $"Device: {deviceInfo}", ipAddress);

            return (user, accessToken, refreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken, string ipAddress = null)
        {
            try
            {
                var tokenEntity = await _authTokenService.GetByTokenAsync(refreshToken);
                if (tokenEntity == null || !await _authTokenService.ValidateTokenAsync(refreshToken))
                {
                    return (null, null);
                }

                var user = await _userService.GetUserWithRolesAndPermissionsAsync(tokenEntity.UserId);
                if (user == null || user.AccountStatus != UserStatus.Active)
                {
                    return (null, null);
                }

                // Revoke old refresh token
                await _authTokenService.RevokeTokenAsync(refreshToken, "Token refreshed");

                var roles = new List<string>();
                var permissions = new List<string>();

                foreach (var role in user.Roles)
                {
                    roles.Add(role.Name);
                    foreach (var permission in role.Permissions)
                    {
                        if (!permissions.Contains(permission.Name))
                            permissions.Add(permission.Name);
                    }
                }

                var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user.Id, roles, permissions);
                var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();

                await _authTokenService.CreateTokenAsync(user.Id, newRefreshToken, TimeSpan.FromDays(30), ipAddress);

                await _auditService.LogUserActionAsync(user.Id, "TokenRefresh", null, ipAddress);

                return (newAccessToken, newRefreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return (null, null);
            }
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            try
            {
                var tokenEntity = await _authTokenService.GetByTokenAsync(refreshToken);
                if (tokenEntity != null)
                {
                    await _authTokenService.RevokeTokenAsync(refreshToken, "User logout");
                    await _auditService.LogUserActionAsync(tokenEntity.UserId, "Logout", null);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        public async Task<bool> LogoutAllDevicesAsync(int userId)
        {
            try
            {
                await _authTokenService.RevokeUserTokensAsync(userId, "Logout all devices");
                await _sessionService.TerminateUserSessionsAsync(userId);
                await _auditService.LogUserActionAsync(userId, "LogoutAllDevices", null);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout all devices for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<User> RegisterAsync(string email, string password, string firstName, string lastName, string phoneNumber = null, string userType = "User")
        {
            try
            {
                var user = await _userService.CreateUserAsync(email, password, firstName, lastName, phoneNumber, userType);

                // Send email verification
                await SendEmailConfirmationAsync(email);

                await _auditService.LogUserActionAsync(user.Id, "Register", $"UserType: {userType}");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", email);
                return null;
            }
        }

        public async Task<bool> ConfirmEmailAsync(string email, string confirmationCode)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                    return false;

                // Convert int userId to Guid for verification service
                var userGuid = Guid.Parse(user.Id.ToString().PadLeft(32, '0'));

                if (!await _verificationService.ValidateVerificationCodeAsync(confirmationCode))
                    return false;

                if (!await _verificationService.UseVerificationCodeAsync(confirmationCode))
                    return false;

                await _userService.UpdateEmailConfirmationAsync(user.Id, true);
                await _auditService.LogUserActionAsync(user.Id, "EmailConfirmed", null);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email confirmation for: {Email}", email);
                return false;
            }
        }

        public async Task<bool> ConfirmPhoneAsync(string phoneNumber, string confirmationCode)
        {
            try
            {
                var user = await _userService.GetByPhoneNumberAsync(phoneNumber);
                if (user == null)
                    return false;

                if (!await _verificationService.ValidateVerificationCodeAsync(confirmationCode))
                    return false;

                if (!await _verificationService.UseVerificationCodeAsync(confirmationCode))
                    return false;

                await _userService.UpdatePhoneConfirmationAsync(user.Id, true);
                await _auditService.LogUserActionAsync(user.Id, "PhoneConfirmed", null);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during phone confirmation for: {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                    return true; // Don't reveal if email exists

                var userGuid = Guid.Parse(user.Id.ToString().PadLeft(32, '0'));
                var verification = await _verificationService.CreateVerificationAsync(userGuid, VerificationTypes.PasswordReset, TimeSpan.FromHours(1));

                await _notificationService.SendPasswordResetEmailAsync(email, verification.Code);
                await _auditService.LogUserActionAsync(user.Id, "PasswordResetRequested", null);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for: {Email}", email);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                    return false;

                if (!await _verificationService.ValidateVerificationCodeAsync(resetCode))
                    return false;

                if (!await _verificationService.UseVerificationCodeAsync(resetCode))
                    return false;

                await _userService.UpdateUserPasswordAsync(user.Id, newPassword);
                await _authTokenService.RevokeUserTokensAsync(user.Id, "Password reset");
                await _sessionService.TerminateUserSessionsAsync(user.Id);

                await _auditService.LogPasswordChangeAsync(user.Id);
                await _auditService.LogUserActionAsync(user.Id, "PasswordReset", null);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for: {Email}", email);
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                    return false;

                if (!await _userService.ValidatePasswordAsync(currentPassword, user.PasswordHash))
                    return false;

                await _userService.UpdateUserPasswordAsync(userId, newPassword);
                await _authTokenService.RevokeUserTokensAsync(userId, "Password changed");
                await _sessionService.TerminateUserSessionsAsync(userId);

                await _auditService.LogPasswordChangeAsync(userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> SendEmailConfirmationAsync(string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                    return false;

                var userGuid = Guid.Parse(user.Id.ToString().PadLeft(32, '0'));
                var verification = await _verificationService.CreateVerificationAsync(userGuid, VerificationTypes.EmailVerification);

                await _notificationService.SendEmailVerificationAsync(email, verification.Code);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email confirmation to: {Email}", email);
                return false;
            }
        }

        public async Task<bool> SendPhoneConfirmationAsync(string phoneNumber)
        {
            try
            {
                var user = await _userService.GetByPhoneNumberAsync(phoneNumber);
                if (user == null)
                    return false;

                var userGuid = Guid.Parse(user.Id.ToString().PadLeft(32, '0'));
                var verification = await _verificationService.CreateVerificationAsync(userGuid, VerificationTypes.PhoneVerification);

                await _notificationService.SendPhoneVerificationAsync(phoneNumber, verification.Code);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending phone confirmation to: {PhoneNumber}", phoneNumber);
                return false;
            }
        }
    }
}