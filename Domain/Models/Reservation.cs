using Domain.Common;
using Domain.Dto.Enums;

namespace Domain.Models;

public class Reservation: BaseAuditableEntity<EventsAppUser>
{
    public DateTime ReservedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public ReservationStatus Status { get; set; }
    
    public Guid EventId { get; set; }
    public virtual Event Event { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public virtual EventsAppUser User { get; set; } = null!;

    public virtual ICollection<SeatReservation> SeatReservation { get; set; } = new List<SeatReservation>();
}