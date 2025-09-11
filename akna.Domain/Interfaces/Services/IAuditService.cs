using System;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface IAuditService
    {
        Task LogUserActionAsync(int userId, string action, string details = null, string ipAddress = null);
        Task LogSecurityEventAsync(int? userId, string eventType, string description, string ipAddress = null);
        Task LogLoginAttemptAsync(string email, bool isSuccessful, string ipAddress = null, string failureReason = null);
        Task LogPasswordChangeAsync(int userId, string ipAddress = null);
        Task LogTwoFactorEventAsync(int userId, string eventType, bool isSuccessful, string ipAddress = null);
    }
}