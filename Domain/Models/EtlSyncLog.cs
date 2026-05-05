using Domain.Common;

namespace Domain.Models;

public class EtlSyncLog : BaseEntity
{
    public string JobName { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool Success { get; set; }
    public String? ErrorMessage { get; set; }
}