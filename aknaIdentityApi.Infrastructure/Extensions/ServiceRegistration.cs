using aknaIdentity_api.Application.Services;
using aknaIdentity_api.Business.Services;
using aknaIdentity_api.Domain.Interfaces.Contracts;
using aknaIdentity_api.Domain.Interfaces.Repositories;
using aknaIdentity_api.Domain.Interfaces.Services;
using aknaIdentity_api.Infrastructure.Contracts;
using aknaIdentity_api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthenticationTokenService, AuthenticationTokenService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IDeviceInfoService, DeviceInfoService>();
            services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
            services.AddScoped<IVerificationService, VerificationService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAuditService, AuditService>();

            // HttpClient for NotificationService
            services.AddHttpClient<INotificationService, NotificationService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSecret = configuration["JWT:Secret"];
            var jwtIssuer = configuration["JWT:Issuer"];
            var jwtAudience = configuration["JWT:Audience"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}