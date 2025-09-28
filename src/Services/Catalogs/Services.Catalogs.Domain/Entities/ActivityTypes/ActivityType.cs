using Common.Domain.Entities;

namespace Services.Catalogs.Domain.Entities.ActivityTypes;
public class ActivityType : BaseEntity
{
    public string Name { get; set; }
    public string ConfigSchema { get; set; }
}
