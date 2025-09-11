using aknaIdentity_api.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace aknaIdentity_api.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IDeviceInfoService _deviceInfoService;

        public NotificationService(
            IConfiguration configuration,
            ILogger<NotificationService> logger,
            HttpClient httpClient,
            IDeviceInfoService deviceInfoService)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            _deviceInfoService = deviceInfoService;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["Email:Username"];
                var smtpPassword = _configuration["Email:Password"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"];

                using var client = new SmtpClient(smtpHost, smtpPort);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail, fromName);
                message.To.Add(to);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to: {Email}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to: {Email}", to);
                return false;
            }
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                // SMS service implementation (Twilio, AWS SNS, etc.)
                var smsApiUrl = _configuration["SMS:ApiUrl"];
                var smsApiKey = _configuration["SMS:ApiKey"];

                if (string.IsNullOrEmpty(smsApiUrl) || string.IsNullOrEmpty(smsApiKey))
                {
                    _logger.LogWarning("SMS configuration is missing");
                    return false;
                }

                var payload = new
                {
                    to = phoneNumber,
                    message = message,
                    apiKey = smsApiKey
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(smsApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("SMS sent successfully to: {PhoneNumber}", phoneNumber);
                    return true;
                }

                _logger.LogWarning("Failed to send SMS to: {PhoneNumber}, Status: {StatusCode}", phoneNumber, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to: {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        public async Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, object> data = null)
        {
            try
            {
                // Firebase Cloud Messaging implementation
                var fcmApiUrl = "https://fcm.googleapis.com/fcm/send";
                var fcmServerKey = _configuration["FCM:ServerKey"];

                if (string.IsNullOrEmpty(fcmServerKey))
                {
                    _logger.LogWarning("FCM configuration is missing");
                    return false;
                }

                var payload = new
                {
                    to = deviceToken,
                    notification = new
                    {
                        title = title,
                        body = body
                    },
                    data = data
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"key={fcmServerKey}");

                var response = await _httpClient.PostAsync(fcmApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Push notification sent successfully to device: {DeviceToken}", deviceToken);
                    return true;
                }

                _logger.LogWarning("Failed to send push notification to device: {DeviceToken}, Status: {StatusCode}", deviceToken, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send push notification to device: {DeviceToken}", deviceToken);
                return false;
            }
        }

        public async Task<bool> SendPushNotificationToUserAsync(System.Guid userId, string title, string body, Dictionary<string, object> data = null)
        {
            try
            {
                var devices = await _deviceInfoService.GetDevicesForNotificationAsync(userId);
                var success = true;

                foreach (var device in devices)
                {
                    if (!string.IsNullOrEmpty(device.PushNotificationToken))
                    {
                        var result = await SendPushNotificationAsync(device.PushNotificationToken, title, body, data);
                        if (!result)
                            success = false;
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send push notification to user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> SendEmailVerificationAsync(string email, string verificationCode)
        {
            var subject = "Email Doğrulama - AKNA";
            var body = $@"
                <h2>Email Adresinizi Doğrulayın</h2>
                <p>Merhaba,</p>
                <p>AKNA hesabınızı oluşturduğunuz için teşekkür ederiz. Email adresinizi doğrulamak için aşağıdaki kodu kullanın:</p>
                <h3 style='color: #007bff; font-family: monospace;'>{verificationCode}</h3>
                <p>Bu kod 15 dakika içinde geçerliliğini yitirecektir.</p>
                <p>Bu işlemi siz yapmadıysanız, bu emaili görmezden gelebilirsiniz.</p>
                <p>Saygılarımızla,<br>AKNA Ekibi</p>
            ";

            return await SendEmailAsync(email, subject, body, true);
        }

        public async Task<bool> SendPhoneVerificationAsync(string phoneNumber, string verificationCode)
        {
            var message = $"AKNA doğrulama kodunuz: {verificationCode}. Bu kod 15 dakika içinde geçerliliğini yitirecektir.";
            return await SendSmsAsync(phoneNumber, message);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetCode)
        {
            var subject = "Şifre Sıfırlama - AKNA";
            var body = $@"
                <h2>Şifre Sıfırlama Talebi</h2>
                <p>Merhaba,</p>
                <p>AKNA hesabınız için şifre sıfırlama talebinde bulundunuz. Yeni şifre oluşturmak için aşağıdaki kodu kullanın:</p>
                <h3 style='color: #dc3545; font-family: monospace;'>{resetCode}</h3>
                <p>Bu kod 1 saat içinde geçerliliğini yitirecektir.</p>
                <p>Bu işlemi siz yapmadıysanız, lütfen derhal bizimle iletişime geçin.</p>
                <p>Saygılarımızla,<br>AKNA Ekibi</p>
            ";

            return await SendEmailAsync(email, subject, body, true);
        }

        public async Task<bool> SendTwoFactorCodeAsync(string destination, string code, string method)
        {
            switch (method.ToLower())
            {
                case "sms":
                    var smsMessage = $"AKNA güvenlik kodunuz: {code}";
                    return await SendSmsAsync(destination, smsMessage);

                case "email":
                    var subject = "Güvenlik Kodu - AKNA";
                    var body = $@"
                        <h2>İki Faktörlü Kimlik Doğrulama</h2>
                        <p>Merhaba,</p>
                        <p>AKNA hesabınıza giriş yapmak için güvenlik kodunuz:</p>
                        <h3 style='color: #28a745; font-family: monospace;'>{code}</h3>
                        <p>Bu kod 5 dakika içinde geçerliliğini yitirecektir.</p>
                        <p>Saygılarımızla,<br>AKNA Ekibi</p>
                    ";
                    return await SendEmailAsync(destination, subject, body, true);

                default:
                    _logger.LogWarning("Unknown two-factor method: {Method}", method);
                    return false;
            }
        }
    }
}