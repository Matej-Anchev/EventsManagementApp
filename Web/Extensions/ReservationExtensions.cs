using Domain.Models;

namespace Web.Extensions;

public static class ReservationExtensions
{
    public static ReservationResponse ToResponse(this Reservation reservation)
    {
        return new ReservationResponse(
            reservation.User.UserName,
            reservation.Event.Title,
            reservation.EventId,
            reservation.Id,
            reservation.ReservedAt,
            reservation.DateLastModified
        );
    }

    public static List<ReservationResponse> ToResponse(this List<Reservation> reservations)
    {
        return reservations.Select(x => x.ToResponse()).ToList();
    }

    public static ReservationBasicResponse ToBasicResponse(this Reservation reservation)
    {
        return new ReservationBasicResponse(
            reservation.EventId,
            reservation.Id,
            reservation.ReservedAt,
            reservation.DateLastModified);
    }
}