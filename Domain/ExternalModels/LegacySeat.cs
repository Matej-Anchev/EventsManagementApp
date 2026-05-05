namespace Domain.ExternalModels;

public class LegacySeat
{
    public int SeatId { get; set; }
    public int SectionId { get; set; }
    public string Row { get; set; } = string.Empty;
    public int Number { get; set; }
    public bool IsAccessible { get; set; }
    public DateTime LastModified { get; set; }
}