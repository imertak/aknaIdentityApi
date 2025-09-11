using aknaIdentity_api.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace aknaIdentity_api.Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly ILogger<AuditService> _logger;

        public AuditService(ILogger<AuditService> logger)
        {
            _logger = logger;
        }

        public async Task LogUserActionAsync(int userId, string action, string details = null, string ipAddress = null)
        {
            try
            {
                var logMessage = $"User Action - UserId: {userId}, Action: {action}";

                if (!string.IsNullOrEmpty(details))
                    logMessage += $", Details: {details}";

                if (!string.IsNullOrEmpty(ipAddress))
                    logMessage += $", IP: {ipAddress}";

                logMessage += $", Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

                _logger.LogInformation(logMessage);

                // Burada veritabanına audit log kaydı yapılabilir
                // Örnek: await _auditRepository.AddAuditLogAsync(new AuditLog { ... });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging user action for UserId: {UserId}, Action: {Action}", userId, action);
            }
        }

        public async Task LogSecurityEventAsync(int? userId, string eventType, string description, string ipAddress = null)
        {
            try
            {
                var logMessage = $"Security Event - Type: {eventType}, Description: {description}";

                if (userId.HasValue)
                    logMessage += $", UserId: {userId.Value}";

                if (!string.IsNullOrEmpty(ipAddress))
                    logMessage += $", IP: {ipAddress}";

                logMessage += $", Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

                _logger.LogWarning(logMessage);

                // Critical security events can be sent to security monitoring systems
                if (IsCriticalSecurityEvent(eventType))
                {
                    _logger.LogCritical("CRITICAL SECURITY EVENT: {LogMessage}", logMessage);
                    // Send to security monitoring system
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging security event: {EventType}", eventType);
            }
        }

        public async Task LogLoginAttemptAsync(string email, bool isSuccessful, string ipAddress = null, string failureReason = null)
        {
            try
            {
                var logMessage = $"Login Attempt - Email: {email}, Success: {isSuccessful}";

                if (!string.IsNullOrEmpty(ipAddress))
                    logMessage += $", IP: {ipAddress}";

                if (!isSuccessful && !string.IsNullOrEmpty(failureReason))
                    logMessage += $", Reason: {failureReason}";

                logMessage += $", Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

                if (isSuccessful)
                    _logger.LogInformation(logMessage);
                else
                    _logger.LogWarning(logMessage);

                // Track failed login attempts for security monitoring
                if (!isSuccessful)
                {
                    await LogSecurityEventAsync(null, "FailedLogin", $"Failed login attempt for {email}: {failureReason}", ipAddress);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging login attempt for email: {Email}", email);
            }
        }

        public async Task LogPasswordChangeAsync(int userId, string ipAddress = null)
        {
            try
            {
                await LogUserActionAsync(userId, "PasswordChange", "User password changed", ipAddress);
                await LogSecurityEventAsync(userId, "PasswordChange", "User password was changed", ipAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging password change for UserId: {UserId}", userId);
            }
        }

        public async Task LogTwoFactorEventAsync(int userId, string eventType, bool isSuccessful, string ipAddress = null)
        {
            try
            {
                var logMessage = $"2FA Event - UserId: {userId}, Type: {eventType}, Success: {isSuccessful}";

                if (!string.IsNullOrEmpty(ipAddress))
                    logMessage += $", IP: {ipAddress}";

                logMessage += $", Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

                if (isSuccessful)
                    _logger.LogInformation(logMessage);
                else
                    _logger.LogWarning(logMessage);

                await LogUserActionAsync(userId, $"TwoFactor_{eventType}", $"Success: {isSuccessful}", ipAddress);

                if (!isSuccessful)
                {
                    await LogSecurityEventAsync(userId, "Failed2FA", $"Failed 2FA attempt: {eventType}", ipAddress);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging 2FA event for UserId: {UserId}, EventType: {EventType}", userId, eventType);
            }
        }

        private bool IsCriticalSecurityEvent(string eventType)
        {
            var criticalEvents = new[]
            {
                "MultipleFailedLogins",
                "SuspiciousLoginPattern",
                "AccountLockout",
                "PrivilegeEscalation",
                "UnauthorizedAccess",
                "DataBreach",
                "SystemCompromise"
            };

            return Array.Exists(criticalEvents, e => e.Equals(eventType, StringComparison.OrdinalIgnoreCase));
        }
    }
}