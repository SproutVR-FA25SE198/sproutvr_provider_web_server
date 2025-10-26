using Common.Application.Helpers;
using Services.Catalogs.Domain.Entities.ObjectLocations;

namespace Services.Catalogs.Application.BusinessLogics.ObjectLocations.Specifications;

public class ObjectLocationSpecification : BaseSpecification<ObjectLocation>
{
    public ObjectLocationSpecification(List<Guid> mapObjectIds) 
        : base(ol => mapObjectIds.Contains(ol.ObjectId))
    {
    }
}
