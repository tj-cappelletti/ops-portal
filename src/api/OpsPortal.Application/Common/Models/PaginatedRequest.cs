namespace OpsPortal.Application.Common.Models;

public abstract record PaginatedRequest
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    internal int Skip => (GetValidPageNumber() - 1) * GetValidPageSize();

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; } = false;

    public int GetValidPageNumber()
    {
        return Math.Max(1, PageNumber);
    }

    public int GetValidPageSize()
    {
        return Math.Min(Math.Max(1, PageSize), MaxPageSize);
    }
}
