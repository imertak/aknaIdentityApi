namespace aknaIdentity_api.Domain.Constants
{
    public static class UserStatus
    {
        public const string Pending = "Pending";
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Suspended = "Suspended";
        public const string Deleted = "Deleted";
    }

    public static class UserTypes
    {
        public const string Admin = "Admin";
        public const string Shipper = "Shipper";
        public const string Carrier = "Carrier";
        public const string Driver = "Driver";
        public const string User = "User";
    }

    public static class VerificationTypes
    {
        public const string EmailVerification = "EmailVerification";
        public const string PhoneVerification = "PhoneVerification";
        public const string PasswordReset = "PasswordReset";
        public const string TwoFactorAuth = "TwoFactorAuth";
    }

    public static class TwoFactorMethods
    {
        public const string SMS = "SMS";
        public const string Email = "Email";
        public const string Authenticator = "Authenticator";
    }

    public static class TokenTypes
    {
        public const string AccessToken = "AccessToken";
        public const string RefreshToken = "RefreshToken";
        public const string ResetPasswordToken = "ResetPasswordToken";
        public const string EmailConfirmationToken = "EmailConfirmationToken";
    }

    public static class Permissions
    {
        // User Management
        public const string CanCreateUser = "can_create_user";
        public const string CanReadUser = "can_read_user";
        public const string CanUpdateUser = "can_update_user";
        public const string CanDeleteUser = "can_delete_user";

        // Role Management
        public const string CanCreateRole = "can_create_role";
        public const string CanReadRole = "can_read_role";
        public const string CanUpdateRole = "can_update_role";
        public const string CanDeleteRole = "can_delete_role";

        // Permission Management
        public const string CanCreatePermission = "can_create_permission";
        public const string CanReadPermission = "can_read_permission";
        public const string CanUpdatePermission = "can_update_permission";
        public const string CanDeletePermission = "can_delete_permission";

        // System Administration
        public const string CanAccessAdminPanel = "can_access_admin_panel";
        public const string CanViewSystemLogs = "can_view_system_logs";
        public const string CanManageSystemSettings = "can_manage_system_settings";
    }

    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Shipper = "Shipper";
        public const string Carrier = "Carrier";
        public const string Driver = "Driver";
        public const string User = "User";
    }
}