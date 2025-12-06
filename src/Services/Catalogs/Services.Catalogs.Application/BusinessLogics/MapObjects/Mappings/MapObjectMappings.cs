using Services.Catalogs.Application.BusinessLogics.MapObjects.DTOs;
using Services.Catalogs.Domain.Entities.MapObjects;

namespace Services.Catalogs.Application.BusinessLogics.MapObjects.Mappings;
public static class MapObjectMappings
{
    public static MapObjectDto ToMapObjectDto(this MapObject mapObject)
    {
        return new MapObjectDto
        {
            Id = mapObject.Id,
            MapId = mapObject.MapId,
            Name = mapObject.Name,
            ImageUrl = mapObject.ImageUrl,
            ObjectCode = mapObject.ObjectCode
        };
    }
}
