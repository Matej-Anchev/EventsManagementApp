namespace Domain.ExternalModels;

public class LegacyVenue
{
    public int VenueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int TotalCapacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastModified { get; set; }
}