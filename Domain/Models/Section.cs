using Domain.Common;

namespace Domain.Models;

public class Section: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    
    public Guid VenueId { get; set; }
    public virtual Venue Venue { get; set; } = null!;
}