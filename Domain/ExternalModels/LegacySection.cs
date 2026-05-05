namespace Domain.ExternalModels;

public class LegacySection
{
    public int SectionId { get; set; }
    public int VenueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public DateTime LastModified { get; set; }
}