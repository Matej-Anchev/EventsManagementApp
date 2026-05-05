using System.Text.Json.Serialization;

namespace Domain.WeatherApiResponse;

public class WeatherApiResponse
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("main")] public WeatherMain MainWeatherData { get; set; } = null!;

    [JsonPropertyName("wind")] public WeatherWind Wind { get; set; } = null!;
}

public abstract class WeatherMain
{
    [JsonPropertyName("temp")] public decimal Temperature { get; set; }

    [JsonPropertyName("feels_like")] public decimal FeelsLike { get; set; }

    [JsonPropertyName("temp_min")] public decimal MinimumTemperature { get; set; }

    [JsonPropertyName("temp_max")] public decimal MaximumTemperature { get; set; }
}

public abstract class WeatherWind
{
    [JsonPropertyName("speed")] public decimal Speed { get; set; }

    [JsonPropertyName("deg")] public int Degrees { get; set; }
}