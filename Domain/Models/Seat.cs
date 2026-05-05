using Domain.Common;

namespace Domain.Models;

public class Seat: BaseEntity
{
    public string Row { get; set; } = string.Empty;
    public int Number { get; set; }
    public string? Label { get; set; }
    public bool IsAccessible { get; set; }
    
    public Guid SectionId { get; set; }
    public virtual Section Section { get; set; } = null!;

    public virtual ICollection<SeatReservation> SeatReservation { get; set; } = new List<SeatReservation>();
}