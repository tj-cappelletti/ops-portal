namespace OpsPortal.Contracts.Common;

public class SearchPaginatedRequest : PaginatedRequest
{
    public string? SearchTerm { get; init; }
}
