using Common.Application.Helpers;
using Services.Catalogs.Domain.Entities.MapObjects;

namespace Services.Catalogs.Application.BusinessLogics.MapObjects.Specifications;

public class MapObjectSpecification : BaseSpecification<MapObject>
{
    public MapObjectSpecification(Guid mapId) 
        : base(mo => mo.MapId == mapId)
    {
    }
}
