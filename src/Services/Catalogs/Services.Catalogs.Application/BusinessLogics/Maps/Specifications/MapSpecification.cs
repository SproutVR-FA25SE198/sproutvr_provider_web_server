using Common.Application.Helpers;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Specifications;
internal sealed class MapSpecification : BaseSpecification<Map>
{
    public MapSpecification(MapParams specParams)
        : base(x =>
            (string.IsNullOrEmpty(specParams.Name) || x.Name.Contains(specParams.Name)) &&
            (specParams.SubjectIds == null || specParams.SubjectIds.Length == 0 || specParams.SubjectIds.Contains(x.SubjectId)) &&
            (string.IsNullOrEmpty(specParams.Description) || x.Description.Contains(specParams.Description)) &&
            (!specParams.MinPrice.HasValue || x.Price >= specParams.MinPrice) &&
            (!specParams.MaxPrice.HasValue || x.Price <= specParams.MaxPrice) &&
            (string.IsNullOrEmpty(specParams.MapCode) || x.MapCode.Contains(specParams.MapCode)) &&
            (string.IsNullOrEmpty(specParams.Status) || x.Status == Enum.Parse<MapStatus>(specParams.Status))
        )
    {
        AddInclude(x => x.Subject);
        AddInclude(x => x.Subject.MasterSubject);
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        switch(specParams.SortBy)
        {
            case "name":
                AddOrderBy(x => x.Name);
                break;
            case "price-asc":
                AddOrderBy(x => x.Price);
                break;
            case "price-desc":
                AddOrderByDescending(x => x.Price);
                break;
            default:
                AddOrderByDescending(x => x.CreatedAtUtc);
                break;
        }
    }

    public MapSpecification(Guid id, bool getDetails = true)
        : base(x =>
            x.Id == id)
    {
        AddInclude(x => x.Subject);
        if (getDetails)
        {
            AddInclude(x => x.Subject.MasterSubject);
            AddInclude(x => x.MapObjects);
            AddInclude(x => x.TaskLocations);
        } 
    }
}
