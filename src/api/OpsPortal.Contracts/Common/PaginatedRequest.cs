namespace OpsPortal.Contracts.Common;

public class PaginatedRequest
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; } = false;
}
