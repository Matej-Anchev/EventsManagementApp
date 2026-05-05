using Domain.Common;

namespace Domain.Models;

public class Venue : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    public int TotalCapacity { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
}