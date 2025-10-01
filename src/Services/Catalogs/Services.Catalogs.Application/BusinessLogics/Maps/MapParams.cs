using Common.Application.Helpers;

namespace Services.Catalogs.Application.BusinessLogics.Maps;
public sealed class MapParams : PagingParams
{
    public string? Name { get; set; }
    public Guid? SubjectId { get; set; }
    public string? Description { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? MapCode { get; set; }
    public string? Status { get; set; }
}
