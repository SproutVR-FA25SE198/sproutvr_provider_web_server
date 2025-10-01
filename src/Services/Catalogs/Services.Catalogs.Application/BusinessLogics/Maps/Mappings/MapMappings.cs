using Services.Catalogs.Application.BusinessLogics.MapObjects.DTOs;
using Services.Catalogs.Application.BusinessLogics.MapObjects.Mappings;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;
using Services.Catalogs.Application.BusinessLogics.Maps.UpdateMap;
using Services.Catalogs.Application.BusinessLogics.Subjects.DTOs;
using Services.Catalogs.Application.BusinessLogics.Subjects.Mappings;
using Services.Catalogs.Application.BusinessLogics.TaskLocations.DTOs;
using Services.Catalogs.Application.BusinessLogics.TaskLocations.Mappings;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Mappings;

public static class MapMappings
{
    public static MapDto ToDto(this Map map)
    {
        return new MapDto
        {
            Id = map.Id,
            Subject = map.Subject?.ToDto() ?? new SubjectDto(),
            Price = map.Price,
            Name = map.Name,
            Description = map.Description,
            ImageUrl = map.ImageUrl,
            Status = map.Status.ToString(),
            MapCode = map.MapCode,
            MapObjects = map.MapObjects?.Select(mo => mo.ToMapObjectDto()).ToList() ?? new List<MapObjectDto>(),
            TaskLocations = map.TaskLocations?.Select(tl => tl.ToTaskLocationDto()).ToList() ?? new List<TaskLocationDto>()
        };
    }

    public static Map ToEntity(this CreateMapDto dto)
    {
        return new Map
        {
            SubjectId = dto.SubjectId,
            Price = dto.Price,
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Status = Enum.Parse<MapStatus>(dto.Status),
            MapCode = dto.MapCode
        };
    }

    public static Map ToEntity(UpdateMapDto dto, Map map)
    {
        map.SubjectId = dto.SubjectId ?? map.SubjectId;
        map.Price = dto.Price ?? map.Price;
        map.Name = dto.Name ?? map.Name;
        map.Description = dto.Description ?? map.Description;
        map.ImageUrl = dto.ImageUrl ?? map.ImageUrl;
        map.Status = dto.Status != null ? Enum.Parse<MapStatus>(dto.Status) : map.Status;
        map.MapCode = dto.MapCode ?? map.MapCode;

        return map;
    } 
}
