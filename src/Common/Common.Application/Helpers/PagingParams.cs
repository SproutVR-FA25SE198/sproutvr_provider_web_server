namespace Common.Application.Helpers;

/// <summary>
/// Base paging params for other specification params classes
/// </summary>
public class PagingParams
{
    private const int MaxPageSize = 30;
    public int PageIndex { get; set; } = 1;

    public bool IsPaginated { get; set; } = true;

    private int _pageSize = 5;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
