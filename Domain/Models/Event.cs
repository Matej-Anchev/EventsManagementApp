using Domain.Common;
using Domain.Dto.Enums;

namespace Domain.Models;

public class Event : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EventStatus Status { get; set; }
    public string? ImageUrl { get; set; }

    public Guid? EventImageId { get; set; }
    public virtual EventsImages? EventImage { get; set; }

    public Guid? VenueId { get; set; }
    public virtual Venue Venue { get; set; } = null!;

    public string? UserId { get; set; }
    public virtual EventsAppUser User { get; set; } = null!;

    public virtual ICollection<EventSectionPricing> SectionPricings { get; set; } = new List<EventSectionPricing>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<SeatReservation> SeatReservations { get; set; } = new List<SeatReservation>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}