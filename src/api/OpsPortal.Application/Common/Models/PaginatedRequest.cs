namespace OpsPortal.Application.Common.Models;

public abstract record PaginatedRequest
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public int Skip => (ValidPageNumber - 1) * ValidPageSize;

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; } = false;

    public int ValidPageNumber => PageNumber < 1 ? 1 : PageNumber;

    public int ValidPageSize => PageSize switch
    {
        <= 0 => DefaultPageSize,
        > MaxPageSize => MaxPageSize,
        _ => PageSize
    };
}
