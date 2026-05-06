using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class ReservationMapper
{
    private readonly IReservationService _reservationService;
    private readonly ICurrentUser _currentUser;

    public ReservationMapper(IReservationService reservationService, ICurrentUser currentUser)
    {
        _reservationService = reservationService;
        _currentUser = currentUser;
    }


    public async Task<List<ReservationResponse>> GetAllAsync()
    {
        var result = await _reservationService.GetAllAsync();
        return result.ToResponse();
    }

    public async Task<ReservationBasicResponse> InsertAsync(ReservationRequest request)
    {
        var userId = _currentUser.GetUserId();
        var result = await _reservationService.InsertAsync(request.EventId, userId!);
        return result.ToBasicResponse();
    }

    public async Task<ReservationBasicResponse> ConfirmAsync(Guid reservationId)
    {
        var reservation = await _reservationService.GetByIdNotNullAsync(reservationId);
        var userId = _currentUser.GetUserId();

        if (reservation.UserId != userId)
            throw new InvalidOperationException("Cannot confirm another reservation");
        
        var result = await _reservationService.ConfirmAsync(reservationId);
        return result.ToBasicResponse();
    }
}