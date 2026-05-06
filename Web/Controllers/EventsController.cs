using Microsoft.AspNetCore.Mvc;
using Repository;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly EventMapper _eventMapper;
    public ApplicationDbContext _context;

    public EventsController(EventMapper eventMapper, ApplicationDbContext context)
    {
        _eventMapper = eventMapper;
        _context = context;
    }

    [HttpGet]
    public async Task<List<EventResponse>> GetAll()
    {
        return await _eventMapper.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _eventMapper.GetById(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] EventRequest eventRequest)
    {
        var result = await _eventMapper.InsertAsync(eventRequest);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _eventMapper.DeleteAsync(id);
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<PaginateResponse<EventResponse>> Paged([FromQuery] PaginateRequest request)
    {
        return await _eventMapper.PaginatedGetAllAsync(request);
    }

    [HttpPost("upload-image/{eventId}")]
    public async Task<IActionResult> UploadImageByIdAsync([FromRoute] Guid eventId, [FromForm] IFormFile file)
    {
        var result = await _eventMapper.UploadImageByIdAsync(eventId, file);
        return Ok(result);
    }

    [HttpPost("upload-image-fs/{eventId}")]
    public async Task<IActionResult> UploadImageByIdInFileSystemAsync([FromRoute] Guid eventId,
        [FromForm] IFormFile file)
    {
        var result = _eventMapper.UploadImageByIdInFileSystemAsync(eventId, file);
        return Ok(result);
    }
}