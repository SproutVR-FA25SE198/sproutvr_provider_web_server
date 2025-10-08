namespace Common.Domain.Entities;

public class BaseEntity
{
    public virtual bool UseIdKey => true;
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
