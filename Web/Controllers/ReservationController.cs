using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController: ControllerBase
{
    private readonly ReservationMapper _reservationMapper;

    public ReservationController(ReservationMapper reservationMapper)
    {
        _reservationMapper = reservationMapper;
    }

    [HttpGet]
    public async Task<List<ReservationResponse>> GetAllASync()
    {
        return await _reservationMapper.GetAllAsync();
    }

    [HttpPost]
    public async Task<IActionResult> InsertAsync([FromBody] ReservationRequest request)
    {
        var result = await _reservationMapper.InsertAsync(request);
        return Ok(result);
    }

    [HttpPost("with-user")]
    public async Task<IActionResult> InsertWithUserAsync([FromBody] ReservationRequest request)
    {
        var result = await _reservationMapper.InsertAsync(request);
        return Ok(result);
    }

    [HttpPatch("confirm/{reservationId}")]
    [Authorize]
    public async Task<IActionResult> ConfirmAsync([FromRoute] Guid reservationId)
    {
        var result = await _reservationMapper.ConfirmAsync(reservationId);
        return Ok(result);
    }
}