using Domain.Dto;

namespace Service.Interface;

public interface IWeatherService
{
    Task<EventWeatherDto> GetWeatherDataForEventIdAsync(Guid eventId);
}