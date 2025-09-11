using aknaIdentities_api.Domain.Entities;
using System.Threading.Tasks;

namespace aknaIdentity_api.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(User User, string AccessToken, string RefreshToken)> LoginAsync(string email, string password, string ipAddress = null, string deviceInfo = null);
        Task<(User User, string AccessToken, string RefreshToken)> LoginWithTwoFactorAsync(string email, string password, string twoFactorCode, string ipAddress = null, string deviceInfo = null);
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken, string ipAddress = null);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> LogoutAllDevicesAsync(int userId);
        Task<User> RegisterAsync(string email, string password, string firstName, string lastName, string phoneNumber = null, string userType = "User");
        Task<bool> ConfirmEmailAsync(string email, string confirmationCode);
        Task<bool> ConfirmPhoneAsync(string phoneNumber, string confirmationCode);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> SendEmailConfirmationAsync(string email);
        Task<bool> SendPhoneConfirmationAsync(string phoneNumber);
    }
}