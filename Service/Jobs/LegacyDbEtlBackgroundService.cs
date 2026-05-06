using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Implementation;

namespace Service.Jobs;

public class LegacyDbEtlBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<LegacyDbEtlBackgroundService> _logger;

    public LegacyDbEtlBackgroundService(IServiceScopeFactory serviceScopeFactory,
        ILogger<LegacyDbEtlBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<VenueEtlService>();

            try
            {
                _logger.LogInformation("Starting legacy DB ETL job");

                await service.SyncAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during legacy DB ETL job");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}