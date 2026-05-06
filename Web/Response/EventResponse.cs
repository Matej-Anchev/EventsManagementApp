using Domain.Dto;

namespace Web.Response;

public record EventResponse(
    string Name,
    string? Description,
    string? ImageUrl,
    DateTime StartDate,
    DateTime EndDate,
    string? VenueName,
    string? VenueCity,
    string? VenueCountry,
    EventWeatherDto? WeatherData
);