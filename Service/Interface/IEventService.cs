using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IEventService
{
    Task<List<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(Guid id);
    Task<Event> GetByIdNotNullAsync(Guid id);
    Task<Event> InsertAsync(EventDto dto);
    Task<Event> UpdateAsync(Guid id, EventDto dto);
    Task<Event> DeleteAsync(Guid id);

    public Task<PaginatedResult<Event>> GetAllPagedAsync(int pageNumber, int pageSize);

    Task<List<Event>> GetAllEventsAsyncWithEventPricingWithoutInclude();
    Task<List<Event>> GetAllEventsAsyncWithEventPricingUsingInclude();
    public Task<Event> UploadImageById(Guid eventId, string fileName, string contentType, int size, byte[] data);
    public Task<Event> UpdateImagePathByIdAsync(Guid eventId, string path);
}