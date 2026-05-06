using Domain.Dto;
using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class EventsExtensions
{
    public static EventResponse? ToResponse(this Event e, EventWeatherDto? weatherDto = null)
    {
        return new EventResponse(
            e.Title,
            e.Description,
            e.ImageUrl,
            e.StartDate,
            e.EndDate,
            e.Venue?.Name,
            e.Venue?.City,
            e.Venue?.Country,
            weatherDto
        );
    }

    public static List<EventResponse> ToResponse(this List<Event> events)
    {
        return events.Select(x => x.ToResponse()).ToList();
    }

    public static EventDto ToDto(this EventRequest request)
    {
        return new EventDto
        {
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            VenueId = request.VenueId,
            UserId = request.UserId
        };
    }
}