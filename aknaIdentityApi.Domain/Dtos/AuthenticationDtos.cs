namespace aknaIdentity_api.Domain.Dtos
{
    // Authentication DTOs (already existing, but adding missing ones)
    public class LoginTwoFactorRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string TwoFactorCode { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string ResetCode { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ConfirmEmailRequest
    {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class ConfirmPhoneRequest
    {
        public string PhoneNumber { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class ResendEmailConfirmationRequest
    {
        public string Email { get; set; }
    }

    public class ResendPhoneConfirmationRequest
    {
        public string PhoneNumber { get; set; }
    }

    // User Management DTOs
    public class UserDetailDto : UserDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateUserStatusRequest
    {
        public string Status { get; set; }
    }

    public class AssignRolesRequest
    {
        public List<int> RoleIds { get; set; }
    }

    // Role & Permission DTOs
    public class CreateRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AssignPermissionsRequest
    {
        public List<int> PermissionIds { get; set; }
    }

    public class CreatePermissionRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdatePermissionRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CheckPermissionRequest
    {
        public string PermissionName { get; set; }
    }

    // Two-Factor Authentication DTOs
    public class TwoFactorStatusResponse
    {
        public bool IsEnabled { get; set; }
        public string Method { get; set; }
        public bool IsConfigured { get; set; }
    }

    public class EnableTwoFactorRequest
    {
        public string Method { get; set; } // SMS, Email, Authenticator
        public string VerificationCode { get; set; } // For verifying current identity
    }

    public class EnableTwoFactorResponse
    {
        public bool IsEnabled { get; set; }
        public string Method { get; set; }
        public string SecretKey { get; set; } // For authenticator apps
        public string QrCodeUrl { get; set; } // QR code for authenticator setup
    }

    public class DisableTwoFactorRequest
    {
        public string VerificationCode { get; set; }
    }

    public class VerifyTwoFactorRequest
    {
        public string Code { get; set; }
    }

    // Session DTOs
    public class SessionDto
    {
        public int Id { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string DeviceInfo { get; set; }
        public string IpAddress { get; set; }
        public bool IsActive { get; set; }
    }

    // Device DTOs
    public class DeviceDto
    {
        public int Id { get; set; }
        public string DeviceIdentifier { get; set; }
        public string DeviceModel { get; set; }
        public string OsType { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
    }

    public class RegisterDeviceRequest
    {
        public string DeviceIdentifier { get; set; }
        public string PushToken { get; set; }
        public string OsType { get; set; }
        public string DeviceModel { get; set; }
    }

    public class UpdateDeviceRequest
    {
        public string PushToken { get; set; }
        public string OsType { get; set; }
        public string DeviceModel { get; set; }
    }

    public class UpdatePushTokenRequest
    {
        public string DeviceIdentifier { get; set; }
        public string PushToken { get; set; }
    }

    // Common DTOs
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
    }

    public class BulkOperationRequest
    {
        public List<int> Ids { get; set; }
    }

    public class BulkOperationResponse
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; }
    }

    // Audit DTOs
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class SecurityEventDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public string Severity { get; set; }
    }

    // Notification DTOs
    public class SendNotificationRequest
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; } // Email, SMS, Push
        public Dictionary<string, object> Data { get; set; }
    }

    public class NotificationResponse
    {
        public bool Sent { get; set; }
        public string Method { get; set; }
        public DateTime SentAt { get; set; }
        public string Error { get; set; }
    }

    // Verification DTOs
    public class VerificationDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RequestVerificationRequest
    {
        public string Type { get; set; } // EmailVerification, PhoneVerification, PasswordReset
        public string Destination { get; set; } // Email or Phone number
    }

    public class UseVerificationRequest
    {
        public string Code { get; set; }
        public string Type { get; set; }
    }

    // Statistics DTOs
    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int PendingUsers { get; set; }
        public int SuspendedUsers { get; set; }
        public Dictionary<string, int> UsersByType { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; }
    }

    public class LoginStatisticsDto
    {
        public int TotalLogins { get; set; }
        public int UniqueUsers { get; set; }
        public int FailedAttempts { get; set; }
        public List<HourlyLoginCount> HourlyBreakdown { get; set; }
    }

    public class HourlyLoginCount
    {
        public int Hour { get; set; }
        public int Count { get; set; }
    }

    // Bulk User Operations
    public class BulkUserStatusUpdateRequest
    {
        public List<int> UserIds { get; set; }
        public string NewStatus { get; set; }
    }

    public class BulkRoleAssignmentRequest
    {
        public List<int> UserIds { get; set; }
        public List<int> RoleIds { get; set; }
        public bool ReplaceExisting { get; set; }
    }

    // Import/Export DTOs
    public class UserImportRequest
    {
        public List<UserImportDto> Users { get; set; }
        public bool SendWelcomeEmail { get; set; }
        public bool RequireEmailVerification { get; set; }
    }

    public class UserImportDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserType { get; set; }
        public List<string> Roles { get; set; }
    }

    public class UserExportRequest
    {
        public List<int> UserIds { get; set; }
        public List<string> Fields { get; set; }
        public string Format { get; set; } // CSV, JSON, Excel
    }

    public class UserDto
    {
        public int Id { get; set; } // int olarak düzeltildi
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountStatus { get; set; }
        public string UserType { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
        public List<RoleDto> Roles { get; set; }
    }

    public class RoleDto
    {
        public int Id { get; set; } // int olarak düzeltildi
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionDto> Permissions { get; set; }
    }



    public class PermissionDto
    {
        public int Id { get; set; } // int olarak düzeltildi
        public string Name { get; set; }
        public string Description { get; set; }
    }



    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; }
        public bool RequiresTwoFactor { get; set; } // 2FA gerekirse
    }

    public class UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountStatus { get; set; }
        public string UserType { get; set; } // Eklendi
    }

    public class RegisterRequest // RegisterUserRequest yerine RegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; } // Eklendi
        public string UserType { get; set; } = "User"; // Varsayılan değer
        public bool AcceptTerms { get; set; } // Eklendi
    }
}