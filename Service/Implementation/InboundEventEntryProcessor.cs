using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.Dto;
using Domain.Dto.Enums;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class InboundEventEntryProcessor
{
    private readonly IRepository<InboundEventEntry> _repository;
    private readonly IVenueService _venueService;

    public InboundEventEntryProcessor(IRepository<InboundEventEntry> repository, IVenueService venueService)
    {
        _repository = repository;
        _venueService = venueService;
    }

    public async Task ProcessEventEntry(InboundEventEntry entry)
    {
        var request = JsonSerializer.Deserialize<InboundEventRequest>(entry.RawPayload);

        var venue = await _venueService.GetByNameAndCityAsync(request.VenueName, request.VenueCity);
        if (venue == null)
        {
            throw new InvalidOperationException($"Venue with name {request.VenueName} does not exist");
        }

        var eventCreate = new Event()
        {
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = EventStatus.Draft,
            VenueId = venue.Id,
        };

        foreach (var pricing in request.Pricing)
        {
            var section = venue.Sections.FirstOrDefault(x => x.Name == pricing.SectionName);

            if (section == null)
                throw new InvalidOperationException($"Section with name {pricing.SectionName} does not exist");

            var eventSectionPricing = new EventSectionPricing()
            {
                EventId = eventCreate.Id,
                Currency = pricing.Currency,
                Price = pricing.Price,
                SectionId = section.Id
            };
        }
    }
}