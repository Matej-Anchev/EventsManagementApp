using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class EventService : IEventService
{
    private readonly IRepository<Event> _repository;
    private readonly IRepository<EventSectionPricing> _sectionPricingRepository;
    private readonly IRepository<Section> _sectionRepository;
    private readonly IRepository<Venue> _venueRepository;

    public EventService(IRepository<Event> repository, IRepository<EventSectionPricing> sectionPricingRepository,
        IRepository<Section> sectionRepository, IRepository<Venue> venueRepository)
    {
        _repository = repository;
        _sectionPricingRepository = sectionPricingRepository;
        _sectionRepository = sectionRepository;
        _venueRepository = venueRepository;
    }

    public async Task<List<Event>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(selector: x => x);
        return result.ToList();
    }

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(selector: x => x, predicate: x => x.Id == id,
            include: x => x.Include(y => y.Venue));
    }

    public async Task<Event> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Event with id {id} not found.");
        return result;
    }

    public async Task<Event> InsertAsync(EventDto dto)
    {
        var eventToAdd = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            ImageUrl = dto.ImageUrl,
            VenueId = dto.VenueId,
            UserId = dto.UserId,
        };

        return await _repository.InsertAsync(eventToAdd);
    }

    public async Task<Event> UpdateAsync(Guid id, EventDto dto)
    {
        var eventToUpdate = await GetByIdNotNullAsync(id);

        eventToUpdate.Title = dto.Title;
        eventToUpdate.Description = dto.Description;
        eventToUpdate.StartDate = dto.StartDate;
        eventToUpdate.EndDate = dto.EndDate;
        eventToUpdate.Status = dto.Status;
        eventToUpdate.ImageUrl = dto.ImageUrl;
        eventToUpdate.VenueId = dto.VenueId;
        eventToUpdate.UserId = dto.UserId;
        return await _repository.UpdateAsync(eventToUpdate);
    }

    public async Task<Event> DeleteAsync(Guid id)
    {
        var eventToDelete = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(eventToDelete);
    }

    public async Task<PaginatedResult<Event>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x => x.Include(y => y.Venue),
            orderBy: x => x.OrderBy(e => e.StartDate),
            asNoTracking: true);
    }

    public async Task<List<Event>> GetAllEventsAsyncWithEventPricingWithoutInclude()
    {
        var events = await GetAllAsync();

        if (!events.Any())
            return events;

        var eventIds = events.Select(e => e.Id).ToList();

        var allSectionPricings = await _sectionPricingRepository.GetAllAsync(
            selector: x => x,
            predicate: x => eventIds.Contains(x.EventId)
        );

        var sectionPricingsList = allSectionPricings.ToList();

        var sectionIds = sectionPricingsList.Select(sp => sp.SectionId).Distinct().ToList();

        var allSections = await _sectionRepository.GetAllAsync(
            selector: x => x,
            predicate: x => sectionIds.Contains(x.Id)
        );

        var sectionsList = allSections.ToList();

        var venueIds = sectionsList.Select(s => s.VenueId).Distinct().ToList();

        var allVenues = await _venueRepository.GetAllAsync(
            selector: x => x,
            predicate: x => venueIds.Contains(x.Id)
        );

        var venueDict = allVenues.ToDictionary(v => v.Id);

        var sectionDict = sectionsList.ToDictionary(s => s.Id);

        foreach (var section in sectionsList)
        {
            if (venueDict.TryGetValue(section.VenueId, out var venue))
                section.Venue = venue;
        }

        var pricingsByEvent = sectionPricingsList
            .GroupBy(sp => sp.EventId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var pricing in sectionPricingsList)
        {
            if (sectionDict.TryGetValue(pricing.SectionId, out var section))
                pricing.Section = section;
        }

        foreach (var ev in events)
        {
            if (pricingsByEvent.TryGetValue(ev.Id, out var pricings))
                ev.SectionPricings = pricings;
        }

        return events;
    }

    public async Task<List<Event>> GetAllEventsAsyncWithEventPricingUsingInclude()
    {
        var events = await _repository.GetAllAsync(selector: x => x,
            include: x => x.Include(y => y.SectionPricings)
                .ThenInclude(y => y.Section)
                .ThenInclude(y => y.Venue));

        return events.ToList();
    }

    public async Task<Event> UploadImageById(Guid eventId, string fileName, string contentType, int size, byte[] data)
    {
        var eventToUpdate = await GetByIdNotNullAsync(eventId);
        var image = new EventsImages
        {
            FileName = fileName,
            ContentType = contentType,
            Data = data,
            Size = size
        };

        eventToUpdate.EventImage = image;
        return await _repository.UpdateAsync(eventToUpdate);
    }

    public async Task<Event> UpdateImagePathByIdAsync(Guid eventId, string path)
    {
        var eventToUpdate = await GetByIdNotNullAsync(eventId);

        eventToUpdate.ImageUrl = path;

        return await _repository.UpdateAsync(eventToUpdate);
    }
}