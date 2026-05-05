using Domain.Common;
using Domain.Dto.Enums;

namespace Domain.Models;

public class Ticket : BaseAuditableEntity<EventsAppUser>
{
    public string Barcode { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public TicketStatus Status { get; set; }

    public Guid EventId { get; set; }
    public virtual Event Event { get; set; } = null!;
    
    public Guid SeatReservationId { get; set; }
    public virtual SeatReservation SeatReservation { get; set; } = null!;
    
    public string UserId { get; set; } = string.Empty;
    public virtual EventsAppUser User { get; set; } = null!;
}