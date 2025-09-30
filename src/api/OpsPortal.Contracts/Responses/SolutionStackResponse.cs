namespace OpsPortal.Contracts.Responses;

public record SolutionStackResponse(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    string Category,
    string Status,
    string Owner,
    DateTime CreatedAt,
    DateTime UpdatedAt);
