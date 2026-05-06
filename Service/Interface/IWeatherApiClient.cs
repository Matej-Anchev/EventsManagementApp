using Domain.Dto;

namespace Service.Interface;

public interface IWeatherApiClient
{
    Task<EventWeatherDto> GetWeatherForecastForCityAndCountry(string city, string country);
}