using Services.Catalogs.Application.BusinessLogics.MapObjects.DTOs;
using Services.Catalogs.Application.BusinessLogics.MapObjects.Mappings;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.CreateMap;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapById;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.UpdateMap;
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
            PreviewUrl = map.PreviewUrl,
            Status = map.Status.ToString(),
            MapCode = map.MapCode
        };
    }

    public static MapDetailsDto ToDetailsDto(this Map map)
    {
        return new MapDetailsDto
        {
            Id = map.Id,
            Subject = map.Subject?.ToDto() ?? new SubjectDto(),
            Price = map.Price,
            Name = map.Name,
            Description = map.Description,
            ImageUrl = map.ImageUrl,
            PreviewUrl = map.PreviewUrl,
            Status = map.Status.ToString(),
            MapCode = map.MapCode,
            MapObjects = map.MapObjects?.Select(mo => mo.ToMapObjectDto()).ToList() ?? new List<MapObjectDto>(),
            TaskLocations = map.TaskLocations?.Select(tl => tl.ToTaskLocationDto()).ToList() ?? new List<TaskLocationDto>(),
            CreatedAtUtc = map.CreatedAtUtc,
            UpdatedAtUtc = map.UpdatedAtUtc
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
            PreviewUrl = dto.PreviewUrl,
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
        map.PreviewUrl = dto.PreviewUrl ?? map.PreviewUrl;
        map.Status = dto.Status != null ? Enum.Parse<MapStatus>(dto.Status) : map.Status;
        map.MapCode = dto.MapCode ?? map.MapCode;

        return map;
    } 
}
