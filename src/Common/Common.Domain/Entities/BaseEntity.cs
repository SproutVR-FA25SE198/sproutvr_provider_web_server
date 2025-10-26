using System.Text.Json.Serialization;

namespace Common.Domain.Entities;

public class BaseEntity
{
    [JsonIgnore]
    public virtual bool UseIdKey => true;
    public Guid Id { get; set; }

    [JsonIgnore]
    public DateTime CreatedAtUtc { get; set; }

    [JsonIgnore]
    public DateTime UpdatedAtUtc { get; set; }
}
