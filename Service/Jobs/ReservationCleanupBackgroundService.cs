using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace Service.Jobs;

public class ReservationCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ReservationCleanupBackgroundService> _logger;

    public ReservationCleanupBackgroundService(IServiceScopeFactory serviceScopeFactory,
        ILogger<ReservationCleanupBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                _logger.LogInformation("Reservation cleanup job started...");

                var reservations = await reservationService.GetAllByDateReservedSince(DateTime.Now.AddMinutes(-15));

                _logger.LogInformation($"Fetched total {reservations.Count} reservations");

                foreach (var reservation in reservations)
                {
                    try
                    {
                        _logger.LogInformation($"Expiring reservation with ID: {reservation.Id}");
                        await reservationService.ExpireAsync(reservation);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while expiring reservation with ID: {reservation.Id}");
                    }
                }

                _logger.LogInformation("Reservation cleanup job finished successfully...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during reservation cleanup job");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}