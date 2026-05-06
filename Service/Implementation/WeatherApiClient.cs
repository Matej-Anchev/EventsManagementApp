using System.Net.Http.Json;
using Domain.Configuration;
using Domain.Dto;
using Domain.WeatherApiResponse;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace Service.Implementation;

public class WeatherApiClient : IWeatherApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherApiClient> _logger;
    private readonly WeatherApiSettings _settings;

    public WeatherApiClient(HttpClient httpClient, ILogger<WeatherApiClient> logger, WeatherApiSettings settings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _settings = settings;
    }

    public async Task<EventWeatherDto> GetWeatherForecastForCityAndCountry(string city, string country)
    {
        var apiKey = _settings.ApiKey;
        var url = $"weather?q={city},{country}&appid={apiKey}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var weatherData = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();

        return new EventWeatherDto()
        {
            Temperature = (double)weatherData!.MainWeatherData.Temperature,
            FeelsLike = (double)weatherData!.MainWeatherData.FeelsLike,
            TempMax = (double)weatherData!.MainWeatherData.MaximumTemperature,
            TempMin = (double)weatherData!.MainWeatherData.MinimumTemperature,
            Humidity = 1,
            WindSpeed = (double)weatherData.Wind.Speed,
            Condition = "",
            Description = "",
            Icon = "",
            RetrievedAt = DateTime.UtcNow
        };
    }
}