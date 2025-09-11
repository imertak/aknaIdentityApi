using aknaIdentity_api.Domain.Interfaces.Contracts;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using aknaIdentity_api.Domain.Interfaces.Services;
using aknaIdentity_api.Infrastructure.Contracts;
using aknaIdentity_api.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace aknaIdentity_api.Infrastructure.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            // Repository registrations
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IAuthenticationTokenRepository, AuthenticationTokenRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IDeviceInfoRepository, DeviceInfoRepository>();
            services.AddScoped<ITwoFactorAuthSettingRepository, TwoFactorAuthSettingRepository>();
            services.AddScoped<IVerificationRepository, VerificationRepository>();

            // Service registrations
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IAuthenticationTokenService, AuthenticationTokenService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IDeviceInfoService, DeviceInfoService>();
            services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
            services.AddScoped<IVerificationService, VerificationService>();

            return services;
        }
    }
}