namespace OpsPortal.Application.Common.Queries;

public abstract record SearchQuery : PaginatedQuery
{
    public string? SearchTerm { get; init; }
}
