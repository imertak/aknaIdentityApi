using aknaIdentity_api.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace aknaIdentity_api.Infrastructure.Extensions
{
    public static class BackgroundServiceExtensions
    {
        public static IServiceCollection AddCleanupBackgroundService(this IServiceCollection services)
        {
            services.AddHostedService<CleanupBackgroundService>();
            return services;
        }
    }
}
