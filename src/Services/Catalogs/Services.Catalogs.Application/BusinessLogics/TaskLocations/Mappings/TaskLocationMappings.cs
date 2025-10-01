using Services.Catalogs.Application.BusinessLogics.TaskLocations.DTOs;
using Services.Catalogs.Domain.Entities.TaskLocations;

namespace Services.Catalogs.Application.BusinessLogics.TaskLocations.Mappings;
public static class TaskLocationMappings
{
    public static TaskLocationDto ToTaskLocationDto(this TaskLocation taskLocation)
    {
        return new TaskLocationDto
        {
            MapId = taskLocation.MapId,
            Name = taskLocation.Name,
            ImageUrl = taskLocation.ImageUrl,
            LocationCode = taskLocation.LocationCode
        };
    }
}
