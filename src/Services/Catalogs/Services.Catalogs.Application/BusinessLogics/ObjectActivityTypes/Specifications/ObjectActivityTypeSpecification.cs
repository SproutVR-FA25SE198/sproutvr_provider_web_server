using Common.Application.Helpers;
using Services.Catalogs.Domain.Entities.ObjectActivityTypes;

namespace Services.Catalogs.Application.BusinessLogics.ObjectActivityTypes.Specifications;

public class ObjectActivityTypeSpecification : BaseSpecification<ObjectActivityType>
{
    public ObjectActivityTypeSpecification(List<Guid> mapObjectIds) 
        : base(oat => mapObjectIds.Contains(oat.MapObjectId))
    {
        AddInclude(oat => oat.ActivityType);
    }
}
