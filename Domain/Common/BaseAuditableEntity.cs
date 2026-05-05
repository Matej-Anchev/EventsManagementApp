namespace Domain.Common;

public class BaseAuditableEntity<TU> : BaseEntity
{
    public string CreatedById { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string LastModifiedById { get; set; } = string.Empty;
    public DateTime? DateLastModified { get; set; }
}