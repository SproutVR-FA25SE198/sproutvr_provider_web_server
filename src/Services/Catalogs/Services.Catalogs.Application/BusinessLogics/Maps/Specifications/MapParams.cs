using Common.Application.Helpers;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Specifications;
public sealed class MapParams : PagingParams
{
    public string? Name { get; set; }
    public Guid[]? SubjectIds { get; set; } 
    public string? Description { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? MapCode { get; set; }
    public string? Status { get; set; }
    public string? SortBy { get; set; }
}
