using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class EventMapper
{
    private readonly IEventService _eventService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IWeatherService _weatherService;

    public EventMapper(IEventService eventService, IFileUploadService fileUploadService, IWeatherService weatherService)
    {
        _eventService = eventService;
        _fileUploadService = fileUploadService;
        _weatherService = weatherService;
    }


    public async Task<List<EventResponse>> GetAll()
    {
        var result = await _eventService.GetAllEventsAsyncWithEventPricingUsingInclude();
        return result.ToResponse();
    }

    public async Task<EventResponse?> GetById(Guid id)
    {
        var result = await _eventService.GetByIdNotNullAsync(id);
        var weatherData = await _weatherService.GetWeatherDataForEventIdAsync(id);
        return result.ToResponse(weatherData);
    }

    public async Task<EventResponse> InsertAsync(EventRequest eventRequest)
    {
        var dto = eventRequest.ToDto();
        var result = await _eventService.InsertAsync(dto);
        return result.ToResponse();
    }

    public async Task<EventResponse> DeleteAsync(Guid id)
    {
        var result = await _eventService.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<PaginateResponse<EventResponse>> PaginatedGetAllAsync(PaginateRequest request)
    {
        var result = await _eventService.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(e => e.ToResponse());
    }

    public async Task<EventResponse> UploadImageByIdAsync(Guid eventId, IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var result = await _eventService.UploadImageById(
            eventId,
            fileName: file.FileName,
            contentType: file.ContentType,
            size: (int)file.Length,
            data: memoryStream.ToArray());

        return result.ToResponse();
    }

    public async Task<EventResponse> UploadImageByIdInFileSystemAsync(Guid eventId, IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var path = await _fileUploadService.UploadFileAsync(
            ms.ToArray(),
            file.FileName);
        
        var result = await _eventService.UpdateImagePathByIdAsync(eventId, path);

        return result.ToResponse();
    }
}