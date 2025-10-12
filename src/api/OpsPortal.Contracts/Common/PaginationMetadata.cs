namespace OpsPortal.Contracts.Common;

public record PaginationMetadata(
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
