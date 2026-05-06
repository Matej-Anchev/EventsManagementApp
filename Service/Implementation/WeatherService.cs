using Domain.Configuration;
using Domain.Dto;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Interface;

namespace Service.Implementation;

public class WeatherService : IWeatherService
{
    private readonly IEventService _eventService;
    private readonly IWeatherApiClient _weatherApiClient;
    private readonly ILogger<WeatherService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly WeatherApiSettings _weatherApiSettings;

    public WeatherService(IEventService eventService, IWeatherApiClient weatherApiClient,
        ILogger<WeatherService> logger, IMemoryCache memoryCache, IOptions<WeatherApiSettings> weatherApiSettings)
    {
        _eventService = eventService;
        _weatherApiClient = weatherApiClient;
        _logger = logger;
        _memoryCache = memoryCache;
        _weatherApiSettings = weatherApiSettings.Value;
    }

    public async Task<EventWeatherDto> GetWeatherDataForEventIdAsync(Guid eventId)
    {
        var eventData = await _eventService.GetByIdNotNullAsync(eventId);

        var city = eventData.Venue.City;
        var country = eventData.Venue.Country;

        var cacheKey = $"weather-api:{city}:{country}";

        if (_memoryCache.TryGetValue(cacheKey, out EventWeatherDto? cached))
        {
            _logger.LogDebug(
                "Cache hit for event {EventId}", eventId);
            return cached!;
        }

        if (cached != null)
            return cached;

        var apiData = await _weatherApiClient.GetWeatherForecastForCityAndCountry(city, country);
        _memoryCache.Set(cacheKey, apiData, TimeSpan.FromMinutes(_weatherApiSettings.CacheExpirationMinutes));

        _logger.LogInformation("Weather cached for event {EventId}: " + "{Condition}, {Temp}°C",
            eventId, apiData.Condition, apiData.Temperature);

        return apiData;
    }
}