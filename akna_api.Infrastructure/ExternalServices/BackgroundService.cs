using aknaIdentity_api.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aknaIdentity_api.Infrastructure.Services
{
    public class CleanupBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CleanupBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(6); // Her 6 saatte bir çalışsın

        public CleanupBackgroundService(IServiceProvider serviceProvider, ILogger<CleanupBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var tokenService = scope.ServiceProvider.GetRequiredService<IAuthenticationTokenService>();
                        var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                        var verificationService = scope.ServiceProvider.GetRequiredService<IVerificationService>();

                        _logger.LogInformation("Starting cleanup process...");

                        // Expired token'ları temizle
                        await tokenService.CleanupExpiredTokensAsync();
                        _logger.LogInformation("Expired tokens cleaned up.");

                        // Old revoked token'ları temizle (30 gün öncesi)
                        await tokenService.CleanupOldRevokedTokensAsync(30);
                        _logger.LogInformation("Old revoked tokens cleaned up.");

                        // Expired session'ları temizle
                        await sessionService.CleanupExpiredSessionsAsync();
                        _logger.LogInformation("Expired sessions cleaned up.");

                        // Expired verification'ları temizle
                        await verificationService.CleanupExpiredVerificationsAsync();
                        _logger.LogInformation("Expired verifications cleaned up.");

                        _logger.LogInformation("Cleanup process completed successfully.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during cleanup process.");
                }

                await Task.Delay(_period, stoppingToken);
            }
        }
    }
}