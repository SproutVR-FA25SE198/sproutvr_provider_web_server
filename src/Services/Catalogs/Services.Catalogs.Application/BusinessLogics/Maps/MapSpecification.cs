using Common.Application.Helpers;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps;
internal sealed class MapSpecification : BaseSpecification<Map>
{
    public MapSpecification(MapParams specParams)
        : base(x =>
            (string.IsNullOrEmpty(specParams.Name) || x.Name.Contains(specParams.Name)) &&
            (!specParams.SubjectId.HasValue || x.SubjectId == specParams.SubjectId) &&
            (string.IsNullOrEmpty(specParams.Description) || x.Description.Contains(specParams.Description)) &&
            (!specParams.MinPrice.HasValue || x.Price >= specParams.MinPrice) &&
            (!specParams.MaxPrice.HasValue || x.Price <= specParams.MaxPrice) &&
            (string.IsNullOrEmpty(specParams.MapCode) || x.MapCode.Contains(specParams.MapCode)) &&
            (string.IsNullOrEmpty(specParams.Status) || x.Status.ToString() == specParams.Status)
        )
    {
        AddInclude(x => x.Subject);
        AddInclude(x => x.Subject.MasterSubject);
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        AddOrderBy(x => x.Name);
    }

    public MapSpecification(Guid id)
        : base(x =>
            x.Id == id)
    {
        AddInclude(x => x.Subject);
        AddInclude(x => x.Subject.MasterSubject);
        AddInclude(x => x.MapObjects);
        AddInclude(x => x.TaskLocations);
    }
}
