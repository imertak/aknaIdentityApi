using System.Collections.Generic;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface INotificationService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task<bool> SendSmsAsync(string phoneNumber, string message);
        Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, object> data = null);
        Task<bool> SendPushNotificationToUserAsync(System.Guid userId, string title, string body, Dictionary<string, object> data = null);
        Task<bool> SendEmailVerificationAsync(string email, string verificationCode);
        Task<bool> SendPhoneVerificationAsync(string phoneNumber, string verificationCode);
        Task<bool> SendPasswordResetEmailAsync(string email, string resetCode);
        Task<bool> SendTwoFactorCodeAsync(string destination, string code, string method);
    }
}
