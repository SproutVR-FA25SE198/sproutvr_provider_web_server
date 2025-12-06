using Common.Application.Helpers;
using Services.Catalogs.Domain.Entities.TaskLocations;

namespace Services.Catalogs.Application.BusinessLogics.TaskLocations.Specifications;

public class TaskLocationSpecification : BaseSpecification<TaskLocation>
{
    public TaskLocationSpecification(Guid mapId) 
        : base(tl => tl.MapId == mapId)
    {
    }
}
